using ePasServices.Data;
using ePasServices.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ePasServices.Controllers
{
    [ApiController]
    [Route("v1/notification")]
    public class NotificationController : ControllerBase
    {
        private readonly ILogger<NotificationController> _logger;
        private readonly PostgreDbContext _context;
        private readonly INotificationService _notificationService;

        public NotificationController(PostgreDbContext context, ILogger<NotificationController> logger, INotificationService notificationService)
        {
            _context = context;
            _logger = logger;
            _notificationService = notificationService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetNotification(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10)
        {
            var username = User.FindFirst("username")?.Value;
            if (string.IsNullOrEmpty(username))
            {
                _logger.LogWarning("Token invalid: username not found in claims");
                return Unauthorized(new ApiResponse("Unauthorized", "Token invalid"));
            }

            var user = await _context.AppUsers
                .FirstOrDefaultAsync(x => x.Username == username && x.Status == "ACTIVE");
            if (user == null)
            {
                _logger.LogWarning("User not found or not active: {Username}", username);
                return NotFound(new ApiResponse("Error", "User tidak ditemukan atau tidak aktif"));
            }

            var (data, total) = await _notificationService.GetNotificationListAsync(page, limit, user.Id);

            if (data == null || !data.Any())
                return NotFound(new ApiResponse("Not Found", "Data tidak ditemukan"));

            return Ok(new
            {
                time = DateTime.Now,
                message = "Success",
                data,
                total,
                page,
                limit
            });
        }

        [HttpPut("{id}/read")]
        [Authorize]
        public async Task<IActionResult> UpdateNotificationStatus(string id)
        {
            var username = User.FindFirst("username")?.Value;
            if (string.IsNullOrEmpty(username))
            {
                _logger.LogWarning("Token invalid: username not found in claims");
                return Unauthorized(new ApiResponse("Unauthorized", "Token invalid"));
            }

            var user = await _context.AppUsers
                .FirstOrDefaultAsync(x => x.Username == username && x.Status == "ACTIVE");
            if (user == null)
            {
                _logger.LogWarning("User not found or not active: {Username}", username);
                return NotFound(new ApiResponse("Error", "User tidak ditemukan atau tidak aktif"));
            }

            var success = await _notificationService.UpdateNotificationStatusAsync(id, "READ", user.Id);

            if (!success)
                _logger.LogWarning("Notification not found or cannot be updated");

            return Ok(new ApiResponse("Success", "Notification status is updated if the ID exists"));
        }
    }
}
