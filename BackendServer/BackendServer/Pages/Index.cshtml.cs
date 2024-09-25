using ContentDB.Migrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BackendServer.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ContentDBContext _context;

        public List<Post> RecentPosts { get; set; }
        public List<Post> PendingPosts { get; set; } 
        public List<Event> Events { get; set; } 
        [BindProperty]
        public Event NewEvent { get; set; }
        [BindProperty]
        public Post NewPost { get; set; }

        public IndexModel(ContentDBContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            await ReloadDataAsync();
        }

        private async Task<User> GetCurrentUserAsync()
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
            if (currentUser == null)
            {
                ModelState.AddModelError(string.Empty, "User not found");
            }
            return currentUser;
        }

        public async Task<IActionResult> OnPostCreatePostAsync()
        {
            ModelState.Clear(); // Clear model state to avoid binding other properties like NewEvent

            if (!TryValidateModel(NewPost, nameof(NewPost)))
            {
                await ReloadDataAsync();
                return Page();
            }

            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                await ReloadDataAsync();
                return Page();
            }

            NewPost.UserID = currentUser.UserID;
            NewPost.User = currentUser;
            NewPost.CreationDate = DateTime.Now;
            NewPost.PublishDate = DateTime.Now;

            try
            {
                _context.Posts.Add(NewPost);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while saving the post: " + ex.Message);
                return Page();
            }

            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostEditPostAsync(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                return NotFound();
            }

            // Add your edit logic here, e.g., post.Title = NewTitle;
            // Ensure the current user is allowed to edit this post.

            try
            {
                await _context.SaveChangesAsync(); // Save changes after editing
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while editing the post: " + ex.Message);
                await ReloadDataAsync();
                return Page();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeletePostAsync(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);

            if (post != null)
            {
                try
                {
                    _context.Posts.Remove(post);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while deleting the post: " + ex.Message);
                    await ReloadDataAsync();
                    return Page();
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Post not found");
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostConfirmPostAsync(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                return NotFound();
            }

            post.PublishDate = DateTime.Now; // Confirm the post by setting the current PublishDate

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while confirming the post: " + ex.Message);
                await ReloadDataAsync();
                return Page();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCreateEventAsync()
        {
            ModelState.Clear(); // Clear model state to avoid binding other properties like NewPost

            if (!TryValidateModel(NewEvent, nameof(NewEvent)))
            {
                await ReloadDataAsync();
                return Page();
            }

            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                await ReloadDataAsync();
                return Page();
            }

            NewEvent.UserID = currentUser.UserID;
            NewEvent.User = currentUser;

            try
            {
                _context.Events.Add(NewEvent);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating the event: " + ex.Message);
                await ReloadDataAsync();
                return Page();
            }

            return RedirectToPage();
        }

        private async Task ReloadDataAsync()
        {
            // Load the recent posts (consider implementing pagination here)
            RecentPosts = await _context.Posts
                .Include(p => p.User)
                .OrderByDescending(p => p.CreationDate)
                .Take(10) // Pagination can be implemented here
                .ToListAsync();

            // Load pending posts
            PendingPosts = await _context.Posts
                .Where(p => p.PublishDate > DateTime.Now)
                .Include(p => p.User)
                .ToListAsync();

            // Load events
            Events = await _context.Events.ToListAsync();
        }
    }
}
