using Microsoft.AspNetCore.Mvc;

namespace ePasServices.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WilayahController : ControllerBase
    {
        private readonly ILogger<WilayahController> _logger;

        public WilayahController(ILogger<WilayahController> logger)
        {
            _logger = logger;
        }

        [HttpGet("zona-waktu")]
        public IActionResult GetWilayahByZonaWaktu()
        {
            var wilayahIndonesia = new List<ZonaWaktuIndonesia>
            {
                new ZonaWaktuIndonesia
                {
                    Zona = "Waktu Indonesia Barat (WIB)",
                    Wilayah = new[]
                    {
                        "Aceh", "Sumatera Utara", "Sumatera Barat", "Riau", "Kepulauan Riau",
                        "Jambi", "Bengkulu", "Sumatera Selatan", "Bangka Belitung", "Lampung",
                        "DKI Jakarta", "Jawa Barat", "Banten", "Jawa Tengah", "DI Yogyakarta", "Jawa Timur"
                    }
                },
                new ZonaWaktuIndonesia
                {
                    Zona = "Waktu Indonesia Tengah (WITA)",
                    Wilayah = new[]
                    {
                        "Bali", "Nusa Tenggara Barat", "Nusa Tenggara Timur",
                        "Kalimantan Selatan", "Kalimantan Tengah", "Kalimantan Timur", "Sulawesi Selatan",
                        "Sulawesi Barat", "Sulawesi Tengah", "Sulawesi Tenggara", "Sulawesi Utara"
                    }
                },
                new ZonaWaktuIndonesia
                {
                    Zona = "Waktu Indonesia Timur (WIT)",
                    Wilayah = new[]
                    {
                        "Maluku", "Maluku Utara", "Papua", "Papua Barat", "Papua Tengah", "Papua Pegunungan", "Papua Selatan", "Papua Barat Daya"
                    }
                }
            };

            return Ok(wilayahIndonesia);
        }
    }

    public class ZonaWaktuIndonesia
    {
        public string Zona { get; set; }
        public IEnumerable<string> Wilayah { get; set; }
    }
}
