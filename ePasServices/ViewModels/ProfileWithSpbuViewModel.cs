namespace ePasServices.ViewModels
{
    public class ProfileWithSpbuViewModel
    {
        public string Name { get; set; }
        public SpbuInfo? Spbu { get; set; }
    }

    public class SpbuInfo
    {
        public string spbu_no { get; set; }
        public string province_name { get; set; }
        public string city_name { get; set; }
        public string type { get; set; }
        public string level { get; set; }

    }

    public class ProfileWithSpbuTempDto
    {
        public string Name { get; set; }
        public string App { get; set; }
        public string spbu_no { get; set; }
        public string province_name { get; set; }
        public string city_name { get; set; }
        public string type { get; set; }
        public string level { get; set; }
    }


}
