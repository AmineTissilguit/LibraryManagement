using ErrorOr;
using LibraryManagement.Application.BorrowingTransactions.Common;
using LibraryManagement.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.BorrowingTransactions.Queries;

public record GetMemberBorrowingHistoryQuery(int MemberId) : IRequest<ErrorOr<List<BorrowingTransactionDto>>>;

public class GetMemberBorrowingHistoryQueryHandler : IRequestHandler<GetMemberBorrowingHistoryQuery, ErrorOr<List<BorrowingTransactionDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetMemberBorrowingHistoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<List<BorrowingTransactionDto>>> Handle(GetMemberBorrowingHistoryQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _context.BorrowingTransactions
            .AsNoTracking()
            .Include(bt => bt.Book)
            .Include(bt => bt.Member)
            .Where(bt => bt.MemberId == request.MemberId)
            .Select(bt => new BorrowingTransactionDto(
                bt.Id,
                bt.BookId,
                bt.Book.Title,
                bt.Book.Author,
                bt.MemberId,
                bt.Member.FullName,
                bt.Member.MembershipNumber,
                bt.BorrowDate,
                bt.DueDate,
                bt.ReturnDate,
                bt.Status,
                bt.FineAmount,
                bt.IsOverdue(),
                bt.GetOverdueDays()
            ))
            .OrderByDescending(bt => bt.BorrowDate)
            .ToListAsync(cancellationToken);

        return transactions;
    }
}