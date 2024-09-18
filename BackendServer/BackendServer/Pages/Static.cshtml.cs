using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BackendServer.Pages
{
    public class StaticModel : PageModel
    {
        private readonly IWebHostEnvironment _environment;

        public StaticModel(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [BindProperty]
        public string SelectedFile { get; set; }

        [BindProperty]
        public string FileContent { get; set; }

        public List<SelectListItem> FileOptions { get; set; }

        public string SaveResult { get; set; }

        public void OnGet()
        {
            // Populate the dropdown options
            FileOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "AGB.txt", Text = "AGB" },
                new SelectListItem { Value = "impressum.txt", Text = "Impressum" },
                new SelectListItem { Value = "FAQ.txt", Text = "FAQ" }
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Populate the dropdown options again for rendering
            OnGet();

            

            // Check if we're saving or loading content
            if (!string.IsNullOrWhiteSpace(SelectedFile))
            {
                var filePath = Path.Combine(_environment.WebRootPath, SelectedFile);

                if (System.IO.File.Exists(filePath))
                {
                    if (!string.IsNullOrWhiteSpace(FileContent))
                    {
                        // If the form is submitted with content, save the content
                        await System.IO.File.WriteAllTextAsync(filePath, FileContent);
                        SaveResult = $"The content of {SelectedFile} has been updated.";
                    }
                    else
                    {
                        // If the form is submitted without content, load the content
                        FileContent = await System.IO.File.ReadAllTextAsync(filePath);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "File not found.");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Please select a file.");
            }

            return Page();
        }
    }
}
