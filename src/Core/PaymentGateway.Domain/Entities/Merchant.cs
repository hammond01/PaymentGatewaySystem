﻿using PaymentGateway.Domain.Common;

namespace PaymentGateway.Domain.Entities;

public class Merchant : BaseAuditableEntity
{
    public string MerchantId { get; set; } = string.Empty;
    public string? MerchantName { get; set; } = string.Empty;
    public string? MerchantWebLink { get; set; } = string.Empty;
    public string? MerchantIpnUrl { get; set; } = string.Empty;
    public string? MerchantReturnUrl { get; set; } = string.Empty;
    public string? SecretKey { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}