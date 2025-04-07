namespace MobileBankingUSSD.API.DTOs.Requests;

public class ChangePinRequest
{
    public Guid CustomerId { get; set; }
    public string OldPin { get; set; }
    public string NewPin { get; set; }
}