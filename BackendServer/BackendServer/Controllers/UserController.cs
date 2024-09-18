using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContentDB.Migrations;
using System.Threading.Tasks;

namespace BackendServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ContentDBContext _context;

        public UserController(ContentDBContext context)
        {
            _context = context;
        }

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
    }
}