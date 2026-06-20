namespace CarRentalAPI.Models;

public class Booking
{
    public int Id { get; set; }

    // USER INFO
    public int UserId { get; set; }
    public User User { get; set; }

    public string CustomerName { get; set; }
    public string Phone { get; set; }

    // CAR INFO
    public int CarId { get; set; }
    public Car Car { get; set; }

    // DRIVER INFO
    public bool WithDriver { get; set; }
    public int? DriverId { get; set; }

    // DATE INFO
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string Time { get; set; }

    // PICKUP / DROP LOCATION 🔥
    public string? PickupLocation { get; set; }
    public string? DropLocation { get; set; }

    // EXTRA
    public decimal? TotalAmount { get; set; }

    public string Status { get; set; } = "Pending";
}