using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Domain.Constants;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Repositories;
using Serilog;

namespace PaymentGateway.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MerchantsController : ControllerBase
{
    private readonly IMerchantServices _services;

    public MerchantsController(IMerchantServices services)
    {
        _services = services;
    }

    [HttpPost("create-merchant")]
    public async Task<IActionResult> CreateMerchant(CreateMerchantModel createMerchant)
    {
        try
        {
            var data = await _services.CreateMerchant(createMerchant);
            return Ok(data);
        }
        catch
        {
            Log.Error(MessageConstants.InternalServerError);
            return StatusCode(500, MessageConstants.InternalServerError);
        }
    }

    [HttpGet("get-all-merchant")]
    public async Task<IActionResult> GetMerchants()
    {
        try
        {
            var data = await _services.GetMerchants();
            return Ok(data);
        }
        catch
        {
            Log.Error(MessageConstants.InternalServerError);
            return StatusCode(500, MessageConstants.InternalServerError);
        }
    }

    [HttpPut("update-name-merchant-name")]
    public async Task<IActionResult> UpdateNameMerchant(string merchantId, UpdateNameMerchantModel nameMerchant)
    {
        try
        {
            var data = await _services.UpdateNameMerchant(merchantId, nameMerchant);
            return Ok(data);
        }
        catch
        {
            Log.Error(MessageConstants.InternalServerError);
            return StatusCode(500, MessageConstants.InternalServerError);
        }
    }

    [HttpPut("is-active-merchant")]
    public async Task<IActionResult> IsActiveMerchant(string merchantId, IsActiveMerchantModel activeMerchant)
    {
        try
        {
            var data = await _services.IsActiveMerchant(merchantId, activeMerchant);
            return Ok(data);
        }
        catch
        {
            Log.Error(MessageConstants.InternalServerError);
            return StatusCode(500, MessageConstants.InternalServerError);
        }
    }
}