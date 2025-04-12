using System;
using System.Collections.Generic;

namespace ePasServices.Models;

public partial class AuditQuestionnaireMedium
{
    public string Id { get; set; } = null!;

    public string AuditQuestionnaireId { get; set; } = null!;

    public string MediaType { get; set; } = null!;

    public string? MediaFilepath { get; set; }

    public virtual AuditQuestionnaire AuditQuestionnaire { get; set; } = null!;
}
