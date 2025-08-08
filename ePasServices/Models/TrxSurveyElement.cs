namespace ePasServices.Models;

public partial class TrxSurveyElement
{
    public string Id { get; set; } = null!;
    public string TrxSurveyId { get; set; } = null!;
    public string MasterQuestionerDetailId { get; set; } = null!;
    public string? ScoreInput { get; set; }
    public string Status { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public string UpdatedBy { get; set; } = null!;
    public DateTime? UpdatedDate { get; set; }

    public virtual TrxSurvey TrxSurvey { get; set; } = null!;
    public virtual MasterQuestionerDetail MasterQuestionerDetail { get; set; } = null!;
}
