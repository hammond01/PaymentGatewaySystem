using Microsoft.AspNetCore.Http;
using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Entities.ThirdParty.VNPayEntities;

namespace PaymentGateway.Domain.Repositories.VNPaySandBox;

public interface IVNPaySandBoxServices
{
    Task<BaseResultWithData<string>> CreatePaymentUrl(HttpContext context, CreateStringUrlRequest urlString);
    Task<BaseResultWithData<object>> PaymentExecute(HttpContext context, IQueryCollection queryCollection);
    Task<BaseResultWithData<object>> Refund(HttpContext context, RefundRequestClient refundRequest);
    Task<BaseResultWithData<object>> GetTransactionDetail(HttpContext context, string transactionId);
}