using Microsoft.AspNetCore.Mvc;
using PaymentGateway.API.Controllers.Base;
using PaymentGateway.Domain.Constants;
using PaymentGateway.Domain.Entities.ThirdParty.VNPayEntities;
using PaymentGateway.Domain.Repositories;
using PaymentGateway.Domain.Repositories.VNPayRestful;
using PaymentGateway.Domain.Repositories.VNPaySandBox;
using PaymentGateway.Domain.Request;
using Serilog;

namespace PaymentGateway.API.Controllers;

public class PaymentController : PaymentGatewayVNPayVersion
{
    private readonly IPaymentTransactionService _paymentTransactionService;
    private readonly IVnPaySandBoxServices _services;
    private readonly IVnPayServices _vnPayServices;
    public PaymentController(IVnPayServices vnPayServices, IVnPaySandBoxServices services, IPaymentTransactionService paymentTransactionService)
    {
        _vnPayServices = vnPayServices;
        _services = services;
        _paymentTransactionService = paymentTransactionService;
    }

    [HttpPost("create-qr-code-string")]
    public async Task<IActionResult> CreateQrCode(CreateQrRequest createQrRequest)
    {
        try
        {
            Log.Information("Start service CreateQRString");
            var data = await _vnPayServices.CreateQrString(createQrRequest);
            Log.Information("End service CreateQRString");
            return Ok(data);
        }
        catch (Exception ex)
        {
            Log.Error($"Error creating QR string: {ex.Message}");
            return StatusCode(500, MessageConstants.InternalServerError);
        }
    }

    [HttpPost("create-payment-url-using-sandbox")]
    public async Task<IActionResult> CreatePaymentUrl([FromBody] CreatePaymentSandboxRequest request)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest();
            var res = await _services.CreatePaymentUrl(HttpContext, request);
            return Ok(res);
        }
        catch
        {
            return StatusCode(500, MessageConstants.InternalServerError);
        }
    }
    [HttpGet("payment-callback-with-sandbox")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> PaymentCallBack()
    {
        try
        {
            var response = await _services.PaymentExecute(HttpContext, Request.Query);
            return Ok(response);
        }
        catch
        {
            return StatusCode(500, MessageConstants.InternalServerError);
        }
    }

    [HttpGet("check-transaction-status-with-sandbox/{transactionNo}")]
    public async Task<IActionResult> CheckTransactionStatus(long transactionNo)
    {
        try
        {
            var response = await _paymentTransactionService.CheckTransactionStatus(transactionNo);
            return Ok(response);
        }
        catch
        {
            return StatusCode(500, MessageConstants.InternalServerError);
        }
    }

    [HttpPost("refund-with-sandbox")]
    public async Task<IActionResult> Refund(RefundRequestClient request)
    {
        try
        {
            var response = await _services.Refund(HttpContext, request);
            return Ok(response);
        }
        catch
        {
            return StatusCode(500, MessageConstants.InternalServerError);
        }
    }
}