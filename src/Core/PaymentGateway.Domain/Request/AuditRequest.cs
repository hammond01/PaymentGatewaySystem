namespace PaymentGateway.Domain.Request;

public class AuditRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string ActionStatus { get; set; } = string.Empty;
    public string ActionIp { get; set; } = string.Empty;
    public string ControllerName { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
}