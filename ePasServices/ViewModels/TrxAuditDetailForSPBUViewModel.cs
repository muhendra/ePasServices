namespace ePasServices.ViewModels
{
    public class TrxAuditDetailForSPBUViewModel
    {
        public string Id { get; set; }
        public string AuditLevel { get; set; }
        public string AuditType { get; set; }
        public DateTime? AuditScheduleDate { get; set; }
        public DateTime? AuditExecutionTime { get; set; }
        public string Status { get; set; }
        public string AuditorName { get; set; }
        public string GoodStatus { get; set; }
        public string ExcellentStatus { get; set; }
        public string BoaStatus { get; set; }
        public string Score { get; set; }
        public string ReportFileGood { get; set; }
        public string ReportFileExcellent { get; set; }
        public string ReportFileBoa { get; set; }
    }
}
