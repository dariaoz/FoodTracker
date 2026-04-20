using FoodTracker.Api.Domain.Entities;
using FoodTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodTracker.Api.Controllers;

[ApiController]
[Route("recipes")]
public class RecipesController : ControllerBase
{
    private readonly IRecipeService _service;

    public RecipesController(IRecipeService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(await _service.GetAllAsync(ct));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken ct)
    {
        var recipe = await _service.GetByIdAsync(id, ct);
        return recipe is null ? NotFound() : Ok(recipe);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Recipe recipe, CancellationToken ct)
    {
        var created = await _service.CreateAsync(recipe, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Recipe recipe, CancellationToken ct)
    {
        var updated = await _service.UpdateAsync(
            new Recipe
            {
                Id = id,
                Name = recipe.Name,
                Servings = recipe.Servings,
                Calories = recipe.Calories,
                Protein = recipe.Protein,
                Carbs = recipe.Carbs,
                Fat = recipe.Fat
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
