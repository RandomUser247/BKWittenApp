using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using ContentDB.Migrations;

namespace BackendServer.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly ContentDBContext _context;

        // Constructor for injecting ContentDBContext into the LoginModel
        public LoginModel(ContentDBContext context)
        {
            _context = context;
        }

        // The InputModel contains the fields that users need to fill in on the login page
        [BindProperty]
        public InputModel Input { get; set; }

        // Model for handling user input (Email and Password) with validation attributes
        public class InputModel
        {
            [Required] // Ensures that the email field is filled out
            [EmailAddress] // Ensures that the input is a valid email address
            public string Email { get; set; }

            [Required] // Ensures that the password field is filled out
            [DataType(DataType.Password)] // Marks the field as a password type (for HTML rendering)
            public string Password { get; set; }
        }

        // Handles the post request when the login form is submitted
        public async Task<IActionResult> OnPostAsync()
        {
            // Check if the submitted form data is valid based on the InputModel attributes
            if (!ModelState.IsValid)
            {
                return Page(); // Return to the login page if the form is invalid
            }

            // Attempt to find the user by their email in the database
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == Input.Email);

            // If user is not found or password verification fails, add a model error and return to the login page
            if (user == null || !PasswordHelper.VerifyPassword(Input.Password, user.PassHash))
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            // Create a list of claims, which will be used to identify the user in the system
            // Claims contain user-specific information like UserID, Email, and roles (IsAdmin, IsTeacher)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()), // Unique user ID
                new Claim(ClaimTypes.Name, user.Email), // User email
                new Claim("FirstName", user.FirstName), // Custom claim for user's first name
                new Claim("IsAdmin", user.IsAdmin.ToString()), // Custom claim for admin role
                new Claim("IsTeacher", user.IsTeacher.ToString()) // Custom claim for teacher role
            };

            // Create a ClaimsIdentity with the claims, using the cookie authentication scheme
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // AuthenticationProperties is used to configure authentication settings
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true // Keeps the user logged in even after the browser is closed
            };

            // Sign the user in by creating an authenticated session with the provided claims and properties
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            // Redirect to the homepage ("/Index") after successful login
            return RedirectToPage("/Index");
        }
    }
}
