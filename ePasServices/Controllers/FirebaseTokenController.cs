using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ePasServices.ViewModels;

[ApiController]
[Route("v1")]
public class FirebaseTokenController : ControllerBase
{
    private readonly IAppUserService _userService;

    public FirebaseTokenController(IAppUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("update-firebase-token")]
    [Authorize]
    public async Task<IActionResult> UpdateFirebaseToken([FromBody] UpdateFirebaseTokenRequest request)
    {
        var username = User.FindFirst("username")?.Value;
        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized(new ApiResponse("Unauthorized", "Token invalid"));
        }

        var success = await _userService.UpdateNotificationTokenAsync(username, request.NotificationToken);
        if (!success)
        {
            return StatusCode(500, new ApiResponse("Failed", "Gagal menyimpan Firebase token"));
        }

        return Ok(new ApiResponse("Success", "Firebase token berhasil diperbarui"));
    }
}