using System.Collections.ObjectModel;
using KnabTest.ExchangeRates.ApiClient.Clients;
using KnabTest.Logic.Enums;
using KnabTest.Logic.Models;
using Microsoft.Extensions.Caching.Memory;

namespace KnabTest.Logic.Services;

/// <inheritdoc cref="ICurrencyService" />
public class CurrencyService : ICurrencyService
{
    private readonly IExchangeRatesWebApi _exchangeRatesWebApi;
        
    private readonly IMemoryCache _memoryCache;
    
    private readonly MemoryCacheEntryOptions _memoryCacheOptions;

    public CurrencyService(IExchangeRatesWebApi exchangeRatesWebApi, 
        IMemoryCache memoryCache)
    {
        _exchangeRatesWebApi = exchangeRatesWebApi;
        _memoryCache = memoryCache;
        _memoryCacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(Constants.CacheExpirationTimeMin));
    }

    /// <inheritdoc cref="ICurrencyService.GetCurrencyExchangeRates" />
    public async Task<IReadOnlyCollection<GetCurrencyExchangeRatesModel>?> GetCurrencyExchangeRates(CurrencyCodeType baseCurrency, 
        IReadOnlyCollection<CurrencyCodeType> currencyCodes)
    {
        var currencyCodesStr = string.Join(",", currencyCodes);
        var cacheKey = baseCurrency + "_" + currencyCodesStr + "_" + nameof(GetCurrencyExchangeRates);

        if (!_memoryCache.TryGetValue(cacheKey, out IReadOnlyCollection<GetCurrencyExchangeRatesModel>? result))
        {
            var rates = await _exchangeRatesWebApi.GetLatestExchangeRates(baseCurrency.ToString(), currencyCodesStr).ConfigureAwait(false);

            result = new ReadOnlyCollection<GetCurrencyExchangeRatesModel>(
                rates.ExchangeRates.Select(x => new GetCurrencyExchangeRatesModel
                {
                    BaseCurrency = baseCurrency,
                    ExchangeCurrency = Enum.Parse<CurrencyCodeType>(x.Key, true),
                    ExchangeRate = x.Value,
                    LastUpdated = DateTimeOffset.FromUnixTimeSeconds(rates.DateTime)
                }).ToList());
                
            _memoryCache.Set(cacheKey, result, _memoryCacheOptions);
        }

        return result;
    }
}