using Polly.CircuitBreaker;
using StoneBreakeven.Api.Models;
using System.Net;
using System.Text.Json;

namespace StoneBreakeven.Api.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Handles the specified <see cref="Exception"/> for the specified <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="httpContext">The HTTP httpContext.</param>
        /// <param name="exception">The exception.</param>
        /// <returns>The HTTP response that is modified based on the exception.</returns>
        private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            (HttpStatusCode statusCode, IReadOnlyCollection<string> errors) = GetErrorsAndStatusCode(exception);

            httpContext.Response.ContentType = "application/json";

            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            string response = JsonSerializer.Serialize(new ExternalServiceErrorResponse(errors.ToList(), statusCode.GetHashCode()), serializerOptions);

            await httpContext.Response.WriteAsync(response);
        }

        private static (HttpStatusCode, IReadOnlyCollection<string>) GetErrorsAndStatusCode(Exception exception) =>
               exception switch
               {
                   HttpRequestException httpRequestException => (httpRequestException.StatusCode!.Value, new[] { httpRequestException.Message }),
                   BrokenCircuitException brokenCircuitException => (HttpStatusCode.TooManyRequests, new[] { brokenCircuitException.Message }),
                   _ => (HttpStatusCode.InternalServerError, new[] { "Internal Server Error" })
               };
    }
}
