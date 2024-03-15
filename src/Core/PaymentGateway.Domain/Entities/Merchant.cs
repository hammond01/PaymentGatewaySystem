﻿using PaymentGateway.Domain.Common;

namespace PaymentGateway.Domain.Entities;

public class Merchant : BaseAuditableEntity
{
    public string MerchantId { get; set; } = string.Empty;
    public string? MerchantName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}