using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Request;
using PaymentGateway.Domain.Response;

namespace PaymentGateway.Domain.Repositories;

public interface IDetailTransactionServices
{
    Task<DetailTransactionResponse> GetDetailTransactionByTransactionId(string transactionId);
    Task<BaseResult> InsertDetailTransaction(DetailTransactionRequest detailTransactionRequest);
}