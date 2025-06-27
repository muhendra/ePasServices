namespace ePasServices.ViewModels
{
    public class MasterQuestionerSurveyResponse
    {
        public string Id { get; set; }
        public List<MasterQuestionerSurveyDetailDto> Detail { get; set; }
    }

    public class MasterQuestionerSurveyDetailDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int OrderNo { get; set; }
    }
}
