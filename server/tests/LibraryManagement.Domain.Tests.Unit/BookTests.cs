using System;
using ErrorOr;
using FluentAssertions;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Domain.Tests.Unit;

public class BookTests
{
    [Fact]
    public void Constructor_ShouldCreateBookWithValidProperties_WhenValidParametersProvided()
    {
        // Arrange
        var isbn = "978-0-547-92822-7";
        var title = "The Hobbit";
        var author = "J.R.R. Tolkien";
        var publisher = "Houghton Mifflin";
        var publicationYear = 1937;
        var genre = "Fantasy";
        var totalCopies = 3;

        // Act
        var book = new Book(isbn, title, author, publisher, publicationYear, genre, totalCopies);

        // Assert
        book.Isbn.Should().Be(isbn);
        book.Title.Should().Be(title);
        book.Author.Should().Be(author);
        book.Publisher.Should().Be(publisher);
        book.PublicationYear.Should().Be(publicationYear);
        book.Genre.Should().Be(genre);
        book.TotalCopies.Should().Be(totalCopies);
        book.AvailableCopies.Should().Be(totalCopies);
        book.Status.Should().Be(BookStatus.Available);
    }

    [Fact]
    public void IsAvailable_ShouldReturnTrue_WhenAvailableCopiesGreaterThanZero()
    {
        // Arrange
        var book = CreateTestBook(totalCopies: 3);

        // Act
        var isAvailable = book.IsAvailable();

        // Assert
        isAvailable.Should().BeTrue();
    }

    [Fact]
    public void IsAvailable_ShouldReturnFalse_WhenAvailableCopiesIsZero()
    {
        // Arrange
        var book = CreateTestBook(totalCopies: 1);
        book.BorrowCopy();

        // Act
        var isAvailable = book.IsAvailable();

        // Assert
        isAvailable.Should().BeFalse();
    }

    [Fact]
    public void BorrowCopy_ShouldReturnSuccess_WhenCopiesAreAvailable()
    {
        // Arrange
        var book = CreateTestBook(totalCopies: 3);
        var initialAvailableCopies = book.AvailableCopies;

        // Act
        var result = book.BorrowCopy();

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<Success>();
        book.AvailableCopies.Should().Be(initialAvailableCopies - 1);
        book.Status.Should().Be(BookStatus.Available);
    }
    

    [Fact]
    public void BorrowCopy_ShouldUpdateStatusToAllBorrowed_WhenLastCopyIsBorrowed()
    {
        // Arrange
        var book = CreateTestBook(totalCopies: 1);

        // Act
        var result = book.BorrowCopy();

        // Assert
        result.IsError.Should().BeFalse();
        book.AvailableCopies.Should().Be(0);
        book.Status.Should().Be(BookStatus.AllBorrowed);
    }

    [Fact]
    public void BorrowCopy_ShouldReturnError_WhenNoCopiesAvailable()
    {
        // Arrange
        var book = CreateTestBook(totalCopies: 1);
        book.BorrowCopy(); // Borrow the only copy

        // Act
        var result = book.BorrowCopy();

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Book.NoCopiesAvailable");
        result.FirstError.Description.Should().Be("No copies available to borrow");
    }

    [Fact]
    public void ReturnCopy_ShouldReturnSuccess_WhenValidReturn()
    {
        // Arrange
        var book = CreateTestBook(totalCopies: 3);
        book.BorrowCopy();
        var availableCopiesAfterBorrow = book.AvailableCopies;

        // Act
        var result = book.ReturnCopy();

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<Success>();
        book.AvailableCopies.Should().Be(availableCopiesAfterBorrow + 1);
        book.Status.Should().Be(BookStatus.Available);
    }

    [Fact]
    public void ReturnCopy_ShouldUpdateStatusToAvailable_WhenReturningToAllBorrowedBook()
    {
        // Arrange
        var book = CreateTestBook(totalCopies: 1);
        book.BorrowCopy();

        // Act
        var result = book.ReturnCopy();

        // Assert
        result.IsError.Should().BeFalse();
        book.AvailableCopies.Should().Be(1);
        book.Status.Should().Be(BookStatus.Available);
    }

    [Fact]
    public void ReturnCopy_ShouldReturnError_WhenTryingToReturnMoreThanTotal()
    {
        // Arrange
        var book = CreateTestBook(totalCopies: 2);

        // Act
        var result = book.ReturnCopy();

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Book.CannotReturnMore");
        result.FirstError.Description.Should().Be("Cannot return more copies than total");
    }

    [Theory]
    [InlineData(5, 3, BookStatus.Available)]
    [InlineData(3, 0, BookStatus.AllBorrowed)]
    public void BorrowCopy_ShouldSetCorrectStatus_WhenBorrowingMultipleCopies(int totalCopies, int expectedAvailableAfterBorrows, BookStatus expectedStatus)
    {
        // Arrange
        var book = CreateTestBook(totalCopies: totalCopies);

        // Act - Borrow copies until we reach the expected available count
        var borrowsNeeded = totalCopies - expectedAvailableAfterBorrows;
        for (int i = 0; i < borrowsNeeded; i++)
        {
            book.BorrowCopy();
        }

        // Assert
        book.AvailableCopies.Should().Be(expectedAvailableAfterBorrows);
        book.Status.Should().Be(expectedStatus);
    }

    private static Book CreateTestBook(
        string isbn = "978-0-547-92822-7",
        string title = "The Hobbit",
        string author = "J.R.R. Tolkien",
        string publisher = "Houghton Mifflin",
        int publicationYear = 1937,
        string genre = "Fantasy",
        int totalCopies = 3)
    {
        return new Book(isbn, title, author, publisher, publicationYear, genre, totalCopies);
    }
}
