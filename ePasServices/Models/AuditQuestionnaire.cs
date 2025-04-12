using System;
using System.Collections.Generic;

namespace ePasServices.Models;

public partial class AuditQuestionnaire
{
    public string Id { get; set; } = null!;

    public string AuditId { get; set; } = null!;

    public string QuestionnaireType { get; set; } = null!;

    public string Questionnaire { get; set; } = null!;

    public int OrderNo { get; set; }

    public string Answer { get; set; } = null!;

    public int Point { get; set; }

    public virtual Audit Audit { get; set; } = null!;

    public virtual ICollection<AuditQuestionnaireMedium> AuditQuestionnaireMedia { get; set; } = new List<AuditQuestionnaireMedium>();
}
