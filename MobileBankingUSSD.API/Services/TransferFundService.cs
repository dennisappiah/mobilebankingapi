using MobileBankingUSSD.API.Repositories.interfaces;
using MobileBankingUSSD.API.Services.interfaces;

namespace MobileBankingUSSD.API.Services;

public class TransferFundService(ITransferFundRepository transferFundRepository, ILogger<TransferFundService> logger) : ITransferFundService
{
    private readonly ITransferFundRepository _transferFundRepository = transferFundRepository ?? throw new ArgumentNullException(nameof(transferFundRepository));
    
    private readonly ILogger<TransferFundService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    
    
    
    public async Task<Guid> TransferFundsAsync(Guid fromAccountId, Guid toAccountId, decimal amount, string reference, CancellationToken cancellationToken)
    {
        try
        {
            return await _transferFundRepository.TransferFundsAsync(fromAccountId, toAccountId, amount, reference,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during fund transfer.");
            throw;
        }
    }
}