using KnabTest.CoinmarketcapApiClient.Options;
using Microsoft.Extensions.Options;

namespace KnabTest.CoinmarketcapApiClient.Handlers
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly CoinmarketcapClientOptions _coinmarketcapClientOptions;

        public AuthHeaderHandler(IOptions<CoinmarketcapClientOptions> coinmarketcapClientOptions)
        {
            _coinmarketcapClientOptions = coinmarketcapClientOptions.Value;
        }
    
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("X-CMC_PRO_API_KEY", _coinmarketcapClientOptions.CoinmarketcapApiKey);
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}