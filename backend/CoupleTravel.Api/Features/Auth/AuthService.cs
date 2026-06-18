using CoupleTravel.Api.Domain.Entities;
using CoupleTravel.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CoupleTravel.Api.Features.Auth;

public class AuthService(AppDbContext db)
{
    public async Task<(User user, Guid coupleId)?> ValidateAsync(string email, string password, CancellationToken ct)
    {
        var user = await db.Users
            .Include(u => u.CoupleMembers)
            .FirstOrDefaultAsync(u => u.Email == email.ToLower(), ct);

        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        var coupleId = user.CoupleMembers.FirstOrDefault()?.CoupleId ?? Guid.Empty;
        return (user, coupleId);
    }

    public async Task<MeResponse?> GetMeAsync(Guid userId, CancellationToken ct)
    {
        var user = await db.Users
            .Include(u => u.CoupleMembers)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user is null) return null;
        var coupleId = user.CoupleMembers.FirstOrDefault()?.CoupleId ?? Guid.Empty;
        return new MeResponse(user.Id, user.Email, user.DisplayName, user.AvatarUrl, coupleId);
    }
}
