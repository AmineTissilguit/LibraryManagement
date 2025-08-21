using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.BorrowingTransactions.Common;

public record BorrowingTransactionDto(
    int Id,
    int BookId,
    string BookTitle,
    string BookAuthor,
    int MemberId,
    string MemberName,
    string MembershipNumber,
    DateTime BorrowDate,
    DateTime DueDate,
    DateTime? ReturnDate,
    BorrowingStatus Status,
    decimal FineAmount,
    bool IsOverdue,
    int DaysOverdue
);
