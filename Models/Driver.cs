namespace CarRentalAPI.Models;

public class Driver
{
    public int Id { get; set; }

    // BASIC
    public string Name { get; set; }
    public string Phone { get; set; }

    // PROFESSIONAL INFO
    public string LicenseNumber { get; set; }
    public DateTime? LicenseExpiry { get; set; }
    public int ExperienceYears { get; set; }

    public string Address { get; set; }
    
    public string Username { get; set; }
public string Password { get; set; }

    // STATUS
    public bool Available { get; set; } = true;

    public ICollection<Booking>? Bookings { get; set; }
}