using KnabTest.Logic.Enums;

namespace KnabTest.Logic.Models;

public class GetUsdExchangeRatesModel
{
    public int CryptocurrencyId { get; set; }
        
    public CryptocurrencyCodeType CryptocurrencyCode { get; set; }
        
    public CurrencyCodeType  ExchangeCurrencyCode { get; set; }
        
    public decimal ExchangeRate { get; set; }
    
    public DateTimeOffset LastUpdated { get; set; }
}