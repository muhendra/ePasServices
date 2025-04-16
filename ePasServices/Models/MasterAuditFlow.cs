using System;
using System.Collections.Generic;

namespace ePasServices.Models;

public partial class MasterAuditFlow
{
    public string Id { get; set; } = null!;

    public string AuditLevel { get; set; } = null!;

    public string PassedAuditLevel { get; set; } = null!;

    public string FailedAuditLevel { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime? UpdatedDate { get; set; }
}
