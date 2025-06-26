using ePasServices.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ePasServices.Controllers
{
    [ApiController]
    [Route("v1/spbu")]
    public class SPBUTrxAuditController : ControllerBase
    {
        private readonly ITrxAuditService _auditService;
        public SPBUTrxAuditController(ITrxAuditService auditService)
        {
            _auditService = auditService;
        }

        [HttpGet("trx-audit")]
        [Authorize]
        public async Task<IActionResult> GetTrxAudit([FromQuery] int page = 1, [FromQuery] int limit = 10, [FromQuery] bool history = false)
        {
            var username = User.FindFirst("username")?.Value;
            var app = User.FindFirst("app")?.Value;
            if (string.IsNullOrEmpty(username) || !string.Equals(app, "SPBU", StringComparison.OrdinalIgnoreCase))
                return Unauthorized(new ApiResponse("Unauthorized", "Token invalid"));

            var (data, total) = await _auditService.GetTrxAuditListForSPBUAsync(page, limit, history, username);

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

        [HttpGet("trx-audit/{id}")]
        [Authorize]
        public async Task<IActionResult> GetTrxAuditDetail(string id)
        {
            var username = User.FindFirst("username")?.Value;
            var app = User.FindFirst("app")?.Value;
            if (string.IsNullOrEmpty(username) || !string.Equals(app, "SPBU", StringComparison.OrdinalIgnoreCase))
                return Unauthorized(new ApiResponse("Unauthorized", "Token invalid"));

            var result = await _auditService.GetTrxAuditDetailForSPBUAsync(id, username);

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
