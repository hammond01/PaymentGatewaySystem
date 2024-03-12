using Microsoft.AspNetCore.Mvc;
namespace PaymentGateway.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        [HttpGet]
        public IActionResult CreateQRCode()
        {
            return Ok("OKLA");
        }
    }
}
