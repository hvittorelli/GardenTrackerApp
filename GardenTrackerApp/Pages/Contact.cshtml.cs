using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GardenTrackerApp.Pages
{
    public class ContactModel : PageModel
    {
        private readonly ILogger<ContactModel> _logger;

        public ContactModel(ILogger<ContactModel> logger)
        {
            _logger = logger;
        }

        [BindProperty]
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters")]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Subject is required")]
        [StringLength(200, ErrorMessage = "Subject cannot exceed 200 characters")]
        public string Subject { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Message is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Message must be between 10 and 2000 characters")]
        public string Message { get; set; } = string.Empty;

        [TempData]
        public bool MessageSent { get; set; }

        public void OnGet()
        {
            _logger.LogInformation("Contact page accessed");
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Log the contact form submission
            _logger.LogInformation(
                "Contact form submitted - Name: {Name}, Email: {Email}, Subject: {Subject}",
                Name,
                Email,
                Subject
            );

            // In a real application, you would:
            // 1. Send an email using an email service
            // 2. Save to a database
            // 3. Integrate with a CRM system

            // For now, we'll just simulate success
            MessageSent = true;

            // Redirect to clear the form
            return RedirectToPage();
        }
    }
}