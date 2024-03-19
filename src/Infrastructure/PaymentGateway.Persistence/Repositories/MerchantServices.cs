using Dapper;
using Microsoft.AspNetCore.Http;
using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Constants;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Repositories;
using PaymentGateway.Ultils.ConfigDBConnection.Impl;
using PaymentGateway.Ultils.Extension;
using Serilog;

namespace PaymentGateway.Persistence.Repositories;

public class MerchantServices : IMerchantServices
{
    private readonly IDataAccess _db;

    public MerchantServices(IDataAccess db)
    {
        _db = db;
    }

    public async Task<BaseResult> CreateMerchant(CreateMerchantModel createMerchant)
    {
        try
        {
            var merchant = new Merchant
            {
                MerchantId = "M" + Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                MerchantName = createMerchant.MerchantName,
                CreatedBy = createMerchant.CreatedBy,
                IsActive = true,
            };
            var query = Extensions.GetInsertQuery("Merchant", "MerchantId", "MerchantName", "IsActive",
                "CreatedBy", "CreatedAt");

            var result = await _db.SaveData(query, merchant);
            if (result)
            {
                Log.Information(MessageConstantsWithValue.createSuccess("merchant"));
                return new BaseResult
                {
                    IsSuccess = true,
                    Message = MessageConstantsWithValue.createSuccess("merchant"),
                    StatusCode = StatusCodes.Status201Created
                };
            }
            Log.Error(MessageConstantsWithValue.createFail("merchant"));
            return new BaseResult
            {
                IsSuccess = false,
                Message = MessageConstantsWithValue.createFail("merchant"),
                StatusCode = StatusCodes.Status404NotFound
            };
        }
        catch
        {
            Log.Error(MessageConstants.InternalServerError);
            throw;
        }
    }

    public async Task<BaseResultWithData<List<GetMerchantModel>>> GetMerchants()
    {
        try
        {
            var query = @"SELECT 
                            m.MerchantName, 
                            m.IsActive, 
                            m.CreatedBy, 
                            m.CreatedAt, 
                            m.LastUpdatedBy, 
                            m.LastUpdatedAt 
                        FROM 
                          Merchant m";
            var data = await _db.GetData<GetMerchantModel, dynamic>(query, new { });
            var getMerchantModels = data as List<GetMerchantModel> ?? data.AsList();
            if (getMerchantModels.Any())
            {
                Log.Information(MessageConstantsWithValue.getDataSuccess("all merchant"));
                return new BaseResultWithData<List<GetMerchantModel>>
                {
                    IsSuccess = true,
                    Data = getMerchantModels,
                    Message = MessageConstantsWithValue.getDataSuccess("all merchant"),
                    StatusCode = StatusCodes.Status200OK
                };
            }
            Log.Error(MessageConstantsWithValue.getDataFail("all merchant"));
            return new BaseResultWithData<List<GetMerchantModel>>
            {
                IsSuccess = false,
                Message = MessageConstantsWithValue.getDataFail("all merchant"),
                StatusCode = StatusCodes.Status404NotFound
            };
        }
        catch
        {
            Log.Error(MessageConstants.InternalServerError);
            throw;
        }
    }

    public async Task<BaseResult> UpdateNameMerchant(string merchantId, UpdateNameMerchantModel nameMerchant)
    {
        try
        {
            if (!await checkMerchantExist(merchantId))
                return new BaseResult
                {
                    IsSuccess = false,
                    Message = MessageConstantsWithValue.notFoundFromDatabase("merchant"),
                    StatusCode = StatusCodes.Status404NotFound
                };
            var query = @"UPDATE Merchant 
                          SET MerchantName = @MerchantName, 
                              LastUpdatedBy = @LastUpdatedBy, 
                              LastUpdatedAt = @LastUpdatedAt 
                          WHERE MerchantId = @MerchantId";
            var LastUpdatedAt = DateTime.Now;

            var data = await _db.SaveData(query,
                new { MerchantId = merchantId, nameMerchant.MerchantName, nameMerchant.LastUpdatedBy, LastUpdatedAt });
            if (data)
            {
                Log.Information(MessageConstantsWithValue.updateSuccess("Merchant"));
                return new BaseResult
                {
                    IsSuccess = true,
                    Message = MessageConstantsWithValue.updateSuccess("Merchant"),
                    StatusCode = StatusCodes.Status200OK
                };
            }
            Log.Error(MessageConstantsWithValue.updateFail("Merchant"));
            return new BaseResult
            {
                IsSuccess = false,
                Message = MessageConstantsWithValue.updateFail("Merchant"),
                StatusCode = StatusCodes.Status404NotFound
            };
        }
        catch
        {
            Log.Error(MessageConstants.InternalServerError);
            throw;
        }
    }

    public async Task<BaseResult> IsActiveMerchant(string merchantId, IsActiveMerchantModel activeMerchant)
    {
        try
        {
            if (!await checkMerchantExist(merchantId))
                return new BaseResult
                {
                    IsSuccess = false,
                    Message = MessageConstantsWithValue.notFoundFromDatabase("merchant"),
                    StatusCode = StatusCodes.Status404NotFound
                };
            var query = @"UPDATE Merchant 
                          SET IsActive = @IsActive, 
                              LastUpdatedBy = @LastUpdatedBy, 
                              LastUpdatedAt = @LastUpdatedAt 
                          WHERE MerchantId = @MerchantId";
            var LastUpdatedAt = DateTime.Now;

            var data = await _db.SaveData(query,
                new { MerchantId = merchantId, activeMerchant.IsActive, activeMerchant.LastUpdatedBy, LastUpdatedAt });
            if (data)
            {
                Log.Information(MessageConstantsWithValue.updateSuccess("Merchant"));
                return new BaseResult
                {
                    IsSuccess = true,
                    Message = MessageConstantsWithValue.updateSuccess("Merchant"),
                    StatusCode = StatusCodes.Status200OK
                };
            }
            Log.Error(MessageConstantsWithValue.updateFail("Merchant"));
            return new BaseResult
            {
                IsSuccess = false,
                Message = MessageConstantsWithValue.updateFail("Merchant"),
                StatusCode = StatusCodes.Status404NotFound
            };
        }
        catch
        {
            Log.Error(MessageConstants.InternalServerError);
            throw;
        }
    }

    private async Task<bool> checkMerchantExist(string merchantId)
    {
        var query = @"SELECT 
                        COUNT(1) 
                      FROM 
                        Merchant 
                      WHERE 
                        MerchantId = @MerchantId";
        var data = await _db.GetData<int, dynamic>(query, new { MerchantId = merchantId });
        return data.FirstOrDefault() > 0;
    }
}