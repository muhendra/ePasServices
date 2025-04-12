using System;
using System.Collections.Generic;

namespace ePasServices.Models;

public partial class MasterGroup
{
    public string Id { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string? Descp { get; set; }

    public string Status { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<Master> Masters { get; set; } = new List<Master>();
}
