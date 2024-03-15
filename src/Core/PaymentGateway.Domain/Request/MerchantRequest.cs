using PaymentGateway.Domain.Common;

namespace PaymentGateway.Domain.Request;

public class MerchantRequest : BaseAuditableEntity
{
    public class UpdateNameMerchant
    {
        public string LastUpdatedBy { get; set; } = string.Empty;
        public string? MerchantName { get; set; } = string.Empty;
    }

    public class IsActiveMerchant
    {
        public bool IsActive { get; set; }
        public string LastUpdatedBy { get; set; } = string.Empty;

    }

    public class CreateMerchant
    {
        public string MerchantName { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
    }
}