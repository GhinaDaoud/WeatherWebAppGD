using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace WeatherWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoricalWeatherController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public HistoricalWeatherController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        private string GetSeasonFromTimestamp(long timestamp)
        {
            var date = DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
            int year = date.Year;
            var seasons = new Dictionary<string, Tuple<DateTime, DateTime>>
            {
                { "Winter", new Tuple<DateTime, DateTime>(new DateTime(year, 12, 21), new DateTime(year, 3, 20)) },
                { "Spring", new Tuple<DateTime, DateTime>(new DateTime(year, 3, 21), new DateTime(year, 6, 20)) },
                { "Summer", new Tuple<DateTime, DateTime>(new DateTime(year, 6, 21), new DateTime(year, 9, 22)) },
                { "Autumn", new Tuple<DateTime, DateTime>(new DateTime(year, 9, 23), new DateTime(year, 12, 20)) }
            };

            foreach (var season in seasons)
            {
                if (date >= season.Value.Item1 && date <= season.Value.Item2)
                {
                    return season.Key;
                }
            }

            return "Winter";
        }

        private async Task<object> FetchWeatherData(string lat, string lon, string startDate, string endDate)
        {
            var url = $"https://archive-api.open-meteo.com/v1/archive?latitude={lat}&longitude={lon}&start_date={startDate}&end_date={endDate}&hourly=temperature_2m,pressure_msl,relative_humidity_2m,cloud_cover,wind_speed_10m,wind_direction_10m&temperature_unit=celsius&wind_speed_unit=ms&timeformat=unixtime";
            var response = await _httpClient.GetStringAsync(url);
            return JsonConvert.DeserializeObject(response);
        }

        [HttpGet("historical")]
        public async Task<IActionResult> GetHistoricalWeather([FromQuery] string lat, [FromQuery] string lon, [FromQuery] string startDate, [FromQuery] string endDate)
        {
            if (string.IsNullOrEmpty(lat) || string.IsNullOrEmpty(lon) || string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
                return BadRequest("Latitude, Longitude, start date, and end date are required.");

            var weatherData = await FetchWeatherData(lat, lon, startDate, endDate);
            var weatherJson = JObject.Parse(weatherData.ToString());

            var timeData = weatherJson["hourly"]["time"].ToObject<List<long>>();

            var seasonData = new List<string>();
            foreach (var timestamp in timeData)
            {
                seasonData.Add(GetSeasonFromTimestamp(timestamp));
            }

            weatherJson["hourly"]["season"] = JArray.FromObject(seasonData);


            System.IO.File.WriteAllText("weather_with_season_final.json", JsonConvert.SerializeObject(weatherData, Formatting.Indented));

            using (var writer = new StreamWriter("weather_data.csv"))
            {
                writer.WriteLine("Time, Temperature, Pressure, Humidity, Cloud Cover, Wind Speed, Wind Direction, Season");

                var hourlyData = weatherJson["hourly"];
                var seasons = hourlyData["season"].ToObject<List<string>>();

                for (int i = 0; i < hourlyData["time"].Count(); i++)
                {
                    var time = hourlyData["time"][i].ToString();
                    var temperature = hourlyData["temperature_2m"][i].ToString();
                    var pressure_msl = hourlyData["pressure_msl"][i].ToString();
                    var relative_humidity_2m = hourlyData["relative_humidity_2m"][i].ToString();
                    var cloud_cover = hourlyData["cloud_cover"][i].ToString();
                    var wind_speed_10m = hourlyData["wind_speed_10m"][i].ToString();
                    var wind_direction_10m = hourlyData["wind_direction_10m"][i].ToString();
                    var season = seasons[i];
                    writer.WriteLine($"{time},{temperature}, {pressure_msl}, {relative_humidity_2m}, {cloud_cover}, {wind_speed_10m}, {wind_direction_10m}, {season}");
                }
            }

            return Ok("Weather data with seasons and CSV file saved.");
        }
    }
}
