namespace PaymentGateway.Domain.Entities;
public class PaymentNotification
{
    public string NotificationId { get; set; } = string.Empty;
    public string PaymentRefId { get; set; } = string.Empty;
    public DateTime? NotificationDate { get; set; }
    public string? NotificationContent { get; set; } = string.Empty;
    public decimal NotificationAmount { get; set; }
    public string? NotificationMessage { get; set; } = string.Empty;
    public string? NotificationSignature { get; set; } = string.Empty;
    public string? PaymentId { get; set; } = string.Empty;
    public string? MerchantId { get; set; } = string.Empty;
    public string? NotificationStatus { get; set; } = string.Empty;
    public DateTime? NotificationResDate { get; set; }
    public string? NotiResMessage { get; set; } = string.Empty;
    public string? NotiResHttpCode { get; set; } = string.Empty;
}
