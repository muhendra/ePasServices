using ePasServices.ViewModels;

namespace ePasServices.Services.Interfaces
{
    public interface ITrxAuditV2Service
    {
        Task<(List<TrxAuditListItemViewV2Model> Data, int Total)> GetTrxAuditListAsync(int page, int limit, bool history, string username);

        Task<(bool Success, string Message)> StartAuditAsync(string username, string auditId);
        Task<(bool Success, string Message)> CancelAuditAsync(string username, string auditId);

        Task<int> CountInProgressAsync(string username);
        
    }
}
