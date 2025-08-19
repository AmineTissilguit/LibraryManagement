using System;
using ErrorOr;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Books.Commands;

public record CreateBookCommand(
    string Isbn,
    string Title,
    string Author,
    string Publisher,
    int PublicationYear,
    string Genre,
    int TotalCopies
) : IRequest<ErrorOr<Book>>;

public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, ErrorOr<Book>>
{
    private readonly IApplicationDbContext _context;

    public CreateBookCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<Book>> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var existingBook = await _context.Books
            .FirstOrDefaultAsync(b => b.Isbn == request.Isbn, cancellationToken);

        if (existingBook is not null)
        {
            return Error.Conflict("Book.IsbnAlreadyExists", "A book with this ISBN already exists");
        }
        
        var book = new Book(
            request.Isbn,
            request.Title,
            request.Author,
            request.Publisher,
            request.PublicationYear,
            request.Genre,
            request.TotalCopies);

        _context.Books.Add(book);
        await _context.SaveChangesAsync(cancellationToken);

        return book;
    }
}
