using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TravelRecommendations.Auth.Model;
using TravelRecommendations.Data.Entities;

namespace TravelRecommendations.Data
{
    public class RestContext : IdentityDbContext<User>
    {
        // Constructor with DbContextOptions injection
        public RestContext(DbContextOptions<RestContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Session> Sessions { get; set; }

        // Remove OnConfiguring entirely
        // Database configuration is now done in Program.cs
    }
}
