using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Repositories;
using PaymentGateway.Domain.Request;

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
    public async Task<IActionResult> CreateMerchant()
    {
        var merchant = new Merchant
        {
            MerchantId = "M" + Guid.NewGuid(),
            MerchantName = "Merchant 1",
            IsActive = true,
            CreatedBy = "Admin",
            CreatedAt = DateTime.Now
        };
        var data = await _services.CreateMerchant(merchant);
        return Ok(data);
    }

    [HttpGet("get-all-merchant")]
    public async Task<IActionResult> GetMerchants()
    {
        var data = await _services.GetMerchants();
        return Ok(data);
    }
    [HttpPost("update-name-merchant")]
    public async Task<IActionResult> UpdateNameMerchant(string merchantId, MerchantRequest.UpdateNameMerchant nameMerchant)
    {
        var data = await _services.UpdateNameMerchant(merchantId, nameMerchant);
        return Ok(data);
    }
}