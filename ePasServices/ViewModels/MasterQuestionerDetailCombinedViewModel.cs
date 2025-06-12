namespace ePasServices.ViewModels
{
    public class MasterQuestionerDetailCombinedViewModel
    {
        public int Version { get; set; }
        public MasterQuestionerDetailGroup Detail { get; set; } = new();
    }

    public class MasterQuestionerDetailGroup
    {
        public List<MasterQuestionerDetailItemViewModel> INTRO { get; set; } = new();
        public List<MasterQuestionerDetailItemViewModel> CHECKLIST { get; set; } = new();
    }

    public class MasterQuestionerDetailItemViewModel
    {
        public string Id { get; set; }
        public string Number { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Type { get; set; }
        public int OrderNo { get; set; }
        public string? ScoreOption { get; set; }
        public List<MasterQuestionerDetailItemViewModel>? Child { get; set; }
    }
}
