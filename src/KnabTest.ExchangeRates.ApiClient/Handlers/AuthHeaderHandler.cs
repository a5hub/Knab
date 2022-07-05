using KnabTest.ExchangeRates.ApiClient.Options;
using Microsoft.Extensions.Options;

namespace KnabTest.ExchangeRates.ApiClient.Handlers
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly ExchangeRatesClientOptions _exchangeRatesClientOptions;

        public AuthHeaderHandler(IOptions<ExchangeRatesClientOptions> exchangeRatesClientOptions)
        {
            _exchangeRatesClientOptions = exchangeRatesClientOptions.Value;
        }
    
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("apikey", _exchangeRatesClientOptions.ExchangeRatesApiKey);
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}