using Microsoft.AspNetCore.Mvc;
namespace PaymentGateway.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MerchantsController : ControllerBase
{
    public IActionResult CreateMerchant()
    {
        return Ok();
    }
}