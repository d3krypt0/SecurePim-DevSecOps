namespace SecurePim.Api.Products;

public static class ProductRequestValidator
{
    public static Dictionary<string, string[]> Validate(CreateProductRequest request)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var sku = request.Sku?.Trim();
        var name = request.Name?.Trim();

        if (string.IsNullOrWhiteSpace(sku))
        {
            errors[nameof(request.Sku)] = ["SKU is required."];
        }
        else if (sku.Length is < 3 or > 32 || !sku.All(IsAllowedSkuCharacter))
        {
            errors[nameof(request.Sku)] =
                ["SKU must be 3-32 characters using letters, numbers, periods, underscores, or hyphens."];
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            errors[nameof(request.Name)] = ["Name is required."];
        }
        else if (name.Length > 100)
        {
            errors[nameof(request.Name)] = ["Name must not exceed 100 characters."];
        }

        if (request.Description?.Length > 1_000)
        {
            errors[nameof(request.Description)] = ["Description must not exceed 1000 characters."];
        }

        return errors;
    }

    private static bool IsAllowedSkuCharacter(char character) =>
        char.IsAsciiLetterOrDigit(character) || character is '.' or '_' or '-';
}
