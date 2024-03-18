using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Repositories;

namespace PaymentGateway.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IPaymentTransactionService _code;

        public ValuesController(IPaymentTransactionService code)
        {
            _code = code;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var test = new PaymentCompletion
            {
                PaymentTransactionId = null,
                PaymentStatus = null,
                PaymentLastMessage = null,
                PaymentCompletionTime = default
            };
            var data = await _code.UpdatePaymentTransactionAsync(test);
            return Ok(data);
        }
    }
}
