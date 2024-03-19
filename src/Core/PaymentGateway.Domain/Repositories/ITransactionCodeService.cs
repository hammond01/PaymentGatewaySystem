using PaymentGateway.Domain.Response;

namespace PaymentGateway.Domain.Repositories;

public interface ITransactionCodeService
{
    Task<List<TransactionCodeResponse>> GetTransactionCodeByTypeAsync(string typeName);
    Task<TransactionCodeResponse> GetTransactionCodeByCodeAsync(string code, string requestTypeName);
}