using ErrorOr;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Books.Queries;

public record GetBookByIdQuery(int Id) : IRequest<ErrorOr<Book>>;

public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, ErrorOr<Book>>
{
    private readonly IApplicationDbContext _context;

    public GetBookByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<Book>> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var book = await _context.Books
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (book is null)
        {
            return Error.NotFound("Book.NotFound", $"Book with ID {request.Id} was not found");
        }

        return book;
    }
}