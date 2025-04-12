using System;
using System.Collections.Generic;

namespace ePasServices.Models;

public partial class AuditPreparation
{
    public string Id { get; set; } = null!;

    public string AuditId { get; set; } = null!;

    public string Preparation { get; set; } = null!;

    public int OrderNo { get; set; }

    public string Answer { get; set; } = null!;

    public virtual Audit Audit { get; set; } = null!;
}
