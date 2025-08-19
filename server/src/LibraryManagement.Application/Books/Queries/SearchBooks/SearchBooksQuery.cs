using ErrorOr;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Books.Queries.SearchBooks;

public record SearchBooksQuery(string SearchTerm) : IRequest<ErrorOr<List<Book>>>;

public class SearchBooksQueryHandler : IRequestHandler<SearchBooksQuery, ErrorOr<List<Book>>>
{
    private readonly IApplicationDbContext _context;

    public SearchBooksQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<List<Book>>> Handle(SearchBooksQuery request, CancellationToken cancellationToken)
    {
        var searchTerm = request.SearchTerm.ToLower();
        
        var books = await _context.Books
            .AsNoTracking()
            .Where(b => b.Title.ToLower().Contains(searchTerm) ||
                       b.Author.ToLower().Contains(searchTerm) ||
                       b.Isbn.Contains(searchTerm))
            .OrderBy(b => b.Title)
            .ToListAsync(cancellationToken);

        return books;
    }
}