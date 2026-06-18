using CoupleTravel.Api.Infrastructure.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace CoupleTravel.Api.Features.Trips;

[ApiController]
[Route("v1/trips")]
[ServiceFilter(typeof(RequireAuthFilter))]
public class TripsController(TripsService trips) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTripRequest req, CancellationToken ct)
    {
        var (coupleId, userId) = GetIds();
        var result = await trips.CreateAsync(coupleId, userId, req, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<IActionResult> Timeline([FromQuery] string? cursor, [FromQuery] int limit = 20, CancellationToken ct = default)
    {
        var (coupleId, _) = GetIds();
        return Ok(await trips.GetTimelineAsync(coupleId, cursor, Math.Clamp(limit, 1, 50), ct));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var (coupleId, _) = GetIds();
        var trip = await trips.GetByIdAsync(coupleId, id, ct);
        return trip is null ? NotFound() : Ok(trip);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTripRequest req, CancellationToken ct)
    {
        var (coupleId, _) = GetIds();
        var trip = await trips.UpdateAsync(coupleId, id, req, ct);
        return trip is null ? NotFound() : Ok(trip);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var (coupleId, _) = GetIds();
        return await trips.DeleteAsync(coupleId, id, ct) ? NoContent() : NotFound();
    }

    private (Guid coupleId, Guid userId) GetIds() => (
        Guid.Parse(HttpContext.Session.GetString("coupleId")!),
        Guid.Parse(HttpContext.Session.GetString("userId")!)
    );
}
