using System.Text.Json.Serialization;

namespace KnabTest.ExchangeRates.ApiClient.Responses;

public class GetLatestExchangeRatesResponse
{
    [JsonPropertyName("timestamp")]
    public long DateTime { get; set; }
    
    [JsonPropertyName("base")]
    public string BaseCurrency { get; set; }
    
    [JsonPropertyName("rates")]
    public Dictionary<string, decimal> ExchangeRates { get; set; }
}