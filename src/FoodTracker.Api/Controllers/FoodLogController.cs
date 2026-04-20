using FoodTracker.Api.Domain.Entities;
using FoodTracker.Api.Services;
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
        return Ok(logs);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken ct)
    {
        var log = await _service.GetByIdAsync(id, ct);
        return log is null ? NotFound() : Ok(log);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FoodLog log, CancellationToken ct)
    {
        var created = await _service.CreateAsync(log, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] FoodLog log, CancellationToken ct)
    {
        var updated = await _service.UpdateAsync(
            new FoodLog
            {
                Id = id,
                Date = log.Date,
                RecipeId = log.RecipeId,
                ProductId = log.ProductId,
                ServingUnit = log.ServingUnit,
                Quantity = log.Quantity,
                PortionQ = log.PortionQ,
                Calories = log.Calories,
                Protein = log.Protein,
                Carbs = log.Carbs,
                Fat = log.Fat
            }, ct);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return NoContent();
    }
}
