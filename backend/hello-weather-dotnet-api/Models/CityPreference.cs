namespace HelloWeatherApi.Models
{
    public class CityPreference
    {
        public string CityName { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsFavorite { get; set; } = false;
    }
}
