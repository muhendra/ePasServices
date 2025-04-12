using System;
using System.Collections.Generic;

namespace ePasServices.Models;

public partial class Notification
{
    public string Id { get; set; } = null!;

    public string AppUserId { get; set; } = null!;

    public string? Token { get; set; }

    public string Type { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string? Data { get; set; }

    public string Status { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime? UpdatedDate { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;
}
