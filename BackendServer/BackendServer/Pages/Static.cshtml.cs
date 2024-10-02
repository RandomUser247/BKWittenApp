using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

/*
 * This Razor Page model allows users with the "Admin" role to manage static text files (AGB, Impressum, FAQ).
 * 
 * 1. **Authorization**: Only accessible to users with the "Admin" role.
 * 2. **File Management**: Handles loading and saving static files (AGB.txt, impressum.txt, FAQ.txt).
 * 3. **Dropdown Options**: Provides a dropdown for selecting which file to edit.
 * 4. **GET Handlers**: Loads file content on request; initializes file options on page load.
 * 5. **POST Handlers**: Handles form submission to save content to the selected file.
 * 6. **Helper Methods**: Validates selected files, checks file existence, and updates file content.
 */



[Authorize(Policy = "RequireAdminRole")] // Restricts access to only users with the "Admin" role
public class StaticModel : PageModel
{
    private readonly IWebHostEnvironment _environment;

    // IWebHostEnvironment service provides access to the web root path
    public StaticModel(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [BindProperty]
    public string SelectedFile { get; set; } // Stores the file selected by the user

    [BindProperty]
    public string FileContent { get; set; } // Stores the content of the selected file

    public List<SelectListItem> FileOptions { get; set; } // List of file options for the dropdown selection

    public string SaveResult { get; set; } // Message indicating the result of a save operation
    public bool FileNotFound { get; set; } // Boolean to track if a file was not found

    // Constants representing file names to be managed through this page
    private const string AGB = "AGB.txt";
    private const string Impressum = "impressum.txt";
    private const string FAQ = "FAQ.txt";

    // Property to expose the web root path (location where static files are stored)
    public string WebRootPath => _environment.WebRootPath;

    // Method to populate the dropdown options for file selection
    private void PopulateFileOptions()
    {
        FileOptions = new List<SelectListItem>
        {
            new SelectListItem { Value = AGB, Text = "AGB" },
            new SelectListItem { Value = Impressum, Text = "Impressum" },
            new SelectListItem { Value = FAQ, Text = "FAQ" }
        };
    }

    // GET handler: called when the page is first accessed
    public void OnGet()
    {
        PopulateFileOptions(); 
    }

    // POST handler: handles form submissions (e.g., when the user clicks "Save")
    public async Task<IActionResult> OnPostAsync(string action)
    {
        PopulateFileOptions(); // Ensure the file options are available after a postback

        // Validate the selected file before proceeding
        if (!IsValidFile(SelectedFile))
        {
            SaveResult = "Invalid file selection."; // Inform the user if the file is invalid
            return Page(); 
        }

        // If the action is "Save", attempt to save the file
        if (action == "Save")
        {
            await SaveFileAsync(); 
        }

        return Page(); 
    }

    // GET handler: loads and returns the content of a selected file
    public IActionResult OnGetLoad(string file)
    {
        // Check if the file is valid and exists before attempting to read it
        if (IsValidFile(file) && FileExists(file))
        {
            var filePath = Path.Combine(WebRootPath, file); 
            var content = System.IO.File.ReadAllText(filePath); 
            return Content(content); // Return the file content as a plain-text response
        }

        return Content("File not found.");
    }

    // saves the content of the selected file
    private async Task SaveFileAsync()
    {
        var filePath = Path.Combine(WebRootPath, SelectedFile); 
        if (!string.IsNullOrWhiteSpace(FileContent)) // Ensure there is content to save
        {
            await System.IO.File.WriteAllTextAsync(filePath, FileContent); // Save content 
            SaveResult = $"The content of {SelectedFile} has been updated."; 
        }
        else
        {
            SaveResult = "File content is empty and was not saved.";
        }
    }

    // Helper method to check if the selected file is valid (i.e., one of the predefined files)
    private bool IsValidFile(string fileName)
    {
        var validFiles = new List<string> { AGB, Impressum, FAQ }; 
        return validFiles.Contains(fileName);
    }

    // Helper method to check if the file exists in the web root directory
    private bool FileExists(string fileName)
    {
        var filePath = Path.Combine(WebRootPath, fileName); 
        return System.IO.File.Exists(filePath); 
    }
}
