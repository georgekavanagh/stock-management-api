using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stock_Management_API.DTO;
using Stock_Management_API.Entities;
using Stock_Management_API.Services;
using BCrypt.Net;

namespace Stock_Management_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly StockManagementContext _context;

        public AuthController(AuthService authService, StockManagementContext context)
        {
            _authService = authService;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return BadRequest("Invalid credentials");
            }

            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (dbUser == null)
            {
                return Unauthorized("Invalid credentials");
            }

            if (!VerifyPassword(loginDto.Password, dbUser.PasswordHash))
            {
                return Unauthorized("Invalid credentials");
            }

            var token = _authService.GenerateToken(dbUser);
            return Ok(new { Token = token });
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }

}
