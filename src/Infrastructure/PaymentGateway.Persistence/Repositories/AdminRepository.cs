using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Repositories;
using PaymentGateway.Ultils.ConfigDBConnection.Impl;

namespace PaymentGateway.Persistence.Repositories;

public class AdminRepository(IDataAccess dataAccess) : IAdminRepository
{
    private readonly IDataAccess _dataAccess = dataAccess;
    public async Task<BaseResultWithData<object>> ManagePaymentTransactions()
    {
        var query = "SELECT d.TransactionNo, d.PaymentTransactionId, d.Amount,d.BankCode, d.OrderInfo,rc.Message,d.PayDate FROM DetailPayment d LEFT JOIN ResponseCode rc ON d.ResponseCodeId = rc.ResponseCodeId";
        
        await Task.FromResult(0);
        return new BaseResultWithData<object>
        {
            Data = new {}
        };
    }
}