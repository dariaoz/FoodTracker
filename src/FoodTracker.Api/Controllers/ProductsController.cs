using FoodTracker.Api.Models.Products;
using FoodTracker.Application.Products.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodTracker.Api.Controllers;

[ApiController]
[Route("products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken ct)
    {
        var products = await _service.GetAllAsync(ct);
        return Ok(products.Select(p => p.ToResponse()).ToList());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(string id, CancellationToken ct)
    {
        var product = await _service.GetByIdAsync(id, ct);
        return product is null ? NotFound() : Ok(product.ToResponse());
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] ProductRequest request, CancellationToken ct)
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
