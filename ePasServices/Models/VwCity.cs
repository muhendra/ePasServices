using System;
using System.Collections.Generic;

namespace ePasServices.Models;

public partial class VwCity
{
    public string? Id { get; set; }

    public string? ProvinceName { get; set; }

    public string? Name { get; set; }

    public string? Status { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
