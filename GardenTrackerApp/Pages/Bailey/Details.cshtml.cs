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
    public class ViewModel : PageModel
    {
        private readonly GardenTrackerAppContext _context;

        public ViewModel(GardenTrackerAppContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Plant Plant { get; set; } = default!;

        public IActionResult OnGet(int id)
        {
            Plant = _context.Plant.FirstOrDefault(p => p.Id == id);

            if (Plant == null)
            {
                return RedirectToPage("Index");
            }

            return Page();
        }
    }
}
