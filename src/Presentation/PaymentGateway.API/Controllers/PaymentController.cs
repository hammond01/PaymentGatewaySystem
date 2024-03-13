using Microsoft.AspNetCore.Mvc;
using PaymentGateway.API.Controllers.Base;
using PaymentGateway.Domain.Repositories;
using PaymentGateway.Domain.Request;
namespace PaymentGateway.API.Controllers
{
    public class PaymentController : PaymentGatewayVersion
    {
        private readonly IVNPayservices _VNPayservices;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IVNPayservices VNPayservices, ILogger<PaymentController> logger)
        {
            _VNPayservices = VNPayservices;
            _logger = logger;
        }
        [HttpPost]
        public async Task<IActionResult> CreateQRCode(CreateQRRequest createQRRequest)
        {
            try
            {
                _logger.LogInformation($"Start service CreateQRString");
                var data = await _VNPayservices.CreateQRString(createQRRequest);
                _logger.LogInformation($"End service CreateQRString");
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating QR string: {ex.Message}");
                return BadRequest();
            }
        }
    }
}
