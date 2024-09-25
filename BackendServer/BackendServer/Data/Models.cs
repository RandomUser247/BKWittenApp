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

        public DateTime CreationDate { get; set; }  // Set this in your page handler

        public DateTime? PublishDate { get; set; }  // Nullable, and set server-side when post is published
        public bool IsPending { get; set; }
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
        [Required(ErrorMessage = "Alt text is required")]
        public string AltText { get; set; }  

        public bool IsVideo { get; set; }

        [Required(ErrorMessage = "File path is required")]
        public string FilePath { get; set; }

        // Foreign key to Post
        public int PostID { get; set; }
        public Post Post { get; set; }
    }


    // Event Entity
    public class Event
    {
        public int EventID { get; set; }

        [Required(ErrorMessage = "Event title is required")]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [DateGreaterThan("StartDate", ErrorMessage = "End date must be after start date")]
        public DateTime EndDate { get; set; }

        // Foreign key to User
        [BindNever]
        public int UserID { get; set; }

        [BindNever]
        [ValidateNever]
        public User User { get; set; }
    }

    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _startDatePropertyName;

        public DateGreaterThanAttribute(string startDatePropertyName)
        {
            _startDatePropertyName = startDatePropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var endDate = (DateTime?)value;
            var startDateProperty = validationContext.ObjectType.GetProperty(_startDatePropertyName);
            var startDate = (DateTime?)startDateProperty?.GetValue(validationContext.ObjectInstance);

            if (endDate.HasValue && startDate.HasValue && endDate.Value <= startDate.Value)
            {
                return new ValidationResult(ErrorMessage ?? "End date must be greater than start date");
            }

            return ValidationResult.Success;
        }
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
            if (!await Users.AsNoTracking().AnyAsync(u => u.Email == "test.user@example.com"))
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
            if (!await Users.AsNoTracking().AnyAsync(u => u.Email == "test.teacher@example.com"))
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
            if (!await Users.AsNoTracking().AnyAsync(u => u.Email == "test.admin@example.com"))
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
                    UserID = 1
                });
                await SaveChangesAsync();
            }
        }

    }
}
