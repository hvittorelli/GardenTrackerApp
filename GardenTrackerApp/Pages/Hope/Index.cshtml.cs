using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GardenTrackerApp.Data;
using GardenTrackerApp.Models;

namespace GardenTrackerApp.Pages.Hope
{
    public class IndexModel : PageModel
    {
        private readonly GardenTrackerAppContext _context;

        public IndexModel(GardenTrackerAppContext context)
        {
            _context = context;
        }

        public IList<Plant> Plant { get; set; } = default!;
        public List<string> TypeList { get; set; } = new();
        public List<string> WaterList { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string CurrentFilter { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public string SelectedType { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public string SelectedWater { get; set; } = "";

        public async Task OnGetAsync()
        {
            // Build filter lists
            TypeList = await _context.Plant.Select(p => p.Type).Distinct().ToListAsync();
            WaterList = await _context.Plant.Select(p => p.WaterFrequency).Distinct().ToListAsync();

            // Base query
            var plants = _context.Plant.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(CurrentFilter))
            {
                plants = plants.Where(p => p.Name.Contains(CurrentFilter));
            }

            if (!string.IsNullOrEmpty(SelectedType))
            {
                plants = plants.Where(p => p.Type == SelectedType);
            }

            if (!string.IsNullOrEmpty(SelectedWater))
            {
                plants = plants.Where(p => p.WaterFrequency == SelectedWater);
            }

            Plant = await plants.ToListAsync();
        }
    }
}
