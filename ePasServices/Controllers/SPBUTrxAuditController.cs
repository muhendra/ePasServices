using ePasServices.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ePasServices.Controllers
{
    [ApiController]
    [Route("v1/spbu/trx-audit")]
    public class SPBUTrxAuditController : ControllerBase
    {
        private readonly ITrxAuditService _auditService;

        public SPBUTrxAuditController(ITrxAuditService auditService)
        {
            _auditService = auditService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetTrxAudit(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            [FromQuery] bool history = false)
        {
            if (!IsAuthorized(out var username))
                return Unauthorized(new ApiResponse("Unauthorized", "Token invalid"));

            var (data, total) = await _auditService.GetTrxAuditListForSPBUAsync(page, limit, history, username);

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

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetTrxAuditDetail(string id)
        {
            if (!IsAuthorized(out var username))
                return Unauthorized(new ApiResponse("Unauthorized", "Token invalid"));

            var result = await _auditService.GetTrxAuditDetailForSPBUAsync(id, username);

            if (result == null)
                return NotFound(new ApiResponse("Not Found", "Data tidak ditemukan"));

            return Ok(new
            {
                time = DateTime.Now,
                message = "Success",
                result
            });
        }

        [HttpGet("detail-list/trx-audit-id/{trxAuditId}/parent-id/{parentId}")]
        [Authorize]
        public async Task<IActionResult> GetTrxAuditDetailList(string trxAuditId, string parentId)
        {
            if (!IsAuthorized(out _))
                return Unauthorized(new ApiResponse("Unauthorized", "Token invalid"));

            var result = await _auditService.GetDetailsByTrxAuditIdAndParentIdAsync(trxAuditId, parentId);

            return Ok(new
            {
                time = DateTime.Now,
                message = "Success",
                result
            });
        }

        private bool IsAuthorized(out string username)
        {
            username = User.FindFirst("username")?.Value;
            var app = User.FindFirst("app")?.Value;

            return !string.IsNullOrEmpty(username) &&
                string.Equals(app, "SPBU", StringComparison.OrdinalIgnoreCase);
        }
    }
}
