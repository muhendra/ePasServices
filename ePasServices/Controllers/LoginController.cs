using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

[ApiController]
[Route("v1")]
public class LoginController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IAppUserService _userService;

    public LoginController(IConfiguration config, IAppUserService userService)
    {
        _config = config;
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var headerKey = Request.Headers["encryption-data"].ToString();
        if (string.IsNullOrEmpty(headerKey))
        {
            return Unauthorized(new ApiResponse("Unauthorized", "Unauthorized [001]"));
        }

        // Generate PBKDF2 hash
        var passwordRaw = request.Username + request.Timestamp + request.Timezone;
        var salt = _config["Auth:MobileSalt"] ?? "SALT";
        var pbkdf2 = new Rfc2898DeriveBytes(passwordRaw, Encoding.UTF8.GetBytes(salt), 4096, HashAlgorithmName.SHA1);
        var hash = Convert.ToHexString(pbkdf2.GetBytes(32)).ToLower();

        if (headerKey != hash)
            return Unauthorized(new ApiResponse("Unauthorized", "Unauthorized [002]"));

        var errors = new Dictionary<string, string>();

        if (string.IsNullOrEmpty(request.Username))
            errors["username"] = "Username wajib diisi";
        if (string.IsNullOrEmpty(request.Password))
            errors["password"] = "Password wajib diisi";
        if (request.Timestamp == 0 || string.IsNullOrEmpty(request.Timezone))
            errors["security-key"] = "SecurityKey wajib diisi";

        if (errors.Count > 0)
            return BadRequest(new ApiResponse("Error Validation", errors));

        var requestTime = DateTimeOffset.FromUnixTimeSeconds(request.Timestamp).LocalDateTime;
        var nowMinus1Hour = DateTime.Now.AddHours(-1);

        if (requestTime < nowMinus1Hour)
            return BadRequest(new ApiResponse("Invalid Request", "Request anda sudah kedaluwarsa"));

        var user = await _userService.GetUserByUsernameAsync(request.Username);
        if (user == null || string.IsNullOrEmpty(user.password_hash) || !BCrypt.Net.BCrypt.Verify(request.Password, user.password_hash))
            return BadRequest(new ApiResponse("Invalid Request", "Username atau password yang anda masukan salah"));

        var jwtKey = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
        var accessToken = TokenHelper.GenerateAccessToken(user.username, user.App, jwtKey);
        var refreshToken = TokenHelper.GenerateRefreshToken(user.username, jwtKey);

        var suffixRefresh = refreshToken[^25..];
        await _userService.UpdateSuffixRefreshToken(user.username, suffixRefresh);

        return Ok(new ApiResponse("Success", new
        {
            accessToken,
            refreshToken
        }));
    }
}