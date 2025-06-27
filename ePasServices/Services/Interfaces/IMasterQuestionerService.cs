using ePasServices.ViewModels;

namespace ePasServices.Services.Interfaces
{
    public interface IMasterQuestionerService
    {
        Task<MasterQuestionerResponse?> GetMasterQuestionerByUserAsync(string id, string username);
        Task<MasterQuestionerResponse?> GetMasterQuestionerMysteryByUserAsync(string id, string username);
        Task<MasterQuestionerDetailCombinedViewModel?> GetQuestionerDetailFromTrxAuditAsync(string trxAuditId);

        Task<MasterQuestionerSurveyResponse?> GetLatestSurveyWithDetailsAsync();
    }

}
