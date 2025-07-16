using HelloWeatherApi.Models;
using Newtonsoft.Json;
using System.Text;

namespace HelloWeatherApi.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _openWeatherApiKey;
        private readonly string _openWeatherBaseUrl = "https://api.openweathermap.org/data/2.5";
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(HttpClient httpClient, IConfiguration configuration, ILogger<WeatherService> logger)
        {
            _httpClient = httpClient;
            _openWeatherApiKey = configuration["OpenWeatherApiKey"] ?? "demo_key";
            _logger = logger;
        }

        public async Task<WeatherData?> GetWeatherByCoordinatesAsync(double lat, double lon)
        {
            try
            {
                var url = $"{_openWeatherBaseUrl}/weather?lat={lat}&lon={lon}&appid={_openWeatherApiKey}&units=metric";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                
                var jsonContent = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<dynamic>(jsonContent);
                
                return ParseOpenWeatherResponse(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching weather by coordinates: {Lat}, {Lon}", lat, lon);
                return null;
            }
        }

        public async Task<WeatherData?> GetWeatherByCityAsync(string cityName, string? countryCode = null)
        {
            try
            {
                var query = countryCode != null ? $"{cityName},{countryCode}" : cityName;
                var url = $"{_openWeatherBaseUrl}/weather?q={Uri.EscapeDataString(query)}&appid={_openWeatherApiKey}&units=metric";
                
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                
                var jsonContent = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<dynamic>(jsonContent);
                
                return ParseOpenWeatherResponse(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching weather for city {CityName}", cityName);
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
            var usCities = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
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

            var indianCities = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
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

            var cityLower = city.ToLower().Trim();
            
            if (usCities.Contains(cityLower))
                return "US";
            else if (indianCities.Contains(cityLower))
                return "IN";
            
            return null;
        }

        private WeatherData ParseOpenWeatherResponse(dynamic data)
        {
            return new WeatherData
            {
                City = data.name,
                Country = data.sys.country,
                Temperature = data.main.temp,
                FeelsLike = data.main.feels_like,
                Description = ((string)data.weather[0].description).ToTitleCase(),
                Humidity = data.main.humidity,
                WindSpeed = data.wind.speed,
                Pressure = data.main.pressure,
                Visibility = data.visibility != null ? (double)data.visibility / 1000.0 : 0.0,
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

            var words = input.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
                }
            }
            return string.Join(" ", words);
        }
    }
}
