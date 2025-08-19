using System;
using ErrorOr;
using LibraryManagement.Domain.Common;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Events;

namespace LibraryManagement.Domain.Entities;

public class BorrowingTransaction : AggregateRoot
{
    public int BookId { get; private set; }
    public int MemberId { get; private set; }
    public DateTime BorrowDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? ReturnDate { get; private set; }
    public BorrowingStatus Status { get; private set; }
    public decimal FineAmount { get; private set; }

    public Book Book { get; private set; }
    public Member Member { get; private set; }

    private BorrowingTransaction() { }

    private BorrowingTransaction(int bookId, int memberId, int loanPeriodDays)
    {
        BookId = bookId;
        MemberId = memberId;
        BorrowDate = DateTime.UtcNow;
        DueDate = BorrowDate.AddDays(loanPeriodDays);
        Status = BorrowingStatus.Active;
        FineAmount = 0;
    }

    public static ErrorOr<BorrowingTransaction> Create(Book book, Member member)
    {
        var memberCanBorrow = member.CanBorrow();
        if (memberCanBorrow.IsError)
            return memberCanBorrow.Errors;

        if (!book.IsAvailable())
            return Error.Conflict("BorrowingTransaction.BookNotAvailable", 
                                "Book is not available for borrowing");

        var transaction = new BorrowingTransaction(book.Id, member.Id, member.GetLoanPeriodDays());

            transaction.RaiseDomainEvent(new BookBorrowedEvent(
        book.Id, 
        member.Id, 
        transaction.Id, 
        transaction.BorrowDate, 
        transaction.DueDate));
        
        return transaction;
    }

    public bool IsOverdue() => DateTime.UtcNow > DueDate && Status == BorrowingStatus.Active;

    public ErrorOr<Success> Return()
    {
        if (Status != BorrowingStatus.Active)
            return Error.Conflict("BorrowingTransaction.NotActive", 
                                "Transaction is not in active status");
    
        var returnDate = DateTime.UtcNow;
        var wasOverdue = returnDate > DueDate;
        
        ReturnDate = returnDate;
        Status = BorrowingStatus.Returned;
    
        // Calculate fine if overdue
        if (ReturnDate > DueDate)
        {
            var overdueDays = (ReturnDate.Value - DueDate).Days;
            FineAmount = CalculateFine(overdueDays);
        }
    
        RaiseDomainEvent(new BookReturnedEvent(
            BookId, 
            MemberId, 
            Id, 
            ReturnDate.Value, 
            FineAmount, 
            wasOverdue));
    
        return Result.Success;
    }

    private decimal CalculateFine(int overdueDays)
    {
        const decimal dailyFineRate = 2.00m;
        return overdueDays * dailyFineRate;
    }

    public void MarkAsOverdue()
    {
        if (Status == BorrowingStatus.Active && DateTime.UtcNow > DueDate)
        {
            Status = BorrowingStatus.Overdue;
        }
    }

    public int GetOverdueDays()
    {
        if (!IsOverdue()) return 0;
        return (DateTime.UtcNow - DueDate).Days;
    }
}