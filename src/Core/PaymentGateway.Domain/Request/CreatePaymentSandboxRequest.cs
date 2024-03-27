using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Domain.Request;

public class CreatePaymentSandboxRequest
{
    [Required(ErrorMessage = "Order Id is required")]
    public long OrderId { get; set; }
    [Required(ErrorMessage = "Payment content is required")]
    public string? PaymentContent { get; set; }
    [Required(ErrorMessage = "Amount is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive number")]
    public long Amount { get; set; }
    public long MerchantId { get; set; }
    [Required]
    public string? Channel { get; set; }
    [Required]
    public string? ClientName { get; set; }
    public string? LastMessage { get; set; }
    [Required]
    public string? UserId { get; set; }
}