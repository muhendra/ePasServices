using System;
using System.Collections.Generic;

namespace ePasServices.Models;

public partial class Master
{
    public string Id { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string MasterGroupCode { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime? UpdatedDate { get; set; }

    public virtual MasterGroup MasterGroupCodeNavigation { get; set; } = null!;
}
