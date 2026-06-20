using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRentalAPI.Data;
using CarRentalAPI.Models;

namespace CarRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CarsController(AppDbContext context)
        {
            _context = context;
        }

        // ================= GET ALL =================
        [HttpGet]
        public async Task<IActionResult> GetCars()
        {
            var cars = await _context.Cars
                .OrderByDescending(c => c.Id)
                .Select(c => new
                {
                    id = c.Id,
                    name = c.Name,
                    brand = c.Brand,
                    model = c.Model,
                    year = c.Year,
                    fuelType = c.FuelType,
                    transmission = c.Transmission,
                    seats = c.Seats,
                    carType = c.CarType,
                    pricePerDay = c.PricePerDay,
                    available = c.Available,
                    registrationNumber = c.RegistrationNumber,
                    mileage = c.Mileage,
                    imageUrl = c.ImageUrl
                })
                .ToListAsync();

            return Ok(cars);
        }

        // ================= AVAILABLE =================
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableCars()
        {
            var cars = await _context.Cars
                .Where(c => c.Available)
                .Select(c => new
                {
                    id = c.Id,
                    name = c.Name,
                    brand = c.Brand,
                    pricePerDay = c.PricePerDay
                })
                .ToListAsync();

            return Ok(cars);
        }

        // ================= ADD =================
        [HttpPost]
public async Task<IActionResult> AddCar([FromForm] CarDto dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    string imagePath = "";

    if (dto.Image != null)
    {
        var folder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot/carimages"
        );

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        var fileName =
            Guid.NewGuid() +
            Path.GetExtension(dto.Image.FileName);

        var filePath = Path.Combine(folder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.Image.CopyToAsync(stream);
        }

        imagePath =
            $"http://localhost:5194/carimages/{fileName}";
    }

    var car = new Car
    {
        Name = dto.Name,
        Brand = dto.Brand,
        Model = dto.Model,
        Year = dto.Year,
        FuelType = dto.FuelType,
        Transmission = dto.Transmission,
        Seats = dto.Seats,
        CarType = dto.CarType,
        PricePerDay = dto.PricePerDay,
        RegistrationNumber = dto.RegistrationNumber,
        Mileage = dto.Mileage,
        ImageUrl = imagePath,
        Available = true
    };

    _context.Cars.Add(car);
    await _context.SaveChangesAsync();

    return Ok(new
    {
        message = "Car added successfully",
        car
    });
}
        [HttpGet("{id}")]
public async Task<IActionResult> GetCar(int id)
{
    var car = await _context.Cars
        .Where(c => c.Id == id)
        .Select(c => new
        {
            id = c.Id,
            name = c.Name,
            brand = c.Brand,
            model = c.Model,
            year = c.Year,
            fuelType = c.FuelType,
            transmission = c.Transmission,
            seats = c.Seats,
            carType = c.CarType,
            pricePerDay = c.PricePerDay,
            available = c.Available,
            registrationNumber = c.RegistrationNumber,
            mileage = c.Mileage,
            imageUrl = c.ImageUrl
        })
        .FirstOrDefaultAsync();

    if (car == null)
        return NotFound("Car not found");

    return Ok(car);
}

        // ================= DELETE =================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);

            if (car == null)
                return NotFound("Car not found");

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return Ok("Deleted");
        }

        // ================= BOOK =================
        [HttpPut("{id}/book")]
        public async Task<IActionResult> BookCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);

            if (car == null)
                return NotFound("Car not found");

            car.Available = false;
            await _context.SaveChangesAsync();

            return Ok("Booked");
        }

        // ================= RELEASE =================
        [HttpPut("{id}/release")]
        public async Task<IActionResult> ReleaseCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);

            if (car == null)
                return NotFound("Car not found");

            car.Available = true;
            await _context.SaveChangesAsync();

            return Ok("Released");
        }
    }

    // ================= DTO FIXED =================
    public class CarDto
    {
        public string? Name { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }
        public string? FuelType { get; set; }
        public string? Transmission { get; set; }
        public int? Seats { get; set; }
        public string? CarType { get; set; }
        public decimal? PricePerDay { get; set; }
        public string? RegistrationNumber { get; set; }
        public int? Mileage { get; set; }   // 🔥 FIXED
       public IFormFile? Image { get; set; }
    }
}



