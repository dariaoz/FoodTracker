using FoodTracker.Application.Products;
using FoodTracker.Domain.Products;
using FoodTracker.Infrastructure.Configuration;
using FoodTracker.Infrastructure.Shared;
using Microsoft.Extensions.Options;

namespace FoodTracker.Infrastructure.Products;

internal class ProductRepository : IProductRepository
{
    private readonly INotionClient _client;
    private readonly string _databaseId;

    public ProductRepository(INotionClient client, IOptions<NotionOptions> options)
    {
        _client = client;
        _databaseId = options.Value.ProductsDatabaseId;
    }

    public async Task<IList<Product>> GetAllAsync(CancellationToken ct = default)
    {
        NotionDatabase db = await _client.QueryDatabaseAsync(_databaseId, ct: ct);
        return db.Results.Select(ProductNotionMapper.ToEntity).ToList();
    }

    public async Task<Product?> GetByIdAsync(string pageId, CancellationToken ct = default)
    {
        NotionPage page = await _client.GetPageAsync(pageId, ct);
        return ProductNotionMapper.ToEntity(page);
    }

    public async Task<Product> CreateAsync(Product entity, CancellationToken ct = default)
    {
        NotionPage page = await _client.CreatePageAsync(_databaseId, ProductNotionMapper.ToNotionProperties(entity), ct);
        return ProductNotionMapper.ToEntity(page);
    }

    public async Task DeleteAsync(string pageId, CancellationToken ct = default) =>
        await _client.ArchivePageAsync(pageId, ct);
}
