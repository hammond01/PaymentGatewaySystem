using Dapper;
using Microsoft.AspNetCore.Http;
using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Constants;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Repositories;
using PaymentGateway.Ultils.ConfigDBConnection.Impl;
using Serilog;

namespace PaymentGateway.Persistence.Repositories;

public class MerchantRepository : IMerchantService
{
    private readonly IDataAccess _db;

    public MerchantRepository(IDataAccess db)
    {
        _db = db;
    }


    /// <summary>
    ///     Creates a new merchant in the system.
    /// </summary>
    /// <param name="createMerchant">The details of the merchant to be created.</param>
    /// <returns>A <see cref="BaseResult" /> indicating whether the operation was successful.</returns>
    public async Task<BaseResult> CreateMerchant(CreateMerchantModel createMerchant)
    {
        try
        {
            // Create a new merchant entity
            var merchant = new Merchant
            {
                MerchantId = "M" + Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                MerchantName = createMerchant.MerchantName,
                CreatedBy = createMerchant.CreatedBy,
                IsActive = true,
                Deleted = false
            };

            // Insert the merchant into the database
            var result = await _db.InsertData("Merchant", merchant);
            if (result)
            {
                // Log the creation success
                Log.Information(MessageConstantsWithValue.createSuccess("merchant"));
                return new BaseResult
                {
                    IsSuccess = true,
                    Message = MessageConstantsWithValue.createSuccess("merchant"),
                    StatusCode = StatusCodes.Status201Created
                };
            }

            // Log the creation failure
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
            // Log the internal server error
            Log.Error(MessageConstants.InternalServerError);
            throw;
        }
    }

    public async Task<BaseResultWithData<List<GetMerchantModel>>> GetMerchants()
    {
        try
        {
            var query = @"SELECT
                            MerchantId,
                            MerchantName,
                            IsActive,
                            CreatedBy,
                            CreatedAt,
                            LastUpdatedBy,
                            LastUpdatedAt,
                        FROM
                          Merchant WHERE Deleted = 0";
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
            var req = new Merchant
            {
                MerchantName = nameMerchant.MerchantName,
                LastUpdatedAt = DateTime.Now,
                LastUpdatedBy = nameMerchant.LastUpdatedBy
            };

            var data = await _db.UpdateData("Merchant", merchantId, req);
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

            var req = new Merchant
            {
                IsActive = activeMerchant.IsActive,
                LastUpdatedBy = activeMerchant.LastUpdatedBy,
                LastUpdatedAt = DateTime.Now
            };

            var data = await _db.UpdateData("Merchant", merchantId, req);
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

    public async Task<BaseResult> DeleteMerchant(int id, DeleteMerchantModel activeMerchant)
    {
        try
        {
            if (!await checkMerchantExist(id))
                return new BaseResult
                {
                    IsSuccess = false,
                    Message = MessageConstantsWithValue.notFoundFromDatabase("merchant"),
                    StatusCode = StatusCodes.Status404NotFound
                };
            var query = @"UPDATE Merchant
                          SET Deleted = 1,
                              LastUpdatedBy = @LastUpdatedBy,
                              LastUpdatedAt = @LastUpdatedAt
                          WHERE Id = @Id";

            var LastUpdatedAt = DateTime.Now;

            var data = await _db.SaveData(query,
                new { Id = id, activeMerchant.LastUpdatedBy, LastUpdatedAt });
            if (data)
            {
                Log.Information(MessageConstantsWithValue.deleteSuccess("Merchant"));
                return new BaseResult
                {
                    IsSuccess = true,
                    Message = MessageConstantsWithValue.deleteSuccess("Merchant"),
                    StatusCode = StatusCodes.Status200OK
                };
            }

            Log.Error(MessageConstantsWithValue.deleteFail("Merchant"));
            return new BaseResult
            {
                IsSuccess = false,
                Message = MessageConstantsWithValue.deleteFail("Merchant"),
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

    private async Task<bool> checkMerchantExist(int id)
    {
        var query = @"SELECT
                        COUNT(1)
                      FROM
                        Merchant
                      WHERE
                        Id = @Id";
        var data = await _db.GetData<int, dynamic>(query, new { Id = id });
        return data.FirstOrDefault() > 0;
    }
}