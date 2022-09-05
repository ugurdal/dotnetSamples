namespace EncorePosApiFaker.Dto;

public record BillPayment
{
    public long Id { get; set; }
    public long PosDocumentId { get; set; }
    public string? DocumentNo { get; set; }
    public DateTime Date { get; set; }
    public BillPaymentType Type { get; set; }
    public string? CompanyName { get; set; }
    public bool IsValid { get; set; }
    
    public int StoreId { get; set; }
    public int PosId { get; set; }
    public int UserId { get; set; }

    public int PaymentId { get; set; }

    public decimal InvoiceAmount { get; set; }
    public decimal FeeAmount { get; set; }
    public decimal TotalAmount => InvoiceAmount + FeeAmount;
        
    public BillPayingDetails Details { get; set; } = new();
}

public enum BillPaymentType : byte
{
    Bill = 0,
    GiftCard = 1
}

public record BillPayingDetails
{
    public string? Description { get; set; }
    public string? CompanyName { get; set; }
    public string? InvoiceNo { get; set; }
        
    public decimal InvoiceAmount { get; set; }
    public decimal FeeAmount { get; set; }
    public decimal TotalAmount => InvoiceAmount + FeeAmount;

    public DateTime? PaymentDate { get; set; }
    public DateTime? InvoiceDate { get; set; }

    public string? SubscriberName { get; set; }
    public string? SubscriberNumber { get; set; }
}