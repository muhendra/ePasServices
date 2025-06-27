using ePasServices.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ePasServices.Controllers
{
    [ApiController]
    [Route("v1/spbu/master-questioner")]
    public class SPBUMasterQuestionerController : ControllerBase
    {
        private readonly IMasterQuestionerService _service;
        public SPBUMasterQuestionerController(IMasterQuestionerService service)
        {
            _service = service;
        }

        [HttpGet("survey")]
        [Authorize]
        public async Task<IActionResult> GetSurvey()
        {
            var username = User.FindFirst("username")?.Value;
            var app = User.FindFirst("app")?.Value;
            if (string.IsNullOrEmpty(username) || !string.Equals(app, "SPBU", StringComparison.OrdinalIgnoreCase))
                return Unauthorized(new ApiResponse("Unauthorized", "Token invalid"));

            var result = await _service.GetLatestSurveyWithDetailsAsync();

            if (result == null)
            {
                return NotFound(new ApiResponse("Not Found", "Data tidak ditemukan"));
            }

            return Ok(new
            {
                time = DateTime.Now,
                message = "Success",
                result,
            });
        }
    }
}
