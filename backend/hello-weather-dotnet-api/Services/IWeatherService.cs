using HelloWeatherApi.Models;

namespace HelloWeatherApi.Services
{
    public interface IWeatherService
    {
        Task<WeatherData?> GetWeatherByCoordinatesAsync(double lat, double lon);
        Task<WeatherData?> GetWeatherByCityAsync(string cityName, string? countryCode = null);
        Task<List<WeatherData>> GetWeatherForMultipleCitiesAsync(List<string> cities);
    }
}
