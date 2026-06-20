namespace CarRentalAPI.Models
{
    public class Coupon
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public decimal DiscountAmount { get; set; }

        public bool IsPercentage { get; set; }

        public DateTime ExpiryDate { get; set; }

        public bool IsActive { get; set; }
            = true;
    }
}