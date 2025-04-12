using System;
using System.Collections.Generic;

namespace ePasServices.Models;

public partial class SpbuImage
{
    public string Id { get; set; } = null!;

    public string SpbuId { get; set; } = null!;

    public string Filepath { get; set; } = null!;

    public virtual Spbu Spbu { get; set; } = null!;
}
