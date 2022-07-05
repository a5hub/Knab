using KnabTest.ExchangeRates.ApiClient.Responses;
using Refit;

namespace KnabTest.ExchangeRates.ApiClient.Clients;

public interface IExchangeRatesWebApi
{
    /// <summary> Get last exchange rates form one currency to list of other currencies </summary>
    /// <param name="base"> Currency from which will be exchange, for ex. "USD"</param>
    /// <param name="symbols"> Currencies to which will be exchange, for ex. "EUR,BRL,GBP,..." </param>
    /// <returns> Exchange rates by each requested currency to base currency </returns>
    [Get("/exchangerates_data/latest")]
    Task<GetLatestExchangeRatesResponse> GetLatestExchangeRates(string @base, string symbols);
}