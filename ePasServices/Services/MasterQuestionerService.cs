using ePasServices.Data;
using ePasServices.Models;
using ePasServices.Services.Interfaces;
using ePasServices.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Dapper;
using Npgsql;

namespace ePasServices.Services
{
    public class MasterQuestionerService : IMasterQuestionerService
    {
        private readonly PostgreDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly NpgsqlConnection _conn;

        public MasterQuestionerService(PostgreDbContext context, IMemoryCache cache, IConfiguration config)
        {
            _context = context;
            _cache = cache;
            _conn = new NpgsqlConnection(config.GetConnectionString("DefaultConnection"));
        }

        public async Task<MasterQuestionerResponse?> GetMasterQuestionerByUserAsync(string id, string username)
        {
            var appUser = await _context.AppUsers.FirstOrDefaultAsync(x => x.Username == username && x.Status == "ACTIVE");
            if (appUser == null) return null;

            var audit = await _context.TrxAudits.FirstOrDefaultAsync(x => x.AppUserId == appUser.Id && x.Status == "ACTIVE");
            if (audit == null || audit.MasterQuestionerId == null) return null;

            var versionKey = $"master_questioner_{audit.MasterQuestionerId}";

            if (_cache.TryGetValue(versionKey, out MasterQuestionerResponse cachedData))
            {
                return cachedData;
            }

            var header = await _context.MasterQuestioners.FirstOrDefaultAsync(x => x.Id == audit.MasterQuestionerId && x.Type == "REGULAR");
            if (header == null) return null;

            var details = await _context.MasterQuestionerDetails
                .Where(x => x.MasterQuestionerId == header.Id && x.Status == "ACTIVE")
                .ToListAsync();

            var tree = BuildTree(details);

            var result = new MasterQuestionerResponse
            {
                Version = header.Version,
                Detail = tree
            };

            _cache.Set(versionKey, result, TimeSpan.FromMinutes(30));

            return result;
        }

        public async Task<MasterQuestionerResponse?> GetMasterQuestionerMysteryByUserAsync(string id, string username)
        {
            var appUser = await _context.AppUsers.FirstOrDefaultAsync(x => x.Username == username && x.Status == "ACTIVE");
            if (appUser == null) return null;

            var audit = await _context.TrxAudits.FirstOrDefaultAsync(x => x.AppUserId == appUser.Id && x.Status == "ACTIVE");
            if (audit == null || audit.MasterQuestionerId == null) return null;

            var versionKey = $"master_questioner_mystery_{audit.MasterQuestionerId}";

            if (_cache.TryGetValue(versionKey, out MasterQuestionerResponse cachedData))
            {
                return cachedData;
            }

            var header = await _context.MasterQuestioners.FirstOrDefaultAsync(x => x.Id == audit.MasterQuestionerId && x.Type == "MYSTERY");
            if (header == null) return null;

            var details = await _context.MasterQuestionerDetails
                .Where(x => x.MasterQuestionerId == header.Id && x.Status == "ACTIVE")
                .ToListAsync();

            var tree = BuildTree(details);

            var result = new MasterQuestionerResponse
            {
                Version = header.Version,
                Detail = tree
            };

            _cache.Set(versionKey, result, TimeSpan.FromMinutes(30));

            return result;
        }

        private List<MasterQuestionerDetailViewModel> BuildTree(List<MasterQuestionerDetail> flatList)
        {
            var lookup = flatList.ToLookup(x => x.ParentId);

            List<MasterQuestionerDetailViewModel> BuildChildren(string parentId)
            {
                return lookup[parentId].OrderBy(x => x.OrderNo).Select(item => new MasterQuestionerDetailViewModel
                {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    Type = item.Type,
                    OrderNo = item.OrderNo,
                    ScoreOption = item.ScoreOption,
                    Child = BuildChildren(item.Id)
                }).ToList();
            }

            return BuildChildren(null);
        }

        public async Task<MasterQuestionerDetailCombinedViewModel?> GetQuestionerDetailFromTrxAuditAsync(string trxAuditId)
        {
            var trxSql = @"SELECT master_questioner_intro_id, master_questioner_checklist_id FROM trx_audit WHERE id = @id";
            var trx = await _conn.QueryFirstOrDefaultAsync<dynamic>(trxSql, new { id = trxAuditId });

            if (trx == null) return null;

            var response = new MasterQuestionerDetailCombinedViewModel();

            if (trx.master_questioner_intro_id != null)
            {
                var introMaster = await _conn.QueryFirstOrDefaultAsync<dynamic>(
                    "SELECT id, type, version, category FROM master_questioner WHERE id = @id",
                    new { id = trx.master_questioner_intro_id });

                if (introMaster != null && introMaster.category == "INTRO")
                {
                    var intro = await GetDetailRecursiveCached(introMaster.id.ToString(), introMaster.type.ToString(), introMaster.version, "INTRO");
                    response.Detail.INTRO = intro ?? new List<MasterQuestionerDetailItemViewModel>();
                    response.Version = introMaster.version;
                }
            }

            if (trx.master_questioner_checklist_id != null)
            {
                var checklistMaster = await _conn.QueryFirstOrDefaultAsync<dynamic>(
                    "SELECT id, type, version, category FROM master_questioner WHERE id = @id",
                    new { id = trx.master_questioner_checklist_id });

                if (checklistMaster != null && checklistMaster.category == "CHECKLIST")
                {
                    var checklist = await GetDetailRecursiveCached(checklistMaster.id.ToString(), checklistMaster.type.ToString(), checklistMaster.version, "CHECKLIST");
                    response.Detail.CHECKLIST = checklist ?? new List<MasterQuestionerDetailItemViewModel>();
                    response.Version = checklistMaster.version;
                }
            }

            return response.Detail.INTRO.Count == 0 && response.Detail.CHECKLIST.Count == 0
                ? null
                : response;
        }

        private async Task<List<MasterQuestionerDetailItemViewModel>?> GetDetailRecursiveCached(string questionerId, string type, int version, string category)
        {
            var cacheKey = $"questioner:{type}:{version}:{category}";

            if (_cache.TryGetValue(cacheKey, out List<MasterQuestionerDetailItemViewModel> cached))
            {
                return cached;
            }

            var details = await BuildHierarchy(questionerId, null);
            _cache.Set(cacheKey, details, TimeSpan.FromMinutes(60));
            return details;
        }

        private async Task<List<MasterQuestionerDetailItemViewModel>> BuildHierarchy(string masterId, string? parentId)
        {
            var sql = @"
            SELECT id, title, description, type, score_option AS ScoreOption, order_no AS OrderNo
            FROM master_questioner_detail
            WHERE master_questioner_id = @masterId AND (@parentId IS NULL AND parent_id IS NULL OR parent_id = @parentId)
            ORDER BY order_no";

            var items = (await _conn.QueryAsync<MasterQuestionerDetailItemViewModel>(sql, new { masterId, parentId })).ToList();

            foreach (var item in items)
            {
                item.Child = await BuildHierarchy(masterId, item.Id);
            }

            return items;
        }
    }
}