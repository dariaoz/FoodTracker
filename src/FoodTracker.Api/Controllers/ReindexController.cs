using FoodTracker.Application.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FoodTracker.Api.Controllers;

[ApiController]
[Route("admin")]
public class ReindexController : ControllerBase
{
    private readonly IIndexingService _indexing;

    public ReindexController(IIndexingService indexing) => _indexing = indexing;

    [HttpPost("reindex")]
    public async Task<IActionResult> ReindexAsync(CancellationToken ct)
    {
        await _indexing.ReindexAsync(ct);
        return Ok();
    }
}
