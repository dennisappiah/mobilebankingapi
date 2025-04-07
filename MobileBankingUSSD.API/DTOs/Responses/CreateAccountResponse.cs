using MobileBankingUSSD.API.Enums;

namespace MobileBankingUSSD.API.DTOs.Responses;

public class CreateAccountResponse
{
    public Guid AccountId { get; set; }
    public string AccountNumber { get; set; }
    public AccountType AccountType { get; set; }
}