using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContentDB.Migrations;
using System.Threading.Tasks;

namespace BackendServer.Controllers
{
    [ApiController]
    [Route("api/Bkw")]
    public class BkwController : ControllerBase
    {
        private readonly ContentDBContext _context;

        public BkwController(ContentDBContext context)
        {
            _context = context;
        }

        #region User CRUD Operations

        // GET: api/bkw/users
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        // GET: api/bkw/users/{id}
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { message = $"User with ID {id} not found." });
            }

            return Ok(user);
        }

        // POST: api/bkw/users
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] User newUser)
        {
            if (newUser == null)
            {
                return BadRequest(new { message = "Invalid user data." });
            }

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserById), new { id = newUser.UserID }, newUser);
        }

        // PUT: api/bkw/users/{id}
        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            if (id != updatedUser.UserID)
            {
                return BadRequest(new { message = "User ID mismatch." });
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = $"User with ID {id} not found." });
            }

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            user.IsTeacher = updatedUser.IsTeacher;
            user.IsAdmin = updatedUser.IsAdmin;
            user.PassHash = updatedUser.PassHash;
            user.TelNr = updatedUser.TelNr;

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"User with ID {id} updated successfully." });
        }

        // DELETE: api/bkw/users/{id}
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = $"User with ID {id} not found." });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"User with ID {id} deleted successfully." });
        }

        #endregion

        #region Post CRUD Operations

        // GET: api/bkw/posts
        [HttpGet("posts")]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await _context.Posts.Include(p => p.User).ToListAsync();
            return Ok(posts);
        }

        // GET: api/bkw/posts/{id}
        [HttpGet("posts/{id}")]
        public async Task<IActionResult> GetPostById(int id)
        {
            var post = await _context.Posts.Include(p => p.User).FirstOrDefaultAsync(p => p.PostID == id);

            if (post == null)
            {
                return NotFound(new { message = $"Post with ID {id} not found." });
            }

            return Ok(post);
        }

        // POST: api/bkw/posts
        [HttpPost("posts")]
        public async Task<IActionResult> CreatePost([FromBody] Post newPost)
        {
            if (newPost == null)
            {
                return BadRequest(new { message = "Invalid post data." });
            }

            _context.Posts.Add(newPost);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPostById), new { id = newPost.PostID }, newPost);
        }

        // PUT: api/bkw/posts/{id}
        [HttpPut("posts/{id}")]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] Post updatedPost)
        {
            if (id != updatedPost.PostID)
            {
                return BadRequest(new { message = "Post ID mismatch." });
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound(new { message = $"Post with ID {id} not found." });
            }

            post.Title = updatedPost.Title;
            post.Description = updatedPost.Description;
            post.CreationDate = updatedPost.CreationDate;
            post.PublishDate = updatedPost.PublishDate;
            post.UserID = updatedPost.UserID;

            _context.Entry(post).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Post with ID {id} updated successfully." });
        }

        // DELETE: api/bkw/posts/{id}
        [HttpDelete("posts/{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound(new { message = $"Post with ID {id} not found." });
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Post with ID {id} deleted successfully." });
        }

        #endregion

        #region Media CRUD Operations

        // GET: api/bkw/media
        [HttpGet("media")]
        public async Task<IActionResult> GetMedia()
        {
            var media = await _context.Media.Include(m => m.Post).ToListAsync();
            return Ok(media);
        }

        // GET: api/bkw/media/{id}
        [HttpGet("media/{id}")]
        public async Task<IActionResult> GetMediaById(int id)
        {
            var media = await _context.Media.Include(m => m.Post).FirstOrDefaultAsync(m => m.MediaID == id);

            if (media == null)
            {
                return NotFound(new { message = $"Media with ID {id} not found." });
            }

            return Ok(media);
        }

        // POST: api/bkw/media
        [HttpPost("media")]
        public async Task<IActionResult> CreateMedia([FromBody] Media newMedia)
        {
            if (newMedia == null)
            {
                return BadRequest(new { message = "Invalid media data." });
            }

            _context.Media.Add(newMedia);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMediaById), new { id = newMedia.MediaID }, newMedia);
        }

        // PUT: api/bkw/media/{id}
        [HttpPut("media/{id}")]
        public async Task<IActionResult> UpdateMedia(int id, [FromBody] Media updatedMedia)
        {
            if (id != updatedMedia.MediaID)
            {
                return BadRequest(new { message = "Media ID mismatch." });
            }

            var media = await _context.Media.FindAsync(id);
            if (media == null)
            {
                return NotFound(new { message = $"Media with ID {id} not found." });
            }

            media.AltText = updatedMedia.AltText;
            media.IsVideo = updatedMedia.IsVideo;
            media.FilePath = updatedMedia.FilePath;
            media.PostID = updatedMedia.PostID;

            _context.Entry(media).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Media with ID {id} updated successfully." });
        }

        // DELETE: api/bkw/media/{id}
        [HttpDelete("media/{id}")]
        public async Task<IActionResult> DeleteMedia(int id)
        {
            var media = await _context.Media.FindAsync(id);
            if (media == null)
            {
                return NotFound(new { message = $"Media with ID {id} not found." });
            }

            _context.Media.Remove(media);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Media with ID {id} deleted successfully." });
        }

        #endregion

        #region Event CRUD Operations

        // GET: api/bkw/events
        [HttpGet("events")]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events.Include(e => e.User).ToListAsync();
            return Ok(events);
        }

        // GET: api/bkw/events/{id}
        [HttpGet("events/{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var eventItem = await _context.Events.Include(e => e.User).FirstOrDefaultAsync(e => e.EventID == id);

            if (eventItem == null)
            {
                return NotFound(new { message = $"Event with ID {id} not found." });
            }

            return Ok(eventItem);
        }

        // POST: api/bkw/events
        [HttpPost("events")]
        public async Task<IActionResult> CreateEvent([FromBody] Event newEvent)
        {
            if (newEvent == null)
            {
                return BadRequest(new { message = "Invalid event data." });
            }

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEventById), new { id = newEvent.EventID }, newEvent);
        }

        // PUT: api/bkw/events/{id}
        [HttpPut("events/{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] Event updatedEvent)
        {
            if (id != updatedEvent.EventID)
            {
                return BadRequest(new { message = "Event ID mismatch." });
            }

            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
            {
                return NotFound(new { message = $"Event with ID {id} not found." });
            }

            eventItem.Title = updatedEvent.Title;
            eventItem.Description = updatedEvent.Description;
            eventItem.StartDate = updatedEvent.StartDate;
            eventItem.EndDate = updatedEvent.EndDate;
            eventItem.JobDescription = updatedEvent.JobDescription;
            eventItem.UserID = updatedEvent.UserID;

            _context.Entry(eventItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Event with ID {id} updated successfully." });
        }

        // DELETE: api/bkw/events/{id}
        [HttpDelete("events/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
            {
                return NotFound(new { message = $"Event with ID {id} not found." });
            }

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Event with ID {id} deleted successfully." });
        }

        #endregion
    }
}
