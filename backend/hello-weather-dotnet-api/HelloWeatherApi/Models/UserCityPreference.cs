using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelloWeatherApi.Models
{
    [Table("user_city_preferences")]
    public class UserCityPreference
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string CityName { get; set; } = string.Empty;
        
        public string CountryCode { get; set; } = string.Empty;
        
        public double Latitude { get; set; }
        
        public double Longitude { get; set; }
        
        public bool IsFavorite { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime LastAccessed { get; set; } = DateTime.UtcNow;
    }
}
