using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using ePasServices.ViewModels;

[ApiController]
[Route("v1")]
public class ProfileController : ControllerBase
{
    private readonly IAppUserService _userService;

    public ProfileController(IAppUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var username = User.FindFirst("username")?.Value;
        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized(new ApiResponse("Unauthorized", "Token invalid"));
        }

        var profile = await _userService.GetUserProfileWithSpbuAsync(username);
        if (profile == null)
        {
            return NotFound(new ApiResponse("Not Found", "Data tidak ditemukan"));
        }

        return Ok(new ApiResponse("Success", profile));
    }
}
