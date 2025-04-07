namespace MobileBankingUSSD.API.Services.interfaces;

public interface ITransferFundService
{
    Task<Guid> TransferFundsAsync(Guid fromAccountId, Guid toAccountId, decimal amount, string reference,
        CancellationToken cancellationToken);
}