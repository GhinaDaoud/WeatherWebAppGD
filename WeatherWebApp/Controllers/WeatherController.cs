using Microsoft.AspNetCore.Mvc; // Provides attributes like [ApiController] and IActionResult
using System.Net.Http;          // For making HTTP requests
using System.Text.Json;         // For JSON serialization/deserialization
using System.Threading.Tasks;   // For async/await

namespace WeatherWebApp.Controllers // Replace 'YourNamespace' with your project's namespace
{
    [ApiController] // Marks this class as a controller
    [Route("api/[controller]")] // Routes requests to /api/weather
    public class WeatherController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        // Constructor: Injects HttpClient
        public WeatherController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        // HTTP GET method: Handles requests like /api/weather?city=London
        [HttpGet]
        public async Task<IActionResult> GetWeather([FromQuery] string city)
        {
            if (string.IsNullOrEmpty(city)) // Validate input
                return BadRequest("City is required.");

            string apiKey = "217ba34e9318036e8bc512141b498c21"; // Replace with your OpenWeatherMap API key
            string weatherApiUrl = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}";

            try
            {
                // Make HTTP request to OpenWeatherMap
                var response = await _httpClient.GetAsync(weatherApiUrl);
                response.EnsureSuccessStatusCode(); // Throw if the status code is not 2xx

                var weatherData = await response.Content.ReadAsStringAsync(); // Get response as string
                var weatherJson = JsonSerializer.Deserialize<object>(weatherData); // Deserialize JSON

                return Ok(weatherJson); // Return data to the client
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"Error calling weather API: {ex.Message}");
                return StatusCode(500, "Unable to fetch weather data. Please try again later.");

            }
        }
    }
}
