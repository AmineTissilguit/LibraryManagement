namespace LibraryManagement.Domain.Events;

public class BookReturnedEvent : BaseDomainEvent
{
    public int BookId { get; }
    public int MemberId { get; }
    public int TransactionId { get; }
    public DateTime ReturnDate { get; }
    public decimal FineAmount { get; }
    public bool WasOverdue { get; }

    public BookReturnedEvent(int bookId, int memberId, int transactionId,
                           DateTime returnDate, decimal fineAmount, bool wasOverdue)
    {
        BookId = bookId;
        MemberId = memberId;
        TransactionId = transactionId;
        ReturnDate = returnDate;
        FineAmount = fineAmount;
        WasOverdue = wasOverdue;
    }
}
