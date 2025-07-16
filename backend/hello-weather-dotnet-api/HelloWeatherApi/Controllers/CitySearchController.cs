using Microsoft.AspNetCore.Mvc;
using HelloWeatherApi.Models;

namespace HelloWeatherApi.Controllers
{
    [ApiController]
    [Route("api/cities/search")]
    public class CitySearchController : ControllerBase
    {
        private readonly ILogger<CitySearchController> _logger;

        public CitySearchController(ILogger<CitySearchController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<List<string>> SearchCities([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                {
                    return Ok(new List<string>());
                }

                var usCities = new List<string>
                {
                    "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia",
                    "San Antonio", "San Diego", "Dallas", "San Jose", "Austin", "Jacksonville",
                    "Fort Worth", "Columbus", "Charlotte", "San Francisco", "Indianapolis",
                    "Seattle", "Denver", "Washington", "Boston", "El Paso", "Detroit", "Nashville",
                    "Portland", "Memphis", "Oklahoma City", "Las Vegas", "Louisville", "Baltimore",
                    "Milwaukee", "Albuquerque", "Tucson", "Fresno", "Mesa", "Sacramento",
                    "Atlanta", "Kansas City", "Colorado Springs", "Miami", "Raleigh", "Omaha",
                    "Long Beach", "Virginia Beach", "Oakland", "Minneapolis", "Tulsa", "Arlington"
                };

                var indianCities = new List<string>
                {
                    "Mumbai", "Delhi", "Bangalore", "Hyderabad", "Ahmedabad", "Chennai",
                    "Kolkata", "Surat", "Pune", "Jaipur", "Lucknow", "Kanpur", "Nagpur",
                    "Indore", "Thane", "Bhopal", "Visakhapatnam", "Pimpri-Chinchwad",
                    "Patna", "Vadodara", "Ghaziabad", "Ludhiana", "Agra", "Nashik",
                    "Faridabad", "Meerut", "Rajkot", "Kalyan-Dombivli", "Vasai-Virar",
                    "Varanasi", "Srinagar", "Aurangabad", "Dhanbad", "Amritsar",
                    "Navi Mumbai", "Allahabad", "Ranchi", "Howrah", "Coimbatore",
                    "Jabalpur", "Gwalior", "Vijayawada", "Jodhpur", "Madurai", "Raipur",
                    "Kota", "Guwahati", "Chandigarh", "Solapur", "Hubli-Dharwad"
                };

                var allCities = usCities.Concat(indianCities).ToList();
                var filteredCities = allCities
                    .Where(city => city.ToLower().Contains(query.ToLower()))
                    .OrderBy(city => city)
                    .Take(10)
                    .ToList();

                return Ok(filteredCities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching cities");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
