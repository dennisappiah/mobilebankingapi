namespace MobileBankingUSSD.API.DTOs.Requests;

public class VerifyPinRequest
{
    public string Msisdn { get; set; }
    public string Pin { get; set; }
}
