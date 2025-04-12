using System;
using System.Collections.Generic;

namespace ePasServices.Models;

public partial class SchemaMigration
{
    public long Version { get; set; }

    public bool Dirty { get; set; }
}
