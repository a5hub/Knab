using KnabTest.Logic.Enums;

namespace KnabTest.Logic.Models;

public class GetCryptocurrencyExchangeRatesModel
{
    public CryptocurrencyCodeType Cryptocurrency { get; set; }
    
    public CurrencyCodeType Currency { get; set; }
    
    public decimal ExchangeRate { get; set; }
    
    public DateTimeOffset UpdatedAt { get; set; }
}