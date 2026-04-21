using FoodTracker.Api.Models.FoodLogs;
using FoodTracker.Application.FoodLogs;
using Microsoft.AspNetCore.Mvc;

namespace FoodTracker.Api.Controllers;

[ApiController]
[Route("foodlog")]
public class FoodLogController : ControllerBase
{
    private readonly IFoodLogService _service;

    public FoodLogController(IFoodLogService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] DateOnly? date, CancellationToken ct)
    {
        var foodLogs = date.HasValue
            ? await _service.GetByDateAsync(date.Value, ct)
            : await _service.GetAllAsync(ct);
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
