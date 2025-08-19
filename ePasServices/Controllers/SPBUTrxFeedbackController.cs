using ePasServices.Data;
using ePasServices.Models;
using ePasServices.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ePasServices.Controllers
{

    [ApiController]
    [Route("v1/spbu/trx-feedback")]
    public class SPBUFeedbackController : ControllerBase
    {
        private readonly ILogger<SPBUFeedbackController> _logger;
        private readonly PostgreDbContext _context;

        private readonly ITrxFeedbackService _feedbackService;

        public SPBUFeedbackController(PostgreDbContext context, ILogger<SPBUFeedbackController> logger, ITrxFeedbackService feedbackService)
        {
            _context = context;
            _logger = logger;
            _feedbackService = feedbackService;
        }

        [HttpGet("count/trx-audit-id/{trxAuditId}/{feedbackType}")]
        [Authorize]
        public async Task<IActionResult> GetFeedbackCountAsync(string trxAuditId, string feedbackType)
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

            // normalize input, only allow certain feedback types
            var allowedTypes = new[] { "COMPLAINT", "BANDING" };
            if (!allowedTypes.Contains(feedbackType.ToUpper()))
            {
                return BadRequest(new ApiResponse("Error", $"Invalid feedback type: {feedbackType}"));
            }

            var count = await _context.TrxFeedbacks
                .CountAsync(x => x.AppUserId == user.Id 
                            && x.TrxAuditId == trxAuditId 
                            && x.FeedbackType == feedbackType.ToUpper());

            return Ok(new { Count = count });
        }

        [HttpPost("data-submit")]
        [Authorize]
        public async Task<IActionResult> SubmitFeedbackDataAsync([FromBody] TrxFeedbackSubmitRequest request)
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

            var audit = await _context.TrxAudits.FirstOrDefaultAsync(x => x.Id == request.TrxAuditId);
            if (audit == null)
            {
                _logger.LogWarning("Audit not found: {AuditId}", request.TrxAuditId);
                return NotFound(new ApiResponse("Error", "Audit tidak ditemukan"));
            }

            // Create Feedback Ticket
            var feedbackId = Guid.NewGuid().ToString();
            var ticketNo = await _feedbackService.GenerateTicketNoAsync(request.TrxAuditId, request.FeedbackType);

            var feedback = new TrxFeedback
            {
                Id = feedbackId,
                TicketNo = ticketNo,
                AppUserId = user.Id,
                TrxAuditId = request.TrxAuditId,
                FeedbackType = request.FeedbackType,
                Status = request.FeedbackType == "COMPLAINT" ? "APPROVE" : "IN_PROGRESS_SUBMIT",
                CreatedBy = username,
                CreatedDate = DateTime.UtcNow.ToLocalTime(),
                UpdatedBy = username,
                UpdatedDate = DateTime.UtcNow.ToLocalTime()
            };
            _context.TrxFeedbacks.Add(feedback);

            if (request.Points != null && request.Points.Any())
            {
                foreach (var point in request.Points)
                {
                    var feedbackPoint = new TrxFeedbackPoint
                    {
                        Id = point.Id,
                        TrxFeedbackId = feedbackId,
                        Description = point.FeedbackDescription,
                        ElementMasterQuestionerDetailId = point.ElementMasterQuestionerDetailId,
                        SubElementMasterQuestionerDetailId = point.SubElementMasterQuestionerDetailId,
                        DetailElementMasterQuestionerDetailId = point.DetailElementMasterQuestionerDetailId,
                        MediaTotal = point.FeedbackMediaTotal,
                        MediaUpload = 0,
                        Status = "ACTIVE",
                        CreatedBy = username,
                        CreatedDate = DateTime.UtcNow.ToLocalTime(),
                        UpdatedBy = username,
                        UpdatedDate = DateTime.UtcNow.ToLocalTime()
                    };

                    _context.TrxFeedbackPoints.Add(feedbackPoint);

                    if (point.PointElements != null && point.PointElements.Any())
                    {
                        foreach (var element in point.PointElements)
                        {
                            var elementEntity = new TrxFeedbackPointElement
                            {
                                Id = Guid.NewGuid().ToString(),
                                TrxFeedbackPointId = point.Id,
                                MasterQuestionerDetailId = element.MasterQuestionerDetailId,
                                Status = "ACTIVE",
                                CreatedBy = username,
                                CreatedDate = DateTime.UtcNow.ToLocalTime(),
                                UpdatedBy = username,
                                UpdatedDate = DateTime.UtcNow.ToLocalTime()
                            };
                            _context.TrxFeedbackPointElements.Add(elementEntity);
                        }
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new ApiResponse("Success", new { Id = feedbackId, Message = "Data berhasil disimpan" }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving feedback data");
                return StatusCode(500, new ApiResponse("Error", "Terjadi kesalahan saat menyimpan data"));
            }
        }

        [HttpPost("media-upload")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFeedbackMediaAsync()
        {
            var form = Request.Form;
            var feedbackPointId = form["TrxFeedbackPointId"].ToString();
            var mediaType = form["MediaType"].ToString();
            var file = form.Files.FirstOrDefault();

            _logger.LogInformation("UploadFeedbackMediaAsync triggered with TrxFeedbackPointId: {FeedbackPointId}", feedbackPointId);
            _logger.LogInformation("UploadFeedbackMediaAsync MediaType: {MediaType}", mediaType);

            if (file == null)
                return BadRequest(new ApiResponse("Error", "File tidak ditemukan"));

            var username = User.FindFirst("username")?.Value;
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new ApiResponse("Unauthorized", "Token invalid"));

            var feedbackPoint = await _context.TrxFeedbackPoints
                .FirstOrDefaultAsync(x => x.Id == feedbackPointId);
            if (feedbackPoint == null)
                return NotFound(new ApiResponse("Error", "Feedback Point tidak ditemukan"));

            var uploadsFolder = Path.Combine("/var/www/epas-asset", "wwwroot", "uploads", "feedback", feedbackPointId);
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            try
            {
                if (file.ContentType.StartsWith("image/"))
                {
                    using var image = await Image.LoadAsync(file.OpenReadStream());
                    image.Mutate(x => x.AutoOrient());
                    await image.SaveAsync(filePath);
                }
                else
                {
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing file upload");
                return BadRequest(new ApiResponse("Error", $"Gagal memproses file: {ex.Message}"));
            }

            var media = new TrxFeedbackPointMedia
            {
                Id = Guid.NewGuid().ToString(),
                TrxFeedbackPointId = feedbackPointId,
                MediaType = mediaType,
                MediaPath = $"/uploads/feedback/{feedbackPointId}/{fileName}",
                Status = "ACTIVE",
                CreatedBy = username,
                CreatedDate = DateTime.UtcNow.ToLocalTime(),
                UpdatedBy = username,
                UpdatedDate = DateTime.UtcNow.ToLocalTime()
            };

            _context.TrxFeedbackPointMedias.Add(media);

            feedbackPoint.MediaUpload = (feedbackPoint.MediaUpload ?? 0) + 1;
            feedbackPoint.UpdatedBy = username;
            feedbackPoint.UpdatedDate = DateTime.UtcNow.ToLocalTime();

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new ApiResponse("Success", "Media berhasil disimpan"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving media to database");
                return StatusCode(500, new ApiResponse("Error", "Terjadi kesalahan saat menyimpan data media"));
            }
        }

        [HttpPost("update-status-under-review")]
        [Authorize]
        public async Task<IActionResult> UpdateFeedbackStatusToUnderReviewAsync([FromBody] TrxFeedbackStatusUpdateRequest request)
        {
            var username = User.FindFirst("username")?.Value;
            if (string.IsNullOrEmpty(username))
            {
                _logger.LogWarning("Token invalid: username not found in claims");
                return Unauthorized(new ApiResponse("Unauthorized", "Token invalid"));
            }

            var feedback = await _context.TrxFeedbacks.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (feedback == null)
            {
                _logger.LogWarning("Feedback not found: {FeedbackId}", request.Id);
                return NotFound(new ApiResponse("Error", "Feedback tidak ditemukan"));
            }

            feedback.Status = "UNDER_REVIEW";
            feedback.UpdatedBy = username;
            feedback.UpdatedDate = DateTime.UtcNow.ToLocalTime();

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new ApiResponse("Success", "Status berhasil diperbarui menjadi UNDER_REVIEW"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating feedback status");
                return StatusCode(500, new ApiResponse("Error", "Terjadi kesalahan saat memperbarui status"));
            }
        }

        [HttpGet("trx-audit-id/{trxAuditId}")]
        [Authorize]
        public async Task<IActionResult> GetTrxFeedback(
            [FromRoute] string trxAuditId, 
            [FromQuery] int page = 1, 
            [FromQuery] int limit = 10,
            [FromQuery] string feedbackType = "")
        {
            var username = User.FindFirst("username")?.Value;
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new ApiResponse("Unauthorized", "Token invalid"));

            var (data, total) = await _feedbackService.GetTrxFeedbackListAsync(page, limit, username, trxAuditId, feedbackType);

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
    }
}
