using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GardenTrackerApp.Data;
using GardenTrackerApp.Models;

namespace GardenTrackerApp.Pages.Bailey
{
    public class CreateModel : PageModel
    {
        private readonly GardenTrackerAppContext _context;
        private readonly IWebHostEnvironment _env;

        public CreateModel(GardenTrackerAppContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [BindProperty]
        public Plant Plant { get; set; } = default!;

        [BindProperty]
        public IFormFile? PlantImage { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (PlantImage != null)
            {
               
                var fileName = Guid.NewGuid() + Path.GetExtension(PlantImage.FileName);
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await PlantImage.CopyToAsync(stream);
                }


                Plant.ImagePath = "/uploads/" + fileName;
            }

            _context.Plant.Add(Plant);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
