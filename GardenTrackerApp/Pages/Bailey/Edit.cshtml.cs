using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GardenTrackerApp.Data;
using GardenTrackerApp.Models;

namespace GardenTrackerApp.Pages.Bailey
{
    public class EditModel : PageModel
    {
        private readonly GardenTrackerAppContext _context;
        private readonly IWebHostEnvironment _env;

        public EditModel(GardenTrackerAppContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [BindProperty]
        public Plant Plant { get; set; } = default!;

        [BindProperty]
        public IFormFile? PlantImage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var plant = await _context.Plant.FirstOrDefaultAsync(p => p.Id == id);
            if (plant == null) return NotFound();

            Plant = plant;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var plantToUpdate = await _context.Plant.FindAsync(Plant.Id);
            if (plantToUpdate == null) return NotFound();

         
            plantToUpdate.Name = Plant.Name;
            plantToUpdate.Type = Plant.Type;
            plantToUpdate.WaterFrequency = Plant.WaterFrequency;
            plantToUpdate.Notes = Plant.Notes;

         
            if (PlantImage != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(PlantImage.FileName);
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await PlantImage.CopyToAsync(stream);
                }

                plantToUpdate.ImagePath = "/uploads/" + fileName;
            }


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Plant.Any(e => e.Id == Plant.Id)) return NotFound();
                throw;
            }

            return RedirectToPage("./Index");
        }
    }
}
