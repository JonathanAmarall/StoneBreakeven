namespace StoneBreakeven.ExampleService
{
    public class ExampleServiceSettings
    {
        public static string SectionName => "ExampleService";

        public string BaseAddress { get; set; } = string.Empty;
        public int RetryCount { get; set; }
        public int RetryInterval { get; set; }
        public int TimeoutInMilliseconds { get; set; }
        public CircuitBreakerSettings CircuitBreakerSettings { get; set; } = new();
    }

    public class CircuitBreakerSettings
    {
        public bool EnableCircuitBreakerPolicy { get; set; }
        public int DurationOfBreakInSeconds { get; set; }
        public double FailureThreshold { get; set; }
        public int SamplingDurationInSeconds { get; set; }
        public int MinimumThroughput { get; set; }
    }
}
