using ePasServices.ViewModels;

namespace ePasServices.Services.Interfaces
{
    public interface ITrxAuditService
    {
        Task<(List<TrxAuditListItemViewModel> Data, int Total)> GetTrxAuditListAsync(int page, int limit, bool history, string username);

        Task<(bool Success, string Message)> StartAuditAsync(string username, string auditId);

    }
}
