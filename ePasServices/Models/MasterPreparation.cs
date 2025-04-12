using System;
using System.Collections.Generic;

namespace ePasServices.Models;

public partial class MasterPreparation
{
    public string Id { get; set; } = null!;

    public string Preparation { get; set; } = null!;

    public int OrderNo { get; set; }

    public string Status { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime? UpdatedDate { get; set; }
}
