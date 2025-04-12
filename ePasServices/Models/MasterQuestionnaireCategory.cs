using System;
using System.Collections.Generic;

namespace ePasServices.Models;

public partial class MasterQuestionnaireCategory
{
    public string Id { get; set; } = null!;

    public string Category { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime? UpdatedDate { get; set; }

    public string? MasterQuestionnaireCategoryId { get; set; }

    public int OrderNo { get; set; }

    public virtual ICollection<Audit> Audits { get; set; } = new List<Audit>();

    public virtual ICollection<MasterQuestionnaire> MasterQuestionnaires { get; set; } = new List<MasterQuestionnaire>();
}
