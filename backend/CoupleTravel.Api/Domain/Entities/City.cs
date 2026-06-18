using NetTopologySuite.Geometries;

namespace CoupleTravel.Api.Domain.Entities;

public class City
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Country { get; set; } = "";
    public Point Location { get; set; } = null!;
    public string GeocodeSource { get; set; } = "nominatim";
    public string? GeocodePlaceId { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Trip> Trips { get; set; } = [];
}
