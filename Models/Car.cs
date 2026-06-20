using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalAPI.Models;

public class Car
{
    public int Id { get; set; }

    // BASIC
    public string? Name { get; set; }
    public string? Brand { get; set; }

    // DETAILS
    public string? Model { get; set; }
    public int? Year { get; set; }
    public string? FuelType { get; set; }
    public string? Transmission { get; set; }
    public int? Seats { get; set; }
    public string? CarType { get; set; }

    // PRICE
    public decimal? PricePerDay { get; set; }

    // MEDIA
    public string? ImageUrl { get; set; }

    [NotMapped] // VERY IMPORTANT
    public IFormFile? Image { get; set; }

    // STATUS
    public bool Available { get; set; } = true;

    // OPTIONAL
    public string? RegistrationNumber { get; set; }
    public int? Mileage { get; set; }

    public ICollection<Booking>? Bookings { get; set; }
}