using KnabTest.Logic.Enums;
using KnabTest.Logic.Models;

namespace KnabTest.Logic.Services
{
    /// <summary> Provides exchanging methods </summary>
    public interface IExchangeService
    {
        /// <summary> Provides exchange rates for cryptocurrencies into currencies </summary>
        /// <param name="cryptocurrencyCodes"> Collection of cryptocurrencies </param>
        /// <param name="targetCurrencyCodes"> Collection of currencies </param>
        /// <returns> Cryptocurrency into currencies exchange rates collection </returns>
        Task<IReadOnlyCollection<GetCryptocurrencyExchangeRatesModel>> GetCryptocurrencyExchangeRates(
            IReadOnlyCollection<CryptocurrencyCodeType> cryptocurrencyCodes, IReadOnlyCollection<CurrencyCodeType> targetCurrencyCodes);
    }
}