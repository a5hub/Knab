using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace KnabTest.Common.ApiClient.ApiPolicies;

public class ApiPolicies : IApiPolicies
{
    private readonly ILogger<ApiPolicies> _logger;
        
    private readonly ApiPolicyOptions _options;

    public ApiPolicies(ILogger<ApiPolicies> logger, IOptions<ApiPolicyOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public IAsyncPolicy<HttpResponseMessage> WaitAndRetryAsync => Policy<HttpResponseMessage>
        .Handle<HttpRequestException>()
        .OrResult(r => !r.IsSuccessStatusCode)
        .FallbackAsync(
            (result, _, _) => Task.FromResult(result.Result),
            async (result, context) =>
            {
                var content = result.Result?.Content;

                if (content != null)
                {
                    var contentStr = await content.ReadAsStringAsync().ConfigureAwait(false);

                    _logger.LogError(
                        "Status Code: {StatusCode}. Content: {Content}. Retry count exceeded",
                        result.Result?.StatusCode,
                        contentStr);

                    if (result.Exception != null)
                    {
                        _logger.LogError(result.Exception, "Retry count exceeded");
                    }
                }
                else
                {
                    switch (result.Exception)
                    {
                        case null:
                            throw new InvalidOperationException(
                                $"Fallback policy called with no exception, nor content, CorrelationId: {context?.CorrelationId}");
                            
                        case HttpRequestException requestException:
                            _logger.LogError(
                                requestException,
                                "Status Code: {StatusCode}: {Message}, retry count exceeded",
                                requestException.StatusCode,
                                requestException.Message);
                            break;
                            
                        default:
                            _logger.LogError(result.Exception, "Exception thrown in request processing");
                            break;
                    }

                    throw result.Exception;
                }
            })
        .WrapAsync(
            HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    _options.MaxRetryCount,
                    sleepDuration => TimeSpan.FromMilliseconds(_options.RetryDelayMs),
                    onRetry: async (
                        response,
                        delay,
                        retryCount,
                        context) =>
                    {
                        if (response.Exception != null)
                        {
                            _logger.LogError(
                                response.Exception,
                                "Delaying for {delay}ms, then making retry {retry} of {maxRetry}.",
                                delay.TotalMilliseconds,
                                retryCount,
                                _options.MaxRetryCount);
                        }
                        else
                        {
                            var content = response?.Result?.Content;
                            var contentStr = string.Empty;

                            if (content != null)
                            {
                                contentStr = await content.ReadAsStringAsync().ConfigureAwait(false);
                            }

                            _logger.LogError(
                                "Status Code: {statusCode}. Content: {content} Delaying for {delay}ms, then making retry {retry} of {maxRetry}.",
                                response?.Result?.StatusCode,
                                contentStr,
                                delay.TotalMilliseconds,
                                retryCount,
                                _options.MaxRetryCount);
                        }
                    })
        );
}