using ePasServices.Data;
using ePasServices.Models;
using ePasServices.Services.Interfaces;
using ePasServices.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ePasServices.Services
{
    public class MasterQuestionerService : IMasterQuestionerService
    {
        private readonly PostgreDbContext _context;
        private readonly IMemoryCache _cache;

        public MasterQuestionerService(PostgreDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<MasterQuestionerResponse> GetMasterQuestionerByUserAsync(string id, string username)
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

        public async Task<MasterQuestionerResponse> GetMasterQuestionerMysteryByUserAsync(string id, string username)
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
    }


}
