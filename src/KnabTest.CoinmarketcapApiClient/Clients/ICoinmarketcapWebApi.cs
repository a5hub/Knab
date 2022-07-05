using KnabTest.CoinmarketcapApiClient.Responses.CryptocurrencyQuotesLatest;
using Refit;

namespace KnabTest.CoinmarketcapApiClient.Clients;

public interface ICoinmarketcapWebApi
{
    /// <summary> Get recent cryptocurrencies quotes prices in USD </summary>
    /// <param name="id"> List of cryptocurrency ids comma-separated, for example, "1" or "1,2,3,..." </param>
    /// <returns> Quotes by each cryptocurrency in USD </returns>
    [Get("/v2/cryptocurrency/quotes/latest")]
    Task<CryptocurrencyQuotesLatestResponse> GetCryptocurrencyLatestUsdQuotes(string id);
}