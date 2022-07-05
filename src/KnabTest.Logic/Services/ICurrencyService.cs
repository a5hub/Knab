using KnabTest.Logic.Enums;
using KnabTest.Logic.Models;

namespace KnabTest.Logic.Services;

/// <summary> Provides method for working with currencies </summary>
public interface ICurrencyService
{
    /// <summary> Get exchange rates for basic currencies </summary>
    /// <param name="baseCurrency"> Base currency for getting exchange rates for it </param>
    /// <param name="currencyCodes"> Collection of target currencies for getting exchange rates </param>
    /// <returns> Collection of exchange rates for basic currency into target currencies </returns>
    Task<IReadOnlyCollection<GetCurrencyExchangeRatesModel>?> GetCurrencyExchangeRates(CurrencyCodeType baseCurrency,
        IReadOnlyCollection<CurrencyCodeType> currencyCodes);
}