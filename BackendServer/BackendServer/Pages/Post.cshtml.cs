using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using ContentDB.Migrations;
using Microsoft.AspNetCore.Identity;

namespace BackendServer.Pages
{
    public class PostModel : PageModel
    {
        private readonly ContentDBContext _context;
        private readonly ILogger<PostModel> _logger;

        public PostModel(ContentDBContext context, ILogger<PostModel> logger)
        {
            _context = context;
            _logger = logger;
        }


        public IList<Post> Posts { get; set; }

        [BindProperty]
        public Post NewPost { get; set; }

        public async Task OnGetAsync()
        {
            Posts = await _context.Posts.ToListAsync();
        }
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostCreateAsync()
        {
            _logger.LogInformation("OnPostCreateAsync called.");

            if (NewPost == null)
            {
                _logger.LogWarning("NewPost is null.");
            }

            if (!ModelState.IsValid)
            {
                // Log ModelState errors
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        var errorMessage = error.ErrorMessage;
                        // For debugging purposes
                        _logger.LogWarning($"Key: {key}, Error: {errorMessage}");
                    }
                }

                Posts = await _context.Posts.ToListAsync();
                return Page();
            }

            NewPost.UserID = 1;
            NewPost.CreationDate = DateTime.Now;
            NewPost.PublishDate = DateTime.Now;

            _context.Posts.Add(NewPost);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
