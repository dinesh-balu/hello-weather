using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace HelloWeatherApi.Models
{
    public class WeatherData
    {
        [JsonProperty("city")]
        public string City { get; set; } = string.Empty;
        
        [JsonProperty("country")]
        public string Country { get; set; } = string.Empty;
        
        [JsonProperty("temperature")]
        public double Temperature { get; set; }
        
        [JsonProperty("feels_like")]
        public double FeelsLike { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;
        
        [JsonProperty("humidity")]
        public double Humidity { get; set; }
        
        [JsonProperty("wind_speed")]
        public double WindSpeed { get; set; }
        
        [JsonProperty("pressure")]
        public double Pressure { get; set; }
        
        [JsonProperty("visibility")]
        public double Visibility { get; set; }
        
        [JsonProperty("uv_index")]
        public double? UvIndex { get; set; }
        
        [JsonProperty("last_updated")]
        public DateTime LastUpdated { get; set; }
    }

    public class WeatherRequest
    {
        [JsonProperty("cities")]
        public List<string> Cities { get; set; } = new List<string>();
        
        [JsonProperty("latitude")]
        public double? Latitude { get; set; }
        
        [JsonProperty("longitude")]
        public double? Longitude { get; set; }
        
        [JsonProperty("use_current_location")]
        public bool UseCurrentLocation { get; set; } = false;
    }

    public class WeatherResponse
    {
        [JsonProperty("weather_data")]
        public List<WeatherData> WeatherData { get; set; } = new List<WeatherData>();
        
        [JsonProperty("success")]
        public bool Success { get; set; }
        
        [JsonProperty("error_message")]
        public string? ErrorMessage { get; set; }
    }

    public class CityPreference
    {
        [JsonProperty("city_name")]
        public string CityName { get; set; } = string.Empty;
        
        [JsonProperty("country_code")]
        public string CountryCode { get; set; } = string.Empty;
        
        [JsonProperty("latitude")]
        public double Latitude { get; set; }
        
        [JsonProperty("longitude")]
        public double Longitude { get; set; }
        
        [JsonProperty("is_favorite")]
        public bool IsFavorite { get; set; } = false;
    }
}
