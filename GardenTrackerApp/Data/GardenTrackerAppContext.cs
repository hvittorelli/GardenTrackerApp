using Microsoft.EntityFrameworkCore;
using GardenTrackerApp.Models;

namespace GardenTrackerApp.Data
{
    public class GardenTrackerAppContext : DbContext
    {
        public GardenTrackerAppContext(DbContextOptions<GardenTrackerAppContext> options)
            : base(options)
        {
        }

        public DbSet<Plant> Plant { get; set; } = null!;
    }
}
