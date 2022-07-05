using System.Text.Json.Serialization;

namespace KnabTest.CoinmarketcapApiClient.Responses.CryptocurrencyQuotesLatest
{
    public class CryptocurrencyQuoteModel
    {
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    
        [JsonPropertyName("last_updated")]
        public DateTimeOffset LastUpdated { get; set; }
    }
}