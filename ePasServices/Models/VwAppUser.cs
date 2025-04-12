using System;
using System.Collections.Generic;

namespace ePasServices.Models;

public partial class VwAppUser
{
    public string? Id { get; set; }

    public string? Username { get; set; }

    public string? Name { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? App { get; set; }

    public string? AppRole { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? Status { get; set; }
}
