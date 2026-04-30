using FoodTracker.Api.Models.Recipes;
using FoodTracker.Application.Recipes;
using FoodTracker.Application.Recipes.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodTracker.Api.Controllers;

[ApiController]
[Route("recipes")]
public class RecipesController : ControllerBase
{
    private readonly IRecipeService _service;

    public RecipesController(IRecipeService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery] string? name, CancellationToken ct)
    {
        var recipes = await _service.SearchAsync(new RecipeFilter(name), ct);
        return Ok(recipes.Select(r => r.ToResponse()).ToList());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(string id, CancellationToken ct)
    {
        var recipe = await _service.GetByIdAsync(id, ct);
        return recipe is null ? NotFound() : Ok(recipe.ToResponse());
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] RecipeRequest request, CancellationToken ct)
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
