using Microsoft.AspNetCore.Mvc;
using CarRentalAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace CarRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InvoiceController(AppDbContext context)
        {
            _context = context;
        }

        // ================= GET INVOICE =================

        [HttpGet("{bookingId}")]
        public async Task<IActionResult> GetInvoice(
            int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Car)
                .FirstOrDefaultAsync(
                    b => b.Id == bookingId);

            if (booking == null)
                return NotFound(
                    "Booking not found");

            decimal subtotal =
                booking.TotalAmount ?? 0;

            // GST REMOVED

            decimal finalAmount =
                subtotal;

            return Ok(new
            {
                invoiceNo =
                    "INV-" + booking.Id,

                customerName =
                    booking.CustomerName,

                phone =
                    booking.Phone,

                carName =
                    booking.Car != null
                        ? booking.Car.Name
                        : "Unknown Car",

                brand =
                    booking.Car != null
                        ? booking.Car.Brand
                        : "",

                fromDate =
                    booking.FromDate,

                toDate =
                    booking.ToDate,

                withDriver =
                    booking.WithDriver,

                subtotal,

                finalAmount,

                status =
                    booking.Status
            });
        }
    }
}