namespace PaymentGateway.Domain.Entities.Audit;
#nullable disable
public class AuditModel
{
    public string AuditLogId { get; set; }
    public string UserId { get; set; }
    public string Action { get; set; }
    public string ControllerName { get; set; }
    public string CreatedAt { get; set; } = DateTime.Now.ToString("dd/MM/yyyy");
    public string ActionStatus { get; set; }
    public string ActionIp { get; set; }
    public string PaymentStatus { get; set; }

}