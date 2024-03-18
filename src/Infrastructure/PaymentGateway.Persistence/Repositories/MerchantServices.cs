using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Repositories;
using PaymentGateway.Domain.Request;
using PaymentGateway.Domain.Response;
using PaymentGateway.Ultils.ConfigDBConnection.Impl;
using PaymentGateway.Ultils.Extension;
using static PaymentGateway.Domain.Request.MerchantRequest;

namespace PaymentGateway.Persistence.Repositories;

public class MerchantServices : IMerchantServices
{
    private readonly IDataAccess _db;

    public MerchantServices(IDataAccess db)
    {
        _db = db;
    }

    public async Task<BaseResult> CreateMerchant(CreateMerchant createMerchant)
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
                return new BaseResult
                { IsSuccess = true, Message = "Create new merchant success", StatusCode = 200 };
            return new BaseResult { IsSuccess = false, Message = "Adding a new record failed!", StatusCode = 404 };
        }
        catch
        {
            return new BaseResult { IsSuccess = false, Message = "Internal server error!", StatusCode = 500 };
        }
    }

    public async Task<BaseResult> GetMerchants()
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
            var data = await _db.GetData<MerchantResponse, dynamic>(query, new { });
            return new BaseResult
            { IsSuccess = true, Message = "Get all merchants success", StatusCode = 200 };
        }
        catch
        {
            return new BaseResult { IsSuccess = false, Message = "Internal server error!", StatusCode = 500 };
        }
    }

    public async Task<CommandResponse> UpdateNameMerchant(string merchantId,
        MerchantRequest.UpdateNameMerchant nameMerchant)
    {
        try
        {
            if (!await checkMerchantExist(merchantId))
                return new CommandResponse
                { IsSuccess = false, Message = "This merchant was not found on the server!", StatusCode = 404 };
            var query = @"UPDATE Merchant 
                          SET MerchantName = @MerchantName, 
                              LastUpdatedBy = @LastUpdatedBy, 
                              LastUpdatedAt = @LastUpdatedAt 
                          WHERE MerchantId = @MerchantId";
            var LastUpdatedAt = DateTime.Now;

            var data = await _db.SaveData(query,
                new { MerchantId = merchantId, nameMerchant.MerchantName, nameMerchant.LastUpdatedBy, LastUpdatedAt });
            if (data)
                return new CommandResponse
                { IsSuccess = true, Message = "Successfully updated merchant name.", StatusCode = 200 };
            return new CommandResponse { IsSuccess = false, Message = "Internal server error.", StatusCode = 500 };
        }
        catch
        {
            return new CommandResponse { IsSuccess = false, Message = "Internal server error.", StatusCode = 500 };
        }
    }

    public async Task<CommandResponse> IsActiveMerchant(string merchantId,
        MerchantRequest.IsActiveMerchant activeMerchant)
    {
        try
        {
            if (!await checkMerchantExist(merchantId))
                return new CommandResponse
                { IsSuccess = false, Message = "This merchant was not found on the server!", StatusCode = 404 };
            var query = @"UPDATE Merchant 
                          SET IsActive = @IsActive, 
                              LastUpdatedBy = @LastUpdatedBy, 
                              LastUpdatedAt = @LastUpdatedAt 
                          WHERE MerchantId = @MerchantId";
            var LastUpdatedAt = DateTime.Now;
            var data = await _db.SaveData(query,
                new { MerchantId = merchantId, activeMerchant.IsActive, activeMerchant.LastUpdatedBy, LastUpdatedAt });
            if (data)
                return new CommandResponse
                { IsSuccess = true, Message = "Successfully updated merchant status.", StatusCode = 200 };
            return new CommandResponse { IsSuccess = false, Message = "Internal server error.", StatusCode = 200 };
        }
        catch
        {
            return new CommandResponse { IsSuccess = false, Message = "Internal server error.", StatusCode = 500 };
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