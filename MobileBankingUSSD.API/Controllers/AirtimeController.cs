using Microsoft.AspNetCore.Mvc;
using MobileBankingUSSD.API.Services;

namespace MobileBankingUSSD.API.Controllers;

[ApiController]
[Route("api/airtime")]
public class AirtimeController(AirtimeService airtimeService) : ControllerBase
{
    // [HttpPost("purchase")]
    // public async Task<IActionResult> PurchaseAirtime([FromBody] AirtimePurchaseRequest request)
    // {
    //     try
    //     {
    //         Guid transactionId = await airtimeService.PurchaseAirtime(request.AccountNumber, request.PhoneNumber, request.Network, request.Amount);
    //         return Ok(new { TransactionId = transactionId });
    //     }
    //     catch (Exception ex)
    //     {
    //         return BadRequest(ex.Message);
    //     }
    // }
}