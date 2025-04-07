namespace MobileBankingUSSD.API.Services.interfaces;

public interface IAirtimeService
{
    Task<Guid> PurchaseAirtime(string accountNumber, string phoneNumber, string network, decimal amount,
        CancellationToken cancellationToken);
}