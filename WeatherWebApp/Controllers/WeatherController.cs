using Microsoft.AspNetCore.Mvc; 
using System.IO;
using System.Net.Http;    
using System.Text.Json;        
using System.Threading.Tasks;  

namespace WeatherWebApp.Controllers 
{
    [ApiController] 
    [Route("api/[controller]")] 
    public class WeatherController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string apiKey_;

        public WeatherController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            apiKey_ = LoadApiKey(); 
        }

        private string LoadApiKey()
        {
            // Path to the txt file containing your API key
            string apiKeyFilePath = Path.Combine(Directory.GetCurrentDirectory(), "apikey.txt");

            if (!System.IO.File.Exists(apiKeyFilePath))
                throw new FileNotFoundException("API key file not found.");

            return System.IO.File.ReadAllText(apiKeyFilePath).Trim();
        }

        [HttpGet]
        public async Task<IActionResult> GetWeather([FromQuery] string city)
        {
            if (string.IsNullOrEmpty(city))
                return BadRequest("City is required.");

            //string apiKey = apiKey_; 
            string weatherApiUrl = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey_}";

            try
            {
                var response = await _httpClient.GetAsync(weatherApiUrl);
                response.EnsureSuccessStatusCode(); 

                var weatherData = await response.Content.ReadAsStringAsync(); 
                var weatherJson = JsonSerializer.Deserialize<object>(weatherData); 

                return Ok(weatherJson); 
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"Error calling weather API: {ex.Message}");
                return StatusCode(500, "Unable to fetch weather data. Please try again later.");

            }
        }
    }
}
