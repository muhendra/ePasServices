using System;
using System.Collections.Generic;

namespace ePasServices.Models;

public partial class Province
{
    public string Id { get; set; } = null!;

    public string UpmsId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<City> Cities { get; set; } = new List<City>();

    public virtual Upm Upms { get; set; } = null!;
}
