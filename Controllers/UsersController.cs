using Microsoft.AspNetCore.Mvc;
using CarRentalAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace CarRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // ================= GET ALL USERS =================
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    id = u.Id,
                    name = u.FullName,
                    email = u.Email,
                    phone = u.Phone,
                    role = u.Role,
                    joinedDate = u.CreatedAt,
                    status = "Active"
                })
                .OrderByDescending(x => x.id)
                .ToListAsync();

            return Ok(users);
        }
    }
}