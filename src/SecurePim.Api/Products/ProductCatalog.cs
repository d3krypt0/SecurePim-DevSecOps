namespace SecurePim.Api.Products;

public sealed class ProductCatalog
{
    private readonly Lock syncRoot = new();
    private readonly List<Product> products =
    [
        new(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "tenant-demo",
            "DEMO-001",
            "Synthetic Trail Shoe",
            "Non-production sample product")
    ];

    public IReadOnlyList<Product> GetAll(string tenantId)
    {
        lock (syncRoot)
        {
            return products
                .Where(product => product.TenantId == tenantId)
                .OrderBy(product => product.Sku, StringComparer.Ordinal)
                .ToArray();
        }
    }

    public Product? GetById(string tenantId, Guid id)
    {
        lock (syncRoot)
        {
            return products.SingleOrDefault(product =>
                product.TenantId == tenantId && product.Id == id);
        }
    }

    public bool TryCreate(
        string tenantId,
        string sku,
        string name,
        string? description,
        out Product product)
    {
        lock (syncRoot)
        {
            if (products.Any(existing =>
                    existing.TenantId == tenantId &&
                    string.Equals(existing.Sku, sku, StringComparison.OrdinalIgnoreCase)))
            {
                product = null!;
                return false;
            }

            product = new Product(Guid.NewGuid(), tenantId, sku, name, description);
            products.Add(product);
            return true;
        }
    }
}
