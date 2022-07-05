using KnabTest.Logic.Enums;
using KnabTest.Logic.Models;

namespace KnabTest.Logic.Services;

/// <summary> Provides method for working with cryptocurrencies </summary>
public interface ICryptocurrencyService
{
    /// <summary> Get exchange rates in USD for basic cryptocurrencies </summary>
    /// <param name="cryptocurrencyCodes"> Collection of cryptocurrencies numeric code </param>
    /// <returns> Collection of exchange rates for cryptocurrencies into USD </returns>
    Task<IReadOnlyCollection<GetUsdExchangeRatesModel>?> GetUsdExchangeRates(
        IReadOnlyCollection<CryptocurrencyCodeType> cryptocurrencyCodes);
}