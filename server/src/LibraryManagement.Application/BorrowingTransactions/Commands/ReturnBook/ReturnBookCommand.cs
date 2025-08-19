using ErrorOr;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.BorrowingTransactions.Commands.ReturnBook;

public record ReturnBookCommand(
    int TransactionId
) : IRequest<ErrorOr<BorrowingTransaction>>;

public class ReturnBookCommandHandler : IRequestHandler<ReturnBookCommand, ErrorOr<BorrowingTransaction>>
{
    private readonly IApplicationDbContext _context;

    public ReturnBookCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<BorrowingTransaction>> Handle(ReturnBookCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _context.BorrowingTransactions
            .Include(bt => bt.Book)
            .Include(bt => bt.Member)
            .FirstOrDefaultAsync(bt => bt.Id == request.TransactionId, cancellationToken);

        if (transaction is null)
        {
            return Error.NotFound("BorrowingTransaction.NotFound", 
                                $"Borrowing transaction with ID {request.TransactionId} was not found");
        }

        var returnResult = transaction.Return();
        if (returnResult.IsError) return returnResult.Errors;

        transaction.Book.ReturnCopy();
        transaction.Member.DecrementActiveBorrowings();

        await _context.SaveChangesAsync(cancellationToken);

        return transaction;
    }
}