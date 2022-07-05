using KnabTest.CoinmarketcapApiClient.Clients;
using KnabTest.CoinmarketcapApiClient.Handlers;
using KnabTest.CoinmarketcapApiClient.Options;
using KnabTest.Common.ApiClient.ApiPolicies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;

namespace KnabTest.CoinmarketcapApiClient.Extensions;

public static class ServiceCollectionCoinmarketcapApiClientExtensions
{
    public static IServiceCollection AddCoinmarketcapApiServices(this IServiceCollection services)
    {
        services
            .AddRefitClient<ICoinmarketcapWebApi>()
            .ConfigureHttpClient(
                (sp, c) =>
                {
                    c.BaseAddress = new Uri(sp.GetRequiredService<IOptions<CoinmarketcapClientOptions>>().Value.CoinmarketcapApiUrl);
                })
            .AddHttpMessageHandler(sp => new AuthHeaderHandler(sp.GetRequiredService<IOptions<CoinmarketcapClientOptions>>()))
            .AddPolicyHandler((sp, req) => sp.GetRequiredService<IApiPolicies>().WaitAndRetryAsync);

        return services;
    }
}