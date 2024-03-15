using PaymentGateway.Domain.Common;

namespace PaymentGateway.Domain.Response;

public class MerchantResponse : BaseAuditableEntity
{
    public string? MerchantName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}