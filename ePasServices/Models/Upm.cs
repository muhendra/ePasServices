using System;
using System.Collections.Generic;

namespace ePasServices.Models;

public partial class Upm
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<Province> Provinces { get; set; } = new List<Province>();
}
