using System;
using System.Collections.Generic;

namespace ePasServices.Models;

public partial class VwMasterQuestionnaireCategory
{
    public string? Id { get; set; }

    public string? Category { get; set; }

    public string? Status { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? OrderNo { get; set; }

    public string? ParentName { get; set; }
}
