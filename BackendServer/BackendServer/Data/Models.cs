using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

/*
 * Models.cs
 * 
 * Contains the entity models for the ContentDBContext including:
 * 
 * - User: Represents users with properties like UserID, FirstName, LastName, Email, IsTeacher, IsAdmin, PassHash, and a collection of Posts.
 * - Post: Represents posts created by users, including Title, Description, CreationDate, PublishDate, and relationships to User and Media.
 * - Media: Represents media files associated with posts, with properties such as AltText, IsVideo, FilePath, and relationships to Post and User.
 * - Event: Represents events with StartDate, EndDate, and a relationship to the User entity.
 * 
 * It also defines the ContentDBContext for interacting with the database, including entity configurations, and a SeedDataAsync method for initial data population.
 * 
 * When adding changes you have to perform a migration
 * 
 * To add a migration:
 * - Install EF Core CLI: `dotnet tool install --global dotnet-ef`
 * - Add a migration: `dotnet ef migrations add InitialMigration`
 * - Update the database: `dotnet ef database update`  +++ at startup this is done automatically 
 */



namespace ContentDB.Migrations
{
    // User Entity
    public class User
    {
        public int UserID { get; set; }
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name can't be longer than 100 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name can't be longer than 100 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        public bool IsTeacher { get; set; }
        public bool IsAdmin { get; set; }
        [Required]
        public string PassHash { get; set; }
        public string? TelNr { get; set; }

        public ICollection<Post> Posts { get; set; }
    }

    // Post Entity
    public class Post
    {
        public int PostID { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        public DateTime CreationDate { get; set; } 

        public DateTime? PublishDate { get; set; }  

        [ValidateNever]
        public int Likes { get; set; }
        public int ViewCount { get; set; }

        public bool IsPending { get; set; }
        public string? Category { get; set; }

        [BindNever]
        public int UserID { get; set; }

        [BindNever]
        [ValidateNever]
        public User User { get; set; }

        [ValidateNever]
        public ICollection<Media> Media { get; set; }

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
        public long FileSize { get; set; }
        public string FileType { get; set; }
        public int UploadedByUserID { get; set; }
        public User UploadedBy { get; set; }

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

            if (DateTime.Compare(endDate.Value, startDate.Value) <= 0)
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
            modelBuilder.Entity<Post>()
                .Property(p => p.Likes)
                .HasDefaultValue(0);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>()
                .Property(p => p.IsPending)
                .HasDefaultValue(false);


        }
        public async Task SeedDataAsync()
        {
            // Seed Users
            if (!await Users.AsNoTracking().AnyAsync(u => u.Email == "test.user@example.com"))
            {
                Users.Add(new User
                {
                    FirstName = "Test",
                    LastName = "User",
                    Email = "test.user@example.com",
                    IsTeacher = false,
                    IsAdmin = false,
                    PassHash = PasswordHelper.HashPassword("testhash", PasswordHelper.GenerateSalt()),
                    TelNr = "123456789"
                });
            }

            if (!await Users.AsNoTracking().AnyAsync(u => u.Email == "test.teacher@example.com"))
            {
                Users.Add(new User
                {
                    FirstName = "Test",
                    LastName = "User",
                    Email = "test.teacher@example.com",
                    IsTeacher = true,
                    IsAdmin = false,
                    PassHash = PasswordHelper.HashPassword("testhash", PasswordHelper.GenerateSalt()),
                    TelNr = "123456789"
                });
            }

            if (!await Users.AsNoTracking().AnyAsync(u => u.Email == "test.admin@example.com"))
            {
                Users.Add(new User
                {
                    FirstName = "Test",
                    LastName = "User",
                    Email = "test.admin@example.com",
                    IsTeacher = true,
                    IsAdmin = true,
                    PassHash = PasswordHelper.HashPassword("testhash", PasswordHelper.GenerateSalt()),
                    TelNr = "123456789"
                });
            }

            await SaveChangesAsync();  // Ensure users are saved to the database first

            // Retrieve the UserID of the newly added users
            var user = await Users.FirstOrDefaultAsync(u => u.Email == "test.user@example.com");
            var teacher = await Users.FirstOrDefaultAsync(u => u.Email == "test.teacher@example.com");
            var admin = await Users.FirstOrDefaultAsync(u => u.Email == "test.admin@example.com");

            // Seed Post
            if (!Posts.Any(p => p.PostID == 1))
            {
                Posts.Add(new Post
                {
                    Title = "Welcome Post",
                    Description = "This is the first post.",
                    CreationDate = DateTime.Now,
                    PublishDate = DateTime.Now,
                    UserID = user.UserID  // Use the UserID from the inserted user
                });
            }

            await SaveChangesAsync();  // Save posts

            // Seed Media
            if (!Media.Any(m => m.MediaID == 1))
            {
                Media.Add(new Media
                {
                    AltText = "Sample Image",
                    IsVideo = false,
                    FilePath = "images/sample.jpg",
                    PostID = 1  // Assuming PostID 1 exists
                });
            }

            await SaveChangesAsync();  // Save media

            // Seed Event
            if (!Events.Any(e => e.EventID == 1))
            {
                Events.Add(new Event
                {
                    Title = "Sample Event",
                    Description = "This is a sample event.",
                    StartDate = DateTime.Now.AddDays(7),
                    EndDate = DateTime.Now.AddDays(10),
                    UserID = user.UserID  // Reference the user
                });
            }

            await SaveChangesAsync();  // Save events
        }


    }
}
