using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GardenTrackerApp.Data;
using GardenTrackerApp.Models;
using Microsoft.EntityFrameworkCore;

namespace GardenTrackerApp.Pages.Hope
{
    public class EditModel : PageModel
    {
        private readonly GardenTrackerAppContext _context;
        private readonly IWebHostEnvironment _environment;

        public EditModel(GardenTrackerAppContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Plant Plant { get; set; } = default!;

        [BindProperty]
        public IFormFile? PlantImage { get; set; }  // <-- Add this

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var plant = await _context.Plant.FirstOrDefaultAsync(p => p.Id == id);

            if (plant == null)
                return NotFound();

            Plant = plant;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var plantToUpdate = await _context.Plant.FindAsync(Plant.Id);
            if (plantToUpdate == null)
                return NotFound();

            // Update properties
            plantToUpdate.Name = Plant.Name;
            plantToUpdate.Type = Plant.Type;
            plantToUpdate.WaterFrequency = Plant.WaterFrequency;
            plantToUpdate.Notes = Plant.Notes;

            // Handle image upload if there is a new file
            if (PlantImage != null)
            {
                string folderPath = Path.Combine(_environment.WebRootPath, "images");
                Directory.CreateDirectory(folderPath);

                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(PlantImage.FileName)}";
                string fullPath = Path.Combine(folderPath, fileName);

                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await PlantImage.CopyToAsync(fileStream);
                }

                plantToUpdate.ImagePath = $"/images/{fileName}";
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
