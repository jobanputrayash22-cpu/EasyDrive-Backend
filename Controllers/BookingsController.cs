using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRentalAPI.Data;
using CarRentalAPI.Models;
using CarRentalAPI.Services;
namespace CarRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

       public BookingsController(
    AppDbContext context,
    EmailService emailService)
{
    _context = context;
    _emailService = emailService;
}

        // ================= CREATE BOOKING =================
        // ================= CREATE BOOKING =================
[HttpPost]
public async Task<IActionResult> Create(
    [FromBody] CreateBookingDto dto)
{
    try
    {
        // =========================
        // VALIDATION
        // =========================

        if (dto == null)
            return BadRequest("Booking data missing");

        if (dto.CarId <= 0)
            return BadRequest("Invalid Car");

        if (string.IsNullOrWhiteSpace(dto.CustomerName))
            return BadRequest("Customer name required");

        if (string.IsNullOrWhiteSpace(dto.Phone))
            return BadRequest("Phone required");

        // =========================
        // FIND CAR
        // =========================

        var car = await _context.Cars
            .FirstOrDefaultAsync(x => x.Id == dto.CarId);

        if (car == null)
            return BadRequest("Car not found");

        // already booked
        if (car.Available == false)
            return BadRequest("Car is currently unavailable");

        // =========================
        // DRIVER VALIDATION
        // =========================

        if (dto.WithDriver == true)
        {
            if (dto.DriverId == null)
                return BadRequest("Driver required");

            var driver = await _context.Drivers
                .FirstOrDefaultAsync(x => x.Id == dto.DriverId);

            if (driver == null)
                return BadRequest("Driver not found");

            if (driver.Available == false)
                return BadRequest("Driver not available");

            // mark driver unavailable
            driver.Available = false;
        }

        // =========================
        // CREATE BOOKING
        // =========================

        var booking = new Booking
{
    UserId = dto.UserId,
    CustomerName = dto.CustomerName,
    Phone = dto.Phone,

    CarId = dto.CarId,

    WithDriver = dto.WithDriver,
    DriverId = dto.DriverId,

    FromDate = dto.FromDate,
    ToDate = dto.ToDate,
    Time = dto.Time,
    PickupLocation = dto.PickupLocation,
    DropLocation = dto.DropLocation,

    TotalAmount = dto.TotalAmount,
    Status = "Pending"
};

_context.Bookings.Add(booking);

car.Available = false;

await _context.SaveChangesAsync();

var user = await _context.Users
    .FirstOrDefaultAsync(
        x => x.Id == dto.UserId
    );

string driverName = "Self Drive";
string driverPhone = "-";

if (dto.DriverId != null)
{
    var driver = await _context.Drivers
        .FirstOrDefaultAsync(
            d => d.Id == dto.DriverId
        );

    if (driver != null)
    {
        driverName = driver.Name;
        driverPhone = driver.Phone;
    }
}


return Ok(new
{
    message = "Booking created successfully",
    bookingId = booking.Id
});
    }
    catch (Exception ex)
    {
        return StatusCode(500, new
        {
            message = "Internal Server Error",
            error = ex.Message
        });
    }
}

        // ================= BOOKINGS MANAGEMENT =================
// ================= BOOKINGS MANAGEMENT =================
[HttpGet("requests")]
public async Task<IActionResult> GetRequests()
{
    var requests = await _context.Bookings
        .Include(b => b.Car)
        .OrderByDescending(b => b.Id)
        .ToListAsync();

    var result = requests.Select(b => new
    {
        id = b.Id,

        customerName = b.CustomerName,
        phone = b.Phone,

        carName = b.Car != null
            ? b.Car.Name
            : "Unknown Car",

        brand = b.Car != null
            ? b.Car.Brand
            : "",

        // FIXED → DriverId check use karo
        driverName = b.DriverId != null
            ? _context.Drivers
                .Where(d => d.Id == b.DriverId)
                .Select(d => d.Name)
                .FirstOrDefault()
                ?? "Driver Not Found"
            : "Self Drive",

        // FIXED → Driver Code
        driverCode = b.DriverId != null
            ? "D" + b.DriverId
            : "-",

        fromDate = b.FromDate,
        toDate = b.ToDate,

        withDriver = b.WithDriver,
        driverId = b.DriverId,

        time = b.Time,

        amount = b.TotalAmount ?? 0,

        status = b.Status
    })
    .ToList();

    return Ok(result);
}

        // ================= USER BOOKINGS =================
        [HttpGet("user/{userId}")]
        public IActionResult GetUserBookings(int userId)
        {
            var bookings = _context.Bookings
                .Include(b => b.Car)
                .Where(b => b.UserId == userId)
                .Select(b => new
                {
                    b.Id,
                    b.CarId,

                    carName = b.Car != null ? b.Car.Name : null,
                    brand = b.Car != null ? b.Car.Brand : null,

                    startDate = b.FromDate,
                    endDate = b.ToDate,
                    time = b.Time,

                    withDriver = b.WithDriver,
                    status = b.Status,
                    totalAmount = b.TotalAmount
                })
                .OrderByDescending(b => b.Id)
                .ToList();

            return Ok(bookings);
        }

        // ================= APPROVE =================
       [HttpPut("{id}/approve")]
public async Task<IActionResult> Approve(int id)
{
    var booking = await _context.Bookings
        .Include(b => b.Car)
        .FirstOrDefaultAsync(b => b.Id == id);

    if (booking == null)
        return NotFound("Booking not found");

    booking.Status = "Approved";

    await _context.SaveChangesAsync();
    string driverName = "Self Drive";
string driverPhone = "-";

if (booking.DriverId != null)
{
    var driver = await _context.Drivers
        .FirstOrDefaultAsync(d => d.Id == booking.DriverId);

    if (driver != null)
    {
        driverName = driver.Name;
        driverPhone = driver.Phone;
    }
}

    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Id == booking.UserId);


    if (user != null)
{
    _emailService.SendEmail(
        user.Email,
        "EasyDrive Booking Approved 🚗",
        $"Hello {booking.CustomerName},\n\n" +

        $"Great news! Your booking has been approved.\n\n" +

        $"Booking Details:\n" +
        $"━━━━━━━━━━━━━━━━━━\n" +
        $"Car: {booking.Car?.Name}\n" +
        $"From: {booking.FromDate:dd-MM-yyyy}\n" +
        $"To: {booking.ToDate:dd-MM-yyyy}\n" +
        $"Driver: {driverName}\n" +
        $"Driver Phone: {driverPhone}\n" +
        $"Total Amount: ₹{booking.TotalAmount}\n" +
        $"━━━━━━━━━━━━━━━━━━\n\n" +

        $"Please carry a valid ID proof at pickup.\n\n" +

        $"Thank you for choosing EasyDrive.\n" +
        $"Have a safe journey! 🚗"
    );
}

    return Ok(new
    {
        message = "Booking approved successfully"
    });
}

        // ================= START RIDE =================
        [HttpPut("{id}/start")]
        public async Task<IActionResult> StartRide(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
                return NotFound("Booking not found");

            booking.Status = "On Ride";

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Ride started successfully"
            });
        }

        // ================= DECLINE =================
       [HttpPut("{id}/decline")]
public async Task<IActionResult> Decline(int id)
{
    var booking = await _context.Bookings
        .FindAsync(id);

    if (booking == null)
        return NotFound("Booking not found");

    booking.Status = "Declined";

    // Release driver if assigned
    if (booking.WithDriver == true &&
        booking.DriverId != null)
    {
        var driver = await _context.Drivers
            .FindAsync(booking.DriverId);

        if (driver != null)
        {
            driver.Available = true;
        }
    }

    // IMPORTANT FIX → Release car also
    var car = await _context.Cars
        .FindAsync(booking.CarId);

    if (car != null)
    {
        car.Available = true;
    }

    await _context.SaveChangesAsync();

    return Ok(new
    {
        message =
        "Booking declined, car and driver released successfully"
    });
}

        // ================= CANCEL =================
       [HttpPut("{id}/cancel")]
public async Task<IActionResult> Cancel(int id)
{
    var booking = await _context.Bookings
        .FindAsync(id);

    if (booking == null)
        return NotFound("Booking not found");

    booking.Status = "Cancelled";

    // Release driver if assigned
    if (booking.WithDriver == true &&
        booking.DriverId != null)
    {
        var driver = await _context.Drivers
            .FindAsync(booking.DriverId);

        if (driver != null)
        {
            driver.Available = true;
        }
    }

    // IMPORTANT FIX → Release car also
    var car = await _context.Cars
        .FindAsync(booking.CarId);

    if (car != null)
    {
        car.Available = true;
    }

    await _context.SaveChangesAsync();

    return Ok(new
    {
        message =
        "Booking cancelled, car and driver released successfully"
    });
}

        // ================= COMPLETE RIDE =================
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompleteRide(int id)
        {
            var booking = await _context.Bookings
                .FindAsync(id);

            if (booking == null)
                return NotFound("Booking not found");

            booking.Status = "Completed";

            // Release driver after ride completion
            if (booking.WithDriver == true &&
                booking.DriverId != null)
            {
                var driver = await _context.Drivers
                    .FindAsync(booking.DriverId);

                if (driver != null)
                {
                    driver.Available = true;
                }
            }
            var car = await _context.Cars.FindAsync(booking.CarId);

if (car != null)
{
    car.Available = true;
}

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Ride completed and driver released successfully"
            });
        }

        // ================= CALENDAR =================
        [HttpGet("calendar")]
        public async Task<IActionResult> Calendar()
        {
            var data = await _context.Bookings
                .Where(b => b.Status != "Declined")
                .Select(b => new
                {
                    id = b.Id,
                    title = "Car #" + b.CarId,

                    start = b.FromDate
                        .ToString("yyyy-MM-dd"),

                    end = b.ToDate
                        .AddDays(1)
                        .ToString("yyyy-MM-dd"),

                    allDay = true,

                    customer = b.CustomerName,
                    phone = b.Phone,
                    driver = b.WithDriver,
                    pickupTime = b.Time,
                    status = b.Status
                })
                .ToListAsync();

            return Ok(data);
        }

[HttpGet("driver/{driverId}")]
public async Task<IActionResult> GetDriverBookings(int driverId)
{
    var rides = await _context.Bookings
        .Include(b => b.Car)
        .Where(b =>
            b.DriverId == driverId &&
            b.WithDriver == true)
        .OrderByDescending(b => b.Id)
        .Select(b => new
        {
            id = b.Id,

            customerName = b.CustomerName,
            phone = b.Phone,

            carName = b.Car != null
                ? b.Car.Name
                : "Unknown Car",

            brand = b.Car != null
                ? b.Car.Brand
                : "",

            fromDate = b.FromDate,
            toDate = b.ToDate,
            time = b.Time,

            // ADD THESE
            pickupLocation = b.PickupLocation,
            dropLocation = b.DropLocation,

            // PAYMENT STATUS
            paymentStatus =
                b.Status == "Completed"
                    ? "Paid"
                    : "Pending",

            totalAmount = b.TotalAmount ?? 0,
            status = b.Status
        })
        .ToListAsync();

    return Ok(rides);
}
    }
    // ================= DTO =================
    public class CreateBookingDto
    {
        public int UserId { get; set; }

        public string CustomerName { get; set; }
        public string Phone { get; set; }

        public int CarId { get; set; }

        public bool WithDriver { get; set; }
        public int? DriverId { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public string Time { get; set; }

          public string? PickupLocation { get; set; }
    public string? DropLocation { get; set; }

        public decimal? TotalAmount { get; set; }
    }
}