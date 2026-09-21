using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using SecurePim.Api.Security;

namespace SecurePim.Api.Tests;

public sealed class ProductEndpointTests : IClassFixture<JwtApiFactory>
{
    private readonly JwtApiFactory factory;
    private readonly HttpClient client;

    public ProductEndpointTests(JwtApiFactory factory)
    {
        this.factory = factory;
        client = CreateClient(JwtApiFactory.CreateToken());
    }

    [Fact]
    public async Task GetProducts_WithValidToken_ReturnsSyntheticProductsWithoutTenantIdentifier()
    {
        using var response = await client.GetAsync("/api/products/");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("DEMO-001", body, StringComparison.Ordinal);
        Assert.DoesNotContain("tenant-demo", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidFields_ReturnsValidationProblem()
    {
        var request = new
        {
            sku = "../",
            name = " ",
            description = new string('x', 1_001)
        };

        using var response = await client.PostAsJsonAsync("/api/products/", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task CreateProduct_WithDuplicateSku_ReturnsConflict()
    {
        var sku = $"TEST-{Guid.NewGuid():N}"[..32];
        var request = new { sku, name = "Synthetic test product" };

        using var firstResponse = await client.PostAsJsonAsync("/api/products/", request);
        using var duplicateResponse = await client.PostAsJsonAsync(
            "/api/products/",
            new { sku = sku.ToLowerInvariant(), name = "Duplicate" });

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WithValidFields_CanBeRetrievedFromLocation()
    {
        var request = new
        {
            sku = $"ITEM-{Guid.NewGuid():N}"[..32],
            name = "  Synthetic Jacket  ",
            description = "  Sample product for integration testing  "
        };

        using var createResponse = await client.PostAsJsonAsync("/api/products/", request);
        var location = createResponse.Headers.Location;

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(location);

        using var getResponse = await client.GetAsync(location);
        var body = await getResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.Contains("Synthetic Jacket", body, StringComparison.Ordinal);
        Assert.DoesNotContain("tenant-demo", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetProducts_WithoutToken_ReturnsUnauthorized()
    {
        using var anonymousClient = CreateClient();

        using var response = await anonymousClient.GetAsync("/api/products/");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProducts_WithWrongAudience_ReturnsUnauthorized()
    {
        using var wrongAudienceClient = CreateClient(JwtApiFactory.CreateToken(audience: "wrong-audience"));

        using var response = await wrongAudienceClient.GetAsync("/api/products/");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProducts_WithUntrustedIssuer_ReturnsUnauthorized()
    {
        using var untrustedIssuerClient = CreateClient(JwtApiFactory.CreateToken(
            issuer: "https://untrusted-issuer.example/"));

        using var response = await untrustedIssuerClient.GetAsync("/api/products/");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProducts_WithInvalidSignature_ReturnsUnauthorized()
    {
        using var invalidSignatureClient = CreateClient(JwtApiFactory.CreateToken(
            useUntrustedSigningKey: true));

        using var response = await invalidSignatureClient.GetAsync("/api/products/");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProducts_WithExpiredToken_ReturnsUnauthorized()
    {
        using var expiredTokenClient = CreateClient(JwtApiFactory.CreateToken(
            expires: DateTime.UtcNow.AddMinutes(-5)));

        using var response = await expiredTokenClient.GetAsync("/api/products/");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProducts_WithoutTenantClaim_ReturnsForbidden()
    {
        using var missingTenantClient = CreateClient(JwtApiFactory.CreateToken(tenantId: null));

        using var response = await missingTenantClient.GetAsync("/api/products/");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WithReadScopeOnly_ReturnsForbidden()
    {
        using var readOnlyClient = CreateClient(JwtApiFactory.CreateToken(ApiSecurity.ReadProductsScope));

        using var response = await readOnlyClient.PostAsJsonAsync(
            "/api/products/",
            new { sku = "READ-ONLY-001", name = "Forbidden write" });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetProducts_WithDifferentTenant_DoesNotReturnDemoTenantProducts()
    {
        using var otherTenantClient = CreateClient(JwtApiFactory.CreateToken(tenantId: "tenant-other"));

        using var response = await otherTenantClient.GetAsync("/api/products/");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain("DEMO-001", body, StringComparison.Ordinal);
    }

    private HttpClient CreateClient(string? token = null)
    {
        var httpClient = factory.CreateClient(new()
        {
            BaseAddress = new Uri("https://localhost")
        });

        if (token is not null)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return httpClient;
    }
}
