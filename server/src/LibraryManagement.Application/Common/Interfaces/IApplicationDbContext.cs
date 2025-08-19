using LibraryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace LibraryManagement.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Book> Books { get; }
    DbSet<Member> Members { get; }
    DbSet<BorrowingTransaction> BorrowingTransactions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}