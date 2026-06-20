using Microsoft.AspNetCore.Mvc;
using CarRentalAPI.Data;
using CarRentalAPI.Models;
using System.Linq;

namespace CarRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReviewsController(AppDbContext context)
        {
            _context = context;
        }

        // GET reviews by car
        [HttpGet("{carId}")]
        public IActionResult GetReviews(int carId)
        {
            var data = _context.Reviews
                .Where(x => x.CarId == carId)
                .OrderByDescending(x => x.Id)
                .ToList();

            return Ok(data);
        }

        // ADD review
        [HttpPost]
        public IActionResult AddReview([FromBody] Review r)
        {
            if (r == null)
                return BadRequest();

            if (r.Rating < 1 || r.Rating > 5)
                return BadRequest("Rating must be 1 to 5");

            _context.Reviews.Add(r);
            _context.SaveChanges();

            return Ok(new
            {
                message = "Review added successfully"
            });
        }
    }
}