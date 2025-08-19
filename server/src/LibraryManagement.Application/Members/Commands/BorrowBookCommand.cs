using ErrorOr;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.BorrowingTransactions.Commands;

public record BorrowBookCommand(
    int BookId,
    int MemberId
) : IRequest<ErrorOr<BorrowingTransaction>>;

public class BorrowBookCommandHandler : IRequestHandler<BorrowBookCommand, ErrorOr<BorrowingTransaction>>
{
    private readonly IApplicationDbContext _context;

    public BorrowBookCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<BorrowingTransaction>> Handle(BorrowBookCommand request, CancellationToken cancellationToken)
    {
        var bookResult = await GetBookByIdAsync(request.BookId, cancellationToken);
        if (bookResult.IsError) return bookResult.Errors;

        var memberResult = await GetMemberByIdAsync(request.MemberId, cancellationToken);
        if (memberResult.IsError) return memberResult.Errors;

        var hasActiveTransaction = await MemberHasActiveBorrowingForBookAsync(request.BookId, request.MemberId, cancellationToken);
        if (hasActiveTransaction)
        {
            return Error.Conflict("BorrowingTransaction.AlreadyBorrowed", 
                                "Member has already borrowed this book");
        }

        var book = bookResult.Value;
        var member = memberResult.Value;

        var transactionResult = BorrowingTransaction.Create(book, member);
        if (transactionResult.IsError) return transactionResult.Errors;

        var transaction = transactionResult.Value;

        var borrowResult = book.BorrowCopy();
        if (borrowResult.IsError) return borrowResult.Errors;

        member.IncrementActiveBorrowings();

        _context.BorrowingTransactions.Add(transaction);
        await _context.SaveChangesAsync(cancellationToken);

        return transaction;
    }

    private async Task<ErrorOr<Book>> GetBookByIdAsync(int bookId, CancellationToken cancellationToken)
    {
        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.Id == bookId, cancellationToken);

        return book is null 
            ? Error.NotFound("Book.NotFound", $"Book with ID {bookId} was not found")
            : book;
    }

    private async Task<ErrorOr<Member>> GetMemberByIdAsync(int memberId, CancellationToken cancellationToken)
    {
        var member = await _context.Members
            .FirstOrDefaultAsync(m => m.Id == memberId, cancellationToken);

        return member is null 
            ? Error.NotFound("Member.NotFound", $"Member with ID {memberId} was not found")
            : member;
    }

    private async Task<bool> MemberHasActiveBorrowingForBookAsync(int bookId, int memberId, CancellationToken cancellationToken)
    {
        return await _context.BorrowingTransactions
            .AnyAsync(bt => bt.BookId == bookId && 
                           bt.MemberId == memberId && 
                           bt.Status == Domain.Enums.BorrowingStatus.Active, 
                     cancellationToken);
    }
}