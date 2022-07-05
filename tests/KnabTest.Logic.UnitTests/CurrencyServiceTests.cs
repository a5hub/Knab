using FluentAssertions;
using KnabTest.ExchangeRates.ApiClient.Clients;
using KnabTest.ExchangeRates.ApiClient.Responses;
using KnabTest.Logic.Enums;
using KnabTest.Logic.Models;
using KnabTest.Logic.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace KnabTest.UnitTests;

public class CurrencyServiceTests
{
    private readonly CurrencyService _sut;
    
    private readonly Mock<IExchangeRatesWebApi> _exchangeRatesWebApi = new();
    private readonly IMemoryCache _memCache = new Microsoft.Extensions.Caching.Memory.MemoryCache(new MemoryCacheOptions());
    
    public CurrencyServiceTests()
    {
        _sut = new CurrencyService(_exchangeRatesWebApi.Object, _memCache);
    }

    [Fact]
    public async Task GetCurrencyExchangeRatesTest_WorksFine_WithoutCache()
    {
        // Arrange
        var baseCurrency = CurrencyCodeType.USD;
        
        var cCodes = new List<CurrencyCodeType>
        {
            CurrencyCodeType.USD,
            CurrencyCodeType.EUR
        };
        var cCodesStr = string.Join(",", cCodes);

        var unixDate = 1657019435;
        var latestExchangeRatesResponse = new GetLatestExchangeRatesResponse
        {
            DateTime = unixDate,
            BaseCurrency = "USD",
            ExchangeRates = new Dictionary<string, decimal>
            {
                {"USD", 10},
                {"EUR", 100}
            }
        };

        var date = DateTimeOffset.FromUnixTimeSeconds(unixDate);
        
        var expected = new List<GetCurrencyExchangeRatesModel>
        {
            new()
            {
                BaseCurrency = CurrencyCodeType.USD,
                ExchangeCurrency = CurrencyCodeType.USD,
                ExchangeRate = 10,
                LastUpdated = date
            },
            new()
            {
                BaseCurrency = CurrencyCodeType.USD,
                ExchangeCurrency = CurrencyCodeType.EUR,
                ExchangeRate = 100,
                LastUpdated = date
            }
        };
        
        // Expectations
        _exchangeRatesWebApi.Setup(_ => _.GetLatestExchangeRates(baseCurrency.ToString(), cCodesStr))
            .ReturnsAsync(latestExchangeRatesResponse);

        // Act
        var actual = await _sut.GetCurrencyExchangeRates(baseCurrency, cCodes);
        _memCache.TryGetValue("USD_USD,EUR_GetCurrencyExchangeRates", out IReadOnlyCollection<GetCurrencyExchangeRatesModel>? cachedValue);

        // Assert
        actual.Should().BeEquivalentTo(expected);
        cachedValue.Should().BeEquivalentTo(expected);
        _exchangeRatesWebApi.Verify(factory => 
            factory.GetLatestExchangeRates(baseCurrency.ToString(), cCodesStr), Times.Once);
    }
    
    [Fact]
    public async Task GetCurrencyExchangeRatesTest_WorksFine_GetFromCache()
    {
        // Arrange
        var baseCurrency = CurrencyCodeType.USD;
        
        var cCodes = new List<CurrencyCodeType>
        {
            CurrencyCodeType.USD,
            CurrencyCodeType.EUR
        };
        
        var cCodesStr = string.Join(",", cCodes);

        var date = DateTimeOffset.Now;
        
        var expected = new List<GetCurrencyExchangeRatesModel>
        {
            new()
            {
                BaseCurrency = CurrencyCodeType.USD,
                ExchangeCurrency = CurrencyCodeType.USD,
                ExchangeRate = 10,
                LastUpdated = date
            },
            new()
            {
                BaseCurrency = CurrencyCodeType.USD,
                ExchangeCurrency = CurrencyCodeType.EUR,
                ExchangeRate = 100,
                LastUpdated = date
            }
        };

        _memCache.Set("USD_USD,EUR_GetCurrencyExchangeRates", expected);

        // Act
        var actual = await _sut.GetCurrencyExchangeRates(baseCurrency, cCodes);
        
        // Assert
        actual.Should().BeEquivalentTo(expected);
        _exchangeRatesWebApi.Verify(factory => 
            factory.GetLatestExchangeRates(baseCurrency.ToString(), cCodesStr), Times.Never);
    }
}