using System;
using System.Collections.Generic;

namespace ePasServices.Models;

public partial class AppUserRole
{
    public string Id { get; set; } = null!;

    public string AppUserId { get; set; } = null!;

    public string AppRoleId { get; set; } = null!;

    public virtual AppRole AppRole { get; set; } = null!;

    public virtual AppUser AppUser { get; set; } = null!;
}
