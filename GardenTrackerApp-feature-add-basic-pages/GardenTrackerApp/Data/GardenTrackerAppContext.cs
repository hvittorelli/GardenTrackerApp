using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GardenTrackerApp.Models;

namespace GardenTrackerApp.Data
{
    public class GardenTrackerAppContext : DbContext
    {
        public GardenTrackerAppContext (DbContextOptions<GardenTrackerAppContext> options)
            : base(options)
        {
        }

        public DbSet<GardenTrackerApp.Models.Plant> Plant { get; set; } = default!;
    }
}
