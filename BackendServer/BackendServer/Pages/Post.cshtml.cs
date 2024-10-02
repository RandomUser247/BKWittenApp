using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContentDB.Migrations;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BackendServer.Pages
{
    [Authorize]
    public class PostModel : PageModel
    {
        private readonly ContentDBContext _context;
        private readonly ILogger<PostModel> _logger;

        // Constructor to initialize context and logger
        public PostModel(ContentDBContext context, ILogger<PostModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        // List to hold posts fetched from the database
        public IList<Post> Posts { get; set; }

        // Bound property to create a new post
        [BindProperty]
        public Post NewPost { get; set; }

        // Method to handle GET requests, fetching posts from the database
        public async Task OnGetAsync()
        {
            Posts = await _context.Posts.ToListAsync();
        }

        // Method to handle POST requests for creating a new post
        [ValidateAntiForgeryToken] // Protects against Cross-Site Request Forgery (CSRF)
        public async Task<IActionResult> OnPostCreateAsync()
        {
            _logger.LogInformation("OnPostCreateAsync called.");

            // Log a warning if the NewPost property is null
            if (NewPost == null)
            {
                _logger.LogWarning("NewPost is null.");
            }

            // If the model state is invalid, reload the page with current posts
            if (!ModelState.IsValid)
            {
                Posts = await _context.Posts.ToListAsync();
                return Page();
            }

            // Set default values for new post properties
            NewPost.UserID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            NewPost.CreationDate = DateTime.Now;
            NewPost.PublishDate = DateTime.Now;

            // Add new post to the database and save changes
            _context.Posts.Add(NewPost);
            await _context.SaveChangesAsync();

            // Redirect to the same page after successfully adding the post
            return RedirectToPage();
        }

        // Method to handle POST requests for deleting a post by ID
        [ValidateAntiForgeryToken] // Protects against Cross-Site Request Forgery (CSRF)
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            // Find the post by its ID in the database
            var post = await _context.Posts.FindAsync(id);

            // If no post is found, return a NotFound result
            if (post == null)
            {
                return NotFound();
            }

            // Remove the post from the database and save changes
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            // Redirect to the same page after successfully deleting the post
            return RedirectToPage();
        }
    }
}

