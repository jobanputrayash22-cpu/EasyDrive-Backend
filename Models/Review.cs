namespace CarRentalAPI.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int CarId { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.Now;
    }
}