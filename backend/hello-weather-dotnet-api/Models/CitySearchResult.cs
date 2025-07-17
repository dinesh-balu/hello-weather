namespace HelloWeatherApi.Models
{
    public class CitySearchResult
    {
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}
