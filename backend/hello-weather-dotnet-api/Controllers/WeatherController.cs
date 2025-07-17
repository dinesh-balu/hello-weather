using HelloWeatherApi.Data;
using HelloWeatherApi.Models;
using HelloWeatherApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HelloWeatherApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;
        private readonly WeatherDbContext _context;

        public WeatherController(IWeatherService weatherService, WeatherDbContext context)
        {
            _weatherService = weatherService;
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<WeatherResponse>> GetWeather([FromBody] WeatherRequest request)
        {
            try
            {
                var weatherData = new List<WeatherData>();

                if (request.UseCurrentLocation && request.Latitude.HasValue && request.Longitude.HasValue)
                {
                    var weather = await _weatherService.GetWeatherByCoordinatesAsync(
                        request.Latitude.Value, request.Longitude.Value);
                    if (weather != null)
                    {
                        weatherData.Add(weather);
                    }
                }

                if (request.Cities.Any())
                {
                    var cityWeather = await _weatherService.GetWeatherForMultipleCitiesAsync(request.Cities);
                    weatherData.AddRange(cityWeather);
                }

                if (!weatherData.Any())
                {
                    return Ok(new WeatherResponse
                    {
                        WeatherData = new List<WeatherData>(),
                        Success = false,
                        ErrorMessage = "No weather data found for the provided locations"
                    });
                }

                return Ok(new WeatherResponse
                {
                    WeatherData = weatherData,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return Ok(new WeatherResponse
                {
                    WeatherData = new List<WeatherData>(),
                    Success = false,
                    ErrorMessage = $"Error fetching weather data: {ex.Message}"
                });
            }
        }
    }
}
