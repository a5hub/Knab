using Polly;

namespace KnabTest.Common.ApiClient.ApiPolicies
{
    public interface IApiPolicies
    {
        IAsyncPolicy<HttpResponseMessage> WaitAndRetryAsync { get; }
    }
}
