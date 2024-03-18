using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Request;
using PaymentGateway.Domain.Response.VNPaySandBox;

namespace PaymentGateway.Domain.Repositories;

public interface IPaymentTransactionService
{
    Task<BaseResultWithData<CreatePaymentResponse>> CreatePaymentTransactionAsync(PaymentTransactionRequest paymentTransactionRequest);
    Task<BaseResult> UpdatePaymentTransactionAsync(PaymentCompletion paymentCompletion);
}