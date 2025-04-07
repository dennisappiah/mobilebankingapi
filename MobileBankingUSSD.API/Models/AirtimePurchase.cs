namespace MobileBankingUSSD.API.Models;

public class AirtimePurchase
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid AccountId { get; set; }
    public string Msisdn { get; set; } = string.Empty;
    public string NetworkProvider { get; set; } = string.Empty; // "MTN", "VODAFONE", etc.
    public decimal Amount { get; set; }
    public Guid? TransactionId { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        
   
    public Customer Customer { get; set; } = null!;
    public Account Account { get; set; } = null!;
    public Transaction? Transaction { get; set; }
}