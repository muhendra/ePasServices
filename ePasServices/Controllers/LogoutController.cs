using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("v1")]
public class LogoutController : ControllerBase
{
    private readonly IAppUserService _userService;

    public LogoutController(IAppUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var username = User.FindFirst("username")?.Value;
        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized(new ApiResponse("Unauthorized", "Token invalid"));
        }

        await _userService.ClearRefreshTokenAsync(username);

        return Ok(new ApiResponse("Success", null));
    }
}
