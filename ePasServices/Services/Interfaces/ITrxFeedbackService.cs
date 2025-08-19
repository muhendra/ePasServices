using ePasServices.ViewModels;

namespace ePasServices.Services.Interfaces
{
    public interface ITrxFeedbackService
    {
        Task<string> GenerateTicketNoAsync(string trxAuditId, string feedbackType);

        Task<(List<TrxFeedbackListItemViewModel> Data, int Total)> GetTrxFeedbackListAsync(int page, int limit, string username, string trxAuditId, string feedbackType);
    }
}
