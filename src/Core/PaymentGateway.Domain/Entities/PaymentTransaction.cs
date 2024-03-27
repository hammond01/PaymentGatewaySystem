namespace PaymentGateway.Domain.Entities;

public class PaymentTransaction
{
    public long PaymentTransactionId { get; set; }
    public string? PaymentContent { get; set; }
    public string? PaymentCurrency { get; set; }
    public DateTime? PaymentDate { get; set; }
    public DateTime? ExpireDate { get; set; }
    public string? PaymentLanguage { get; set; }
    public long MerchantId { get; set; }
    public decimal? PaidAmount { get; set; }
    public string? PaymentStatus { get; set; }
    public string? PaymentLastMessage { get; set; }
    public long ResponseCodeId { get; set; }
    public string? PaymentCompletionTime { get; set; }
    public string? Channel { get; set; }
    public string? ClientName { get; set; }
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
            PaymentStatus = request.PaymentStatus,
            PaymentLastMessage = request.PaymentLastMessage,
            PaymentCompletionTime = request.PaymentCompletionTime,
            Channel = request.Channel,
            ClientName = request.ClientName,
            ResponseCodeId = request.ResponseCodeId
        };
        return paymentTransaction;
    }
}

//update payment transaction when payment is completed
public class PaymentCompletion
{
    public string? PaymentStatus { get; set; }
    public string? PaymentLastMessage { get; set; }
    public string? PaymentCompletionTime { get; set; }
    public long ResponseCodeId { get; set; }
}

public class CreatePaymentTransactionModel
{
    public string? UserId { get; set; }
    public long PaymentTransactionId { get; set; }
    public string? PaymentContent { get; set; }
    public string? PaymentCurrency { get; set; }
    public long PaidAmount { get; set; }
    public string? PaymentLanguage { get; set; }
    public long MerchantId { get; set; }
    public string? PaymentStatus { get; set; }
    public string? IpAddress { get; set; }
    public string? PaymentLastMessage { get; set; }
    public string? PaymentCompletionTime { get; set; }
    public string? Channel { get; set; }
    public string? ClientName { get; set; }
    public long ResponseCodeId { get; set; }
}

public class CheckTransactionStatus
{
    public long TransactionNo { get; set; }
    public string? PaymentContent { get; set; }
    public string? MerchantName { get; set; }
    public long PaidAmount { get; set; }
    public string? PaymentStatus { get; set; }
    public string? PaymentCompletionTime { get; set; }
}

public class GetAllPaymentTransaction
{
    public long PaymentTransactionId { get; set; }
    public string? PaymentContent { get; set; }
    public string? PaymentCurrency { get; set; }
    public DateTime? PaymentDate { get; set; }
    public DateTime? ExpireDate { get; set; }
    public string? PaymentLanguage { get; set; }
    public string? MerchantName { get; set; }
    public long PaidAmount { get; set; }
    public string? PaymentStatus { get; set; }
    public string? PaymentLastMessage { get; set; }
    public string? PaymentCompletionTime { get; set; }
    public string? Channel { get; set; }
    public string? ClientName { get; set; }
}