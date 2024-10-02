using BackendServer.Services.Interfaces;
using ContentDB.Migrations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace BackendServer.Services
{
    public class PostService : IPostService
    {
        private readonly ContentDBContext _context;
        private readonly IConfiguration _configuration;
        public PostService(ContentDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<List<Post>> GetRecentPostsAsync(int currentPage, int pageSize)
        {
            return await _context.Posts
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Media)
                .OrderByDescending(p => p.CreationDate)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // Fetch pending posts
        public async Task<List<Post>> GetPendingPostsAsync()
        {
            return await _context.Posts
                .AsNoTracking()
                .Where(p => p.IsPending)
                .Include(p => p.User)
                .Include(p => p.Media)
                .ToListAsync();
        }

        // Fetch post by ID
        public async Task<Post> GetPostByIdAsync(int postId)
        {
            return await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Media)
                .FirstOrDefaultAsync(p => p.PostID == postId);
        }

        public async Task<double> GetTotalPostCountAsync()
        {
            return await _context.Posts.CountAsync();
        }


        // Create a new post with media uploads
        public async Task CreatePostAsync(Post post, List<IFormFile> images, IFormFile video, string altText)
        {
            post.CreationDate = DateTime.Now;
            _context.Posts.Add(post);
            await _context.SaveChangesAsync(); // Save to generate PostID

            // Handle media uploads
            if (images != null && images.Count > 0)
            {
                foreach (var image in images)
                {
                    var media = new Media
                    {
                        AltText = altText,
                        IsVideo = false,
                        FilePath = await SaveFileAsync(image),
                        UploadedByUserID = post.UserID,
                        PostID = post.PostID
                    };
                    _context.Media.Add(media);
                }
            }

            if (video != null)
            {
                var media = new Media
                {
                    AltText = "Video for post",
                    IsVideo = true,
                    FilePath = await SaveFileAsync(video),
                    UploadedByUserID = post.UserID,
                    PostID = post.PostID
                };
                _context.Media.Add(media);
            }

            await _context.SaveChangesAsync();
        }

        // Edit an existing post
        public async Task EditPostAsync(Post post, List<IFormFile> images, IFormFile video, string altText)
        {
            var existingPost = await _context.Posts
                .Include(p => p.Media)
                .FirstOrDefaultAsync(p => p.PostID == post.PostID);

            if (existingPost == null) throw new Exception("Post not found");

            // Update fields
            existingPost.Title = post.Title;
            existingPost.Description = post.Description;
            existingPost.Category = post.Category;

            // Handle media updates (adding new images/videos)
            if (images != null && images.Count > 0)
            {
                foreach (var image in images)
                {
                    var media = new Media
                    {
                        AltText = altText,
                        IsVideo = false,
                        FilePath = await SaveFileAsync(image),
                        UploadedByUserID = post.UserID,
                        PostID = post.PostID
                    };
                    _context.Media.Add(media);
                }
            }

            if (video != null)
            {
                var media = new Media
                {
                    AltText = "Video for post",
                    IsVideo = true,
                    FilePath = await SaveFileAsync(video),
                    UploadedByUserID = post.UserID,
                    PostID = post.PostID
                };
                _context.Media.Add(media);
            }

            await _context.SaveChangesAsync();
        }

        // Delete a post by ID
        public async Task DeletePostAsync(int postId)
        {
            var post = await _context.Posts
                .Include(p => p.Media)
                .FirstOrDefaultAsync(p => p.PostID == postId);

            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Post not found");
            }
        }

        // Confirm (publish) a post
        public async Task ConfirmPostAsync(int postId)
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.PostID == postId);

            if (post != null)
            {
                post.PublishDate = DateTime.Now;
                post.IsPending = false;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Post not found");
            }
        }

        // Save file logic (for images/videos)
        private async Task<string> SaveFileAsync(IFormFile file)
        {
            var uploadsFolder = _configuration["UploadFolder"];
            if (string.IsNullOrEmpty(uploadsFolder)) throw new Exception("Upload folder not configured");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/uploads/{uniqueFileName}";
        }
    }

}
