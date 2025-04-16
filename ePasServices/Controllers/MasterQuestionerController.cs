using ePasServices.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ePasServices.Controllers
{
    [ApiController]
    [Route("v1/master-questioner")]
    public class MasterQuestionerController : ControllerBase
    {
        private readonly IMasterQuestionerService _service;
        private readonly IAppUserService _appUserService;

        public MasterQuestionerController(IMasterQuestionerService service, IAppUserService appUserService)
        {
            _service = service;
            _appUserService = appUserService;
        }

        [HttpGet("regular")]
        public async Task<IActionResult> GetRegularQuestioner()
        {
            var username = User.FindFirst("username")?.Value;
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new ApiResponse("Unauthorized", "Token invalid"));

            var appUserId = await _appUserService.GetAppUserIdByUsernameAsync(username);
            var result = await _service.GetMasterQuestionerByUserAsync(appUserId, username);

            if (result == null)
                return NotFound(new { message = "Data not found" });

            return Ok(new
            {
                time = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7)),
                message = "Success",
                data = result
            });
        }

        [HttpGet("mystery")]
        public async Task<IActionResult> GetMysteryQuestioner()
        {
            var username = User.FindFirst("username")?.Value;
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new ApiResponse("Unauthorized", "Token invalid"));

            var appUserId = await _appUserService.GetAppUserIdByUsernameAsync(username);
            var result = await _service.GetMasterQuestionerMysteryByUserAsync(appUserId, username);

            if (result == null)
                return NotFound(new { message = "Data not found" });

            return Ok(new
            {
                time = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7)),
                message = "Success",
                data = result
            });
        }
    }

}
