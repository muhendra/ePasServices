using Microsoft.AspNetCore.Mvc;
using ePasServices.Services.Interfaces;
using ePasServices.ViewModels;
using Microsoft.AspNetCore.Identity.Data;

[ApiController]
[Route("v1")]
public class ForgotPasswordController : ControllerBase
{
    private readonly IAppUserService _userService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;

    public ForgotPasswordController(IAppUserService userService, IEmailService emailService, IConfiguration config)
    {
        _userService = userService;
        _emailService = emailService;
        _config = config;
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ePasServices.ViewModels.ForgotPasswordRequest request)
    {
        var user = await _userService.GetUserByEmailAsync(request.Email);
        if (user == null)
        {
            return NotFound(new ApiResponse("Not Found", "Email tidak ditemukan"));
        }

        // generate reset token Nanti
        //var token = TokenHelper.GenerateResetToken(user.Username, _config["Jwt:Key"]);
        //
        //var resetLink = $"{_config["AppSettings:ResetPasswordUrl"]}?token={token}";
        //var body = $"Klik link berikut untuk reset password (berlaku 30 menit): <a href=\"{resetLink}\">Reset Password</a>";

        //SETUP SMTP Nanti
        //await _emailService.SendEmailAsync(user.Email, "Reset Password", body);

        return Ok(new
        {
            message = "Success",
            status = "email forgot password sent to email"
        });
    }
}
