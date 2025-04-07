using MobileBankingUSSD.API.Enums;

namespace MobileBankingUSSD.API.Models;

public class Transaction
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public TransactionType TransactionType{ get; set; }  // DEPOSIT, WITHDRAW, TRANSFER, BILL_PAYMENT, AIRTIME_PURCHASE
    public decimal Amount { get; set; }
    public string? Reference { get; set; }
    public string? BeneficiaryAccount { get; set; }
    public TransactionStatus? Status { get; set; } // PENDING, COMPLETED, FAILED, REVERSED 
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public Guid? ReversalReference { get; set; }
    public Account Account { get; set; }
    public AirtimePurchase? AirtimePurchase { get; set; }
    public BillPayment? BillPayment { get; set; }
    public Transaction? ReversedTransaction { get; set; }
}


