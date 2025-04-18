using ePasServices.Services.Interfaces;
using ePasServices.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ePasServices.Controllers
{
    [ApiController]
    [Route("v1")]
    public class TrxAuditController : ControllerBase
    {
        private readonly ITrxAuditService _auditService;

        public TrxAuditController(ITrxAuditService auditService)
        {
            _auditService = auditService;
        }

        [HttpGet("trx-audit")]
        [Authorize]
        public async Task<IActionResult> GetTrxAudit([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            var (data, total) = await _auditService.GetTrxAuditListAsync(page, limit);

            if (data == null || !data.Any())
            {
                return NotFound(new ApiResponse("Not Found", "Data tidak ditemukan"));
            }

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

        [HttpPost("trx-audit/start")]
        [Authorize]
        public async Task<IActionResult> StartAudit([FromBody] TrxAuditStartRequest request)
        {
            var username = User.FindFirst("username")?.Value;
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new ApiResponse("Unauthorized", "Token invalid"));

            var result = await _auditService.StartAuditAsync(username, request.Id);
            if (!result.Success)
                return BadRequest(new ApiResponse("Invalid Request", result.Message));

            return Ok(new ApiResponse("Success", result.Message));
        }
    }

}
