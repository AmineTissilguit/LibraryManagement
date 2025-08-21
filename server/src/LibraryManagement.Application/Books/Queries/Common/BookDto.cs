using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.Books.Queries.Common;

public record BookDto(
    int Id,
    string Isbn,
    string Title,
    string Author,
    string Publisher,
    int PublicationYear,
    string Genre,
    int TotalCopies,
    int AvailableCopies,
    BookStatus Status,
    bool IsAvailable
);
