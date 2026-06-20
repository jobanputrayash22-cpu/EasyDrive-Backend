using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRentalAPI.Data;
using CarRentalAPI.Models;

namespace CarRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriversController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DriversController(AppDbContext context)
        {
            _context = context;
        }

        // ================= GET ALL DRIVERS =================
        [HttpGet]
        public async Task<IActionResult> GetDrivers()
        {
            var drivers = await _context.Drivers
                .OrderByDescending(d => d.Id)
                .Select(d => new
                {
                    id = d.Id,
                    name = d.Name,
                    phone = d.Phone,
                    licenseNumber = d.LicenseNumber,
                    licenseExpiry = d.LicenseExpiry,
                    experienceYears = d.ExperienceYears,
                    address = d.Address,

                    // frontend ke liye proper boolean
                    available = d.Available == true
                })
                .ToListAsync();

            return Ok(drivers);
        }

        // ================= GET AVAILABLE DRIVERS =================
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableDrivers()
        {
            var drivers = await _context.Drivers
                .Where(d => d.Available == true)
                .Select(d => new
                {
                    id = d.Id,
                    name = d.Name,
                    phone = d.Phone,
                    licenseNumber = d.LicenseNumber,
                    experienceYears = d.ExperienceYears
                })
                .ToListAsync();

            return Ok(drivers);
        }

        // ================= DRIVER LOGIN =================
        [HttpPost("login")]
        public IActionResult DriverLogin(
            [FromBody] DriverLoginDto dto)
        {
            var driver = _context.Drivers
                .FirstOrDefault(d =>
                    d.Username == dto.Username &&
                    d.Password == dto.Password);

            if (driver == null)
                return BadRequest("Invalid Login");

            return Ok(new
            {
                message = "Login Success",
                driverId = driver.Id,
                name = driver.Name,
                phone = driver.Phone,
                available = driver.Available
            });
        }

        // ================= ADD DRIVER =================
        [HttpPost]
        public async Task<IActionResult> AddDriver([FromBody] Driver driver)
        {
            if (driver == null)
                return BadRequest("Driver data is required");

            // force available true on create
            driver.Available = true;

            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Driver added successfully",
                driver
            });
        }

        // ================= DELETE DRIVER =================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDriver(int id)
        {
            var driver = await _context.Drivers.FindAsync(id);

            if (driver == null)
                return NotFound("Driver not found");

            _context.Drivers.Remove(driver);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Driver deleted successfully"
            });
        }

        // ================= ASSIGN DRIVER =================
        [HttpPut("{id}/assign")]
        public async Task<IActionResult> AssignDriver(int id)
        {
            var driver = await _context.Drivers.FindAsync(id);

            if (driver == null)
                return NotFound("Driver not found");

            driver.Available = false;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Driver assigned successfully"
            });
        }

        // ================= RELEASE DRIVER =================
        [HttpPut("{id}/release")]
        public async Task<IActionResult> ReleaseDriver(int id)
        {
            var driver = await _context.Drivers.FindAsync(id);

            if (driver == null)
                return NotFound("Driver not found");

            driver.Available = true;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Driver released successfully"
            });
        }
    }

    // ================= DTO =================
    public class DriverLoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}