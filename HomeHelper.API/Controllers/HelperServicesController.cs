using HomeHelper.Api.Data;
using HomeHelper.Api.Models;
using HomeHelper.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HomeHelper.Api.Controllers;

[ApiController]
[Route("api/helper-services")]
[Authorize(Roles = "Helper")]
public class HelperServicesController : ControllerBase
{
    private readonly AppDbContext _context;

    public HelperServicesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/helper-services/mine
    [HttpGet("mine")]
    public async Task<IActionResult> GetMine()
    {
        var helperId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (helperId is null)
            return Unauthorized();

        var offers = await _context.HelperServices
            .AsNoTracking()
            .Where(hs => hs.HelperId == helperId)
            .Select(hs => new
            {
                hs.Id,
                hs.ServiceId,
                ServiceName = hs.Service!.Name,
                PricingType = hs.PricingType.ToString(),
                hs.Price,
                hs.Notes,
                hs.IsActive
            })
            .ToListAsync();

        return Ok(offers);
    }

    // POST: api/helper-services
    [HttpPost]
    public async Task<IActionResult> Create(CreateHelperServiceRequest request)
    {
        var helperId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (helperId is null)
            return Unauthorized();

        if (!Enum.TryParse<PricingType>(
                request.PricingType, true, out var pricingType)
            || !Enum.IsDefined(pricingType))
        {
            return BadRequest(new
            {
                message = "PricingType must be Fixed, StartingAt, or Quote."
            });
        }

        if (pricingType == PricingType.Quote)
        {
            if (request.Price is not null)
                return BadRequest(new
                {
                    message = "Quote pricing must not include a price."
                });
        }
        else if (request.Price is null || request.Price <= 0)
        {
            return BadRequest(new
            {
                message = "Fixed and StartingAt pricing require a price greater than zero."
            });
        }

        var serviceExists = await _context.Services.AnyAsync(
            s => s.Id == request.ServiceId && s.IsActive);

        if (!serviceExists)
            return BadRequest(new { message = "Active service not found." });

        var alreadyOffered = await _context.HelperServices.AnyAsync(
            hs => hs.HelperId == helperId
                  && hs.ServiceId == request.ServiceId);

        if (alreadyOffered)
            return Conflict(new
            {
                message = "You already have an offer for this service."
            });

        var offer = new HelperService
        {
            HelperId = helperId,
            ServiceId = request.ServiceId,
            PricingType = pricingType,
            Price = request.Price,
            Notes = request.Notes?.Trim(),
            IsActive = true
        };

        _context.HelperServices.Add(offer);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetMine),
            new { },
            new
            {
                offer.Id,
                offer.ServiceId,
                PricingType = offer.PricingType.ToString(),
                offer.Price,
                offer.Notes,
                offer.IsActive
            });
    }

    // PUT: api/helper-services/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id, UpdateHelperServiceRequest request)
    {
        var helperId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (helperId is null)
            return Unauthorized();

        var offer = await _context.HelperServices
            .FirstOrDefaultAsync(hs =>
                hs.Id == id && hs.HelperId == helperId);

        if (offer is null)
            return NotFound(new { message = "Your service offer was not found." });

        if (!Enum.TryParse<PricingType>(
                request.PricingType, true, out var pricingType)
            || !Enum.IsDefined(pricingType))
        {
            return BadRequest(new
            {
                message = "PricingType must be Fixed, StartingAt, or Quote."
            });
        }

        if (pricingType == PricingType.Quote)
        {
            if (request.Price is not null)
                return BadRequest(new
                {
                    message = "Quote pricing must not include a price."
                });
        }
        else if (request.Price is null || request.Price <= 0)
        {
            return BadRequest(new
            {
                message = "Fixed and StartingAt pricing require a price greater than zero."
            });
        }

        if (request.Notes?.Length > 1000)
            return BadRequest(new { message = "Notes cannot exceed 1000 characters." });

        offer.PricingType = pricingType;
        offer.Price = request.Price;
        offer.Notes = request.Notes?.Trim();

        await _context.SaveChangesAsync();

        return Ok(new
        {
            offer.Id,
            offer.ServiceId,
            PricingType = offer.PricingType.ToString(),
            offer.Price,
            offer.Notes,
            offer.IsActive
        });
    }

    // PATCH: api/helper-services/1/deactivate
    [HttpPatch("{id:int}/deactivate")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var helperId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (helperId is null)
            return Unauthorized();

        var offer = await _context.HelperServices
            .FirstOrDefaultAsync(hs =>
                hs.Id == id && hs.HelperId == helperId);

        if (offer is null)
            return NotFound(new { message = "Your service offer was not found." });

        offer.IsActive = false;
        await _context.SaveChangesAsync();

        return NoContent();
    }

}

public class CreateHelperServiceRequest
{
    public int ServiceId { get; set; }
    public string PricingType { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public string? Notes { get; set; }
}


public class UpdateHelperServiceRequest
{
    public string PricingType { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public string? Notes { get; set; }
}

