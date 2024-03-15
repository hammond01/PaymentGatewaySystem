using Microsoft.AspNetCore.Mvc;
namespace PaymentGateway.Domain.Common;

[BindProperties]
public class BasePagingQuery
{
    public string Criteria { get; set; } = string.Empty;
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}