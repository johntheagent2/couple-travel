namespace CoupleTravel.Api.Domain.Entities;

public class Photo
{
    public Guid Id { get; set; }
    public Guid TripId { get; set; }
    public Trip Trip { get; set; } = null!;

    public Guid CoupleId { get; set; }

    public string ObjectKey { get; set; } = "";
    public string ThumbKey { get; set; } = "";
    public int Width { get; set; }
    public int Height { get; set; }
    public string? Caption { get; set; }
    public DateTime? TakenAt { get; set; }
    public int OrderIndex { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
