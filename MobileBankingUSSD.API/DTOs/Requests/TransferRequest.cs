namespace MobileBankingUSSD.API.DTOs.Requests;

public class TransferRequest
{
    public string FromAccount { get; set; }
    public string ToAccount { get; set; }
    public decimal Amount { get; set; }
    public string Reference { get; set; } 
}