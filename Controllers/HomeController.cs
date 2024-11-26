using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using MashupProgresivo.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MashupProgresivo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Acción para Clima con integración de APIs
        public async Task<IActionResult> Clima(string city = "Puebla", string country = "MX")
        {
            // Geolocalización con Nominatim
            string geoUrl = $"https://nominatim.openstreetmap.org/search?q={city},{country}&format=json";

            // Configurar encabezado User-Agent
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, geoUrl);
            requestMessage.Headers.Add("User-Agent", "MashupProgresivo/1.0 (utp0153413@alumno.utpuebla.edu.mx)");

            var geoResponse = await _httpClient.SendAsync(requestMessage);

            if (!geoResponse.IsSuccessStatusCode)
            {
                // Si hay error, mostrar un mensaje o una vista de error
                return View("Error");
            }

            var geoResponseContent = await geoResponse.Content.ReadAsStringAsync();
            dynamic geoData = JsonConvert.DeserializeObject(geoResponseContent);

            if (geoData.Count == 0) return View("Error"); // Si no encuentra la ciudad.

            string latitude = geoData[0].lat;
            string longitude = geoData[0].lon;

            // Datos climatológicos con OpenWeatherMap
            string apiKey = "d797593401de41512116d5672461f22f"; // Reemplaza con tu API Key
            string weatherUrl = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={apiKey}&lang=es&units=metric";
            var weatherResponse = await _httpClient.GetStringAsync(weatherUrl);
            dynamic weatherData = JsonConvert.DeserializeObject(weatherResponse);

            var weatherModel = new WeatherModel
            {
                Description = weatherData.weather[0].description,
                Temperature = weatherData.main.temp,
                Humidity = weatherData.main.humidity,
                Icon = $"https://openweathermap.org/img/wn/{weatherData.weather[0].icon}@2x.png"
            };

            return View(weatherModel);
        }

        // Acción para Mapa
        public IActionResult Mapa()
        {
            ViewData["Latitude"] = "19.0413";
            ViewData["Longitude"] = "-98.2062";
            return View();
        }

        // Conversor de Divisas
        public IActionResult Divisas()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Divisas(string baseCurrency, string targetCurrency, double amount)
        {
            string apiKey = "ca404472e820f199058b5a21"; // Reemplaza con tu API Key
            string url = $"https://v6.exchangerate-api.com/v6/{apiKey}/latest/{baseCurrency}";

            // Realiza la solicitud a la API
            var response = await _httpClient.GetStringAsync(url);
            dynamic data = JsonConvert.DeserializeObject(response);

            // Obtén la tasa de cambio
            double rate = data.conversion_rates[targetCurrency];

            // Realiza la conversión
            double convertedAmount = rate * amount;

            // Pasa los datos a la vista
            ViewData["BaseCurrency"] = baseCurrency;
            ViewData["TargetCurrency"] = targetCurrency;
            ViewData["Amount"] = amount;
            ViewData["ConvertedAmount"] = convertedAmount;

            return View();
        }

        public IActionResult YouTube()
        {
            return View();
        }

        public IActionResult Gasolina()
        {
            return View();
        }
    }
}
