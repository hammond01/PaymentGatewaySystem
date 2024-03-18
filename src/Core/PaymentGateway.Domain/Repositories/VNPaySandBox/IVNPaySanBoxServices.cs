using Microsoft.AspNetCore.Http;
using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Response.VNPaySandBox;
using static PaymentGateway.Domain.Request.VNPaySandBox.VNPaySanBoxRequest;

namespace PaymentGateway.Domain.Repositories.VNPaySandBox;

public interface IVNPaySandBoxServices
{
    Task<BaseResultWithData<PaymentUrlResponse>> CreatePaymentUrl(HttpContext context, CreateStringUrl urlString);
    Task<BaseResultWithData<object>> PaymentExecute(HttpContext context, IQueryCollection queryCollection);
}