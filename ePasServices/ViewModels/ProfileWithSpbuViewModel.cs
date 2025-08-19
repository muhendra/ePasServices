namespace ePasServices.ViewModels
{
    public class ProfileWithSpbuViewModel
    {
        public string Name { get; set; }
        public SpbuInfo? Spbu { get; set; }
    }

    public class SpbuInfo
    {
        public string SpbuNo { get; set; }
        public string ProvinceName { get; set; }
        public string CityName { get; set; }
        public string Address { get; set; }
        public string Type { get; set; }
        public string Level { get; set; }

    }

    public class ProfileWithSpbuTempDto
    {
        public string Name { get; set; }
        public string App { get; set; }
        public string SpbuNo { get; set; }
        public string ProvinceName { get; set; }
        public string CityName { get; set; }
        public string Address { get; set; }
        public string Type { get; set; }
        public string Level { get; set; }
    }


}
