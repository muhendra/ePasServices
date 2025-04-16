using System;
using System.Collections.Generic;

namespace ePasServices.Models;

public partial class MasterQuestioner
{
    public string Id { get; set; } = null!;

    public string Type { get; set; } = null!;

    public int Version { get; set; }

    public DateOnly EffectiveStartDate { get; set; }

    public DateOnly EffectiveEndDate { get; set; }

    public string Status { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<MasterQuestionerDetail> MasterQuestionerDetails { get; set; } = new List<MasterQuestionerDetail>();

    public virtual ICollection<TrxAudit> TrxAudits { get; set; } = new List<TrxAudit>();
}
