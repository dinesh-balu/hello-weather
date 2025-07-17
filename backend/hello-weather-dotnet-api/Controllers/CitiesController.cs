using HelloWeatherApi.Data;
using HelloWeatherApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelloWeatherApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        private readonly WeatherDbContext _context;

        private readonly List<CitySearchResult> _usCities = new List<CitySearchResult>
        {
            new CitySearchResult { Name = "New York", Country = "US", Lat = 40.7128, Lon = -74.0060 },
            new CitySearchResult { Name = "Los Angeles", Country = "US", Lat = 34.0522, Lon = -118.2437 },
            new CitySearchResult { Name = "Chicago", Country = "US", Lat = 41.8781, Lon = -87.6298 },
            new CitySearchResult { Name = "Houston", Country = "US", Lat = 29.7604, Lon = -95.3698 },
            new CitySearchResult { Name = "Phoenix", Country = "US", Lat = 33.4484, Lon = -112.0740 },
            new CitySearchResult { Name = "Philadelphia", Country = "US", Lat = 39.9526, Lon = -75.1652 },
            new CitySearchResult { Name = "San Antonio", Country = "US", Lat = 29.4241, Lon = -98.4936 },
            new CitySearchResult { Name = "San Diego", Country = "US", Lat = 32.7157, Lon = -117.1611 },
            new CitySearchResult { Name = "Dallas", Country = "US", Lat = 32.7767, Lon = -96.7970 },
            new CitySearchResult { Name = "San Jose", Country = "US", Lat = 37.3382, Lon = -121.8863 }
        };

        private readonly List<CitySearchResult> _indianCities = new List<CitySearchResult>
        {
            new CitySearchResult { Name = "Mumbai", Country = "IN", Lat = 19.0760, Lon = 72.8777 },
            new CitySearchResult { Name = "Delhi", Country = "IN", Lat = 28.7041, Lon = 77.1025 },
            new CitySearchResult { Name = "Bangalore", Country = "IN", Lat = 12.9716, Lon = 77.5946 },
            new CitySearchResult { Name = "Hyderabad", Country = "IN", Lat = 17.3850, Lon = 78.4867 },
            new CitySearchResult { Name = "Ahmedabad", Country = "IN", Lat = 23.0225, Lon = 72.5714 },
            new CitySearchResult { Name = "Chennai", Country = "IN", Lat = 13.0827, Lon = 80.2707 },
            new CitySearchResult { Name = "Kolkata", Country = "IN", Lat = 22.5726, Lon = 88.3639 },
            new CitySearchResult { Name = "Surat", Country = "IN", Lat = 21.1702, Lon = 72.8311 },
            new CitySearchResult { Name = "Pune", Country = "IN", Lat = 18.5204, Lon = 73.8567 },
            new CitySearchResult { Name = "Jaipur", Country = "IN", Lat = 26.9124, Lon = 75.7873 }
        };

        public CitiesController(WeatherDbContext context)
        {
            _context = context;
        }

        [HttpGet("preferences")]
        public async Task<ActionResult<List<object>>> GetCityPreferences()
        {
            var preferences = await _context.UserCityPreferences
                .OrderByDescending(p => p.LastAccessed)
                .ToListAsync();

            var result = preferences.Select(pref => new
            {
                id = pref.Id,
                city_name = pref.CityName,
                country_code = pref.CountryCode,
                latitude = pref.Latitude,
                longitude = pref.Longitude,
                is_favorite = pref.IsFavorite,
                last_accessed = pref.LastAccessed
            }).ToList();

            return Ok(result);
        }

        [HttpPost("preferences")]
        public async Task<ActionResult<object>> SaveCityPreference([FromBody] CityPreference preference)
        {
            var existing = await _context.UserCityPreferences
                .FirstOrDefaultAsync(p => p.CityName == preference.CityName && 
                                         p.CountryCode == preference.CountryCode);

            if (existing != null)
            {
                existing.IsFavorite = preference.IsFavorite;
                existing.LastAccessed = DateTime.UtcNow;
            }
            else
            {
                var newPreference = new UserCityPreference
                {
                    CityName = preference.CityName,
                    CountryCode = preference.CountryCode,
                    Latitude = preference.Latitude,
                    Longitude = preference.Longitude,
                    IsFavorite = preference.IsFavorite,
                    LastAccessed = DateTime.UtcNow
                };
                _context.UserCityPreferences.Add(newPreference);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "City preference saved successfully" });
        }

        [HttpGet("search")]
        public ActionResult<List<CitySearchResult>> SearchCities([FromQuery] string q)
        {
            var allCities = _usCities.Concat(_indianCities).ToList();
            var queryLower = q.ToLower();

            var matchingCities = allCities
                .Where(city => city.Name.ToLower().Contains(queryLower))
                .Take(10)
                .ToList();

            return Ok(matchingCities);
        }
    }
}
