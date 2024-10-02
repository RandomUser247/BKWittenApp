using ContentDB.Migrations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using BackendServer.Services.Interfaces;
using BackendServer.Services;

/*
 * This file sets up an ASP.NET Core web app with SQLite and Razor Pages.
 * 
 * 1. **DbContext**: Registers `ContentDBContext` using SQLite for data persistence.
 * 2. **Authentication**: Implements cookie-based authentication with session expiration.
 * 3. **Authorization**: Adds role-based policies for "Teacher" and "Admin" access.
 * 4. **Razor Pages**: Handles UI with server-side rendering.
 * 5. **Database Seeding**: Seeds initial data if the database is not already populated.
 * 6. **Pipeline**: Configures middleware for routing, static files, HTTPS, authentication, and authorization.
 * 7. **Password Helper**: Provides bcrypt-based password hashing and verification.
 */

var builder = WebApplication.CreateBuilder(args);

// Register the DbContext 
builder.Services.AddDbContext<ContentDBContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ContentDB")));

builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });


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
    options.AddPolicy("RequireTeacherRole", policy =>
        policy.RequireClaim("isTeacher", "True"));

    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireClaim("isAdmin", "True"));
});

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
    try
    {
        await context.SeedDataAsync();
        Console.WriteLine("Data Seeded");
    }
    catch (Exception ex)
    {
        // logger.Exception($"Error seeding database: {ex.Message}");
    }
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

app.Run();

// Helper class for password hashing and verification using bcrypt
public static class PasswordHelper
{
    // Generate a salt for password hashing using bcrypt
    public static string GenerateSalt(int workFactor = 12)
    {
        return BCrypt.Net.BCrypt.GenerateSalt(workFactor); 
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
