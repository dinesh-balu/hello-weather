namespace HelloWeatherApi.Models
{
    public class WeatherResponse
    {
        public List<WeatherData> WeatherData { get; set; } = new List<WeatherData>();
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
