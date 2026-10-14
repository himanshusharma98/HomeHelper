
using HomeHelper.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeHelper.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ServicesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /api/services
    [HttpGet]
    public async Task<IActionResult> GetServices()
    {
        var services = await _context.Services
            .AsNoTracking()
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .Select(s => new
            {
                s.Id,
                s.Name,
                s.Description,
                PricingType = s.PricingType.ToString(),
                s.BasePrice,
                s.IsActive,
                s.CreatedAtUtc
            })
            .ToListAsync();

        return Ok(services);
    }

    // GET: /api/services/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetService(int id)
    {
        var service = await _context.Services
            .AsNoTracking()
            .Where(s => s.Id == id && s.IsActive)
            .Select(s => new
            {
                s.Id,
                s.Name,
                s.Description,
                PricingType = s.PricingType.ToString(),
                s.BasePrice,
                s.IsActive,
                s.CreatedAtUtc
            })
            .FirstOrDefaultAsync();

        if (service is null)
            return NotFound(new { message = "Service not found." });

        return Ok(service);
    }
}

