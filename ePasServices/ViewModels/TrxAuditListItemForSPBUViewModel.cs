namespace ePasServices.ViewModels
{
    public class TrxAuditListItemForSPBUViewModel
    {
        public string Id { get; set; }
        public string AuditLevel { get; set; }
        public string AuditType { get; set; }
        public DateTime? AuditScheduleDate { get; set; }
        public string Status { get; set; }
        public string AuditorName { get; set; }
    }
}
