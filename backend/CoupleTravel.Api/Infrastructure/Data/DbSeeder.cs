using CoupleTravel.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoupleTravel.Api.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Users.AnyAsync()) return;

        var user1 = new User
        {
            Id = Guid.Parse("11111111-0000-0000-0000-000000000001"),
            Email = "partner1@example.com",
            DisplayName = "Partner1",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("changeme"),
            CreatedAt = DateTime.UtcNow,
        };
        var user2 = new User
        {
            Id = Guid.Parse("11111111-0000-0000-0000-000000000002"),
            Email = "partne2r@example.com",
            DisplayName = "Partner2",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("changeme"),
            CreatedAt = DateTime.UtcNow,
        };
        var couple = new Couple
        {
            Id = Guid.Parse("22222222-0000-0000-0000-000000000001"),
            Name = "Our Adventure",
            CreatedAt = DateTime.UtcNow,
        };

        db.Users.AddRange(user1, user2);
        db.Couples.Add(couple);
        await db.SaveChangesAsync();

        db.CoupleMembers.AddRange(
            new CoupleMember { CoupleId = couple.Id, UserId = user1.Id },
            new CoupleMember { CoupleId = couple.Id, UserId = user2.Id }
        );
        await db.SaveChangesAsync();
    }
}
