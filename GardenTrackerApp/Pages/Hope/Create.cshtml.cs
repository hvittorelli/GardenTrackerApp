using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using GardenTrackerApp.Data;
using GardenTrackerApp.Models;


namespace GardenTrackerApp.Pages.Hope
{
    public class CreateModel : PageModel
    {
        private readonly GardenTrackerApp.Data.GardenTrackerAppContext _context;
        private readonly IWebHostEnvironment _environment;

        public CreateModel(GardenTrackerApp.Data.GardenTrackerAppContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Plant Plant { get; set; } = default!;

        [BindProperty]
        public IFormFile? PlantImage { get; set; }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

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

                Plant.ImagePath = $"/images/{fileName}";
            }

            _context.Plant.Add(Plant);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
