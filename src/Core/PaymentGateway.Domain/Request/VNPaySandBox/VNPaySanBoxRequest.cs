namespace PaymentGateway.Domain.Request.VNPaySandBox;

public class VNPaySanBoxRequest
{
    public class CreateStringUrl
    {
        public int OrderId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PaymentContent { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? MerchantId { get; set; } = string.Empty;

    }
    public class CreatePayment
    {
        public string PaymentId { get; set; } = Guid.NewGuid().ToString();
        public string PaymentContent { get; set; } = string.Empty;
        public string PaymentCurrency { get; set; } = string.Empty;
        public DateTime? PaymentDate { get; set; } = DateTime.Now;
        public decimal PaidAmount { get; set; }
        public DateTime? ExpireDate { get; set; } = DateTime.Now.AddMinutes(15);
        public string? PaymentLanguage { get; set; } = string.Empty;
        public string? MerchantId { get; set; } = string.Empty;
        public string? PaymentStatus { get; set; } = string.Empty;
    }
}