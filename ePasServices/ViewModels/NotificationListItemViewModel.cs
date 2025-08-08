namespace ePasServices.ViewModels
{
    public class NotificationListItemViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}
