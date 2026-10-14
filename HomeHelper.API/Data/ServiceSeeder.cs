
using HomeHelper.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeHelper.Api.Data;

public static class ServiceSeeder
{
    public static async Task SeedServicesAsync(AppDbContext context)
    {
        if (await context.Services.AnyAsync())
            return;

        var services = new List<Service>
        {
            new()
            {
                Name = "Fan Installation",
                Description = "Installation of a standard ceiling fan.",
                PricingType = PricingType.Fixed,
                BasePrice = 499m
            },
            new()
            {
                Name = "Bathroom Cleaning",
                Description = "Bathroom cleaning; final price depends on size and condition.",
                PricingType = PricingType.StartingAt,
                BasePrice = 399m
            },
            new()
            {
                Name = "Major Plumbing Repair",
                Description = "Complex plumbing repairs requiring inspection and a custom quote.",
                PricingType = PricingType.Quote,
                BasePrice = null
            }
        };

        context.Services.AddRange(services);
        await context.SaveChangesAsync();
    }
}
