namespace ePasServices.ViewModels
{
    public class TrxAuditListItemViewModel
    {
        public string Id { get; set; }
        public string AuditLevel { get; set; }
        public string AuditType { get; set; }
        public DateTime? AuditScheduleDate { get; set; }
        public string Status { get; set; }

        public string AuditStatus => Status switch
        {
            "BELUM_DIMULAI" => "Belum Dimulai",
            "SEDANG_BERLANGSUNG" => "Sedang Berlangsung",
            "REVIEW" => "Sedang Ditinjau",
            "VERIFIED" => "Terverfikasi",
            _ => Status
        };

        // Ini gantikan yang sebelumnya
        public SpbuViewModel Spbu { get; set; }
    }

    public class SpbuViewModel
    {
        public string SpbuNo { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string[] Images { get; set; } = Array.Empty<string>();
    }
}
