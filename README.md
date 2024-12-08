# WeatherWebAppGD - Weather Web API

A simple ASP.NET Core Web API that retrieves current weather data from the OpenWeatherMap API based on a city name.

## Features:
- Fetches weather and temperature (in Celsius).
- Securely handles API key via environment variable.

## How to Run:
1. Set `weather_api_key` environment variable with your OpenWeatherMap API key.
2. Run via Visual Studio or `dotnet run`.
3. Access the API at `/api/weather?city=<city_name>`.

