using System;
using System.Collections.Generic;

namespace ePasServices.Models;

public partial class TrxAudit
{
    public string Id { get; set; } = null!;

    public string SpbuId { get; set; } = null!;

    public string? AppUserId { get; set; }

    public string? MasterQuestionerIntroId { get; set; }

    public string? MasterQuestionerChecklistId { get; set; }

    public string AuditLevel { get; set; } = null!;

    public string AuditType { get; set; } = null!;

    public DateOnly? AuditScheduleDate { get; set; }

    public DateOnly? AuditExecutionDate { get; set; }

    public int? AuditMediaUpload { get; set; }

    public int? AuditMediaTotal { get; set; }

    public string? AuditMomIntro { get; set; }

    public string? AuditMomChecklist { get; set; }

    public string Status { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime? UpdatedDate { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual MasterQuestioner? MasterQuestionerChecklist { get; set; }

    public virtual MasterQuestioner? MasterQuestionerIntro { get; set; }

    public virtual Spbu Spbu { get; set; } = null!;

    public virtual ICollection<TrxAuditChecklist> TrxAuditChecklists { get; set; } = new List<TrxAuditChecklist>();

    public virtual ICollection<TrxAuditMedium> TrxAuditMedia { get; set; } = new List<TrxAuditMedium>();

    public virtual ICollection<TrxAuditQq> TrxAuditQqs { get; set; } = new List<TrxAuditQq>();
}
