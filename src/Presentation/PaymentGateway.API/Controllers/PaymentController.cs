using Microsoft.AspNetCore.Mvc;
using PaymentGateway.API.Controllers.Base;
using PaymentGateway.Domain.Repositories.VNPayRestful;
using PaymentGateway.Domain.Repositories.VNPaySandBox;
using PaymentGateway.Domain.Request;
using static PaymentGateway.Domain.Request.VNPaySandBox.VNPaySanBoxRequest;

namespace PaymentGateway.API.Controllers;

public class PaymentController : PaymentGatewayVNPayVersion
{
    private readonly IVnPayServices _vnPayServices;
    private readonly ILogger<PaymentController> _logger;
    private readonly IVNPaySandBoxServices _services;

    public PaymentController(IVnPayServices vnPayServices, ILogger<PaymentController> logger, IVNPaySandBoxServices services)
    {
        _vnPayServices = vnPayServices;
        _logger = logger;
        _services = services;

    }
    [HttpPost("create-qr-code-string")]
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

    [HttpPost("create-payment-url-with-sandbox")]
    public async Task<IActionResult> CreatePaymentUrl([FromBody] CreateStringUrl request)
    {
        var res = await _services.CreatePaymentUrl(HttpContext, request);
        return Ok(res);
    }
    [HttpGet("payment-callback-with-sandbox")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> PaymentCallBack()
    {
        var response = await _services.PaymentExecute(Request.Query);
        if (response.VnPayResponseCode != "00") return Ok("Đã xảy ra lỗi");
        return Ok(response);
    }
}