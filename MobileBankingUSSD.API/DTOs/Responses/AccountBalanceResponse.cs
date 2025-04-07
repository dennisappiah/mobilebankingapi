
namespace MobileBankingUSSD.API.DTOs.Responses;


public class AccountBalanceResponse
{
    public string AccountNumber { get; set; }
    public decimal Balance { get; set; }
    public decimal AvailableBalance { get; set; }
    public int AccountTypeId { get; set; }
    public DateTime LastUpdated { get; set; }
}