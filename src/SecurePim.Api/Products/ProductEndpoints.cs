using System.Security.Claims;
using SecurePim.Api.Security;

namespace SecurePim.Api.Products;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/products");

        group.MapGet("/", (ClaimsPrincipal user, ProductCatalog catalog) =>
            Results.Ok(catalog
                .GetAll(GetTenantId(user))
                .Select(ProductResponse.FromProduct)))
            .RequireAuthorization(ApiSecurity.ReadProductsPolicy);

        group.MapGet("/{id:guid}", (Guid id, ClaimsPrincipal user, ProductCatalog catalog) =>
        {
            var product = catalog.GetById(GetTenantId(user), id);
            return product is null
                ? Results.NotFound()
                : Results.Ok(ProductResponse.FromProduct(product));
        }).RequireAuthorization(ApiSecurity.ReadProductsPolicy);

        group.MapPost("/", (
            CreateProductRequest request,
            ClaimsPrincipal user,
            ProductCatalog catalog,
            ILogger<ProductCatalog> logger) =>
        {
            var errors = ProductRequestValidator.Validate(request);
            if (errors.Count > 0)
            {
                return Results.ValidationProblem(errors);
            }

            var sku = request.Sku!.Trim().ToUpperInvariant();
            var name = request.Name!.Trim();
            var description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim();
            var tenantId = GetTenantId(user);

            if (!catalog.TryCreate(tenantId, sku, name, description, out var product))
            {
                return Results.Conflict(new
                {
                    title = "A product with this SKU already exists."
                });
            }

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.ProductCreated(
                    product.Id,
                    user.FindFirst("sub")!.Value,
                    tenantId);
            }

            return Results.Created(
                $"/api/products/{product.Id}",
                ProductResponse.FromProduct(product));
        }).RequireAuthorization(ApiSecurity.WriteProductsPolicy);

        return endpoints;
    }

    private static string GetTenantId(ClaimsPrincipal user) =>
        user.FindFirst(ApiSecurity.TenantIdClaim)!.Value;
}
