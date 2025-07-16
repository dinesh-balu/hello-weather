using Microsoft.AspNetCore.Mvc;
using HelloWeatherApi.Models;
using HelloWeatherApi.Services;

namespace HelloWeatherApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _weatherService;
        private readonly ILogger<WeatherController> _logger;

        public WeatherController(WeatherService weatherService, ILogger<WeatherController> logger)
        {
            _weatherService = weatherService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<WeatherResponse>> GetWeather([FromBody] WeatherRequest request)
        {
            try
            {
                var weatherData = new List<WeatherData>();

                if (request.UseCurrentLocation && request.Latitude.HasValue && request.Longitude.HasValue)
                {
                    var weather = await _weatherService.GetWeatherByCoordinatesAsync(request.Latitude.Value, request.Longitude.Value);
                    if (weather != null)
                    {
                        weatherData.Add(weather);
                    }
                }
                else if (request.Cities.Any())
                {
                    weatherData = await _weatherService.GetWeatherForMultipleCitiesAsync(request.Cities);
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
                _logger.LogError(ex, "Error processing weather request");
                return StatusCode(500, new WeatherResponse
                {
                    WeatherData = new List<WeatherData>(),
                    Success = false,
                    ErrorMessage = "Internal server error"
                });
            }
        }
    }
}
