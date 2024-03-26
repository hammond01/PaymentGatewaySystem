using Dapper;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Exceptions.ErrorMessage;
using PaymentGateway.Domain.Repositories;
using PaymentGateway.Ultils.ConfigDBConnection.Impl;
using Serilog;

namespace PaymentGateway.Persistence.Repositories;

public class TransactionCodeRepository : ITransactionCodeService
{
    private readonly IDataAccess _db;
    private readonly ILogger _logger;

    public TransactionCodeRepository(IDataAccess db, ILogger logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<TransactionCode>> GetTransactionCodeByTypeAsync(string typeName)
    {
        try
        {
            var query = @"SELECT rc.ReponseCodeId,
                               rc.ResponseCode,
                               rc.Message,
                               rc.RequestTypeId,
                               rt.RequestName
                        FROM ResponseCode rc
                        LEFT JOIN RequestType rt ON rc.RequestTypeId = rt.RequestTypeId
                        WHERE rt.RequestName = @typeName";
            var data = await _db.GetData<TransactionCode, dynamic>(query, new { typeName });
            _logger.Information("Get transaction by type name success.");
            return data.AsList();
        }
        catch (Exception e)
        {
            Log.Error(LayerErrorMessage.ERROR_AT_PERSISTENCE(e.Message));
            throw;
        }
    }

    public async Task<TransactionCode> GetTransactionCodeByCodeAsync(string code, string requestTypeName)
    {
        try
        {
            var query = @"SELECT rc.ReponseCodeId,
                               rc.ResponseCode,
                               rc.Message,
                               rc.RequestTypeId,
                               rt.RequestName
                        FROM ResponseCode rc
                        LEFT JOIN RequestType rt ON rc.RequestTypeId = rt.RequestTypeId
                        WHERE rc.ResponseCode = @code  AND rt.RequestName = @requestTypeName";
            var data = await _db.GetData<TransactionCode, dynamic>(query, new { code, requestTypeName });
            _logger.Information("Get transaction by code success.");
            return data.FirstOrDefault()!;
        }
        catch (Exception e)
        {
            Log.Error(LayerErrorMessage.ERROR_AT_PERSISTENCE(e.Message));
            throw;
        }
    }
}