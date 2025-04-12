using System;
using System.Collections.Generic;

namespace ePasServices.Models;

public partial class Audit
{
    public string Id { get; set; } = null!;

    public string AuditorId { get; set; } = null!;

    public string SpbuId { get; set; } = null!;

    public string MasterQuestionnaireCategoryId { get; set; } = null!;

    public string MasterQuestionnaireCategoryName { get; set; } = null!;

    public DateOnly PlanDate { get; set; }

    public DateTime? ExecuteTime { get; set; }

    public DateTime? SendTime { get; set; }

    public DateTime? SurveyTime { get; set; }

    public int? ResultPoint { get; set; }

    public string? ResultFilepath { get; set; }

    public string Status { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<AuditPreparation> AuditPreparations { get; set; } = new List<AuditPreparation>();

    public virtual ICollection<AuditQuestionnaire> AuditQuestionnaires { get; set; } = new List<AuditQuestionnaire>();

    public virtual ICollection<AuditSurvey> AuditSurveys { get; set; } = new List<AuditSurvey>();

    public virtual AppUser Auditor { get; set; } = null!;

    public virtual MasterQuestionnaireCategory MasterQuestionnaireCategory { get; set; } = null!;

    public virtual Spbu Spbu { get; set; } = null!;
}
