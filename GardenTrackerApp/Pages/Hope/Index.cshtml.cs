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
        private readonly GardenTrackerApp.Data.GardenTrackerAppContext _context;

        public IndexModel(GardenTrackerApp.Data.GardenTrackerAppContext context)
        {
            _context = context;
        }

        public IList<Plant> Plant { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Plant = await _context.Plant.ToListAsync();
        }
    }
}
