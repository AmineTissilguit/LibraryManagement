namespace LibraryManagement.Domain.Events;

public class BookBorrowedEvent : BaseDomainEvent
{
    public int BookId { get; }
    public int MemberId { get; }
    public int TransactionId { get; }
    public DateTime BorrowDate { get; }
    public DateTime DueDate { get; }

    public BookBorrowedEvent(int bookId, int memberId, int transactionId,
                           DateTime borrowDate, DateTime dueDate)
    {
        BookId = bookId;
        MemberId = memberId;
        TransactionId = transactionId;
        BorrowDate = borrowDate;
        DueDate = dueDate;
    }
}
