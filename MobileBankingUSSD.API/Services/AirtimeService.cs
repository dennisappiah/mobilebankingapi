using MobileBankingUSSD.API.Repositories.interfaces;
using MobileBankingUSSD.API.Services.interfaces;

namespace MobileBankingUSSD.API.Services;

public class AirtimeService(IAirtimeRespository  airtimeRespository) : IAirtimeService
{
   private readonly IAirtimeRespository _airtimeRespository = airtimeRespository ?? throw new ArgumentNullException(nameof(airtimeRespository));
   
    public async Task<Guid> PurchaseAirtime(string accountNumber, string phoneNumber, string network, decimal amount, CancellationToken cancellationToken)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be positive.");
        }

        if (string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length < 10)
        {
            throw new ArgumentException("Invalid phone number");
        }

        if (network != "MTN" && network != "VODAFONE" && network != "AIRTELTIGO" && network != "GLO")
        {
            throw new ArgumentException("Invalid network provider");
        }

        // decimal availableBalance = await _accountBalanceService.GetAvailableBalance(accountNumber);
        // decimal minBalance = 10;
        // decimal transactionFee = 2; 
        //
        // decimal totalDebit = amount + transactionFee;
        //
        // if(availableBalance < totalDebit)
        // {
        //     throw new InvalidOperationException("Insufficient funds");
        // }
        //
        // if((availableBalance - totalDebit) < minBalance)
        // {
        //     throw new InvalidOperationException("Purchase would violate minimum balance requirement");
        // }
        //
        // string reference = "AIRTIME-" + Guid.NewGuid();
        // Guid transactionId = await _airtimeRespository.PurchaseAirtimeAsync(accountNumber, phoneNumber, network, amount, reference, cancellationToken);

        // //Telco API integration here.
        // bool telcoSuccess = await _telcoApiService.PurchaseAirtime(phoneNumber, network, amount, reference);
        //
        // if(!telcoSuccess)
        // {
        //     await _airtimeRepository.HandleAirtimeFailureAsync(transactionId, "Telco API failure");
        //     throw new Exception("Telco API failure, Transaction reversed");
        // }
        // //Log transaction fee.
        // await _airtimeRepository.PurchaseAirtimeAsync(accountNumber, "Fee", "Fee", transactionFee, reference + "-Fee");
        //
        // return transactionId;

        return Guid.NewGuid();

    }
}