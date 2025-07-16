using Microsoft.EntityFrameworkCore;
using HelloWeatherApi.Models;

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
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<UserCityPreference>(entity =>
            {
                entity.HasIndex(e => e.CityName);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");
                entity.Property(e => e.LastAccessed).HasDefaultValueSql("datetime('now')");
            });
        }
    }
}
