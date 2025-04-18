using ePasServices.Data;
using ePasServices.Models;
using ePasServices.Services.Interfaces;
using ePasServices.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ePasServices.Controllers
{
    [ApiController]
    [Route("v1")]
    public class TrxAuditController : ControllerBase
    {
        private readonly ITrxAuditService _auditService;
        private readonly PostgreDbContext _context;
        private readonly IConfiguration _config;

        public TrxAuditController(ITrxAuditService auditService, PostgreDbContext context, IConfiguration config)
        {
            _auditService = auditService;
            _context = context;
            _config = config;
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

        [HttpPost("data-submit")]
        [Authorize]
        public async Task<(bool Success, string Message)> SubmitAuditDataAsync(string username, TrxAuditSubmitRequest request)
        {
            var user = await _context.AppUsers.FirstOrDefaultAsync(x => x.Username == username && x.Status == "ACTIVE");
            if (user == null) return (false, "User tidak ditemukan atau tidak aktif");

            var audit = await _context.TrxAudits.FirstOrDefaultAsync(x => x.Id == request.Id && x.AppUserId == user.Id);
            if (audit == null) return (false, "Audit tidak ditemukan atau bukan milik user ini");

            audit.AuditExecutionDate = DateOnly.FromDateTime(request.AuditExecutionDate);
            audit.AuditMediaUpload = 0;
            audit.AuditMediaTotal = request.AuditMediaTotal;
            audit.Status = "IN_PROGRESS_SUBMIT";

            if (!string.IsNullOrWhiteSpace(request.AuditMomIntro))
            {
                audit.AuditMomIntro = request.AuditMomIntro;
            }

            if (!string.IsNullOrWhiteSpace(request.AuditMomChecklist))
            {
                audit.AuditMomChecklist = request.AuditMomChecklist;
            }

            await _context.SaveChangesAsync();

            if (audit.AuditType == "Mystery Audit" || audit.AuditType == "Regular Audit")
            {
                if (request.Checklist != null && request.Checklist.Any())
                {
                    foreach (var item in request.Checklist)
                    {
                        var checklist = new TrxAuditChecklist
                        {
                            Id = Guid.NewGuid().ToString(),
                            TrxAuditId = audit.Id,
                            MasterQuestionerDetailId = item.QuestionerDetailId,
                            ScoreInput = item.ScoreInput,
                            Comment = item.Comment,
                            CreatedBy = username,
                            CreatedDate = DateTime.UtcNow,
                            UpdatedBy = username,
                            UpdatedDate = DateTime.UtcNow,
                        };
                        _context.TrxAuditChecklists.Add(checklist);
                    }
                }

                if (request.QQ != null && request.QQ.Any())
                {
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
                            CreatedDate = DateTime.UtcNow,
                            UpdatedBy = username,
                            UpdatedDate = DateTime.UtcNow,
                        };
                        _context.TrxAuditQqs.Add(auditQQ);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return (true, "Data audit berhasil disimpan");
        }

        [HttpPost("media-submit")]
        public async Task<(bool Success, string Message)> SubmitAuditMediaAsync(TrxAuditMediaSubmitRequest request, IFormFile file, string username)
        {
            var audit = await _context.TrxAudits.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (audit == null) return (false, "Audit tidak ditemukan");

            // Simpan file
            var uploadsFolder = Path.Combine("wwwroot", "uploads", audit.Id);
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var media = new TrxAuditMedium
            {
                Id = Guid.NewGuid().ToString(),
                TrxAuditId = request.Id,
                Type = request.Type,
                MediaType = request.MediaType,
                MediaPath = $"/uploads/{audit.Id}/{fileName}",
                MasterQuestionerDetailId = request.Type == "MOM" ? null : request.DetailId,
                CreatedBy = username,
                CreatedDate = DateTime.UtcNow,
                UpdatedBy = username,
                UpdatedDate = DateTime.UtcNow,
            };

            _context.TrxAuditMedia.Add(media);

            audit.AuditMediaUpload = (audit.AuditMediaUpload ?? 0) + 1;
            audit.UpdatedBy = username;
            audit.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return (true, "Media berhasil disimpan");
        }

    }



}
