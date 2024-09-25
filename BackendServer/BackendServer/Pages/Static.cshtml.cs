using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[Authorize(Policy = "RequireAdminRole")]
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
    public bool FileNotFound { get; set; }

    private const string AGB = "AGB.txt";
    private const string Impressum = "impressum.txt";
    private const string FAQ = "FAQ.txt";

    public string WebRootPath => _environment.WebRootPath;

    private void PopulateFileOptions()
    {
        FileOptions = new List<SelectListItem>
        {
            new SelectListItem { Value = AGB, Text = "AGB" },
            new SelectListItem { Value = Impressum, Text = "Impressum" },
            new SelectListItem { Value = FAQ, Text = "FAQ" }
        };
    }

    public void OnGet()
    {
        PopulateFileOptions();
    }

    public async Task<IActionResult> OnPostAsync(string action)
    {
        PopulateFileOptions();

        if (!IsValidFile(SelectedFile))
        {
            SaveResult = "Invalid file selection.";
            return Page();
        }

        if (action == "Save")
        {
            await SaveFileAsync();
        }

        return Page();
    }

    public IActionResult OnGetLoad(string file)
    {
        if (IsValidFile(file) && FileExists(file))
        {
            var filePath = Path.Combine(WebRootPath, file);
            var content = System.IO.File.ReadAllText(filePath);
            return Content(content);
        }

        return Content("File not found.");
    }

    private async Task SaveFileAsync()
    {
        var filePath = Path.Combine(WebRootPath, SelectedFile);
        if (!string.IsNullOrWhiteSpace(FileContent))
        {
            await System.IO.File.WriteAllTextAsync(filePath, FileContent);
            SaveResult = $"The content of {SelectedFile} has been updated.";
        }
        else
        {
            SaveResult = "File content is empty and was not saved.";
        }
    }

    private bool IsValidFile(string fileName)
    {
        var validFiles = new List<string> { AGB, Impressum, FAQ };
        return validFiles.Contains(fileName);
    }

    private bool FileExists(string fileName)
    {
        var filePath = Path.Combine(WebRootPath, fileName);
        return System.IO.File.Exists(filePath);
    }
}
