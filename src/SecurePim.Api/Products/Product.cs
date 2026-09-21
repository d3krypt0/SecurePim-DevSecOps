namespace SecurePim.Api.Products;

public sealed record Product(
    Guid Id,
    string TenantId,
    string Sku,
    string Name,
    string? Description);
