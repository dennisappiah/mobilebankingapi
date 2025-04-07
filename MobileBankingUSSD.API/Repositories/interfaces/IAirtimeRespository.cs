namespace MobileBankingUSSD.API.Repositories.interfaces;

public interface IAirtimeRespository
{
    Task<Guid> PurchaseAirtimeAsync(string accountNumber, string phoneNumber, string network, decimal amount,
        string reference, CancellationToken cancellationToken);
}