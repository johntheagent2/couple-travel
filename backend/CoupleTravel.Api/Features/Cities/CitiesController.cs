using CoupleTravel.Api.Infrastructure.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace CoupleTravel.Api.Features.Cities;

[ApiController]
[Route("v1/cities")]
[ServiceFilter(typeof(RequireAuthFilter))]
public class CitiesController(CitiesService cities) : ControllerBase
{
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            return BadRequest(new { error = "Query must be at least 2 characters" });

        var results = await cities.SearchAsync(q, ct);
        return Ok(results);
    }
}
