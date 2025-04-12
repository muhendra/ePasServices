using System;
using System.Collections.Generic;

namespace ePasServices.Models;

public partial class Spbu
{
    public string Id { get; set; } = null!;

    public string? Code { get; set; }

    public string Name { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string? AreaCode { get; set; }

    public string? Address { get; set; }

    public string Status { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<Audit> Audits { get; set; } = new List<Audit>();

    public virtual ICollection<SpbuEmail> SpbuEmails { get; set; } = new List<SpbuEmail>();

    public virtual ICollection<SpbuImage> SpbuImages { get; set; } = new List<SpbuImage>();
}
