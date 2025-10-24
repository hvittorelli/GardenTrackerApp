using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GardenTrackerApp.Data;
using GardenTrackerApp.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace GardenTrackerApp.Pages.Cade
{
    public class EditModel : PageModel
    {
        private readonly GardenTrackerApp.Data.GardenTrackerAppContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EditModel(GardenTrackerApp.Data.GardenTrackerAppContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public Plant Plant { get; set; } = default!;

        [BindProperty]
        public IFormFile? ImageUpload { get; set; }

        [BindProperty]
        public bool RemoveImage { get; set; }

        public string? CurrentImagePath { get; set; }

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
            Plant = plant;
            CurrentImagePath = plant.ImagePath;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Get the existing plant from database to preserve the old image path
            var existingPlant = await _context.Plant.AsNoTracking().FirstOrDefaultAsync(p => p.Id == Plant.Id);
            string? oldImagePath = existingPlant?.ImagePath;

            // Handle image removal
            if (RemoveImage && !string.IsNullOrEmpty(oldImagePath))
            {
                DeleteImageFile(oldImagePath);
                Plant.ImagePath = null;
            }
            // Handle new image upload
            else if (ImageUpload != null)
            {
                // Delete old image if it exists
                if (!string.IsNullOrEmpty(oldImagePath))
                {
                    DeleteImageFile(oldImagePath);
                }

                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                // Create images folder if it doesn't exist
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate unique filename
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageUpload.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageUpload.CopyToAsync(fileStream);
                }

                // Store relative path in database
                Plant.ImagePath = "/images/" + uniqueFileName;
            }
            else
            {
                // Keep existing image
                Plant.ImagePath = oldImagePath;
            }

            _context.Attach(Plant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlantExists(Plant.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool PlantExists(int id)
        {
            return _context.Plant.Any(e => e.Id == id);
        }

        private void DeleteImageFile(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
        }
    }
}