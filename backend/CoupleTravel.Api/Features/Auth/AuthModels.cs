namespace CoupleTravel.Api.Features.Auth;

public record LoginRequest(string Email, string Password);

public record MeResponse(Guid Id, string Email, string DisplayName, string? AvatarUrl, Guid CoupleId);
