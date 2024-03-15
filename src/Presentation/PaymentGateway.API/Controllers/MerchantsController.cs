using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Domain.Repositories;
using static PaymentGateway.Domain.Request.MerchantRequest;

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
    public async Task<IActionResult> CreateMerchant(CreateMerchant createMerchant)
    {
        var data = await _services.CreateMerchant(createMerchant);
        return Ok(data);
    }

    [HttpGet("get-all-merchant")]
    public async Task<IActionResult> GetMerchants()
    {
        var data = await _services.GetMerchants();
        return Ok(data);
    }

    [HttpPost("update-name-merchant-name")]
    public async Task<IActionResult> UpdateNameMerchant(string merchantId,
        UpdateNameMerchant nameMerchant)
    {
        var data = await _services.UpdateNameMerchant(merchantId, nameMerchant);
        return Ok(data);
    }

    [HttpPost("is-active-merchant")]
    public async Task<IActionResult> IsActiveMerchant(string merchantId,
        IsActiveMerchant activeMerchant)
    {
        var data = await _services.IsActiveMerchant(merchantId, activeMerchant);
        return Ok(data);
    }

}