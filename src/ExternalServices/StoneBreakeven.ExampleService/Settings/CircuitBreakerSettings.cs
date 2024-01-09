namespace StoneBreakeven.ExampleService.Settings
{
    public class CircuitBreakerSettings
    {
        public int DurationOfBreakInSeconds { get; set; }
        public double FailureThreshold { get; set; }
        public int SamplingDurationInSeconds { get; set; }
        public int MinimumThroughput { get; set; }
        public int HandledEventsAllowedBeforeBreaking { get; set; }
    }
}
