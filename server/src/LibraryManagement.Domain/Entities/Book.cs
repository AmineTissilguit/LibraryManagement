using System;
using ErrorOr;
using LibraryManagement.Domain.Common;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Domain.Entities;

public class Book : AggregateRoot
{
    public string Isbn { get; private set; }
    public string Title { get; private set; }
    public string Author { get; private set; }
    public string Publisher { get; private set; }
    public int PublicationYear { get; private set; }
    public string Genre { get; private set; }
    public int TotalCopies { get; private set; }
    public int AvailableCopies { get; private set; }
    public BookStatus Status { get; private set; }

    private Book() { }

    public Book(string isbn, string title, string author, string publisher, 
               int publicationYear, string genre, int totalCopies)
    {
        Isbn = isbn;
        Title = title;
        Author = author;
        Publisher = publisher;
        PublicationYear = publicationYear;
        Genre = genre;
        TotalCopies = totalCopies;
        AvailableCopies = totalCopies;
        Status = BookStatus.Available;
    }

    public bool IsAvailable() => AvailableCopies > 0;

    public ErrorOr<Success> BorrowCopy()
    {
        if (!IsAvailable())
            return Error.Conflict("Book.NoCopiesAvailable", "No copies available to borrow");
        
        AvailableCopies--;
        Status = AvailableCopies == 0 ? BookStatus.AllBorrowed : BookStatus.Available;
        
        return Result.Success;
    }

public ErrorOr<Success> ReturnCopy()
    {
        if (AvailableCopies >= TotalCopies)
            return Error.Conflict("Book.CannotReturnMore", "Cannot return more copies than total");
        
        AvailableCopies++;
        Status = BookStatus.Available;
        
        return Result.Success;
    }
}