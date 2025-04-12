﻿using System;
using System.Collections.Generic;

namespace ePasServices.Models;

public partial class AuditSurvey
{
    public string Id { get; set; } = null!;

    public string AuditId { get; set; } = null!;

    public string Survey { get; set; } = null!;

    public int OrderNo { get; set; }

    public string Answer { get; set; } = null!;

    public virtual Audit Audit { get; set; } = null!;
}
