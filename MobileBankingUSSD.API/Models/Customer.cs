namespace MobileBankingUSSD.API.Models;

public class Customer
{
    public Guid Id { get; set; }
    public string Msisdn { get; set; }
    public string PinHash { get; set; }
    public string FullName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
    
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
    public ICollection<AirtimePurchase> AirtimePurchases { get; set; } = new List<AirtimePurchase>();
    public ICollection<BillPayment> BillPayments { get; set; } = new List<BillPayment>();
}