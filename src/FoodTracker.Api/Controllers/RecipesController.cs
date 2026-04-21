using FoodTracker.Api.Models;
using FoodTracker.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodTracker.Api.Controllers;

[ApiController]
[Route("recipes")]
public class RecipesController : ControllerBase
{
    private readonly IRecipeService _service;

    public RecipesController(IRecipeService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var recipes = await _service.GetAllAsync(ct);
        return Ok(recipes.Select(r => r.ToResponse()).ToList());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken ct)
    {
        var recipe = await _service.GetByIdAsync(id, ct);
        return recipe is null ? NotFound() : Ok(recipe.ToResponse());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RecipeRequest request, CancellationToken ct)
    {
        var created = await _service.CreateAsync(request.ToDomain(), ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToResponse());
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return NoContent();
    }
}
