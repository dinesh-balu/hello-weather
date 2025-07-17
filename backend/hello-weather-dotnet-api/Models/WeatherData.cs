namespace HelloWeatherApi.Models
{
    public class WeatherData
    {
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public double Temperature { get; set; }
        public double FeelsLike { get; set; }
        public string Description { get; set; } = string.Empty;
        public double Humidity { get; set; }
        public double WindSpeed { get; set; }
        public double Pressure { get; set; }
        public double Visibility { get; set; }
        public double? UvIndex { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
