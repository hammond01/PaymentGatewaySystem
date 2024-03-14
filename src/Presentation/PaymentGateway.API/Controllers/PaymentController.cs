using Microsoft.AspNetCore.Mvc;
using PaymentGateway.API.Controllers.Base;
using PaymentGateway.Domain.Repositories;
using PaymentGateway.Domain.Request;

namespace PaymentGateway.API.Controllers;

public class PaymentController : PaymentGatewayVNPayVersion
{
    private readonly IVnPayServices _vnPayServices;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IVnPayServices vnPayServices, ILogger<PaymentController> logger)
    {
        _vnPayServices = vnPayServices;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateQrCode(CreateQrRequest createQrRequest)
    {
        try
        {
            _logger.LogInformation("Start service CreateQRString");
            var data = await _vnPayServices.CreateQrString(createQrRequest);
            _logger.LogInformation("End service CreateQRString");
            return Ok(data);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating QR string: {ex.Message}");
            return BadRequest();
        }
    }
}