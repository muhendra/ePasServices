using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ePasServices.ViewModels;

[ApiController]
[Route("v1")]
public class ChangePasswordController : ControllerBase
{
    private readonly IAppUserService _userService;

    public ChangePasswordController(IAppUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("update-password")]
    [Authorize]
    public async Task<IActionResult> UpdatePassword([FromBody] ChangePasswordRequest request)
    {
        var username = User.FindFirst("username")?.Value;
        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized(new ApiResponse("Unauthorized", "Token invalid"));
        }

        var user = await _userService.GetUserByUsernameAsync(username);
        if (user == null)
        {
            return NotFound(new ApiResponse("Not Found", "User tidak ditemukan atau tidak aktif"));
        }

        if (string.IsNullOrEmpty(user.password_hash) || !BCrypt.Net.BCrypt.Verify(request.OldPassword, user.password_hash))
        {
            return BadRequest(new ApiResponse("Invalid Request", "Password lama salah"));
        }

        var success = await _userService.ChangePasswordAsync(username, request);
        if (!success)
        {
            return StatusCode(500, new ApiResponse("Failed", "Gagal mengubah password"));
        }

        return Ok(new ApiResponse("Success", "Password berhasil diperbarui"));
    }
}
