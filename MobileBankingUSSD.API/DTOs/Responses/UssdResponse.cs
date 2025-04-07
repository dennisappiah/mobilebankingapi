namespace MobileBankingUSSD.API.DTOs.Responses;

/// <summary>
/// based on USSD provider
/// </summary>
public class UssdResponse
{
    public string Response { get; set; }
    public bool EndSession { get; set; }
}