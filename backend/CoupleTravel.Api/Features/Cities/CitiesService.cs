using CoupleTravel.Api.Domain.Entities;
using CoupleTravel.Api.Infrastructure.Data;
using CoupleTravel.Api.Infrastructure.External;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace CoupleTravel.Api.Features.Cities;

public class CitiesService(AppDbContext db, NominatimClient nominatim)
{
    private static readonly GeometryFactory Gf = new(new PrecisionModel(), 4326);

    public async Task<List<CityCandidate>> SearchAsync(string query, CancellationToken ct)
    {
        var results = await nominatim.SearchAsync(query, ct);
        return results.Select(r =>
        {
            var name = r.Address.City ?? r.Address.Town ?? r.Address.Village ?? r.DisplayName.Split(',')[0].Trim();
            var country = r.Address.Country ?? "";
            return new CityCandidate(r.PlaceId.ToString(), name, country, r.DisplayName, r.Lat, r.Lon);
        }).ToList();
    }

    public async Task<City> GetOrCreateAsync(string placeId, string name, string country, double lat, double lng, string source, CancellationToken ct)
    {
        if (source == "nominatim")
        {
            var existing = await db.Cities.FirstOrDefaultAsync(c => c.GeocodePlaceId == placeId, ct);
            if (existing is not null) return existing;
        }

        var city = new City
        {
            Name = name,
            Country = country,
            Location = Gf.CreatePoint(new Coordinate(lng, lat)),
            GeocodeSource = source,
            GeocodePlaceId = source == "nominatim" ? placeId : null,
            CreatedAt = DateTime.UtcNow,
        };
        db.Cities.Add(city);
        await db.SaveChangesAsync(ct);
        return city;
    }
}
