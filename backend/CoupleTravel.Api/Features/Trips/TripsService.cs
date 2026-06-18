using CoupleTravel.Api.Domain.Entities;
using CoupleTravel.Api.Features.Cities;
using CoupleTravel.Api.Infrastructure.Data;
using CoupleTravel.Api.Infrastructure.External;
using Microsoft.EntityFrameworkCore;

namespace CoupleTravel.Api.Features.Trips;

public class TripsService(AppDbContext db, CitiesService cities, IBlobStorage blob)
{
    public async Task<TripDetail> CreateAsync(Guid coupleId, Guid userId, CreateTripRequest req, CancellationToken ct)
    {
        var existing = await db.Trips
            .Include(t => t.City)
            .Include(t => t.Photos)
            .FirstOrDefaultAsync(t => t.ClientUuid == req.ClientUuid && t.CoupleId == coupleId, ct);
        if (existing is not null) return MapDetail(existing, blob);

        Guid cityId;
        if (req.CityId.HasValue)
        {
            cityId = req.CityId.Value;
        }
        else if (req.CityName is not null && req.CityLat.HasValue && req.CityLng.HasValue)
        {
            var city = await cities.GetOrCreateAsync(
                req.CityPlaceId ?? Guid.NewGuid().ToString(),
                req.CityName, req.CityCountry ?? "",
                req.CityLat.Value, req.CityLng.Value,
                req.CityGeocodeSource ?? "nominatim", ct);
            cityId = city.Id;
        }
        else
        {
            throw new ArgumentException("Must provide CityId or city geocode details");
        }

        var trip = new Trip
        {
            CoupleId = coupleId,
            CityId = cityId,
            Title = req.Title,
            Note = req.Note,
            StartDate = req.StartDate,
            EndDate = req.EndDate,
            CreatedBy = userId,
            ClientUuid = req.ClientUuid,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        db.Trips.Add(trip);
        await db.SaveChangesAsync(ct);

        await db.Entry(trip).Reference(t => t.City).LoadAsync(ct);
        return MapDetail(trip, blob);
    }

    public async Task<TimelineResponse> GetTimelineAsync(Guid coupleId, string? cursor, int limit, CancellationToken ct)
    {
        var query = db.Trips
            .Where(t => t.CoupleId == coupleId && t.DeletedAt == null)
            .Include(t => t.City)
            .Include(t => t.CoverPhoto)
            .Include(t => t.Photos.Where(p => p.DeletedAt == null))
            .OrderByDescending(t => t.StartDate)
            .ThenByDescending(t => t.CreatedAt)
            .AsQueryable();

        if (cursor is not null && DateTime.TryParse(cursor, out var cursorDate))
            query = query.Where(t => t.StartDate < DateOnly.FromDateTime(cursorDate));

        var trips = await query.Take(limit + 1).ToListAsync(ct);
        string? nextCursor = null;
        if (trips.Count > limit)
        {
            trips.RemoveAt(trips.Count - 1);
            nextCursor = trips.Last().StartDate.ToString("O");
        }

        return new TimelineResponse(trips.Select(t => MapSummary(t, blob)).ToList(), nextCursor);
    }

    public async Task<TripDetail?> GetByIdAsync(Guid coupleId, Guid id, CancellationToken ct)
    {
        var trip = await db.Trips
            .Where(t => t.Id == id && t.CoupleId == coupleId && t.DeletedAt == null)
            .Include(t => t.City)
            .Include(t => t.CoverPhoto)
            .Include(t => t.Photos.Where(p => p.DeletedAt == null))
            .FirstOrDefaultAsync(ct);
        return trip is null ? null : MapDetail(trip, blob);
    }

    public async Task<TripDetail?> UpdateAsync(Guid coupleId, Guid id, UpdateTripRequest req, CancellationToken ct)
    {
        var trip = await db.Trips
            .Include(t => t.City)
            .Include(t => t.CoverPhoto)
            .Include(t => t.Photos.Where(p => p.DeletedAt == null))
            .FirstOrDefaultAsync(t => t.Id == id && t.CoupleId == coupleId && t.DeletedAt == null, ct);
        if (trip is null) return null;

        if (req.Title is not null) trip.Title = req.Title;
        if (req.Note is not null) trip.Note = req.Note;
        if (req.StartDate.HasValue) trip.StartDate = req.StartDate.Value;
        if (req.EndDate.HasValue) trip.EndDate = req.EndDate.Value;
        if (req.CityId.HasValue) trip.CityId = req.CityId.Value;
        if (req.CoverPhotoId.HasValue) trip.CoverPhotoId = req.CoverPhotoId.Value;
        trip.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
        await db.Entry(trip).Reference(t => t.City).LoadAsync(ct);
        return MapDetail(trip, blob);
    }

    public async Task<bool> DeleteAsync(Guid coupleId, Guid id, CancellationToken ct)
    {
        var trip = await db.Trips.FirstOrDefaultAsync(t => t.Id == id && t.CoupleId == coupleId && t.DeletedAt == null, ct);
        if (trip is null) return false;
        trip.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static TripSummary MapSummary(Trip t, IBlobStorage blob) => new(
        t.Id, t.Title, t.Note, t.StartDate, t.EndDate,
        MapCity(t.City),
        t.CoverPhoto is not null ? MapPhoto(t.CoverPhoto, blob) : null,
        t.Photos.Count(p => p.DeletedAt == null),
        t.CreatedAt
    );

    private static TripDetail MapDetail(Trip t, IBlobStorage blob) => new(
        t.Id, t.Title, t.Note, t.StartDate, t.EndDate,
        MapCity(t.City),
        t.CoverPhoto is not null ? MapPhoto(t.CoverPhoto, blob) : null,
        t.Photos.Where(p => p.DeletedAt == null).OrderBy(p => p.OrderIndex).Select(p => MapPhoto(p, blob)).ToList(),
        t.CreatedAt, t.UpdatedAt
    );

    private static CitySummary MapCity(City c) => new(c.Id, c.Name, c.Country, c.Location.Y, c.Location.X);

    private static PhotoSummary MapPhoto(Photo p, IBlobStorage blob) => new(
        p.Id, blob.GetPublicUrl(p.ObjectKey), blob.GetPublicUrl(p.ThumbKey),
        p.Width, p.Height, p.Caption, p.OrderIndex
    );
}
