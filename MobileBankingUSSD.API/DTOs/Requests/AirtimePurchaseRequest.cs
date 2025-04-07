namespace MobileBankingUSSD.API.DTOs.Requests;

public class AirtimePurchaseRequest
{
    public string AccountNumber { get; set; }
    public string PhoneNumber { get; set; }
    public string Network { get; set; }
    public decimal Amount { get; set; }
}