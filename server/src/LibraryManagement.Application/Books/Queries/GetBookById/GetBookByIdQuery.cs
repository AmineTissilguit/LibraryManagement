using ErrorOr;
using LibraryManagement.Application.Books.Queries.Common;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Books.Queries.GetBookById;

public record GetBookByIdQuery(int Id) : IRequest<ErrorOr<BookDto>>;

public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, ErrorOr<BookDto>>
{
    private readonly IApplicationDbContext _context;

    public GetBookByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<BookDto>> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
var book = await _context.Books
            .AsNoTracking()
            .Where(b => b.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (book is null)
        {
            return Error.NotFound("Book.NotFound", $"Book with ID {request.Id} was not found");
        }

        return book;
    }
}