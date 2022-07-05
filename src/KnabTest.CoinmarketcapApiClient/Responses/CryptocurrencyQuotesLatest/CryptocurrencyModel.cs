using System.Text.Json.Serialization;

namespace KnabTest.CoinmarketcapApiClient.Responses.CryptocurrencyQuotesLatest
{
    public class CryptocurrencyModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } 
        
        [JsonPropertyName("quote")]
        public Dictionary<string, CryptocurrencyQuoteModel>? Quote { get; set; }
    }
}