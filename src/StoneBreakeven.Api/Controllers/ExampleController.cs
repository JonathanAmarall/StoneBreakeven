using Microsoft.AspNetCore.Mvc;
using StoneBreakeven.ExampleService;

namespace StoneBreakeven.Api.Controllers;

[ApiController]
[Route("/api/example")]
public class ExampleController : ControllerBase
{
    private readonly IExampleService _exampleService;

    public ExampleController(IExampleService exampleService)
    {
        _exampleService = exampleService;
    }

    [HttpGet("retry")]
    public async Task<ActionResult> GetRetry(CancellationToken cancellationToken)
    {
        var response = await _exampleService.GetRandomResult(cancellationToken);
        return Ok(new { Message = response });
    }

    [HttpGet("timeout")]
    public async Task<ActionResult> GetTimeout(CancellationToken cancellationToken)
    {
        var response = await _exampleService.GetWithDelay(10000, cancellationToken);
        return Ok(new { Message = response });
    }

    [HttpGet("fallback")]
    public async Task<ActionResult> GetFallback(CancellationToken cancellationToken)
    {
        var response = await _exampleService.GetWithUnavailable(cancellationToken);
        return Ok(new { Message = response });
    }

    [HttpGet("circuit-breaker")]
    public async Task<ActionResult> GetCircuitBreaker(CancellationToken cancellationToken)
    {
        var response = await _exampleService.GetWithInternalServerError(cancellationToken);
        return Ok(new { Message = response });
    }
}
