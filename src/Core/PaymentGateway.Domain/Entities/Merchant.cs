using PaymentGateway.Domain.Common;

namespace PaymentGateway.Domain.Entities;

public class Merchant : BaseAuditableEntity
{
    public int Id { get; set; }
    public long MerchantId { get; set; }
    public string? MerchantName { get; set; }
    public bool IsActive { get; set; }
    public bool Deleted { get; set; }
}

public class CreateMerchantModel
{
    public string? MerchantName { get; set; }
    public string? CreatedBy { get; set; }
}

public class GetMerchantModel : BaseAuditableEntity
{
    public long MerchantId { get; set; }
    public string? MerchantName { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateNameMerchantModel
{
    public string? LastUpdatedBy { get; set; }
    public string? MerchantName { get; set; }
    //public DateTime LastUpdatedAt { get; set; }
}
public class IsActiveMerchantModel
{
    public bool IsActive { get; set; }
    public string? LastUpdatedBy { get; set; }
}
public class DeleteMerchantModel
{
    public string? LastUpdatedBy { get; set; }
}