using FluentAssertions;
using KnabTest.CoinmarketcapApiClient.Clients;
using KnabTest.CoinmarketcapApiClient.Responses.CryptocurrencyQuotesLatest;
using KnabTest.Logic.Enums;
using KnabTest.Logic.Models;
using KnabTest.Logic.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace KnabTest.UnitTests;

public class CryptocurrencyServiceTests
{
    private readonly CryptocurrencyService _sut;
    
    private readonly Mock<ICoinmarketcapWebApi> _coinmarketcapWebApi = new();
    private readonly IMemoryCache _memCache = new Microsoft.Extensions.Caching.Memory.MemoryCache(new MemoryCacheOptions());
    
    public CryptocurrencyServiceTests()
    {
        _sut = new CryptocurrencyService(_coinmarketcapWebApi.Object, _memCache);
    }

    [Fact]
    public async Task GetUsdExchangeRatesTest_WorksFine_WithoutCache()
    {
        // Arrange
        var ccCodes = new List<CryptocurrencyCodeType>
        {
            CryptocurrencyCodeType.BTC
        };

        var dateTimeOffset = DateTimeOffset.Now;
        var price = 50M;
        
        var cryptocurrencyQuotesLatestResponse = new CryptocurrencyQuotesLatestResponse
        {
            Cryptocurrencies = new Dictionary<string, CryptocurrencyModel>
            {
                {"1", new CryptocurrencyModel
                {
                    Id = 1,
                    Symbol = "BTC",
                    Quote = new Dictionary<string, CryptocurrencyQuoteModel>
                    {
                        {"USD", new CryptocurrencyQuoteModel
                        {
                            Price = price,
                            LastUpdated = dateTimeOffset,
                        }}
                    }
                        
                }}
            }
        };

        var expected = new List<GetUsdExchangeRatesModel>
        {
            new()
            {
                CryptocurrencyId = 1,
                CryptocurrencyCode = CryptocurrencyCodeType.BTC,
                ExchangeCurrencyCode = CurrencyCodeType.USD,
                ExchangeRate = price,
                LastUpdated = dateTimeOffset
            }
        };
        
        var cryptocurrencyIdsStr = string.Join(",", ccCodes.Select(x => (int)x));
        
        // Expectations
        _coinmarketcapWebApi.Setup(_ => _.GetCryptocurrencyLatestUsdQuotes(cryptocurrencyIdsStr))
            .ReturnsAsync(cryptocurrencyQuotesLatestResponse);
            
        // Act
        var actual = await _sut.GetUsdExchangeRates(ccCodes);
        _memCache.TryGetValue("1_GetUsdExchangeRates", out IReadOnlyCollection<GetUsdExchangeRatesModel>? cachedValue);

        // Assert
        actual.Should().BeEquivalentTo(expected);
        cachedValue.Should().BeEquivalentTo(expected);
        _coinmarketcapWebApi.Verify(factory => 
            factory.GetCryptocurrencyLatestUsdQuotes(cryptocurrencyIdsStr), Times.Once);
    }
    
    [Fact]
    public async Task GetUsdExchangeRatesTest_WorksFine_GetFromCache()
    {
        // Arrange
        var ccCodes = new List<CryptocurrencyCodeType>
        {
            CryptocurrencyCodeType.BTC
        };

        var dateTimeOffset = DateTimeOffset.Now;
        var price = 50M;

        var expected = new List<GetUsdExchangeRatesModel>
        {
            new()
            {
                CryptocurrencyId = 1,
                CryptocurrencyCode = CryptocurrencyCodeType.BTC,
                ExchangeCurrencyCode = CurrencyCodeType.USD,
                ExchangeRate = price,
                LastUpdated = dateTimeOffset
            }
        };
        
        var cryptocurrencyIdsStr = string.Join(",", ccCodes.Select(x => (int)x));
        
        _memCache.Set("1_GetUsdExchangeRates", expected);
            
        // Act
        var actual = await _sut.GetUsdExchangeRates(ccCodes);

        // Assert
        actual.Should().BeEquivalentTo(expected);
        _coinmarketcapWebApi.Verify(factory => 
            factory.GetCryptocurrencyLatestUsdQuotes(cryptocurrencyIdsStr), Times.Never);
    }
}