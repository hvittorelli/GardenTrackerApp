using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GardenTrackerApp.Data;
using GardenTrackerApp.Models;

namespace GardenTrackerApp.Pages.Bailey
{
    public class IndexModel : PageModel
    {
        private readonly GardenTrackerApp.Data.GardenTrackerAppContext _context;

        public IndexModel(GardenTrackerApp.Data.GardenTrackerAppContext context)
        {
            _context = context;
        }

        public IList<Plant> Plant { get;set; } = default!;

        //sort
        public string NameSort { get; set; } = "name_desc";
        public string TypeSort { get; set; }
        public string WaterFrequencySort { get; set; }

        public string CurrentSort { get; set; }

        //search

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        public async Task OnGetAsync(string? sortOrder)
        {
            CurrentSort = sortOrder;
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            TypeSort = sortOrder == "Type" ? "type_desc" : "Type";
            WaterFrequencySort = sortOrder == "WaterFrequency" ? "waterfrequency_desc" : "WaterFrequency";


            var plants = from p in _context.Plant
                         select p;

            //filter
            if (!string.IsNullOrEmpty(SearchString))
            {
                plants = plants.Where(p =>
                    p.Name.Contains(SearchString) ||
                    p.Type.Contains(SearchString) ||
                    p.WaterFrequency.Contains(SearchString)
                );
            }


            // sort
            plants = sortOrder switch
            {
                "name_desc" => plants.OrderByDescending(p => p.Name),
                "Type" => plants.OrderBy(p => p.Type),
                "type_desc" => plants.OrderByDescending(p => p.Type),
                "WaterFrequency" => plants.OrderBy(p => p.WaterFrequency),
                "waterfrequency_desc" => plants.OrderByDescending(p => p.WaterFrequency),
                _ => plants.OrderBy(p => p.Name),
            };

            Plant = await plants.AsNoTracking().ToListAsync();
        }
    }
}
