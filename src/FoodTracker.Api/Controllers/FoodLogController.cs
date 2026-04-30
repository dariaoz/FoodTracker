using FoodTracker.Api.Models.FoodLogs;
using FoodTracker.Application.FoodLogs;
using FoodTracker.Application.FoodLogs.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodTracker.Api.Controllers;

[ApiController]
[Route("foodlog")]
public class FoodLogController : ControllerBase
{
    private readonly IFoodLogService _service;

    public FoodLogController(IFoodLogService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] DateOnly? from, [FromQuery] DateOnly? to, CancellationToken ct)
    {
        var foodLogs = await _service.GetAsync(new FoodLogFilter(from, to), ct);
        return Ok(foodLogs.Select(fl => fl.ToResponse()).ToList());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(string id, CancellationToken ct)
    {
        var foodLog = await _service.GetByIdAsync(id, ct);
        return foodLog is null ? NotFound() : Ok(foodLog.ToResponse());
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] FoodLogRequest request, CancellationToken ct)
    {
        var created = await _service.CreateAsync(request.ToDomain(), ct);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = created.Id }, created.ToResponse());
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(string id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return NoContent();
    }
}
