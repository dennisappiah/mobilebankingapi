namespace MobileBankingUSSD.API.Models;

public class BillPayment
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid AccountId { get; set; }
    public string BillType { get; set; } = string.Empty; // "ELECTRICITY", "WATER", etc.
    public string AccountNumber { get; set; } = string.Empty; // Biller account number
    public decimal Amount { get; set; }
    public Guid? TransactionId { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    
    public Customer Customer { get; set; } = null!;
    public Account Account { get; set; } = null!;
    public Transaction? Transaction { get; set; }
}
