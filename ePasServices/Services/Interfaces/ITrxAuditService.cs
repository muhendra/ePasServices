using ePasServices.ViewModels;

namespace ePasServices.Services.Interfaces
{
    public interface ITrxAuditService
    {
        Task<(List<TrxAuditListItemViewModel> Data, int Total)> GetTrxAuditListAsync(int page, int limit, bool history, string username);

        Task<(List<TrxAuditListItemForSPBUViewModel> Data, int Total)> GetTrxAuditListForSPBUAsync(int page, int limit, bool history, string username);

        Task<TrxAuditDetailForSPBUViewModel?> GetTrxAuditDetailForSPBUAsync(string id, string username);

        Task<(bool Success, string Message)> StartAuditAsync(string username, string auditId);
        Task<(bool Success, string Message)> CancelAuditAsync(string username, string auditId);

        Task<int> CountInProgressAsync(string username);

        Task<List<TrxAuditDetailListResponse>> GetDetailsByTrxAuditIdAndParentIdAsync(string trxAuditId, string parentId);
        
    }
}
