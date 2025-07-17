namespace HelloWeatherApi.Models
{
    public class WeatherRequest
    {
        public List<string> Cities { get; set; } = new List<string>();
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool UseCurrentLocation { get; set; } = false;
    }
}
