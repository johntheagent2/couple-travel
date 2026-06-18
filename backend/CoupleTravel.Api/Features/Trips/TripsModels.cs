namespace CoupleTravel.Api.Features.Trips;

public record CreateTripRequest(
    string Title,
    string? Note,
    DateOnly StartDate,
    DateOnly EndDate,
    Guid ClientUuid,
    // City selection: either pick existing city by id, or create from geocode result
    Guid? CityId,
    // When creating a new city from geocode selection:
    string? CityPlaceId,
    string? CityName,
    string? CityCountry,
    double? CityLat,
    double? CityLng,
    string? CityGeocodeSource
);

public record UpdateTripRequest(
    string? Title,
    string? Note,
    DateOnly? StartDate,
    DateOnly? EndDate,
    Guid? CityId,
    Guid? CoverPhotoId
);

public record TripSummary(
    Guid Id,
    string Title,
    string? Note,
    DateOnly StartDate,
    DateOnly EndDate,
    CitySummary City,
    PhotoSummary? CoverPhoto,
    int PhotoCount,
    DateTime CreatedAt
);

public record TripDetail(
    Guid Id,
    string Title,
    string? Note,
    DateOnly StartDate,
    DateOnly EndDate,
    CitySummary City,
    PhotoSummary? CoverPhoto,
    List<PhotoSummary> Photos,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CitySummary(Guid Id, string Name, string Country, double Lat, double Lng);

public record PhotoSummary(Guid Id, string Url, string ThumbUrl, int Width, int Height, string? Caption, int OrderIndex);

public record TimelineResponse(List<TripSummary> Trips, string? NextCursor);
