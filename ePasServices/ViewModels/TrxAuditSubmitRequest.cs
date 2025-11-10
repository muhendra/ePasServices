public class TrxAuditSubmitRequest
{
    public string Id { get; set; }
    public int? AuditMediaTotal { get; set; }
    public DateTime? AuditExecutionDate { get; set; }
    public string? AuditMomIntro { get; set; }
    public string? AuditMomFinal { get; set; }
    public string? AuditMomChecklist { get; set; }
    public List<ChecklistAnswer>? Checklist { get; set; }
    public List<AuditQQ>? QQ { get; set; }
}

public class ChecklistAnswer
{
    public string QuestionerDetailId { get; set; }
    public string ScoreInput { get; set; }
    public string? Comment { get; set; }
}

public class AuditQQ
{
    public string NozzleNumber { get; set; }
    public string DuMake { get; set; }
    public string DuSerialNo { get; set; }
    public string Product { get; set; }
    public string Mode { get; set; }
    public decimal QuantityVariationWithMeasure { get; set; }
    public decimal QuantityVariationInPercentage { get; set; }
    public decimal ObservedDensity { get; set; }
    public decimal ObservedTemp { get; set; }
    public decimal ObservedDensity15Degree { get; set; }
    public decimal ReferenceDensity15Degree { get; set; }
    public int TankNumber { get; set; }
    public decimal DensityVariation { get; set; }
}
