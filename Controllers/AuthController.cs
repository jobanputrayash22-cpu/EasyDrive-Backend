using Microsoft.AspNetCore.Mvc;
using CarRentalAPI.Data;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarRentalAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(
            AppDbContext context,
            IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // ================= REGISTER =================
        [HttpPost("register")]
        public IActionResult Register(RegisterDto dto)
        {
            if (_context.Users.Any(u => u.Email == dto.Email))
                return BadRequest("Email already exists");

            var user = new Models.User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                Password = dto.Password,

                // ROLE BASED LOGIN
                // Admin / Driver / User
                Role = string.IsNullOrEmpty(dto.Role)
                    ? "User"
                    : dto.Role,

                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new
            {
                message = "User registered successfully"
            });
        }

        // ================= LOGIN =================
        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var user = _context.Users
                .FirstOrDefault(u =>
                    u.Email == dto.Email &&
                    u.Password == dto.Password);

            if (user == null)
                return Unauthorized("Invalid credentials");

            return Ok(CreateToken(user));
        }

        // ================= GOOGLE LOGIN =================
        [HttpPost("google")]
        public IActionResult GoogleLogin(GoogleDto dto)
        {
            var user = _context.Users
                .FirstOrDefault(u =>
                    u.Email == dto.Email);

            if (user == null)
            {
                user = new Models.User
                {
                    FullName = "Google User",
                    Email = dto.Email,
                    Phone = "",
                    Password = "",

                    // Special admin email
                    Role = dto.Email == "youradmin@gmail.com"
                        ? "Admin"
                        : "User",

                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                _context.SaveChanges();
            }

            return Ok(CreateToken(user));
        }

        // ================= GET ALL USERS =================
        // Useful for admin panel
        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            var users = _context.Users
                .Select(u => new
                {
                    id = u.Id,
                    fullName = u.FullName,
                    email = u.Email,
                    phone = u.Phone,
                    role = u.Role,
                    createdAt = u.CreatedAt
                })
                .OrderByDescending(u => u.id)
                .ToList();

            return Ok(users);
        }

        // ================= UPDATE ROLE =================
        // Admin can change User -> Driver / Admin
        [HttpPut("update-role/{id}")]
        public IActionResult UpdateRole(
            int id,
            UpdateRoleDto dto)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
                return NotFound("User not found");

            user.Role = dto.Role;

            _context.SaveChanges();

            return Ok(new
            {
                message = "User role updated successfully"
            });
        }

        // ================= TOKEN CREATOR =================
        private object CreateToken(Models.User user)
        {
            var claims = new[]
            {
                new Claim(
                    ClaimTypes.Name,
                    user.Email
                ),

                new Claim(
                    ClaimTypes.Role,
                    user.Role
                ),

                new Claim(
                    "UserId",
                    user.Id.ToString()
                )
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _config["Jwt:Key"]
                )
            );

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler()
                .WriteToken(token);

            return new
            {
                token = jwt,
                email = user.Email,
                role = user.Role,
                userId = user.Id,
                fullName = user.FullName,
                phone = user.Phone
            };
        }
    }

    // ================= DTOs =================

    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }

        // NEW
        public string? Role { get; set; }
    }

    public class GoogleDto
    {
        public string Email { get; set; }
    }

    public class UpdateRoleDto
    {
        public string Role { get; set; }
    }
}