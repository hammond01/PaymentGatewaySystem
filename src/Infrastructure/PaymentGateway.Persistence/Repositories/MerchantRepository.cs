using Dapper;
using Microsoft.AspNetCore.Http;
using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Constants;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Exceptions.ErrorMessage;
using PaymentGateway.Domain.Repositories;
using PaymentGateway.Ultils.ConfigDBConnection.Impl;
using PaymentGateway.Ultils.Extension;
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
                MerchantId = Extension.GenerateUniqueId(),
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
            Log.Error(MessageConstantsWithValue.createFail("merchant", ""));
            return new BaseResult
            {
                IsSuccess = false,
                Message = MessageConstantsWithValue.createFail("merchant", ""),
                StatusCode = StatusCodes.Status404NotFound
            };
        }
        catch (Exception e)
        {
            Log.Error(LayerErrorMessage.ERROR_AT_PERSISTENCE(e.Message));
            throw;
        }
    }

    /// <summary>
    ///     Returns a list of all merchants in the system.
    /// </summary>
    /// <returns>A <see /> containing a list of merchants, or an error message if the operation fails.</returns>
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
                            LastUpdatedAt
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

            Log.Error(MessageConstantsWithValue.getDataFail("all merchant", ""));
            return new BaseResultWithData<List<GetMerchantModel>>
            {
                IsSuccess = false,
                Message = MessageConstantsWithValue.getDataFail("all merchant", ""),
                StatusCode = StatusCodes.Status404NotFound
            };
        }
        catch (Exception e)
        {
            Log.Error(LayerErrorMessage.ERROR_AT_PERSISTENCE(e.Message));
            throw;
        }
    }

    /// <summary>
    ///     Updates the name of a merchant in the system.
    /// </summary>
    /// <param name="merchantId">The ID of the merchant to update.</param>
    /// <param name="nameMerchant">The details of the merchant to update.</param>
    /// <returns>A <see cref="BaseResult" /> indicating whether the operation was successful.</returns>
    public async Task<BaseResult> UpdateNameMerchant(long merchantId, UpdateNameMerchantModel nameMerchant)
    {
        try
        {
            // Checks if a merchant with the specified merchant ID exists in the system.
            // <returns><c>true</c> if a merchant with the specified ID exists, otherwise <c>false</c>.</returns>
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

            Log.Error(MessageConstantsWithValue.updateFail("Merchant", ""));
            return new BaseResult
            {
                IsSuccess = false,
                Message = MessageConstantsWithValue.updateFail("Merchant", ""),
                StatusCode = StatusCodes.Status404NotFound
            };
        }
        catch (Exception e)
        {
            Log.Error(LayerErrorMessage.ERROR_AT_PERSISTENCE(e.Message));
            throw;
        }
    }

    /// <summary>
    ///     Updates the status of a merchant in the system.
    /// </summary>
    /// <param name="merchantId">The ID of the merchant to update.</param>
    /// <param name="activeMerchant">The details of the merchant to update.</param>
    /// <returns>A <see cref="BaseResult" /> indicating whether the operation was successful.</returns>
    public async Task<BaseResult> IsActiveMerchant(long merchantId, IsActiveMerchantModel activeMerchant)
    {
        try
        {
            // Checks if a merchant with the specified merchant ID exists in the system.
            // <returns><c>true</c> if a merchant with the specified ID exists, otherwise <c>false</c>.</returns>
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

            Log.Error(MessageConstantsWithValue.updateFail("Merchant", ""));
            return new BaseResult
            {
                IsSuccess = false,
                Message = MessageConstantsWithValue.updateFail("Merchant", ""),
                StatusCode = StatusCodes.Status404NotFound
            };
        }
        catch (Exception e)
        {
            Log.Error(LayerErrorMessage.ERROR_AT_PERSISTENCE(e.Message));
            throw;
        }
    }

    /// <summary>
    ///     Deletes a merchant from the system.
    /// </summary>
    /// <param name="merchantId">The ID of the merchant to be deleted.</param>
    /// <returns>A <see cref="BaseResult" /> indicating whether the operation was successful.</returns>
    public async Task<BaseResult> DeleteMerchant(long merchantId)
    {
        try
        {
            // Checks if a merchant with the specified merchant ID exists in the system.
            // <returns><c>true</c> if a merchant with the specified ID exists, otherwise <c>false</c>.</returns>
            if (!await checkMerchantExist(merchantId))
                return new BaseResult
                {
                    IsSuccess = false,
                    Message = MessageConstantsWithValue.notFoundFromDatabase("merchant"),
                    StatusCode = StatusCodes.Status404NotFound
                };
            var data = await _db.DeleteDataFromClient("Merchant", merchantId);

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

            Log.Error(MessageConstantsWithValue.deleteFail("Merchant", ""));
            return new BaseResult
            {
                IsSuccess = false,
                Message = MessageConstantsWithValue.deleteFail("Merchant", ""),
                StatusCode = StatusCodes.Status404NotFound
            };
        }
        catch (Exception e)
        {
            Log.Error(LayerErrorMessage.ERROR_AT_PERSISTENCE(e.Message));
            throw;
        }
    }

    /// <summary>
    ///     Checks if a merchant with the specified merchant ID exists in the system.
    /// </summary>
    /// <param name="merchantId">The ID of the merchant to check.</param>
    /// <returns><c>true</c> if a merchant with the specified ID exists, otherwise <c>false</c>.</returns>
    private async Task<bool> checkMerchantExist(long merchantId)
    {
        // Define the SQL query to check for the merchant
        var query = @"SELECT
                    COUNT(1)
                  FROM
                    Merchant
                  WHERE
                    MerchantId = @MerchantId";

        // Execute the query and retrieve the data
        var data = await _db.GetData<int, dynamic>(query, new { MerchantId = merchantId });

        // Return whether the merchant exists or not
        return data.FirstOrDefault() > 0;
    }
}