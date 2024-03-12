using PaymentGateway.Domain.Common;

namespace PaymentGateway.Domain.Entities;
public class PaymentDestination : BaseAuditableEntity
{
    public string DestinationId { get; set; } = string.Empty;
    public string? DestinationName { get; set; } = string.Empty;
    public string? DestinationShortName { get; set; } = string.Empty;
    public string? DestinationParentId { get; set; } = string.Empty;
    public string? DestinationLogo { get; set; } = string.Empty;
    public int DestinationSortIndex { get; set; }
    public bool IsActive { get; set; }
}
