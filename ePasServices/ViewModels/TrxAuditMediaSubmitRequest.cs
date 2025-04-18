namespace ePasServices.ViewModels
{
    // Model untuk TrxAuditMediaSubmitRequest
    public class TrxAuditMediaSubmitRequest
    {
        public string Id { get; set; } = null!; // trx_audit_id
        public string Type { get; set; } = null!; // QUESTION / MOM
        public string? DetailId { get; set; } // boleh null kalau type = MOM
        public string MediaType { get; set; } = null!; // IMAGE / VIDEO / DOCUMENT
    }

    // Entity class untuk TrxAuditMedia
    public class TrxAuditMediaVIewModel
    {
        public string Id { get; set; } = null!;
        public string TrxAuditId { get; set; } = null!;
        public string? Type { get; set; }
        public string? MasterQuestionerDetailId { get; set; }
        public string MediaType { get; set; } = null!;
        public string MediaPath { get; set; } = null!;
        public string Status { get; set; } = "ACTIVE";
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; } = null!;
        public DateTime? UpdatedDate { get; set; }
    }
}
