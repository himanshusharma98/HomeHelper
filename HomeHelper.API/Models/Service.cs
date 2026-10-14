
namespace HomeHelper.API.Models;

public enum PricingType
{
    Fixed = 1,
    StartingAt = 2,
    Quote = 3
}

public class Service
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public PricingType PricingType { get; set; }

    // Fixed and StartingAt require a price.
    // Quote services can leave this null.
    public decimal? BasePrice { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
