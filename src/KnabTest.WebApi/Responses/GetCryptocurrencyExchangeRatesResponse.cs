using KnabTest.Logic.Enums;

namespace KnabTest.Api.Responses;

public class GetCryptocurrencyExchangeRatesResponse
{
    public CryptocurrencyCodeType CryptocurrencyId { get; set; }
    
    public string CryptocurrencyCode { get; set; }
    
    public CurrencyCodeType CurrencyId  { get; set; }
    
    public string CurrencyCode { get; set; }
    
    public decimal ExchangeRate { get; set; }
    
    public DateTimeOffset UpdatedAt { get; set; }
}