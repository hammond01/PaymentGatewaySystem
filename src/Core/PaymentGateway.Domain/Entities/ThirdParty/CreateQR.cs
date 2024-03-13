using System.ComponentModel.DataAnnotations;
namespace PaymentGateway.Domain.Entities.ThirdParty;
public class CreateQR
{
    [Required(ErrorMessage = "AppId is required")]
    [MaxLength(100, ErrorMessage = "AppId cannot exceed 100 characters")]
    public string appId { get; set; } = string.Empty;

    [Required(ErrorMessage = "MerchantName is required")]
    [MaxLength(25, ErrorMessage = "MerchantName cannot exceed 25 characters")]
    public string merchantName { get; set; } = string.Empty;

    [Required(ErrorMessage = "ServiceCode is required")]
    [MaxLength(20, ErrorMessage = "ServiceCode cannot exceed 20 characters")]
    public string serviceCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "CountryCode is required")]
    [MaxLength(2, ErrorMessage = "CountryCode cannot exceed 2 characters")]
    public string countryCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "MerchantCode is required")]
    [MaxLength(20, ErrorMessage = "MerchantCode cannot exceed 20 characters")]
    public string merchantCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "TerminalId is required")]
    [MaxLength(8, ErrorMessage = "TerminalId cannot exceed 8 characters")]
    public string terminalId { get; set; } = string.Empty;

    [Required(ErrorMessage = "PayType is required")]
    [MaxLength(4, ErrorMessage = "PayType cannot exceed 4 characters")]
    public string payType { get; set; } = string.Empty;

    [MaxLength(20, ErrorMessage = "ProductId cannot exceed 20 characters")]
    public string productId { get; set; } = string.Empty;

    [MaxLength(15, ErrorMessage = "TxnId cannot exceed 15 characters")]
    public string txnId { get; set; } = string.Empty;

    [MaxLength(20, ErrorMessage = "BillNumber cannot exceed 20 characters")]
    public string billNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Amount is required")]
    [MaxLength(13, ErrorMessage = "Amount cannot exceed 13 characters")]
    public string amount { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ccy is required")]
    [MaxLength(3, ErrorMessage = "Ccy cannot exceed 3 characters")]
    public string ccy { get; set; } = string.Empty;

    [Required(ErrorMessage = "ExpDate is required")]
    [MaxLength(14, ErrorMessage = "ExpDate cannot exceed 14 characters")]
    public string expDate { get; set; } = string.Empty;

    [MaxLength(19, ErrorMessage = "Description cannot exceed 19 characters")]
    public string desc { get; set; } = string.Empty;

    [Required(ErrorMessage = "MasterMerCode is required")]
    [MaxLength(100, ErrorMessage = "MasterMerCode cannot exceed 100 characters")]
    public string masterMerCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "MerchantType is required")]
    [MaxLength(9, ErrorMessage = "MerchantType cannot exceed 9 characters")]
    public string merchantType { get; set; } = string.Empty;

    [MaxLength(20, ErrorMessage = "TipAndFee cannot exceed 20 characters")]
    public string tipAndFee { get; set; } = string.Empty;

    [MaxLength(20, ErrorMessage = "ConsumerId cannot exceed 20 characters")]
    public string? consumerId { get; set; } = string.Empty;

    [MaxLength(19, ErrorMessage = "Purpose cannot exceed 19 characters")]
    public string purpose { get; set; } = string.Empty;

    [Required(ErrorMessage = "Checksum is required")]
    [MaxLength(32, ErrorMessage = "Checksum cannot exceed 32 characters")]
    public string checksum { get; set; } = string.Empty;
}

