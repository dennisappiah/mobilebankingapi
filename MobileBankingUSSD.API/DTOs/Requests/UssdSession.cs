namespace MobileBankingUSSD.API.DTOs.Requests;

public class UssdSession
{
    public string SessionId { get; set; }
    public string Msisdn { get; set; }
    public string CurrentMenu { get; set; }
    public Dictionary<string, string> SessionData { get; set; } = new Dictionary<string, string>();
    public DateTime LastActivity { get; set; }
}