using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HelloWeatherApi.Models;
using HelloWeatherApi.Data;

namespace HelloWeatherApi.Controllers
{
    [ApiController]
    [Route("api/cities/preferences")]
    public class CityPreferencesController : ControllerBase
    {
        private readonly WeatherDbContext _context;
        private readonly ILogger<CityPreferencesController> _logger;

        public CityPreferencesController(WeatherDbContext context, ILogger<CityPreferencesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<CityPreference>>> GetCityPreferences()
        {
            try
            {
                var preferences = await _context.UserCityPreferences
                    .OrderByDescending(p => p.LastAccessed)
                    .Take(10)
                    .ToListAsync();

                var result = preferences.Select(p => new CityPreference
                {
                    CityName = p.CityName,
                    CountryCode = p.CountryCode,
                    Latitude = p.Latitude,
                    Longitude = p.Longitude,
                    IsFavorite = p.IsFavorite
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving city preferences");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddCityPreference([FromBody] CityPreference cityPreference)
        {
            try
            {
                var existingPreference = await _context.UserCityPreferences
                    .FirstOrDefaultAsync(p => p.CityName.ToLower() == cityPreference.CityName.ToLower());

                if (existingPreference != null)
                {
                    existingPreference.LastAccessed = DateTime.UtcNow;
                    existingPreference.IsFavorite = cityPreference.IsFavorite;
                }
                else
                {
                    var newPreference = new UserCityPreference
                    {
                        CityName = cityPreference.CityName,
                        CountryCode = cityPreference.CountryCode,
                        Latitude = cityPreference.Latitude,
                        Longitude = cityPreference.Longitude,
                        IsFavorite = cityPreference.IsFavorite,
                        CreatedAt = DateTime.UtcNow,
                        LastAccessed = DateTime.UtcNow
                    };

                    _context.UserCityPreferences.Add(newPreference);
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = "City preference saved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving city preference");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
