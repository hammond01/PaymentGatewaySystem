using Microsoft.AspNetCore.Http;
using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Entities.ThirdParty.VNPayEntities;
using PaymentGateway.Domain.Request;

namespace PaymentGateway.Domain.Repositories.VNPaySandBox;

public interface IVnPaySandBoxServices
{
    Task<BaseResultWithData<string>> CreatePaymentUrl(HttpContext context, CreatePaymentSandboxRequest urlString);
    Task<BaseResult> PaymentExecute(HttpContext context, IQueryCollection queryCollection);
    Task<BaseResultWithData<object>> Refund(HttpContext context, RefundRequestClient refundRequest);
    Task<BaseResultWithData<object>> GetTransactionDetail(HttpContext context, string transactionId);
}