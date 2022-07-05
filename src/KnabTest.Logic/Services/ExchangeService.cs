using System.Collections.ObjectModel;
using KnabTest.Logic.Enums;
using KnabTest.Logic.Exceptions;
using KnabTest.Logic.Models;

namespace KnabTest.Logic.Services;

/// <inheritdoc cref="IExchangeService" />
public class ExchangeService : IExchangeService
{
    private readonly ICryptocurrencyService _cryptocurrencyService;

    private readonly ICurrencyService _currencyService;

    public ExchangeService(ICryptocurrencyService cryptocurrencyService, 
        ICurrencyService currencyService)
    {
        _cryptocurrencyService = cryptocurrencyService;
        _currencyService = currencyService;
    }

    /// <inheritdoc cref="IExchangeService.GetCryptocurrencyExchangeRates" />
    public async Task<IReadOnlyCollection<GetCryptocurrencyExchangeRatesModel>> GetCryptocurrencyExchangeRates(
        IReadOnlyCollection<CryptocurrencyCodeType> cryptocurrencyCodes, IReadOnlyCollection<CurrencyCodeType> targetCurrencyCodes)
    {
        // Get cryptocurrency exchange rates
        var btcUsdExchangeRates = await _cryptocurrencyService
            .GetUsdExchangeRates(cryptocurrencyCodes).ConfigureAwait(false);
        if (btcUsdExchangeRates == null || !btcUsdExchangeRates.Any())
        {
            throw new BusinessException(Resources.NoCryptocurrencyExchangeRates);
        }

        // Get currencies exchange rates
        var currencyExchangeRates = await _currencyService
            .GetCurrencyExchangeRates(CurrencyCodeType.USD, targetCurrencyCodes).ConfigureAwait(false);
        if (currencyExchangeRates == null || !currencyExchangeRates.Any())
        {
            throw new BusinessException(Resources.NoCurrencyExchangeRates);
        }

        // Recalculate cryptocurrency exchange rates from usd to another currencies
        var result = currencyExchangeRates.Join(btcUsdExchangeRates!,
            c => c.BaseCurrency,
            cc => cc.ExchangeCurrencyCode,
            (c, cc) => new GetCryptocurrencyExchangeRatesModel
            {
                Cryptocurrency = cc.CryptocurrencyCode,
                Currency = c.ExchangeCurrency,
                ExchangeRate = c.ExchangeRate * cc.ExchangeRate,
                UpdatedAt = c.LastUpdated
            }).ToList();
        if (!result.Any())
        {
            throw new BusinessException(Resources.NoRatesForCombination);
        }

        return new ReadOnlyCollection<GetCryptocurrencyExchangeRatesModel>(result);
    }
}