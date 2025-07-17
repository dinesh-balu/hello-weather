using HelloWeatherApi.Models;
using System.Text.Json;

namespace HelloWeatherApi.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        private readonly HashSet<string> _usCities = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "new york", "los angeles", "chicago", "houston", "phoenix", "philadelphia",
            "san antonio", "san diego", "dallas", "san jose", "austin", "jacksonville",
            "fort worth", "columbus", "charlotte", "san francisco", "indianapolis",
            "seattle", "denver", "washington", "boston", "el paso", "detroit", "nashville",
            "portland", "memphis", "oklahoma city", "las vegas", "louisville", "baltimore",
            "milwaukee", "albuquerque", "tucson", "fresno", "mesa", "sacramento",
            "atlanta", "kansas city", "colorado springs", "miami", "raleigh", "omaha",
            "long beach", "virginia beach", "oakland", "minneapolis", "tulsa", "arlington"
        };

        private readonly HashSet<string> _indianCities = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "mumbai", "delhi", "bangalore", "hyderabad", "ahmedabad", "chennai",
            "kolkata", "surat", "pune", "jaipur", "lucknow", "kanpur", "nagpur",
            "indore", "thane", "bhopal", "visakhapatnam", "pimpri-chinchwad",
            "patna", "vadodara", "ghaziabad", "ludhiana", "agra", "nashik",
            "faridabad", "meerut", "rajkot", "kalyan-dombivli", "vasai-virar",
            "varanasi", "srinagar", "aurangabad", "dhanbad", "amritsar",
            "navi mumbai", "allahabad", "ranchi", "howrah", "coimbatore",
            "jabalpur", "gwalior", "vijayawada", "jodhpur", "madurai", "raipur",
            "kota", "guwahati", "chandigarh", "solapur", "hubli-dharwad"
        };

        public WeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _apiKey = Environment.GetEnvironmentVariable("OPENWEATHER_API_KEY") ?? 
                     _configuration["WeatherApi:OpenWeatherMapApiKey"] ?? "demo_key";
            _baseUrl = _configuration["WeatherApi:OpenWeatherMapBaseUrl"] ?? 
                      "https://api.openweathermap.org/data/2.5";
        }

        public async Task<WeatherData?> GetWeatherByCoordinatesAsync(double lat, double lon)
        {
            try
            {
                var url = $"{_baseUrl}/weather?lat={lat}&lon={lon}&appid={_apiKey}&units=metric";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                
                var jsonString = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<JsonElement>(jsonString);
                
                return ParseOpenWeatherResponse(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching weather by coordinates: {ex.Message}");
                return null;
            }
        }

        public async Task<WeatherData?> GetWeatherByCityAsync(string cityName, string? countryCode = null)
        {
            try
            {
                var query = countryCode != null ? $"{cityName},{countryCode}" : cityName;
                var url = $"{_baseUrl}/weather?q={query}&appid={_apiKey}&units=metric";
                
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                
                var jsonString = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<JsonElement>(jsonString);
                
                return ParseOpenWeatherResponse(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching weather for city {cityName}: {ex.Message}");
                return null;
            }
        }

        public async Task<List<WeatherData>> GetWeatherForMultipleCitiesAsync(List<string> cities)
        {
            var weatherData = new List<WeatherData>();
            
            foreach (var city in cities)
            {
                var countryCode = DetermineCountryCode(city);
                var weather = await GetWeatherByCityAsync(city, countryCode);
                if (weather != null)
                {
                    weatherData.Add(weather);
                }
            }
            
            return weatherData;
        }

        private string? DetermineCountryCode(string city)
        {
            var cityLower = city.ToLower().Trim();
            
            if (_usCities.Contains(cityLower))
                return "US";
            else if (_indianCities.Contains(cityLower))
                return "IN";
            
            return null;
        }

        private WeatherData ParseOpenWeatherResponse(JsonElement data)
        {
            return new WeatherData
            {
                City = data.GetProperty("name").GetString() ?? "",
                Country = data.GetProperty("sys").GetProperty("country").GetString() ?? "",
                Temperature = data.GetProperty("main").GetProperty("temp").GetDouble(),
                FeelsLike = data.GetProperty("main").GetProperty("feels_like").GetDouble(),
                Description = data.GetProperty("weather")[0].GetProperty("description").GetString()?.ToTitleCase() ?? "",
                Humidity = data.GetProperty("main").GetProperty("humidity").GetDouble(),
                WindSpeed = data.GetProperty("wind").GetProperty("speed").GetDouble(),
                Pressure = data.GetProperty("main").GetProperty("pressure").GetDouble(),
                Visibility = data.TryGetProperty("visibility", out var vis) ? vis.GetDouble() / 1000.0 : 0,
                LastUpdated = DateTime.UtcNow
            };
        }
    }

    public static class StringExtensions
    {
        public static string ToTitleCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
        }
    }
}
