using ErrorOr;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Books.Queries.GetAllBooks;

public record GetAllBooksQuery() : IRequest<ErrorOr<List<Book>>>;

public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, ErrorOr<List<Book>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllBooksQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<List<Book>>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
    {
        var books = await _context.Books
            .AsNoTracking()
            .OrderBy(b => b.Title)
            .ToListAsync(cancellationToken);

        return books;
    }
}