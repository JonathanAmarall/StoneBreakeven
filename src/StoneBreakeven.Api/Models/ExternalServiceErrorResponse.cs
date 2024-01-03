namespace StoneBreakeven.Api.Models
{
    public record ExternalServiceErrorResponse(List<string> Errors, int StatusCode, string Description = "ExampleService");
}
