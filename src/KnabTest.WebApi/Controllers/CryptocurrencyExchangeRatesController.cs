using System.Net;
using KnabTest.Api.Responses;
using KnabTest.Logic.Enums;
using KnabTest.Logic.Models;
using KnabTest.Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace KnabTest.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class CryptocurrencyExchangeRatesController : ControllerBase
{
    private readonly IExchangeService _exchangeService;

    public CryptocurrencyExchangeRatesController(IExchangeService exchangeService)
    {
        _exchangeService = exchangeService;
    }

    [HttpGet(Name = "get-cryptocurrency-exchange-rates")]
    [ProducesResponseType(typeof(IEnumerable<GetCryptocurrencyExchangeRatesResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetCryptocurrencyExchangeRates()
    {
        var ccCodes = new List<CryptocurrencyCodeType>
        {
            CryptocurrencyCodeType.BTC
        };
            
        var cCodes = new List<CurrencyCodeType>
        {
            CurrencyCodeType.USD,
            CurrencyCodeType.EUR,
            CurrencyCodeType.BRL,
            CurrencyCodeType.GBP,
            CurrencyCodeType.AUD
        };
                
        var result = await _exchangeService
            .GetCryptocurrencyExchangeRates(ccCodes, cCodes).ConfigureAwait(false);

        return Ok(result.Select(x => new GetCryptocurrencyExchangeRatesResponse
        {
            CryptocurrencyId = x.Cryptocurrency,
            CryptocurrencyCode = x.Cryptocurrency.ToString(),
            CurrencyId = x.Currency,
            CurrencyCode = x.Currency.ToString(),
            ExchangeRate = x.ExchangeRate,
            UpdatedAt = x.UpdatedAt
        }));
    }
}