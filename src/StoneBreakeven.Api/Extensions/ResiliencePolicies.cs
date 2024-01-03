using Polly;
using Polly.Extensions.Http;
using Polly.Registry;
using Polly.Retry;
using Polly.Timeout;
using StoneBreakeven.ExampleService;
using System.Net;
using System.Net.Sockets;

namespace StoneBreakeven.Api.Extensions
{
    public static class ResiliencePolicies
    {
        public static PolicyRegistry CreatePolicies(ExampleServiceSettings settings)
        {
            var registry = new PolicyRegistry();

            var timeoutPolicy = CreateTimeoutPolicy(settings);
            var retryPolicy = CreateRetryPolicy(settings);
            var circuitBreaker = CreateCircuitBreakPolicy(settings);
            var fallbackPolicy = CreateFallbackPolicy();

            registry.Add(ExampleServiceSettings.SectionName, Policy.WrapAsync(fallbackPolicy, circuitBreaker, retryPolicy, timeoutPolicy));

            return registry;
        }

        private static AsyncTimeoutPolicy<HttpResponseMessage> CreateTimeoutPolicy(ExampleServiceSettings settings)
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(
                   timeout: TimeSpan.FromMilliseconds(settings.TimeoutInMilliseconds),
                   timeoutStrategy: TimeoutStrategy.Optimistic,
                   onTimeoutAsync: (context, timespan, _, exc) =>
                   {
                       LogRequest(timespan, LogType.Timeout);
                       return Task.CompletedTask;
                   });
        }

        private static AsyncRetryPolicy<HttpResponseMessage> CreateRetryPolicy(ExampleServiceSettings settings)
        {
            return Policy<HttpResponseMessage>
                   .Handle<HttpRequestException>()
                   .Or<SocketException>()
                   .Or<TimeoutException>()
                   .Or<TaskCanceledException>()
                   //.Or<TimeoutRejectedException>()
                   .Or<HttpRequestException>(x => x.StatusCode == HttpStatusCode.FailedDependency)
                   .OrTransientHttpError()
                   .WaitAndRetryAsync(
                        retryCount: settings.RetryCount,
                        sleepDurationProvider: retryAttempt => TimeSpan.FromMilliseconds(settings.RetryInterval * Math.Pow(2, retryAttempt)),
                        onRetry: (httpResponse, timeSpan, count, context) =>
                        {
                            LogRequest(timeSpan, LogType.Retry);
                        }
                    );
        }

        private static IAsyncPolicy<HttpResponseMessage> CreateFallbackPolicy()
        {
            using var defaultFallBack = new HttpResponseMessage(HttpStatusCode.FailedDependency);
            defaultFallBack.RequestMessage = new HttpRequestMessage();

            return Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(result => result.StatusCode == HttpStatusCode.ServiceUnavailable)
                .FallbackAsync(fallbackValue: defaultFallBack, onFallbackAsync: (http, context) =>
                {
                    LogRequest(default, LogType.Fallback);
                    return Task.CompletedTask;
                });
        }

        private static IAsyncPolicy<HttpResponseMessage> CreateCircuitBreakPolicy(ExampleServiceSettings settings)
        {
            return HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .AdvancedCircuitBreakerAsync(
                        failureThreshold: settings.CircuitBreakerSettings.FailureThreshold,
                        samplingDuration: TimeSpan.FromSeconds(settings.CircuitBreakerSettings.SamplingDurationInSeconds),
                        minimumThroughput: settings.CircuitBreakerSettings.MinimumThroughput,
                        durationOfBreak: TimeSpan.FromSeconds(settings.CircuitBreakerSettings.DurationOfBreakInSeconds),
                        onBreak: (ex, _) =>
                        {
                            LogRequest(default, LogType.CircuitBreaker, "onBreak");
                        },
                        onReset: () => LogRequest(default, LogType.CircuitBreaker, "onReset"),
                        onHalfOpen: () => LogRequest(default, LogType.CircuitBreaker, "onHalfOpen"));
        }

        private static void LogRequest(TimeSpan timespan, LogType type, string? description = default)
        {
            var previousBackgroundColor = Console.BackgroundColor;
            var previousForegroundColor = Console.ForegroundColor;

            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.ForegroundColor = ConsoleColor.Black;

            Console.Out.WriteLineAsync($" ***** {DateTime.Now:HH:mm:ss} | " +
                $"{type}\n" + description ?? string.Empty +
                $"Tempo de Espera em segundos: {timespan.TotalSeconds} **** ");

            Console.BackgroundColor = previousBackgroundColor;
            Console.ForegroundColor = previousForegroundColor;
        }
    }

    public enum LogType
    {
        Timeout,
        Retry,
        CircuitBreaker,
        Fallback
    }
}
