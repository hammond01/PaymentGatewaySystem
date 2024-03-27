using Microsoft.AspNetCore.Http;
using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Constants;
using PaymentGateway.Domain.Entities.ThirdParty.VNPayEntities;
using PaymentGateway.Domain.Exceptions.ErrorMessage;
using PaymentGateway.Domain.Repositories;
using PaymentGateway.Ultils.ConfigDBConnection.Impl;
using Serilog;

namespace PaymentGateway.Persistence.Repositories;

public class TransactionRefundRepository : ITransactionRefundRepository
{
    private readonly IDataAccess _db;

    public TransactionRefundRepository(IDataAccess db)
    {
        _db = db;
    }

    public async Task<BaseResult> InsertDataToRefundTable(RefundResponse model)
    {
        try
        {
            var result = await _db.InsertData("TransactionRefund", model);
            return new BaseResult
            {
                IsSuccess = result,
                StatusCode = StatusCodes.Status201Created,
                Message = MessageConstantsWithValue.createSuccess("transaction refund")
            };
        }
        catch (Exception e)
        {
            Log.Error(LayerErrorMessage.ERROR_AT_PERSISTENCE(e.Message));
            throw;
        }
    }
}