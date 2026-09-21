using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SecurePim.Api.Security;

namespace SecurePim.Api.Tests;

public sealed class JwtApiFactory : WebApplicationFactory<Program>
{
    public const string TestIssuer = "https://test-issuer.securepim.local/";
    public const string TestAudience = "https://securepim-api";

    private static readonly SymmetricSecurityKey SigningKey = new(
        Encoding.UTF8.GetBytes("local-test-signing-key-with-at-least-32-bytes"));
    private static readonly SymmetricSecurityKey UntrustedSigningKey = new(
        Encoding.UTF8.GetBytes("untrusted-signing-key-with-at-least-32-bytes"));

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.PostConfigure<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.Authority = null;
                    options.MetadataAddress = string.Empty;
                    options.ConfigurationManager = null!;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = TestIssuer,
                        ValidateAudience = true,
                        ValidAudience = TestAudience,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = SigningKey,
                        ClockSkew = TimeSpan.Zero,
                        NameClaimType = "sub"
                    };
                });
        });
    }

    public static string CreateToken(
        string scopes = $"{ApiSecurity.ReadProductsScope} {ApiSecurity.WriteProductsScope}",
        string? tenantId = "tenant-demo",
        string audience = TestAudience,
        DateTime? expires = null,
        string issuer = TestIssuer,
        bool useUntrustedSigningKey = false)
    {
        var claims = new List<Claim>
        {
            new Claim("sub", "test-user-123"),
            new Claim("scope", scopes)
        };

        if (tenantId is not null)
        {
            claims.Add(new Claim(ApiSecurity.TenantIdClaim, tenantId));
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow.AddMinutes(-10),
            expires: expires ?? DateTime.UtcNow.AddMinutes(5),
            signingCredentials: new SigningCredentials(
                useUntrustedSigningKey ? UntrustedSigningKey : SigningKey,
                SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
