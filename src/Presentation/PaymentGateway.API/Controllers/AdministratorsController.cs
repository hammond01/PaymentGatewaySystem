using Microsoft.AspNetCore.Mvc;

namespace PaymentGateway.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdministratorsController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ManagePaymentTransactions()
    {
        await Task.FromResult(0);
        return Ok();
    }
}