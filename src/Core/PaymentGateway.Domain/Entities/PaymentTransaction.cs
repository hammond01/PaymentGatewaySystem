using PaymentGateway.Domain.Constants;

namespace PaymentGateway.Domain.Entities;

public class PaymentTransaction
{
    public string PaymentTransactionId { get; set; } = string.Empty;
    public string PaymentContent { get; set; } = string.Empty;
    public string PaymentCurrency { get; set; } = string.Empty;
    public DateTime? PaymentDate { get; set; }
    public DateTime? ExpireDate { get; set; }
    public string? PaymentLanguage { get; set; } = string.Empty;
    public string? MerchantId { get; set; } = string.Empty;
    public decimal? PaidAmount { get; set; }
    public string? PaymentStatus { get; set; } = string.Empty;
    public string? PaymentLastMessage { get; set; } = string.Empty;
    public string? PaymentCompletionTime { get; set; } = string.Empty;

    public static PaymentTransaction GeneratePaymentTransaction(CreatePaymentTransactionModel request)
    {
        var paymentTransaction = new PaymentTransaction
        {
            PaymentTransactionId = request.PaymentTransactionId,
            PaymentContent = request.PaymentContent,
            PaymentCurrency = request.PaymentCurrency,
            PaymentDate = DateTime.Now,
            ExpireDate = DateTime.Now.AddMinutes(15),
            PaymentLanguage = request.PaymentLanguage,
            MerchantId = request.MerchantId,
            PaidAmount = request.PaidAmount,
            PaymentStatus = PaymentStatusConstants.Pending,
            PaymentLastMessage = "",
            PaymentCompletionTime = request.PaymentCompletionTime
        };
        return paymentTransaction;
    }
}
//update payment transaction when payment is completed
public class PaymentCompletion
{
    public string PaymentTransactionId { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string PaymentLastMessage { get; set; } = string.Empty;
    public string PaymentCompletionTime { get; set; } = string.Empty;

}
public class CreatePaymentTransactionModel
{
    public string PaymentTransactionId { get; set; } = string.Empty;
    public string PaymentContent { get; set; } = string.Empty;
    public string PaymentCurrency { get; set; } = string.Empty;
    public decimal PaidAmount { get; set; }
    public string? PaymentLanguage { get; set; } = string.Empty;
    public string? MerchantId { get; set; } = string.Empty;
    public string? PaymentStatus { get; set; } = string.Empty;
    public string? IpAddress { get; set; } = string.Empty;
    public string? PaymentCompletionTime { get; set; } = string.Empty;
}

public class CheckTransactionStatus
{
    public string PaymentTransactionId { get; set; } = string.Empty;
    public string PaymentContent { get; set; } = string.Empty;
    public string MerchantName { get; set; } = string.Empty;
    public decimal PaidAmount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public string? PaymentCompletionTime { get; set; } = string.Empty;
}