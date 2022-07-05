using KnabTest.CoinmarketcapApiClient.Clients;
using KnabTest.Logic.Enums;
using KnabTest.Logic.Models;
using Microsoft.Extensions.Caching.Memory;

namespace KnabTest.Logic.Services;

/// <inheritdoc cref="ICryptocurrencyService" />
public class CryptocurrencyService : ICryptocurrencyService
{
    private readonly ICoinmarketcapWebApi _coinmarketcapWebApi;
        
    private readonly IMemoryCache _memoryCache;

    private readonly MemoryCacheEntryOptions _memoryCacheOptions;

    public CryptocurrencyService(ICoinmarketcapWebApi coinmarketcapWebApi, 
        IMemoryCache memoryCache)
    {
        _coinmarketcapWebApi = coinmarketcapWebApi;
        _memoryCache = memoryCache;
        _memoryCacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(Constants.CacheExpirationTimeMin));
    }

    /// <inheritdoc cref="ICryptocurrencyService.GetUsdExchangeRates" />
    public async Task<IReadOnlyCollection<GetUsdExchangeRatesModel>?> GetUsdExchangeRates(
        IReadOnlyCollection<CryptocurrencyCodeType> cryptocurrencyCodes)
    {
        var cryptocurrencyCodesStr = string.Join(",", cryptocurrencyCodes.Select(x => (int)x));
        var cacheKey = cryptocurrencyCodesStr + "_" + nameof(GetUsdExchangeRates);
            
        if (!_memoryCache.TryGetValue(cacheKey, out IReadOnlyCollection<GetUsdExchangeRatesModel>? result))
        {
            var quotes = await _coinmarketcapWebApi.GetCryptocurrencyLatestUsdQuotes(cryptocurrencyCodesStr).ConfigureAwait(false);

            result = quotes.Cryptocurrencies?
                .SelectMany(x => x.Value.Quote?
                    .Select(y => new GetUsdExchangeRatesModel
                    {
                        CryptocurrencyId = x.Value.Id,
                        CryptocurrencyCode = Enum.Parse<CryptocurrencyCodeType>(x.Value.Symbol, true),
                        ExchangeCurrencyCode = Enum.Parse<CurrencyCodeType>(y.Key, true),
                        ExchangeRate = y.Value.Price,
                        LastUpdated = y.Value.LastUpdated
                    }) ?? Array.Empty<GetUsdExchangeRatesModel>()).ToList();
                
            _memoryCache.Set(cacheKey, result, _memoryCacheOptions);
        }

        return result;
    }
}