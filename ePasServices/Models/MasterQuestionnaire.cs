using System;
using System.Collections.Generic;

namespace ePasServices.Models;

public partial class MasterQuestionnaire
{
    public string Id { get; set; } = null!;

    public string MasterQuestionnaireCategoryId { get; set; } = null!;

    public string QuestionnaireType { get; set; } = null!;

    public string Questionnaire { get; set; } = null!;

    public int Weight { get; set; }

    public int OrderNo { get; set; }

    public string Status { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime? UpdatedDate { get; set; }

    public virtual MasterQuestionnaireCategory MasterQuestionnaireCategory { get; set; } = null!;

    public virtual ICollection<MasterQuestionnaireMultipleChoice> MasterQuestionnaireMultipleChoices { get; set; } = new List<MasterQuestionnaireMultipleChoice>();
}
