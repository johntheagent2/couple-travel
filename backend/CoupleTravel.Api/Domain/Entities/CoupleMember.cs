namespace CoupleTravel.Api.Domain.Entities;

public class CoupleMember
{
    public Guid CoupleId { get; set; }
    public Couple Couple { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
