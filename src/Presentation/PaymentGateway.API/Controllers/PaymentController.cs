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
    private readonly IVnPayServices _vnPayServices;
    private readonly IVNPaySandBoxServices _services;
    private readonly IPaymentTransactionService _paymentTransactionService;

    public PaymentController(IVnPayServices vnPayServices, IVNPaySandBoxServices services, IPaymentTransactionService paymentTransactionService)
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

    [HttpPost("create-payment-url-with-sandbox")]
    public async Task<IActionResult> CreatePaymentUrl(CreateStringUrlRequest request)
    {
        try
        {
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

    [HttpGet("check-transaction-status-with-sandbox/{transactionId}")]
    public async Task<IActionResult> CheckTransactionStatus(string transactionId)
    {
        try
        {
            var response = await _paymentTransactionService.CheckTransactionStatus(transactionId);
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