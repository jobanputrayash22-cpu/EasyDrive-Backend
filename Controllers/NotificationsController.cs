using Microsoft.AspNetCore.Mvc;
using CarRentalAPI.Data;
using CarRentalAPI.Models; // YE ADD KARO
using System.Linq;

namespace CarRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotificationsController(AppDbContext context)
        {
            _context = context;
        }

        // ================= GET NOTIFICATIONS =================
        [HttpGet("{userId}")]
        public IActionResult GetNotifications(int userId)
        {
            var data = _context.Notifications
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Id)
                .ToList();

            return Ok(data);
        }

        // ================= ADD NOTIFICATION =================
        [HttpPost]
        public IActionResult AddNotification([FromBody] Notification n)
        {
            if (n == null)
                return BadRequest("Invalid notification");

            _context.Notifications.Add(n);
            _context.SaveChanges();

            return Ok(new
            {
                message = "Notification created successfully"
            });
        }
    }
}