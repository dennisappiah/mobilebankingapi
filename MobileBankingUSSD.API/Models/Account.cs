using MobileBankingUSSD.API.Enums;

namespace MobileBankingUSSD.API.Models;

public class Account
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string AccountNumber { get; set; } 
    public AccountType AccountType { get; set; }
    public decimal Balance { get; set; } = 0.00m;  //total funds
    public decimal AvailableBalance { get; set; } = 0.00m;  // usable funds
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    
    public Customer Customer { get; set; } = null!;
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<AirtimePurchase> AirtimePurchases { get; set; } = new List<AirtimePurchase>();
    public ICollection<BillPayment> BillPayments { get; set; } = new List<BillPayment>();
}