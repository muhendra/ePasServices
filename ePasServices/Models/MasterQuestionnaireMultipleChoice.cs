using System;
using System.Collections.Generic;

namespace ePasServices.Models;

public partial class MasterQuestionnaireMultipleChoice
{
    public string Id { get; set; } = null!;

    public string MasterQuestionnaireId { get; set; } = null!;

    public string Answer { get; set; } = null!;

    public int Weight { get; set; }

    public int OrderNo { get; set; }

    public virtual MasterQuestionnaire MasterQuestionnaire { get; set; } = null!;
}
