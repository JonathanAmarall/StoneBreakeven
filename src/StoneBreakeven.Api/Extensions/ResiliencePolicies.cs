using Polly;
using Polly.CircuitBreaker;
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
                       TimeoutLogRequest(timespan);
                       return Task.CompletedTask;
                   });
        }

        private static AsyncRetryPolicy<HttpResponseMessage> CreateRetryPolicy(ExampleServiceSettings settings)
        {
            return Policy<HttpResponseMessage>
                   .Handle<HttpRequestException>()
                   .OrResult(result => result.StatusCode == HttpStatusCode.InternalServerError)
                   .Or<SocketException>()
                   .Or<TimeoutException>()
                   .Or<TaskCanceledException>()
                   .WaitAndRetryAsync(
                        retryCount: settings.RetryCount,
                        sleepDurationProvider: retryAttempt => TimeSpan.FromMilliseconds(settings.RetryInterval * Math.Pow(2, retryAttempt)),
                        onRetry: (httpResponse, timeSpan, attempt, context) =>
                        {
                            RetryLogRequest(timeSpan, attempt);
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
                    FallbackLogRequest(http.Result.StatusCode, defaultFallBack.StatusCode);
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
                        onBreak: (ex, timeSpan) =>
                        {
                            CircuitBreakerLogRequest(CircuitState.Open);
                        },
                        onReset: () => CircuitBreakerLogRequest(CircuitState.Closed),
                        onHalfOpen: () => CircuitBreakerLogRequest(CircuitState.HalfOpen));
        }

        private static void LogRequest(string title, string message)
        {
            var previousBackgroundColor = Console.BackgroundColor;
            var previousForegroundColor = Console.ForegroundColor;

            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;

            Console.Out.WriteLineAsync($" *****\n" +
                $"{title} | {DateTime.Now:HH:mm:ss} |\n{message} \n" +
                $"*****");

            Console.BackgroundColor = previousBackgroundColor;
            Console.ForegroundColor = previousForegroundColor;
        }

        private static void RetryLogRequest(TimeSpan timespan, int attempt)
        {
            LogRequest("RETRY", $"Tempo de Espera em segundos: {timespan.TotalSeconds} |\nTentativa: {attempt}");
        }

        private static void TimeoutLogRequest(TimeSpan timespan)
        {
            LogRequest("TIMEOUT", $"Tempo Limite de espera em segundos: {timespan.TotalSeconds}");
        }

        private static void FallbackLogRequest(HttpStatusCode externalServiceStatusCode, HttpStatusCode defaultStatusCode)
        {
            LogRequest("FALLBACK", $"Status Code recebido do serviço externo: {externalServiceStatusCode} |\nStatus Code retornado:  {defaultStatusCode}");
        }

        private static void CircuitBreakerLogRequest(CircuitState circuitState)
        {
            LogRequest("CIRCUIT BREAKER", $"Estado atual: {circuitState}");
        }
    }
}
