using System;
using System.Collections.Generic;

namespace ePasServices.Models;

public partial class AuditTransaction
{
    public string Id { get; set; } = null!;

    public string SpbuId { get; set; } = null!;

    public string? AppUserId { get; set; }

    public string AuditLevel { get; set; } = null!;

    public string AuditType { get; set; } = null!;

    public DateOnly? AuditScheduleDate { get; set; }

    public string Status { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime? UpdatedDate { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual Spbu Spbu { get; set; } = null!;
}
