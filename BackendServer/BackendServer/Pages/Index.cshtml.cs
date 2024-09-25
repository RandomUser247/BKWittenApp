using ContentDB.Migrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BackendServer.Pages
{
    // PageModel for handling the main content creator dashboard.
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ContentDBContext _context;

        // Lists to store recent posts, pending posts, and events.
        public List<Post> RecentPosts { get; set; }
        public List<Post> PendingPosts { get; set; }
        public List<Event> Events { get; set; }

        // Bound properties for creating new events and posts.
        [BindProperty]
        public Event NewEvent { get; set; }
        [BindProperty]
        public Post NewPost { get; set; }
        [BindProperty]
        public Post EditedPost { get; set; }

        // Pagination properties.
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        // Constant to control how many posts are shown per page.
        private const int PageSize = 10;

        // Properties for handling media uploads (images and videos).
        [BindProperty]
        public List<IFormFile> PostImages { get; set; } = new();
        [BindProperty]
        public IFormFile PostVideo { get; set; }
        [BindProperty]
        public string ImageAltText { get; set; }

        // Constructor to initialize the database context.
        public IndexModel(ContentDBContext context)
        {
            _context = context;
        }

        // Method to load the page data when accessed via GET request.
        public async Task<IActionResult> OnGetAsync(int currentPage = 1)
        {
            CurrentPage = currentPage; // Set the current page based on input.
            await ReloadDataAsync(); // Load recent posts, pending posts, and events.
            return Page(); // Render the page.
        }

        // Method to handle creating a new post.
        public async Task<IActionResult> OnPostCreatePostAsync()
        {
            ModelState.Clear(); // Clear any previous validation state.

            // Validate only the NewPost model.
            if (!TryValidateModel(NewPost, nameof(NewPost)))
            {
                await ReloadDataAsync();
                return Page(); // Reload the page if validation fails.
            }

            // Fetch the currently logged-in user.
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                await ReloadDataAsync();
                return Page();
            }

            // Set post details based on the current user and time.
            NewPost.UserID = currentUser.UserID;
            NewPost.User = currentUser;
            NewPost.CreationDate = DateTime.Now;
            NewPost.PublishDate = DateTime.Now;
            NewPost.IsPending = !currentUser.IsTeacher; // Posts are marked pending if the user isn't a teacher.
            _context.Posts.Add(NewPost); // Add the new post to the database.

            // Handle image uploads, if any.
            if (PostImages != null && PostImages.Count > 0)
            {
                foreach (var image in PostImages)
                {
                    var imageEntity = new Media
                    {
                        PostID = NewPost.PostID,
                        IsVideo = false,
                        AltText = ImageAltText,
                        FilePath = await SaveFileAsync(image), // Save image and get its file path.
                    };
                    _context.Media.Add(imageEntity); // Add the image media entity to the database.
                }
            }

            // Handle video upload, if any.
            if (PostVideo != null)
            {
                var videoEntity = new Media
                {
                    PostID = NewPost.PostID,
                    IsVideo = true,
                    AltText = "Video for post", // Default alt text for videos (can be customized).
                    FilePath = await SaveFileAsync(PostVideo), // Save video and get its file path.
                };
                _context.Media.Add(videoEntity); // Add the video media entity to the database.
            }

            try
            {
                await _context.SaveChangesAsync(); // Save all changes (post and media) to the database.
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while saving the post: " + ex.Message);
                await ReloadDataAsync();
                return Page(); // Show error message and reload the page in case of failure.
            }

            return RedirectToPage(); // Redirect to the page after successful post creation.
        }

        // Method to handle post editing.
        // Method to handle post editing.
        public async Task<IActionResult> OnPostEditPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await ReloadDataAsync();
                return Page();
            }

            var post = await _context.Posts.FindAsync(EditedPost.PostID); // Find the post by ID.
            if (post == null)
            {
                await ReloadDataAsync();
                return NotFound(); // Return 404 if the post doesn't exist.
            }

            // Ensure that the current user is allowed to edit this post (optional logic to check permissions).
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null || (post.UserID != currentUser.UserID && !currentUser.IsAdmin))
            {
                ModelState.AddModelError(string.Empty, "You do not have permission to edit this post.");
                await ReloadDataAsync();
                return Page();
            }

            // Update post fields.
            post.Title = EditedPost.Title;
            post.Description = EditedPost.Description;

            // Update media if there are new images or videos.
            if (PostImages != null && PostImages.Count > 0)
            {
                foreach (var image in PostImages)
                {
                    var imageEntity = new Media
                    {
                        PostID = post.PostID,
                        IsVideo = false,
                        AltText = ImageAltText,
                        FilePath = await SaveFileAsync(image), // Save image and get its file path.
                    };
                    _context.Media.Add(imageEntity);
                }
            }

            if (PostVideo != null)
            {
                var videoEntity = new Media
                {
                    PostID = post.PostID,
                    IsVideo = true,
                    AltText = "Video for post",
                    FilePath = await SaveFileAsync(PostVideo), // Save video and get its file path.
                };
                _context.Media.Add(videoEntity);
            }

            try
            {
                await _context.SaveChangesAsync(); // Save the changes to the database.
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while editing the post: " + ex.Message);
                await ReloadDataAsync();
                return Page();
            }

            await ReloadDataAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeletePostAsync(int postId)
        {
            var post = await _context.Posts.FindAsync(postId); // Find the post by ID.

            if (post != null)
            {
                try
                {
                    _context.Posts.Remove(post); // Remove the post from the database.
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Post deleted successfully!";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while deleting the post: " + ex.Message);
                    await ReloadDataAsync();
                    return Page(); // Reload the page with an error message if something fails.
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Post not found"); // If the post isn't found, show an error.
            }

            await ReloadDataAsync(); // Reload data after deletion.
            return RedirectToPage();
        }

        // Method to confirm (publish) a post.
        public async Task<IActionResult> OnPostConfirmPostAsync(int postId)
        {
            var post = await _context.Posts.FindAsync(postId); // Find the post by ID.
            if (post == null)
            {
                return NotFound(); // Return 404 if the post doesn't exist.
            }

            // Confirm the post by setting the publish date and marking it as not pending.
            post.PublishDate = DateTime.Now;
            post.IsPending = false;

            try
            {
                await _context.SaveChangesAsync(); // Save changes to the database.
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while confirming the post: " + ex.Message);
                await ReloadDataAsync();
                return Page(); // Show error message and reload if something fails.
            }

            return RedirectToPage(); // Redirect after successful confirmation.
        }

        // Method to handle creating a new event.
        public async Task<IActionResult> OnPostCreateEventAsync()
        {
            ModelState.Clear(); // Clear model state for clean validation of NewEvent.

            // Validate only the NewEvent model.
            if (!TryValidateModel(NewEvent, nameof(NewEvent)))
            {
                await ReloadDataAsync();
                return Page(); // Reload the page if validation fails.
            }

            var currentUser = await GetCurrentUserAsync(); // Get the current logged-in user.
            if (currentUser == null)
            {
                await ReloadDataAsync();
                return Page(); // Reload the page if user is not found.
            }

            // Set event details based on the current user.
            NewEvent.UserID = currentUser.UserID;
            NewEvent.User = currentUser;

            try
            {
                _context.Events.Add(NewEvent); // Add the event to the database.
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating the event: " + ex.Message);
                await ReloadDataAsync();
                return Page(); // Show error message and reload the page if something fails.
            }

            return RedirectToPage(); // Redirect after event creation.
        }

        // Method to reload the data, including recent posts, pending posts, and events.
        private async Task ReloadDataAsync()
        {
            var totalPostsCount = await _context.Posts.CountAsync(); // Get total post count for pagination.
            TotalPages = (int)Math.Ceiling(totalPostsCount / (double)PageSize); // Calculate total pages.

            // Fetch the recent posts for the current page.
            RecentPosts = await _context.Posts
                                .Include(p => p.User)
                                .OrderByDescending(p => p.CreationDate)
                                .Skip((CurrentPage - 1) * PageSize)
                                .Take(PageSize)
                                .ToListAsync();

            // Fetch the pending posts.
            PendingPosts = await _context.Posts.Where(p => p.IsPending).ToListAsync();

            // Fetch all events.
            Events = await _context.Events.ToListAsync();
        }

        // Method to get the currently logged-in user based on their identity.
        private async Task<User> GetCurrentUserAsync()
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
            if (currentUser == null)
            {
                ModelState.AddModelError(string.Empty, "User not found");
            }
            return currentUser; // Return the current user, or null if not found.
        }

        // Method to save uploaded files (both images and videos) and return the file path.
        private async Task<string> SaveFileAsync(IFormFile file)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder); // Create the uploads folder if it doesn't exist.
            }

            // Create a unique filename for the uploaded file.
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save the file to the specified path.
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/uploads/{uniqueFileName}"; // Return the relative path to the uploaded file.
        }
    }
}
