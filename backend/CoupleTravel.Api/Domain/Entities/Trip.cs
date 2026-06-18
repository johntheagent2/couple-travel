namespace CoupleTravel.Api.Domain.Entities;

public class Trip
{
    public Guid Id { get; set; }
    public Guid CoupleId { get; set; }
    public Couple Couple { get; set; } = null!;

    public Guid CityId { get; set; }
    public City City { get; set; } = null!;

    public string Title { get; set; } = "";
    public string? Note { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public Guid? CoverPhotoId { get; set; }
    public Photo? CoverPhoto { get; set; }

    public Guid CreatedBy { get; set; }
    public Guid ClientUuid { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<Photo> Photos { get; set; } = [];
}
