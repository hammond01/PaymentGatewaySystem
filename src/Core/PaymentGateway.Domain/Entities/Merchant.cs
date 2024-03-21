using PaymentGateway.Domain.Common;

namespace PaymentGateway.Domain.Entities;

public class Merchant : BaseAuditableEntity
{
    public int Id { get; set; }
    public string MerchantId { get; set; } = string.Empty;
    public string? MerchantName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool Deleted { get; set; }
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
    public DateTime LastUpdatedAt { get; set; }
}
public class IsActiveMerchantModel
{
    public bool IsActive { get; set; }
    public string LastUpdatedBy { get; set; } = string.Empty;
}
public class DeleteMerchantModel
{
    public string LastUpdatedBy { get; set; } = string.Empty;
}