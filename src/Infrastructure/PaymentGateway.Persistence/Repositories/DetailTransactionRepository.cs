using Microsoft.AspNetCore.Http;
using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Exceptions.ErrorMessage;
using PaymentGateway.Domain.Repositories;
using PaymentGateway.Ultils.ConfigDBConnection.Impl;
using PaymentGateway.Ultils.Extension;
using Serilog;

namespace PaymentGateway.Persistence.Repositories;

public class DetailTransactionRepository : IDetailTransactionServices
{
    private readonly IDataAccess _db;
    private readonly ILogger _logger;

    public DetailTransactionRepository(IDataAccess db, ILogger logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<BaseResult> InsertDetailTransaction(CreateDetailTransaction detailTransactionRequest)
    {
        try
        {
            var detailTransactionModel = DetailTransaction.DetailTransactionGenerator(detailTransactionRequest);
            var query = Extension.GetInsertQuery("DetailTransaction", "DetailTransactionId",
                "DetailTransactionName",
                "DetailTransactionIpAddress", "DetailTransactionCreateAt", "DetailTransactionUserId",
                "PaymentTransactionId", "ReponseCodeId",
                "BankCode");
            var save = await _db.SaveData(query, detailTransactionModel);
            if (save)
            {
                _logger.Information("Insert detail transaction success!");
                return new BaseResult
                {
                    IsSuccess = save,
                    Message = "Insert detail transaction success!",
                    StatusCode = StatusCodes.Status201Created
                };
            }

            _logger.Error("Insert detail transaction fail!");
            return new BaseResult
            {
                IsSuccess = save,
                Message = "Insert detail transaction fail!",
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
        catch (Exception e)
        {
            Log.Error(LayerErrorMessage.ERROR_AT_PERSISTENCE(e.Message));
            throw;
        }
    }
}