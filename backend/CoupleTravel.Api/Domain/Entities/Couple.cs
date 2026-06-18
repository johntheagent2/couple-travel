namespace CoupleTravel.Api.Domain.Entities;

public class Couple
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public DateOnly? AnniversaryDate { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<CoupleMember> Members { get; set; } = [];
    public ICollection<Trip> Trips { get; set; } = [];
}
