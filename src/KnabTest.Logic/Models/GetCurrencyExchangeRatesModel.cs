using KnabTest.Logic.Enums;

namespace KnabTest.Logic.Models;

public class GetCurrencyExchangeRatesModel
{
    public CurrencyCodeType BaseCurrency { get; set; }
    
    public CurrencyCodeType ExchangeCurrency { get; set; }
    
    public decimal ExchangeRate { get; set; }

    public DateTimeOffset LastUpdated { get; set; }
}