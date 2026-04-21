using FoodTracker.Api.Models;
using FoodTracker.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodTracker.Api.Controllers;

[ApiController]
[Route("foodlog")]
public class FoodLogController : ControllerBase
{
    private readonly IFoodLogService _service;

    public FoodLogController(IFoodLogService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DateOnly? date, CancellationToken ct)
    {
        var logs = date.HasValue
            ? await _service.GetByDateAsync(date.Value, ct)
            : await _service.GetAllAsync(ct);
        return Ok(logs.Select(l => l.ToResponse()).ToList());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken ct)
    {
        var log = await _service.GetByIdAsync(id, ct);
        return log is null ? NotFound() : Ok(log.ToResponse());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FoodLogRequest request, CancellationToken ct)
    {
        var created = await _service.CreateAsync(request.ToDomain(), ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToResponse());
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] FoodLogRequest request, CancellationToken ct)
    {
        var updated = await _service.UpdateAsync(request.ToDomain(id), ct);
        return Ok(updated.ToResponse());
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return NoContent();
    }
}
