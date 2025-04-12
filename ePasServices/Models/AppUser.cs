using System;
using System.Collections.Generic;

namespace ePasServices.Models;

public partial class AppUser
{
    public string Id { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? NotificationToken { get; set; }

    public DateTime? LastChangePasswdDt { get; set; }

    public DateTime? LastLoginDt { get; set; }

    public string? SuffixRefreshToken { get; set; }

    public string Status { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<AppUserRole> AppUserRoles { get; set; } = new List<AppUserRole>();

    public virtual ICollection<Audit> Audits { get; set; } = new List<Audit>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<SpbuEmail> SpbuEmails { get; set; } = new List<SpbuEmail>();
}
