using MobileBankingUSSD.API.Enums;

namespace MobileBankingUSSD.API.DTOs.Requests;

public class CreateAccountRequest
{
    public Guid CustomerId { get; set; }
    public AccountType AccountType { get; set; }
    public decimal InitialBalance { get; set; }
    public bool isActive  { get; set; }
}