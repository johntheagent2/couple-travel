using CoupleTravel.Api.Infrastructure.Data;
using CoupleTravel.Api.Infrastructure.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoupleTravel.Api.Features.Map;

public record CityPin(Guid CityId, string Name, string Country, double Lat, double Lng, int TripCount);

[ApiController]
[Route("v1/map")]
[ServiceFilter(typeof(RequireAuthFilter))]
public class MapController(AppDbContext db) : ControllerBase
{
    [HttpGet("cities")]
    public async Task<IActionResult> Cities(CancellationToken ct)
    {
        var coupleId = Guid.Parse(HttpContext.Session.GetString("coupleId")!);

        var pins = await db.Trips
            .Where(t => t.CoupleId == coupleId && t.DeletedAt == null)
            .Include(t => t.City)
            .GroupBy(t => t.City)
            .Select(g => new CityPin(
                g.Key.Id,
                g.Key.Name,
                g.Key.Country,
                g.Key.Location.Y,
                g.Key.Location.X,
                g.Count()
            ))
            .ToListAsync(ct);

        return Ok(pins);
    }
}
