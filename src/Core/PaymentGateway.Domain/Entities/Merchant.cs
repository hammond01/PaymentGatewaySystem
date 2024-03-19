using PaymentGateway.Domain.Common;

namespace PaymentGateway.Domain.Entities;

public class Merchant : BaseAuditableEntity
{
    public string MerchantId { get; set; } = string.Empty;
    public string? MerchantName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CreateMerchantModel
{
    public string MerchantName { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
}

public class GetMerchantModel : BaseAuditableEntity
{
    public string? MerchantName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class UpdateNameMerchantModel
{
    public string LastUpdatedBy { get; set; } = string.Empty;
    public string? MerchantName { get; set; } = string.Empty;
}
public class IsActiveMerchantModel
{
    public bool IsActive { get; set; }
    public string LastUpdatedBy { get; set; } = string.Empty;

}