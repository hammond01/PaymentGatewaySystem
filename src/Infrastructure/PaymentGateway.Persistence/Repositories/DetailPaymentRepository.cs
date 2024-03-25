using Microsoft.AspNetCore.Http;
using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Constants;
using PaymentGateway.Domain.Entities.ThirdParty.VNPayEntities;
using PaymentGateway.Domain.Exceptions.ErrorMessage;
using PaymentGateway.Domain.Repositories;
using PaymentGateway.Ultils.ConfigDBConnection.Impl;
using Serilog;

namespace PaymentGateway.Persistence.Repositories;

public class DetailPaymentRepository : IDetailPaymentService
{
    private readonly IDataAccess _db;

    public DetailPaymentRepository(IDataAccess db)
    {
        _db = db;
    }

    public async Task<BaseResult> CreateDataToDetailPaymentAsync(DetailPaymentVNPSandBox request)
    {
        try
        {
            var result = await _db.InsertData("DetailPayment", request);
            return new BaseResult
            {
                IsSuccess = result,
                StatusCode = StatusCodes.Status201Created,
                Message = MessageConstantsWithValue.createSuccess("detail payment")
            };
        }
        catch (Exception e)
        {
            Log.Error(LayerErrorMessage.ERROR_AT_PERSISTENCE(e.Message));
            throw;
        }
    }

    public async Task<BaseResultWithData<RefundRequestInfo>> GetTransactionInfoForRefundRequest(long transactionNo)
    {
        try
        {
            var query = $@"SELECT TxnRef, PayDate FROM DetailPayment WHERE TransactionNo =  @TransactionNo";
            var result = await _db.GetData<RefundRequestInfo, dynamic>(query, new { transactionNo });
            return new BaseResultWithData<RefundRequestInfo>
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Message = MessageConstantsWithValue.getDataSuccess("transaction info"),
                Data = result.FirstOrDefault()
            };
        }
        catch (Exception e)
        {
            Log.Error(LayerErrorMessage.ERROR_AT_PERSISTENCE(e.Message));
            throw;
        }
    }
}