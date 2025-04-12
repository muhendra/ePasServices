using System;
using System.Collections.Generic;

namespace ePasServices.Models;

public partial class SpbuEmail
{
    public string Id { get; set; } = null!;

    public string SpbuId { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? AppUserId { get; set; }

    public virtual AppUser? AppUser { get; set; }

    public virtual Spbu Spbu { get; set; } = null!;
}
