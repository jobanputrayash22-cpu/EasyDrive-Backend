using Microsoft.AspNetCore.Mvc;
using CarRentalAPI.Data;
using CarRentalAPI.Models;
using System.Linq;

namespace CarRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CouponsController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
public IActionResult GetCoupons()
{
    var coupons = _context.Coupons
        .Where(x =>
            x.IsActive == true &&
            x.ExpiryDate >= DateTime.Now)
        .Select(x => new
        {
            x.Id,
            x.Code,
            x.DiscountAmount,
            x.IsPercentage
        })
        .ToList();

    return Ok(coupons);
}

        [HttpPost("apply")]
        public IActionResult ApplyCoupon(
            [FromBody] ApplyCouponDto dto)
        {
            var coupon = _context.Coupons
                .FirstOrDefault(x =>
                    x.Code == dto.Code &&
                    x.IsActive == true &&
                    x.ExpiryDate >= DateTime.Now);

            if (coupon == null)
                return BadRequest("Invalid coupon");

            decimal discount = 0;

            if (coupon.IsPercentage)
            {
                discount =
                    (dto.Amount * coupon.DiscountAmount)
                    / 100;
            }
            else
            {
                discount =
                    coupon.DiscountAmount;
            }

            var finalAmount =
                dto.Amount - discount;

            return Ok(new
            {
                discount,
                finalAmount
            });
        }
    }

    public class ApplyCouponDto
    {
        public string Code { get; set; }
        public decimal Amount { get; set; }
    }
}