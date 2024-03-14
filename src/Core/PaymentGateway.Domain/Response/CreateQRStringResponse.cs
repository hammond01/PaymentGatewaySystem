using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Domain.Response;

public class CreateQRStringResponse
{
    [Required]
    [MaxLength(20, ErrorMessage = "Code cannot exceed 20 characters")]
    public string code { get; set; } = string.Empty;

    [Required]
    [MaxLength(100, ErrorMessage = "Message cannot exceed 100 characters")]
    public string message { get; set; } = string.Empty;

    public string data { get; set; } = string.Empty;

    [Required]
    [MaxLength(32, ErrorMessage = "Checksum cannot exceed 32 characters")]
    public string checksum { get; set; } = string.Empty;

    public string idQrCode { get; set; } = string.Empty;
    //public string visa { get; set; } = string.Empty;
    //public string master { get; set; } = string.Empty;
    //public string unionPay { get; set; } = string.Empty;
    //public string url { get; set; } = string.Empty;
    //public string isDelete { get; set; } = string.Empty;
}