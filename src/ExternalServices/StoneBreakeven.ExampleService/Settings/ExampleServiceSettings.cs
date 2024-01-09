namespace StoneBreakeven.ExampleService.Settings
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
}
