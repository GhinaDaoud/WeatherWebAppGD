using Microsoft.AspNetCore.Mvc; 
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

        public WeatherController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        

        [HttpGet]
        public async Task<IActionResult> GetWeather([FromQuery] string city)
        {
            if (string.IsNullOrEmpty(city))
                return BadRequest("City is required.");

            string apiKey = "217ba34e9318036e8bc512141b498c21"; 
            string weatherApiUrl = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}";

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
