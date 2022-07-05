using FluentAssertions;
using KnabTest.Logic.Enums;
using KnabTest.Logic.Exceptions;
using KnabTest.Logic.Models;
using KnabTest.Logic.Services;
using Moq;

namespace KnabTest.UnitTests;

public class ExchangeServiceTests
{
    private readonly ExchangeService _sut;
    
    private readonly Mock<ICryptocurrencyService> _cryptocurrencyService = new();
    private readonly Mock<ICurrencyService> _currencyService = new();
    
    public ExchangeServiceTests()
    {
        _sut = new ExchangeService(_cryptocurrencyService.Object, _currencyService.Object);
    }
    
    [Fact]
    public async Task GetCryptocurrencyExchangeRatesTest_WorksFine()
    {
        #region Arrange
        
        var ccCodes = new List<CryptocurrencyCodeType>
        {
            CryptocurrencyCodeType.BTC,
            CryptocurrencyCodeType.LTC
        };
            
        var cCodes = new List<CurrencyCodeType>
        {
            CurrencyCodeType.USD,
            CurrencyCodeType.EUR
        };

        var dateTimeOffset = DateTimeOffset.Now;
        
        var usdExchangeRates = new List<GetUsdExchangeRatesModel>
        {
            new()
            {
                CryptocurrencyId = 1,
                CryptocurrencyCode = CryptocurrencyCodeType.BTC,
                ExchangeCurrencyCode = CurrencyCodeType.USD,
                ExchangeRate = 100,
                LastUpdated = dateTimeOffset
            },
            new()
            {
                CryptocurrencyId = 2,
                CryptocurrencyCode = CryptocurrencyCodeType.LTC,
                ExchangeCurrencyCode = CurrencyCodeType.USD,
                ExchangeRate = 10,
                LastUpdated = dateTimeOffset
            }
        };

        var currencyExchangeRates = new List<GetCurrencyExchangeRatesModel>
        {
            new()
            {
                BaseCurrency = CurrencyCodeType.USD,
                ExchangeCurrency = CurrencyCodeType.USD,
                ExchangeRate = 1,
                LastUpdated = dateTimeOffset
            },
            new()
            {
                BaseCurrency = CurrencyCodeType.USD,
                ExchangeCurrency = CurrencyCodeType.EUR,
                ExchangeRate = 0.5M,
                LastUpdated = dateTimeOffset
            }
        };

        var expected = new List<GetCryptocurrencyExchangeRatesModel>
        {
            new()
            {
                Cryptocurrency = CryptocurrencyCodeType.BTC,
                Currency = CurrencyCodeType.USD,
                ExchangeRate = 100,
                UpdatedAt  = dateTimeOffset
            },
            new()
            {
                Cryptocurrency = CryptocurrencyCodeType.BTC,
                Currency = CurrencyCodeType.EUR,
                ExchangeRate = 50,
                UpdatedAt  = dateTimeOffset
            },
            new()
            {
                Cryptocurrency = CryptocurrencyCodeType.LTC,
                Currency = CurrencyCodeType.USD,
                ExchangeRate = 10,
                UpdatedAt  = dateTimeOffset
            },
            new()
            {
                Cryptocurrency = CryptocurrencyCodeType.LTC,
                Currency = CurrencyCodeType.EUR,
                ExchangeRate = 5,
                UpdatedAt  = dateTimeOffset
            }
        };
        
        #endregion
        
        // Expectations
        _cryptocurrencyService.Setup(_ => _.GetUsdExchangeRates(ccCodes))
            .ReturnsAsync(usdExchangeRates);

        _currencyService.Setup(_ => _.GetCurrencyExchangeRates(CurrencyCodeType.USD, cCodes))
            .ReturnsAsync(currencyExchangeRates);

        // Act
        var actual = await _sut.GetCryptocurrencyExchangeRates(ccCodes, cCodes);

        // Assert
        actual.Should().BeEquivalentTo(expected);
        
        _cryptocurrencyService.Verify(factory => 
            factory.GetUsdExchangeRates(ccCodes), Times.Once);
        _currencyService.Verify(factory => 
            factory.GetCurrencyExchangeRates(CurrencyCodeType.USD, cCodes), Times.Once);
    }

    [Fact]
    public async Task GetCryptocurrencyExchangeRatesTest_UsdExchangeRates_IsNull()
    {
        // Arrange
        var ccCodes = new List<CryptocurrencyCodeType>
        {
            CryptocurrencyCodeType.BTC,
            CryptocurrencyCodeType.LTC
        };
            
        var cCodes = new List<CurrencyCodeType>
        {
            CurrencyCodeType.USD,
            CurrencyCodeType.EUR
        };

        List<GetUsdExchangeRatesModel> usdExchangeRates = null; 
        
        // Expectations
        _cryptocurrencyService.Setup(_ => _.GetUsdExchangeRates(ccCodes))
            .ReturnsAsync(usdExchangeRates);
        
        // Act
        var func = async () => await _sut.GetCryptocurrencyExchangeRates(ccCodes, cCodes);

        // Assert
        await func.Should().ThrowExactlyAsync<BusinessException>();
        
        _cryptocurrencyService.Verify(factory => 
            factory.GetUsdExchangeRates(ccCodes), Times.Once);
        _currencyService.Verify(factory => 
            factory.GetCurrencyExchangeRates(CurrencyCodeType.USD, cCodes), Times.Never);
    }
    
    [Fact]
    public async Task GetCryptocurrencyExchangeRatesTest_UsdExchangeRates_IsEmpty()
    {
        // Arrange
        var ccCodes = new List<CryptocurrencyCodeType>
        {
            CryptocurrencyCodeType.BTC,
            CryptocurrencyCodeType.LTC
        };
            
        var cCodes = new List<CurrencyCodeType>
        {
            CurrencyCodeType.USD,
            CurrencyCodeType.EUR
        };

        var usdExchangeRates = new List<GetUsdExchangeRatesModel>(); 
        
        // Expectations
        _cryptocurrencyService.Setup(_ => _.GetUsdExchangeRates(ccCodes))
            .ReturnsAsync(usdExchangeRates);
        
        // Act
        var func = async () => await _sut.GetCryptocurrencyExchangeRates(ccCodes, cCodes);

        // Assert
        await func.Should().ThrowExactlyAsync<BusinessException>();
        _cryptocurrencyService.Verify(factory => 
            factory.GetUsdExchangeRates(ccCodes), Times.Once);
        _currencyService.Verify(factory => 
            factory.GetCurrencyExchangeRates(CurrencyCodeType.USD, cCodes), Times.Never);
    }

    [Fact]
    public async Task GetCryptocurrencyExchangeRatesTest_CurrencyExchangeRates_IsNull()
    {
        
        // Arrange
        var ccCodes = new List<CryptocurrencyCodeType>
        {
            CryptocurrencyCodeType.BTC,
            CryptocurrencyCodeType.LTC
        };
            
        var cCodes = new List<CurrencyCodeType>
        {
            CurrencyCodeType.USD,
            CurrencyCodeType.EUR
        };
        
        var dateTimeOffset = DateTimeOffset.Now;
        
        var usdExchangeRates = new List<GetUsdExchangeRatesModel>
        {
            new()
            {
                CryptocurrencyId = 1,
                CryptocurrencyCode = CryptocurrencyCodeType.BTC,
                ExchangeCurrencyCode = CurrencyCodeType.USD,
                ExchangeRate = 100,
                LastUpdated = dateTimeOffset
            },
            new()
            {
                CryptocurrencyId = 2,
                CryptocurrencyCode = CryptocurrencyCodeType.LTC,
                ExchangeCurrencyCode = CurrencyCodeType.USD,
                ExchangeRate = 10,
                LastUpdated = dateTimeOffset
            }
        };

        List<GetCurrencyExchangeRatesModel> currencyExchangeRates = null;
        
        // Expectations
        _cryptocurrencyService.Setup(_ => _.GetUsdExchangeRates(ccCodes))
            .ReturnsAsync(usdExchangeRates);

        _currencyService.Setup(_ => _.GetCurrencyExchangeRates(CurrencyCodeType.USD, cCodes))
            .ReturnsAsync(currencyExchangeRates);
        
        // Act
        var func = async () => await _sut.GetCryptocurrencyExchangeRates(ccCodes, cCodes);
        
        // Assert
        await func.Should().ThrowExactlyAsync<BusinessException>();
        
        _cryptocurrencyService.Verify(factory => 
            factory.GetUsdExchangeRates(ccCodes), Times.Once);
        _currencyService.Verify(factory => 
            factory.GetCurrencyExchangeRates(CurrencyCodeType.USD, cCodes), Times.Once);
    }
    
    [Fact]
    public async Task GetCryptocurrencyExchangeRatesTest_CurrencyExchangeRates_IsEmpty()
    {
        
        // Arrange
        var ccCodes = new List<CryptocurrencyCodeType>
        {
            CryptocurrencyCodeType.BTC,
            CryptocurrencyCodeType.LTC
        };
            
        var cCodes = new List<CurrencyCodeType>
        {
            CurrencyCodeType.USD,
            CurrencyCodeType.EUR
        };
        
        var dateTimeOffset = new DateTimeOffset(2022, 01, 01, 10, 20, 30, new TimeSpan());
        
        var usdExchangeRates = new List<GetUsdExchangeRatesModel>
        {
            new()
            {
                CryptocurrencyId = 1,
                CryptocurrencyCode = CryptocurrencyCodeType.BTC,
                ExchangeCurrencyCode = CurrencyCodeType.USD,
                ExchangeRate = 100,
                LastUpdated = dateTimeOffset
            },
            new()
            {
                CryptocurrencyId = 2,
                CryptocurrencyCode = CryptocurrencyCodeType.LTC,
                ExchangeCurrencyCode = CurrencyCodeType.USD,
                ExchangeRate = 10,
                LastUpdated = dateTimeOffset
            }
        };

        var currencyExchangeRates = new List<GetCurrencyExchangeRatesModel>();
        
        // Expectations
        _cryptocurrencyService.Setup(_ => _.GetUsdExchangeRates(ccCodes))
            .ReturnsAsync(usdExchangeRates);

        _currencyService.Setup(_ => _.GetCurrencyExchangeRates(CurrencyCodeType.USD, cCodes))
            .ReturnsAsync(currencyExchangeRates);
        
        // Act
        var func = async () => await _sut.GetCryptocurrencyExchangeRates(ccCodes, cCodes);
        
        // Assert
        await func.Should().ThrowExactlyAsync<BusinessException>();
        
        _cryptocurrencyService.Verify(factory => 
            factory.GetUsdExchangeRates(ccCodes), Times.Once);
        _currencyService.Verify(factory => 
            factory.GetCurrencyExchangeRates(CurrencyCodeType.USD, cCodes), Times.Once);
    }
    
    [Fact]
    public async Task GetCryptocurrencyExchangeRatesTest_ResultIsEmpty()
    {
        #region Arrange
        
        var ccCodes = new List<CryptocurrencyCodeType>
        {
            CryptocurrencyCodeType.BTC,
            CryptocurrencyCodeType.LTC
        };
            
        var cCodes = new List<CurrencyCodeType>
        {
            CurrencyCodeType.USD,
            CurrencyCodeType.EUR
        };

        var dateTimeOffset = new DateTimeOffset(2022, 01, 01, 10, 20, 30, new TimeSpan());
        
        var usdExchangeRates = new List<GetUsdExchangeRatesModel>
        {
            new()
            {
                CryptocurrencyId = 1,
                CryptocurrencyCode = CryptocurrencyCodeType.BTC,
                ExchangeCurrencyCode = CurrencyCodeType.USD,
                ExchangeRate = 100,
                LastUpdated = dateTimeOffset
            },
            new()
            {
                CryptocurrencyId = 2,
                CryptocurrencyCode = CryptocurrencyCodeType.LTC,
                ExchangeCurrencyCode = CurrencyCodeType.USD,
                ExchangeRate = 10,
                LastUpdated = dateTimeOffset
            }
        };

        var currencyExchangeRates = new List<GetCurrencyExchangeRatesModel>
        {
            new()
            {
                BaseCurrency = CurrencyCodeType.GBP,
                ExchangeCurrency = CurrencyCodeType.BRL,
                ExchangeRate = 1,
                LastUpdated = dateTimeOffset
            },
            new()
            {
                BaseCurrency = CurrencyCodeType.EUR,
                ExchangeCurrency = CurrencyCodeType.GBP,
                ExchangeRate = 0.5M,
                LastUpdated = dateTimeOffset
            }
        };

        #endregion
        
        // Expectations
        _cryptocurrencyService.Setup(_ => _.GetUsdExchangeRates(ccCodes))
            .ReturnsAsync(usdExchangeRates);

        _currencyService.Setup(_ => _.GetCurrencyExchangeRates(CurrencyCodeType.USD, cCodes))
            .ReturnsAsync(currencyExchangeRates);

        // Act
        var func = async () => await _sut.GetCryptocurrencyExchangeRates(ccCodes, cCodes);
        
        // Assert
        await func.Should().ThrowExactlyAsync<BusinessException>();
        
        _cryptocurrencyService.Verify(factory => 
            factory.GetUsdExchangeRates(ccCodes), Times.Once);
        _currencyService.Verify(factory => 
            factory.GetCurrencyExchangeRates(CurrencyCodeType.USD, cCodes), Times.Once);
    }
}