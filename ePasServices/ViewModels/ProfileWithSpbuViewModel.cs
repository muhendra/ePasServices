namespace ePasServices.ViewModels
{
    public class ProfileWithSpbuViewModel
    {
        public string Name { get; set; }
        public SpbuInfo? Spbu { get; set; }
    }

    public class SpbuInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Level { get; set; }
    }

    public class ProfileWithSpbuTempDto
    {
        public string Name { get; set; }
        public string App { get; set; }
        public string? Code { get; set; }
        public string? SpbuName { get; set; }
        public string? Level { get; set; }
    }


}
