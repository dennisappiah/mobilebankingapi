namespace MobileBankingUSSD.API.DTOs.Requests;

public class UssdRequest
{
    public string SessionId { get; set; }
    public string Msisdn { get; set; } 
    public string ServiceCode { get; set; } 
    public string NetworkCode { get; set; }
    public string Text { get; set; }
}