using Microsoft.AspNetCore.Mvc;
using CarRentalAPI.Data;

namespace CarRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        // ================= DASHBOARD STATS =================
        [HttpGet]
        public IActionResult GetDashboardStats()
        {
            var totalCars = _context.Cars.Count();

            var totalDrivers = _context.Drivers.Count();

            var totalBookings = _context.Bookings.Count();

            var pendingRequests = _context.Bookings
                .Count(x => x.Status == "Pending");

            var approvedBookings = _context.Bookings
                .Count(x =>
                    x.Status == "Approved" ||
                    x.Status == "Confirmed");

            var onRide = _context.Bookings
                .Count(x => x.Status == "On Ride");

            var completedRides = _context.Bookings
                .Count(x => x.Status == "Completed");

            var cancelledBookings = _context.Bookings
                .Count(x =>
                    x.Status == "Cancelled" ||
                    x.Status == "Declined");

            var totalRevenue = _context.Bookings
                .Where(x => x.Status == "Completed")
                .Sum(x => x.TotalAmount ?? 0);

            var recentBookings = _context.Bookings
                .OrderByDescending(x => x.Id)
                .Take(5)
                .Select(x => new
                {
                    id = x.Id,
                    customerName = x.CustomerName,
                    carId = x.CarId,
                    amount = x.TotalAmount ?? 0,
                    status = x.Status
                })
                .ToList();

            return Ok(new
            {
                totalCars,
                totalDrivers,
                totalBookings,
                pendingRequests,
                approvedBookings,
                onRide,
                completedRides,
                cancelledBookings,
                totalRevenue,
                recentBookings
            });
        }
    }
}