using System.Text.Json.Serialization;

namespace KnabTest.CoinmarketcapApiClient.Responses.CryptocurrencyQuotesLatest
{
    public class CryptocurrencyQuotesLatestResponse
    {
        [JsonPropertyName("data")]
        public Dictionary<string, CryptocurrencyModel>? Cryptocurrencies { get; set; }
    }
}