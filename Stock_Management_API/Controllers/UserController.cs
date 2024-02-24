using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Stock_Management_API.DTO;
using Stock_Management_API.Entities;

namespace Stock_Management_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly StockManagementContext _context;

        public UserController(StockManagementContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);

                // Create a new User object with hashed password
                var user = new User
                {
                    Email = createUserDto.Email,
                    PasswordHash = hashedPassword // Store the hashed password as a string
                };


                // Add user to the context
                _context.Users.Add(user);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Return the created user with HTTP status 201 Created
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return StatusCode(500, "An error occurred while creating the user");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
    }
}
