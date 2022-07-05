using KnabTest.Common.ApiClient.ApiPolicies;
using KnabTest.ExchangeRates.ApiClient.Clients;
using KnabTest.ExchangeRates.ApiClient.Handlers;
using KnabTest.ExchangeRates.ApiClient.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;

namespace KnabTest.ExchangeRates.ApiClient.Extensions;

public static class ServiceCollectionExchangeRatesApiClientExtensions
{
    public static IServiceCollection AddExchangeRatesApiServices(this IServiceCollection services)
    {
        services
            .AddRefitClient<IExchangeRatesWebApi>()
            .ConfigureHttpClient(
                (sp, c) =>
                {
                    c.BaseAddress = new Uri(sp.GetRequiredService<IOptions<ExchangeRatesClientOptions>>().Value.ExchangeRatesApiUrl);
                })
            .AddHttpMessageHandler(sp => new AuthHeaderHandler(sp.GetRequiredService<IOptions<ExchangeRatesClientOptions>>()))
            .AddPolicyHandler((sp, req) => sp.GetRequiredService<IApiPolicies>().WaitAndRetryAsync);

        return services;
    }
}