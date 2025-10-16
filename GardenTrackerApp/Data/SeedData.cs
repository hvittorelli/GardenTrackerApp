using GardenTrackerApp.Models;
using Microsoft.EntityFrameworkCore;

namespace GardenTrackerApp.Data
{
    public static class SeedData
    {
        public static void Initialize(GardenTrackerAppContext context)
        {
            context.Database.EnsureCreated();

            if (context.Plant.Any()) return; // DB has data

            context.Plant.AddRange(
                new Plant { Name = "Basil", Type = "Herb", WaterFrequency = "Daily", Notes = "Pinch flowers to encourage leaves", ImagePath = "/images/basil.jpg" },
                new Plant { Name = "Spider Plant", Type = "Houseplant", WaterFrequency = "Daily", Notes = "Likes humidity", ImagePath = "/images/spiderplant.jpg" },
                new Plant { Name = "Tomato", Type = "Vegetable", WaterFrequency = "Every other day", Notes = "Stake early", ImagePath = "/images/tomato.jpg" }
            );

            context.SaveChanges();
        }
    }
}
