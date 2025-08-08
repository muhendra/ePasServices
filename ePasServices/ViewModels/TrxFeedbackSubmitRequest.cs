public class TrxFeedbackSubmitRequest
{
    public string TrxAuditId { get; set; }
    public string FeedbackType { get; set; }
    public List<FeedbackPoint>? Points { get; set; }
}

public class FeedbackPoint
{
    public string Id { get; set; }
    public string FeedbackDescription { get; set; }
    public string ElementMasterQuestionerDetailId { get; set; }
    public string? SubElementMasterQuestionerDetailId { get; set; }
    public string? DetailElementMasterQuestionerDetailId { get; set; }
    public int FeedbackMediaTotal { get; set; }
    public List<FeedbackPointElement>? PointElements { get; set; }
}

public class FeedbackPointElement
{
    public string MasterQuestionerDetailId { get; set; }
}
