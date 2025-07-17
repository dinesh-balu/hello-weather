using HelloWeatherApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HelloWeatherApi.Data
{
    public class WeatherDbContext : DbContext
    {
        public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options)
        {
        }

        public DbSet<UserCityPreference> UserCityPreferences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserCityPreference>(entity =>
            {
                entity.ToTable("user_city_preferences");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CityName).HasColumnName("city_name");
                entity.Property(e => e.CountryCode).HasColumnName("country_code");
                entity.Property(e => e.Latitude).HasColumnName("latitude");
                entity.Property(e => e.Longitude).HasColumnName("longitude");
                entity.Property(e => e.IsFavorite).HasColumnName("is_favorite");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.LastAccessed).HasColumnName("last_accessed");
                
                entity.HasIndex(e => e.CityName);
            });
        }
    }
}
