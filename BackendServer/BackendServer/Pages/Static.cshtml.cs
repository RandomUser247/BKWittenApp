using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;

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
        PopulateFileOptions();

        if (string.IsNullOrEmpty(SelectedFile))
        {
            // Default to AGB.txt if no file is selected on the first load
            SelectedFile = "AGB.txt";
        }

        LoadFileContent();
    }

    private void PopulateFileOptions()
    {
        // Populate the dropdown options
        FileOptions = new List<SelectListItem>
        {
            new SelectListItem { Value = "AGB.txt", Text = "AGB" },
            new SelectListItem { Value = "impressum.txt", Text = "Impressum" },
            new SelectListItem { Value = "FAQ.txt", Text = "FAQ" }
        };
    }

    private void LoadFileContent()
    {
        var filePath = Path.Combine(_environment.WebRootPath, SelectedFile);
        if (System.IO.File.Exists(filePath))
        {
            FileContent = System.IO.File.ReadAllText(filePath); // Load the content of the selected file
        }
        else
        {
            FileContent = string.Empty; // Clear content if file is not found
        }
    }

    public async Task<IActionResult> OnPostAsync(string action)
    {
        // Repopulate dropdown options during the POST request
        PopulateFileOptions();

        if (action == "Load")
        {
            // Load the content of the newly selected file
            LoadFileContent();
        }
        else if (action == "Save")
        {
            // Save the content of the selected file
            var filePath = Path.Combine(_environment.WebRootPath, SelectedFile);
            if (!string.IsNullOrWhiteSpace(FileContent))
            {
                await System.IO.File.WriteAllTextAsync(filePath, FileContent);
                SaveResult = $"The content of {SelectedFile} has been updated.";
            }
        }

        return Page();
    }
}
