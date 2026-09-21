namespace SecurePim.Api.Products;

public sealed record CreateProductRequest(string? Sku, string? Name, string? Description);

public sealed record ProductResponse(Guid Id, string Sku, string Name, string? Description)
{
    public static ProductResponse FromProduct(Product product) =>
        new(product.Id, product.Sku, product.Name, product.Description);
}
