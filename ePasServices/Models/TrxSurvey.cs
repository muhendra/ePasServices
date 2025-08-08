namespace ePasServices.Models;

public partial class TrxSurvey
{
    public string Id { get; set; } = null!;
    public string AppUserId { get; set; } = null!;
    public string MasterQuestionerId { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public string UpdatedBy { get; set; } = null!;
    public DateTime? UpdatedDate { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;
    public virtual MasterQuestioner MasterQuestioner { get; set; } = null!;
    public virtual ICollection<TrxSurveyElement> TrxSurveyElements { get; set; } = new List<TrxSurveyElement>();
}
