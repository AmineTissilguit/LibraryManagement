using ErrorOr;
using LibraryManagement.Application.Books.Queries.Common;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Books.Queries.GetAllBooks;

public record GetAllBooksQuery() : IRequest<ErrorOr<List<BookDto>>>;

public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, ErrorOr<List<BookDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllBooksQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<List<BookDto>>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
    {
         var books = await _context.Books
            .AsNoTracking()
            .Select(b => new BookDto(
                b.Id,
                b.Isbn,
                b.Title,
                b.Author,
                b.Publisher,
                b.PublicationYear,
                b.Genre,
                b.TotalCopies,
                b.AvailableCopies,
                b.Status,
                b.IsAvailable()
            ))
            .OrderBy(b => b.Title)
            .ToListAsync(cancellationToken);

        return books;
    }
}