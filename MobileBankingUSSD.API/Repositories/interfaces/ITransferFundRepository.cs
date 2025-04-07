namespace MobileBankingUSSD.API.Repositories.interfaces;

public interface ITransferFundRepository
{
    Task<Guid> TransferFundsAsync(Guid fromAccountId, Guid toAccountId, decimal amount, string reference,
        CancellationToken cancellationToken);
}