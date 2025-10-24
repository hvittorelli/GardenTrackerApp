using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using GardenTrackerApp.Data;
using GardenTrackerApp.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace GardenTrackerApp.Pages.Cade
{
    public class CreateModel : PageModel
    {
        private readonly GardenTrackerApp.Data.GardenTrackerAppContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CreateModel(GardenTrackerApp.Data.GardenTrackerAppContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Plant Plant { get; set; } = default!;

        [BindProperty]
        public IFormFile? ImageUpload { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Handle image upload
            if (ImageUpload != null)
            {
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

            _context.Plant.Add(Plant);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}