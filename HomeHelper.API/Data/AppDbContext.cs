using HomeHelper.Api.Models;
using HomeHelper.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HomeHelper.Api.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<Service> Services => Set<Service>();
    public DbSet<HelperService> HelperServices => Set<HelperService>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Service>(entity =>
        {
            entity.Property(s => s.Name)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(s => s.Description)
                .HasMaxLength(1000)
                .IsRequired();

            // Store pricing types as readable strings in SQL Server.
            entity.Property(s => s.PricingType)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            // Support prices such as 499.00 and allow null for Quote.
            entity.Property(s => s.BasePrice)
                .HasPrecision(10, 2);
        });

        builder.Entity<HelperService>(entity =>
        {
            entity.HasOne(hs => hs.Helper)
                .WithMany()
                .HasForeignKey(hs => hs.HelperId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(hs => hs.Service)
                .WithMany()
                .HasForeignKey(hs => hs.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(hs => hs.PricingType)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(hs => hs.Price)
                .HasPrecision(10, 2);

            entity.Property(hs => hs.Notes)
                .HasMaxLength(1000);
        });

    }

}

