namespace MobileBankingUSSD.API.Exceptions;

public class ErrorDetails
{
    public string? Code { get; set; }    
    public string? Message { get; set; }  
    public string? Target { get; set; } 
    
    public ErrorDetails() { }

    public ErrorDetails(string code, string message, string? target = null)
    {
        Code = code;
        Message = message;
        Target = target;
    }
}