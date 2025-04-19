﻿using System;
using System.Collections.Generic;

namespace ePasServices.Models;

public partial class Spbu
{
    public string Id { get; set; } = null!;

    public string SpbuNo { get; set; } = null!;

    public string Region { get; set; } = null!;

    public string ProvinceName { get; set; } = null!;

    public string CityName { get; set; } = null!;

    public string? Address { get; set; }

    public string? OwnerName { get; set; }

    public string? ManagerName { get; set; }

    public string? OwnerType { get; set; }

    public int? Quater { get; set; }

    public int? Year { get; set; }

    public string? Mor { get; set; }

    public string? SalesArea { get; set; }

    public string? Sbm { get; set; }

    public string? Sam { get; set; }

    public string? Type { get; set; }

    public string? PhoneNumber1 { get; set; }

    public string? PhoneNumber2 { get; set; }

    public string? Level { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string? AuditCurrent { get; set; }

    public string? AuditNext { get; set; }

    public string? StatusGood { get; set; }

    public string? StatusExcellent { get; set; }

    public decimal? AuditCurrentScore { get; set; }

    public DateTime? AuditCurrentTime { get; set; }

    public string Status { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<AppUserRole> AppUserRoles { get; set; } = new List<AppUserRole>();

    public virtual ICollection<SpbuImage> SpbuImages { get; set; } = new List<SpbuImage>();

    public virtual ICollection<TrxAudit> TrxAudits { get; set; } = new List<TrxAudit>();
}
