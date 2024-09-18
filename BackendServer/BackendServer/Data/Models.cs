using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ContentDB.Migrations
{
    // User Entity
    public class User
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsTeacher { get; set; }
        public bool IsAdmin { get; set; }
        public string PassHash { get; set; }
        public string TelNr { get; set; }
    }

    // Post Entity
    public class Post
    {
        public int PostID { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }


        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.Now;

        public DateTime PublishDate { get; set; } = DateTime.Now;

        [BindNever]
        public int UserID { get; set; }

        [BindNever]
        [ValidateNever]
        public User User { get; set; }
    }


    // Media Entity
    public class Media
    {
        public int MediaID { get; set; }
        public string AltText { get; set; }
        public bool IsVideo { get; set; }
        public string FilePath { get; set; }

        // Foreign key to Post
        public int PostID { get; set; }
        public Post Post { get; set; }
    }

    // Event Entity
    public class Event
    {
        public int EventID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string JobDescription { get; set; }

        // Foreign key to User
        public int UserID { get; set; }
        public User User { get; set; }
    }

    // DbContext for SQLite
    public class ContentDBContext : DbContext
    {
        public ContentDBContext(DbContextOptions<ContentDBContext> options)
       : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Media> Media { get; set; }
        public DbSet<Event> Events { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
        public async Task SeedDataAsync()
        {
            // Seed User
            if (!Users.Any(u => u.Email == "test.user@example.com"))
            {
                Users.Add(new User
                {
                    UserID = 1,
                    FirstName = "Test",
                    LastName = "User",
                    Email = "test.user@example.com",
                    IsTeacher = false,
                    IsAdmin = false,
                    PassHash = PasswordHelper.HashPassword("testhash"),
                    TelNr = "123456789"
                }); 
                await SaveChangesAsync();
            }

            // Seed Teacher-User
            if (!Users.Any(u => u.Email == "test.teacher@example.com"))
            {
                Users.Add(new User
                {
                    UserID = 2,
                    FirstName = "Test",
                    LastName = "User",
                    Email = "test.teacher@example.com",
                    IsTeacher = true,
                    IsAdmin = false,
                    PassHash = PasswordHelper.HashPassword("testhash"),
                    TelNr = "123456789"
                });
                await SaveChangesAsync();
            }

            // Seed Admin-User
            if (!Users.Any(u => u.Email == "test.admin@example.com"))
            {
                Users.Add(new User
                {
                    UserID = 3,
                    FirstName = "Test",
                    LastName = "User",
                    Email = "test.admin@example.com",
                    IsTeacher = true,
                    IsAdmin = true,
                    PassHash = PasswordHelper.HashPassword("testhash"),
                    TelNr = "123456789"
                });
                await SaveChangesAsync();
            }

            // Seed Post
            if (!Posts.Any(p => p.PostID == 1))
            {
                Posts.Add(new Post
                {
                    PostID = 1,
                    Title = "Welcome Post",
                    Description = "This is the first post.",
                    CreationDate = DateTime.Now,
                    PublishDate = DateTime.Now,
                    UserID = 1
                });
                await SaveChangesAsync();
            }

            // Seed Media
            if (!Media.Any(m => m.MediaID == 1))
            {
                Media.Add(new Media
                {
                    MediaID = 1,
                    AltText = "Sample Image",
                    IsVideo = false,
                    FilePath = "images/sample.jpg",
                    PostID = 1
                });
                await SaveChangesAsync();
            }

            // Seed Event
            if (!Events.Any(e => e.EventID == 1))
            {
                Events.Add(new Event
                {
                    EventID = 1,
                    Title = "Sample Event",
                    Description = "This is a sample event.",
                    StartDate = DateTime.Now.AddDays(7),
                    EndDate = DateTime.Now.AddDays(10),
                    JobDescription = "Volunteer work",
                    UserID = 1
                });
                await SaveChangesAsync();
            }
        }

    }
}
