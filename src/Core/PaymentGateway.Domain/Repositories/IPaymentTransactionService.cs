using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain.Repositories;

public interface IPaymentTransactionService
{
    Task<BaseResultWithData<long>> CreatePaymentTransactionAsync(CreatePaymentTransactionModel paymentTransactionRequest);
    Task<BaseResult> UpdatePaymentTransactionAsync(PaymentCompletion paymentCompletion);
    Task<BaseResultWithData<CheckTransactionStatus>> CheckTransactionStatus(string transactionId);
}