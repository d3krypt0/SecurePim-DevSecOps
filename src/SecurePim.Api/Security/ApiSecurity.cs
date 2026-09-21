using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace SecurePim.Api.Security;

public static class ApiSecurity
{
    public const string ReadProductsPolicy = "ReadProducts";
    public const string WriteProductsPolicy = "WriteProducts";
    public const string ReadProductsScope = "products:read";
    public const string WriteProductsScope = "products:write";
    public const string TenantIdClaim = "https://securepim.example/tenant_id";

    public static IServiceCollection AddApiSecurity(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var authority = configuration["Authentication:Authority"]
            ?? throw new InvalidOperationException("Authentication:Authority is required.");
        var audience = configuration["Authentication:Audience"]
            ?? throw new InvalidOperationException("Authentication:Audience is required.");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience = audience;
                options.RequireHttpsMetadata = true;
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.FromMinutes(1),
                    NameClaimType = "sub"
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy(ReadProductsPolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("sub");
                policy.RequireClaim(TenantIdClaim);
                policy.RequireAssertion(context => HasScope(context.User, ReadProductsScope));
            })
            .AddPolicy(WriteProductsPolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("sub");
                policy.RequireClaim(TenantIdClaim);
                policy.RequireAssertion(context => HasScope(context.User, WriteProductsScope));
            });

        return services;
    }

    private static bool HasScope(System.Security.Claims.ClaimsPrincipal user, string requiredScope) =>
        user.FindAll("scope")
            .SelectMany(claim => claim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Contains(requiredScope, StringComparer.Ordinal);
}
