namespace MobileBankingUSSD.API.Models;

public class CustomerVerificationResult
{
    public bool IsValid { get; set; }
    public Guid CustomerId { get; set; }
    public string FullName { get; set; }
}