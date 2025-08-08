namespace ePasServices.ViewModels
{
    public class TrxAuditDetailListResponse
    {
        public string Id { get; set; } = default!;
        public string Number { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public int OrderNo { get; set; }
        public string Type { get; set; } = default!;

        public string? TrxAuditChecklistId { get; set; }
    }
}