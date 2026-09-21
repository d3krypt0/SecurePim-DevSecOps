namespace SecurePim.Api.Products;

internal static partial class ProductAuditLog
{
    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Information,
        Message = "AUDIT ProductCreated ProductId={ProductId} SubjectId={SubjectId} TenantId={TenantId}")]
    public static partial void ProductCreated(
        this ILogger logger,
        Guid productId,
        string subjectId,
        string tenantId);
}
