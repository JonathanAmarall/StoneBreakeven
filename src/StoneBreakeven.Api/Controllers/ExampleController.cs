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
        var response = await _exampleService.GetWithInternalServerError(cancellationToken);
        return Ok(response);
    }

    [HttpGet("timeout")]
    public ActionResult GetTimeout(CancellationToken cancellationToken)
    {
        return Ok();
    }

    [HttpGet("fallback")]
    public ActionResult GetFallback(CancellationToken cancellationToken)
    {
        return Ok();
    }

    [HttpGet("circuit-breaker")]
    public ActionResult GetCircuitBreaker(CancellationToken cancellationToken)
    {
        return Ok();
    }
}
