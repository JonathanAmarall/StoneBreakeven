using Polly;
using Polly.Extensions.Http;
using Polly.Registry;
using Polly.Retry;
using Polly.Timeout;
using System.Net;
using System.Net.Sockets;

namespace StoneBreakeven.Api.Extensions
{
    public static class ResiliencePolicies
    {
        public static PolicyRegistry CreatePolicies(string registryKeyName)
        {
            var registry = new PolicyRegistry();

            var timeoutPolicy = CreateTimeoutPolicy();
            var retryPolicy = CreateRetryPolicy();
            var fallbackPolicy = CreateFallbackPolicy();

            registry.Add(registryKeyName, Policy.WrapAsync(timeoutPolicy, retryPolicy, fallbackPolicy));

            return registry;
        }

        private static AsyncTimeoutPolicy<HttpResponseMessage> CreateTimeoutPolicy()
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(
                   timeout: TimeSpan.FromMilliseconds(10000),
                   timeoutStrategy: TimeoutStrategy.Optimistic,
                   onTimeoutAsync: (context, timespan, _, exc) =>
                   {
                       LogRequest(timespan, LogType.Timeout);
                       return Task.CompletedTask;
                   });
        }

        private static AsyncRetryPolicy<HttpResponseMessage> CreateRetryPolicy()
        {
            return Policy<HttpResponseMessage>
                   .Handle<HttpRequestException>()
                   .Or<SocketException>()
                   .Or<TimeoutException>()
                   .Or<TaskCanceledException>()
                   .Or<TimeoutRejectedException>()
                   .Or<HttpRequestException>(x => x.StatusCode == HttpStatusCode.FailedDependency)
                   .OrTransientHttpError()
                   .WaitAndRetryAsync(retryCount: 5,
                    retryAttempt => TimeSpan.FromMilliseconds(300 * Math.Pow(2, retryAttempt)),
                    onRetry: (httpResponse, timeSpan, count, context) =>
                        LogRequest(timeSpan, LogType.Retry)
                    );
        }

        private static IAsyncPolicy<HttpResponseMessage> CreateFallbackPolicy()
        {
            using var defaultFallBack = new HttpResponseMessage(HttpStatusCode.FailedDependency);
            defaultFallBack.RequestMessage = new HttpRequestMessage();

            return Policy<HttpResponseMessage>
                .HandleInner<HttpRequestException>()
                .Or<SocketException>()
                .Or<TimeoutException>()
                .Or<TimeoutRejectedException>()
                .FallbackAsync(fallbackValue: defaultFallBack, onFallbackAsync: (http, context) =>
                {
                    LogRequest(default, LogType.Fallback);
                    return Task.CompletedTask;
                });
        }

        private static void LogRequest(TimeSpan timespan, LogType type)
        {
            var previousBackgroundColor = Console.BackgroundColor;
            var previousForegroundColor = Console.ForegroundColor;

            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.ForegroundColor = ConsoleColor.Black;

            Console.Out.WriteLineAsync($" ***** {DateTime.Now:HH:mm:ss} | " +
                $"{type}\n" +
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
