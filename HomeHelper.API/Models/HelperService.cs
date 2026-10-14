
using HomeHelper.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeHelper.Api.Models;

public class HelperService
{
    public int Id { get; set; }

    [Required]
    public string HelperId { get; set; } = string.Empty;

    [ForeignKey(nameof(HelperId))]
    public ApplicationUser? Helper { get; set; }

    public int ServiceId { get; set; }

    public Service? Service { get; set; }

    public PricingType PricingType { get; set; }

    // Required for Fixed and StartingAt; null for Quote.
    public decimal? Price { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
