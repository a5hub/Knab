namespace KnabTest.Common.ApiClient.ApiPolicies
{
    public record ApiPolicyOptions
    {
        public const string Key = "ApiPolicy";
        
        public int MaxRetryCount { get; init; }
        
        public int RetryDelayMs { get; init; }

        /// <summary>
        ///     Timeout in milliseconds after which request will be cancelled as timed out
        /// </summary>
        public int TimeoutMs { get; init; }

        /// <summary>
        ///     The maximum backoff in milliseconds places an upper limit on exponential backoff growth
        /// </summary>
        public int MaxBackoffMs { get; init; }

        /// <summary>
        ///     The backoff will be multiplied by this value after each retry attempt
        ///     and will increase exponentially when the multiplier is greater than 1
        /// </summary>
        public double BackoffMultiplier { get; init; }
    }
}
