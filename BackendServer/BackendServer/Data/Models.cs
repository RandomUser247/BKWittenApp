using Microsoft.EntityFrameworkCore;

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
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime PublishDate { get; set; }

        // Foreign key to User
        public int UserID { get; set; }
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
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Media> Media { get; set; }
        public DbSet<Event> Events { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=content.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // additional constrains etc.
        }
    }
}
