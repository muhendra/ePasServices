namespace ePasServices.ViewModels
{
    public class MasterQuestionerDetailViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int OrderNo { get; set; }
        public string ScoreOption { get; set; }
        public List<MasterQuestionerDetailViewModel> Child { get; set; }
    }

}
