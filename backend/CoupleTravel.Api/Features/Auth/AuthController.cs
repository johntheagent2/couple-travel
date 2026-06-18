using Microsoft.AspNetCore.Mvc;

namespace CoupleTravel.Api.Features.Auth;

[ApiController]
[Route("v1/auth")]
public class AuthController(AuthService auth) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
    {
        var result = await auth.ValidateAsync(req.Email, req.Password, ct);
        if (result is null) return Unauthorized(new { error = "Invalid email or password" });

        var (user, coupleId) = result.Value;
        HttpContext.Session.SetString("userId", user.Id.ToString());
        HttpContext.Session.SetString("coupleId", coupleId.ToString());

        return Ok(new MeResponse(user.Id, user.Email, user.DisplayName, user.AvatarUrl, coupleId));
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return NoContent();
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var userIdStr = HttpContext.Session.GetString("userId");
        if (userIdStr is null) return Unauthorized();

        var me = await auth.GetMeAsync(Guid.Parse(userIdStr), ct);
        return me is null ? Unauthorized() : Ok(me);
    }
}
