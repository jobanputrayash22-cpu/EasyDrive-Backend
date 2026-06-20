namespace CarRentalAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }

        // NEW
        public string? FullName { get; set; }

        // NEW
        public string? Phone { get; set; }

        // NEW
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}