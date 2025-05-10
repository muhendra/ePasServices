using ePasServices.Data;
using ePasServices.Models;
using ePasServices.Services.Interfaces;
using ePasServices.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;


namespace ePasServices.Controllers
{
    [ApiController]
    [Route("v1")]
    public class TrxAuditController : ControllerBase
    {
        private readonly ILogger<TrxAuditController> _logger;
        private readonly ITrxAuditService _auditService;
        private readonly PostgreDbContext _context;
        private readonly IConfiguration _config;

        public TrxAuditController(ITrxAuditService auditService, PostgreDbContext context, IConfiguration config, ILogger<TrxAuditController> logger)
        {
            _auditService = auditService;
            _context = context;
            _config = config;
            _logger = logger;
        }

        [HttpGet("trx-audit")]
        [Authorize]
        public async Task<IActionResult> GetTrxAudit([FromQuery] int page = 1, [FromQuery] int limit = 10, [FromQuery] bool history = false)
        {
            var username = User.FindFirst("username")?.Value;
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new ApiResponse("Unauthorized", "Token invalid"));

            var (data, total) = await _auditService.GetTrxAuditListAsync(page, limit, history, username);

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

        [HttpGet("trx-audit/count-in-progress")]
        [Authorize]
        public async Task<IActionResult> GetCountInProgress()
        {
            var username = User.FindFirst("username")?.Value;
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new ApiResponse("Unauthorized", "Token invalid"));

            try
            {
                var count = await _auditService.CountInProgressAsync(username);
                return Ok(new
                {
                    Success = true,
                    Count = count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Terjadi kesalahan pada server.",
                    Detail = ex.Message
                });
            }
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

        [HttpPost("trx-audit/data-submit")]
        [Authorize]
        public async Task<IActionResult> SubmitAuditDataAsync([FromBody] TrxAuditSubmitRequest request)
        {
            _logger.LogInformation("SubmitAuditDataAsync triggered with AuditId: {AuditId}", request.Id);

            var username = User.FindFirst("username")?.Value;
            if (string.IsNullOrEmpty(username))
            {
                _logger.LogWarning("Token invalid: username not found in claims");
                return Unauthorized(new ApiResponse("Unauthorized", "Token invalid"));
            }

            _logger.LogInformation("Authenticated user: {Username}", username);

            var user = await _context.AppUsers
                .FirstOrDefaultAsync(x => x.Username == username && x.Status == "ACTIVE");
            if (user == null)
            {
                _logger.LogWarning("User not found or not active: {Username}", username);
                return NotFound(new ApiResponse("Error", "User tidak ditemukan atau tidak aktif"));
            }

            var audit = await _context.TrxAudits
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.AppUserId == user.Id);
            if (audit == null)
            {
                _logger.LogWarning("Audit not found or not owned by user: AuditId={AuditId}, UserId={UserId}", request.Id, user.Id);
                return NotFound(new ApiResponse("Error", "Audit tidak ditemukan atau bukan milik user ini"));
            }

            // Update basic audit data
            _logger.LogInformation("Updating audit basic info for AuditId: {AuditId}", audit.Id);
            audit.AuditExecutionTime = request.AuditExecutionDate;
            audit.AuditMediaUpload = 0;
            audit.AuditMediaTotal = request.AuditMediaTotal;
            audit.Status = "IN_PROGRESS_SUBMIT";
            audit.UpdatedDate = DateTime.Now;
            audit.UpdatedBy = username;
            audit.AuditMomIntro = request.AuditMomIntro;
            audit.AuditMomFinal = request.AuditMomFinal;

            // Hapus data lama
            var existingChecklist = _context.TrxAuditChecklists.Where(x => x.TrxAuditId == audit.Id);
            _context.TrxAuditChecklists.RemoveRange(existingChecklist);
            _logger.LogInformation("Existing checklist removed: {Count}", existingChecklist.Count());

            var existingQQ = _context.TrxAuditQqs.Where(x => x.TrxAuditId == audit.Id);
            _context.TrxAuditQqs.RemoveRange(existingQQ);
            _logger.LogInformation("Existing QQ removed: {Count}", existingQQ.Count());

            var existingMedia = _context.TrxAuditMedia.Where(x => x.TrxAuditId == audit.Id);
            _context.TrxAuditMedia.RemoveRange(existingMedia);
            _logger.LogInformation("Existing media removed: {Count}", existingMedia.Count());

            // Simpan checklist
            if (request.Checklist != null && request.Checklist.Any())
            {
                _logger.LogInformation("Saving new checklist items: {Count}", request.Checklist.Count);
                foreach (var item in request.Checklist)
                {
                    var checklist = new TrxAuditChecklist
                    {
                        Id = Guid.NewGuid().ToString(),
                        TrxAuditId = audit.Id,
                        MasterQuestionerDetailId = item.QuestionerDetailId,
                        ScoreInput = item.ScoreInput,
                        Comment = item.Comment,
                        Status = "ok",
                        CreatedBy = username,
                        CreatedDate = DateTime.UtcNow.ToLocalTime(),
                        UpdatedBy = username,
                        UpdatedDate = DateTime.UtcNow.ToLocalTime(),
                    };
                    _context.TrxAuditChecklists.Add(checklist);
                }
            }

            // Simpan QQ
            if (request.QQ != null && request.QQ.Any())
            {
                _logger.LogInformation("Saving new QQ items: {Count}", request.QQ.Count);
                foreach (var qq in request.QQ)
                {
                    var auditQQ = new TrxAuditQq
                    {
                        Id = Guid.NewGuid().ToString(),
                        TrxAuditId = audit.Id,
                        NozzleNumber = qq.NozzleNumber,
                        DuMake = qq.DuMake,
                        DuSerialNo = qq.DuSerialNo,
                        Product = qq.Product,
                        Mode = qq.Mode,
                        QuantityVariationWithMeasure = qq.QuantityVariationWithMeasure,
                        QuantityVariationInPercentage = qq.QuantityVariationInPercentage,
                        ObservedDensity = qq.ObservedDensity,
                        ObservedTemp = qq.ObservedTemp,
                        ObservedDensity15Degree = qq.ObservedDensity15Degree,
                        ReferenceDensity15Degree = qq.ReferenceDensity15Degree,
                        TankNumber = qq.TankNumber,
                        DensityVariation = qq.DensityVariation,
                        CreatedBy = username,
                        Status = "ok",
                        CreatedDate = DateTime.UtcNow.ToLocalTime(),
                        UpdatedBy = username,
                        UpdatedDate = DateTime.UtcNow.ToLocalTime(),
                    };
                    _context.TrxAuditQqs.Add(auditQQ);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Audit data saved successfully for AuditId: {AuditId}", audit.Id);
                return Ok(new ApiResponse("Success", "Data berhasil disimpan"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving audit data for AuditId: {AuditId}", audit.Id);
                return StatusCode(500, new ApiResponse("Error", "Terjadi kesalahan saat menyimpan data"));
            }
        }


        [HttpPost("trx-audit/media-submit")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SubmitAuditMediaAsync()
        {
            var form = Request.Form;

            var id = form["Id"].ToString();
            var type = form["Type"].ToString();
            var mediaType = form["MediaType"].ToString();
            var detailId = form.ContainsKey("DetailId") ? form["DetailId"].ToString() : null;
            var file = form.Files.FirstOrDefault();

            if (file == null)
                return BadRequest(new ApiResponse("Error", "File tidak ditemukan"));

            var username = User.FindFirst("username")?.Value;
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new ApiResponse("Unauthorized", "Token invalid"));

            var audit = await _context.TrxAudits.FirstOrDefaultAsync(x => x.Id == id);
            if (audit == null)
                return NotFound(new ApiResponse("Error", "Audit tidak ditemukan"));

            var uploadsFolder = Path.Combine("wwwroot", "uploads", id);
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
                return BadRequest(new ApiResponse("Error", $"Gagal memproses file: {ex.Message}"));
            }

            // Validasi MasterQuestionerDetailId
            string? masterQuestionerDetailId = null;
            if (type != "FINAL")
            {
                masterQuestionerDetailId = detailId;
            }

            var media = new TrxAuditMedium
            {
                Id = Guid.NewGuid().ToString(),
                TrxAuditId = id,
                Type = type,
                MediaType = mediaType,
                MediaPath = $"/uploads/{id}/{fileName}",
                MasterQuestionerDetailId = masterQuestionerDetailId,
                Status = "ok",
                CreatedBy = username,
                CreatedDate = DateTime.UtcNow.ToLocalTime(),
                UpdatedBy = username,
                UpdatedDate = DateTime.UtcNow.ToLocalTime(),
            };

            _context.TrxAuditMedia.Add(media);

            audit.AuditMediaUpload = (audit.AuditMediaUpload ?? 0) + 1;
            if (audit.AuditMediaUpload == audit.AuditMediaTotal)
            {
                audit.Status = "UNDER_REVIEW";
            }
            audit.UpdatedBy = username;
            audit.UpdatedDate = DateTime.UtcNow.ToLocalTime();

            await _context.SaveChangesAsync();
            return Ok(new ApiResponse("Success", "Media berhasil disimpan"));
        }


    }
}
