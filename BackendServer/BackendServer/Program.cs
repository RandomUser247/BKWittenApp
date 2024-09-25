using ContentDB.Migrations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;

var builder = WebApplication.CreateBuilder(args);

// Register the DbContext with dependency injection
builder.Services.AddDbContext<ContentDBContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ContentDB")));
// This sets up the connection to the SQLite database using the connection string from the configuration file.


// Add cookie authentication to the app
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // Redirects to the login page if the user is not authenticated
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Sets the session to expire after 60 minutes
        options.SlidingExpiration = true; // Resets the expiration timer with each request
    });

// Add authorization policies
builder.Services.AddAuthorization(options =>
{
    // Policy that requires the user to have the claim "isTeacher" set to "True"
    options.AddPolicy("RequireTeacherRole", policy =>
        policy.RequireClaim("isTeacher", "True"));

    // Policy that requires the user to have the claim "isAdmin" set to "True"
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireClaim("isAdmin", "True"));
});
// You can define different policies here that control access to certain areas based on user claims.

// Add services to the container, including Razor Pages for handling UI and server-side code
builder.Services.AddRazorPages();

var app = builder.Build();

// Seed the database with initial data, if necessary
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ContentDBContext>();

    // Ensure the database is created before seeding data
    context.Database.EnsureCreated();

    // Call the SeedDataAsync method to populate the database with initial data
    await context.SeedDataAsync();
}

// Configure the HTTP request pipeline

if (!app.Environment.IsDevelopment())
{
    // In production, use an exception handler to redirect users to a custom error page
    app.UseExceptionHandler("/Error");

    // Enforce HTTP Strict Transport Security (HSTS) to secure communication in production
    app.UseHsts();
}
// Redirects HTTP requests to HTTPS for secure communication
app.UseHttpsRedirection(); 

// Enables serving of static files like CSS, JS, and images
app.UseStaticFiles(); 

// Sets up routing for the application
app.UseRouting(); 

 // Enable authentication middleware to handle login and session tracking
app.UseAuthentication();

// Enable authorization middleware to enforce access control
app.UseAuthorization(); 

// Map Razor Pages (UI) to be handled by the routing system
app.MapRazorPages(); 

// Run the application
app.Run();

// Helper class for password hashing and verification using bcrypt
public static class PasswordHelper
{
    // Generate a salt for password hashing using bcrypt
    public static string GenerateSalt(int workFactor = 12)
    {
        return BCrypt.Net.BCrypt.GenerateSalt(workFactor); // Work factor determines the complexity of the hash
    }

    // Hash a password using bcrypt with the generated salt
    public static string HashPassword(string password, string salt)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, salt);
    }

    // Verify a password against a stored hash
    public static bool VerifyPassword(string password, string storedHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, storedHash);
    }
}
