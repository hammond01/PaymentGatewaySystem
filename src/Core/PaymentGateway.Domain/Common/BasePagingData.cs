﻿namespace PaymentGateway.Domain.Common;

public class BasePagingData<T>
{
    public List<T> Items { get; set; } = new();
    public int PageSize { get; set; }
    public int PageIndex { get; set; }
    public int TotalPage { get; set; }
    public int TotalItems { get; set; }
    public string? NextPageUrl { get; set; } = string.Empty;
    public string? PreviousPageUrl { get; set; } = string.Empty;
}