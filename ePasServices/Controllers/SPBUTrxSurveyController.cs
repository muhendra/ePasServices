using ePasServices.Data;
using ePasServices.Models;
using ePasServices.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ePasServices.Controllers
{
    [ApiController]
    [Route("v1/spbu/trx-survey")]
    public class SPBUTrxSurveyController : ControllerBase
    {
        private readonly ILogger<SPBUFeedbackController> _logger;
        private readonly PostgreDbContext _context;
        private readonly IMasterQuestionerService _service;
        public SPBUTrxSurveyController(PostgreDbContext context, ILogger<SPBUFeedbackController> logger, IMasterQuestionerService service)
        {
            _context = context;
            _logger = logger;
            _service = service;
        }

        [HttpGet]
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

        [HttpPost("data-submit")]
        [Authorize]
        public async Task<IActionResult> SubmitSurveyDataAsync([FromBody] TrxSurveySubmitRequest request)
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

            var survey = new TrxSurvey
            {
                Id = Guid.NewGuid().ToString(),
                AppUserId = user.Id,
                TrxAuditId = request.TrxAuditId,
                MasterQuestionerId = request.MasterQuestionerId,
                Status = "ACTIVE",
                CreatedBy = username,
                CreatedDate = DateTime.UtcNow.ToLocalTime(),
                UpdatedBy = username,
                UpdatedDate = DateTime.UtcNow.ToLocalTime()
            };
            _context.TrxSurveys.Add(survey);

            if (request.Elements != null && request.Elements.Any())
            {
                foreach (var item in request.Elements)
                {
                    var element = new TrxSurveyElement
                    {
                        Id = Guid.NewGuid().ToString(),
                        TrxSurveyId = survey.Id,
                        MasterQuestionerDetailId = item.MasterQuestionerDetailId,
                        ScoreInput = item.ScoreInput,
                        Status = "ACTIVE",
                        CreatedBy = username,
                        CreatedDate = DateTime.UtcNow.ToLocalTime(),
                        UpdatedBy = username,
                        UpdatedDate = DateTime.UtcNow.ToLocalTime(),
                    };
                    _context.TrxSurveyElements.Add(element);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Survey data saved successfully for SurveyId: {SurveyId}", survey.Id);
                return Ok(new ApiResponse("Success", "Data berhasil disimpan"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving survey data for SurveyId: {SurveyId}", survey.Id);
                return StatusCode(500, new ApiResponse("Error", "Terjadi kesalahan saat menyimpan data"));
            }
        }

        [HttpGet("count/trx-audit-id/{trxAuditId}")]
        [Authorize]
        public async Task<IActionResult> GetSurveyCountAsync(string trxAuditId)
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

            var trxAudit = await _context.TrxAudits
                .FirstOrDefaultAsync(x => x.Id == trxAuditId);
            if (trxAudit == null)
            {
                _logger.LogWarning("TrxAudit not found: {TrxAuditId}", trxAuditId);
                return NotFound(new ApiResponse("Error", "TrxAudit tidak ditemukan"));
            }

            var count = await _context.TrxSurveys
                .CountAsync(x => x.AppUserId == user.Id && x.TrxAuditId == trxAuditId);

            return Ok(new
            {
                Count = count
            });
        }
    }
}
