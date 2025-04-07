using Microsoft.AspNetCore.Mvc;
using MobileBankingUSSD.API.Services.interfaces;

namespace MobileBankingUSSD.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class TransferFundsController(ITransferFundService transferFundService, ILogger<TransferFundsController> logger)
    : ControllerBase
{
    private readonly ITransferFundService _transferFundService = transferFundService ?? throw new ArgumentNullException(nameof(transferFundService));
    private readonly ILogger<TransferFundsController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    [HttpPost("transfer")]
    public async Task<IActionResult> TransferFunds(Guid fromAccountId, Guid toAccountId, decimal amount, string reference, CancellationToken cancellationToken)
    {
        try
        {
            Guid transactionId = await _transferFundService.TransferFundsAsync(fromAccountId, toAccountId, amount, reference, cancellationToken);
            return Ok(new { TransactionId = transactionId, Message = "Transfer successful." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to transfer funds.");
            return BadRequest(new { Message = ex.Message });
        }
    }
}