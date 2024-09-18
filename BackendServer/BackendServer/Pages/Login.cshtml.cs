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

        public LoginModel(ContentDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var hashedPassword = PasswordHelper.HashPassword(Input.Password);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == Input.Email && u.PassHash == hashedPassword);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            // Create claims (user ID, email, roles, etc.)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("FirstName", user.FirstName),
                new Claim("IsAdmin", user.IsAdmin.ToString()),
                new Claim("IsTeacher", user.IsTeacher.ToString())
            };

            // Create a ClaimsIdentity and authenticate the user
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true // Set to true if you want the session to persist across browser sessions
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            return RedirectToPage("/Index"); // Redirect after successful login
        }
    }
}
