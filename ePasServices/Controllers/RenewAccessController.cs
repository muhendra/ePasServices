using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using ePasServices.ViewModels;

[ApiController]
[Route("v1")]
public class RenewAccessController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IAppUserService _userService;

    public RenewAccessController(IConfiguration config, IAppUserService userService)
    {
        _config = config;
        _userService = userService;
    }

    [HttpPost("renew-access")]
    public async Task<IActionResult> RenewAccess()
    {
        string? token = Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
        if (string.IsNullOrEmpty(token))
            return Unauthorized(new ApiResponse("Unauthorized - Invalid Token", "Token tidak ditemukan"));

        // Decode refresh token
        var claims = TokenHelper.DecodeToken(token);
        if (claims == null || !claims.TryGetValue("username", out var username))
            return Unauthorized(new ApiResponse("Unauthorized - Invalid Refresh Token", "Gagal membaca token"));

        string suffixRefreshToken = token[^25..];
        var refreshInfo = await _userService.GetSuffixRefreshTokenInfoAsync(username);

        if (refreshInfo == null)
            return StatusCode(500, new ApiResponse("Error generate token [001]", "User tidak ditemukan"));

        if (refreshInfo.SuffixRefreshToken != suffixRefreshToken)
            return Unauthorized(new ApiResponse("Your account has been used by someone else", null));

        var jwtKey = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
        var newAccessToken = TokenHelper.GenerateAccessToken(username, refreshInfo.App, jwtKey);

        return Ok(new ApiResponse("Success", new
        {
            accessToken = newAccessToken
        }));
    }
}