namespace ePasServices.ViewModels
{
    public class TrxFeedbackListItemViewModel
    {
        public string Id { get; set; }
        public string TicketNo { get; set; } = string.Empty;
        public string FeedbackType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string Numbers { get; set; } = string.Empty;
    }
}
