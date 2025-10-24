using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GardenTrackerApp.Data;
using GardenTrackerApp.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace GardenTrackerApp.Pages.Cade
{
    public class DeleteModel : PageModel
    {
        private readonly GardenTrackerApp.Data.GardenTrackerAppContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DeleteModel(GardenTrackerApp.Data.GardenTrackerAppContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public Plant Plant { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plant = await _context.Plant.FirstOrDefaultAsync(m => m.Id == id);
            if (plant == null)
            {
                return NotFound();
            }
            else
            {
                Plant = plant;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plant = await _context.Plant.FindAsync(id);
            if (plant != null)
            {
                Plant = plant;

                // Delete associated image file if it exists
                if (!string.IsNullOrEmpty(plant.ImagePath))
                {
                    DeleteImageFile(plant.ImagePath);
                }

                _context.Plant.Remove(Plant);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }

        private void DeleteImageFile(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    try
                    {
                        System.IO.File.Delete(fullPath);
                    }
                    catch (Exception)
                    {
                        // Log error if needed, but don't prevent deletion of the plant record
                    }
                }
            }
        }
    }
}