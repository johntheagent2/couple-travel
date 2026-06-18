namespace CoupleTravel.Api.Features.Cities;

public record CityCandidate(
    string PlaceId,
    string Name,
    string Country,
    string DisplayName,
    double Lat,
    double Lng
);
