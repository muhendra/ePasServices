public class TrxSurveySubmitRequest
{
    public string TrxAuditId { get; set; } = null!;
    public string MasterQuestionerId { get; set; } = null!;
    public List<TrxSurveyElementRequest> Elements { get; set; } = new();
}

public class TrxSurveyElementRequest
{
    public string MasterQuestionerDetailId { get; set; } = null!;
    public string? ScoreInput { get; set; }
}