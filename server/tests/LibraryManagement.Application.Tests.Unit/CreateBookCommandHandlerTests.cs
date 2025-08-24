using System;
using FluentAssertions;
using LibraryManagement.Application.Books.Commands.CreateBook;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Domain.Entities;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace LibraryManagement.Application.Tests.Unit;

public class CreateBookCommandHandlerTests
{
    private readonly IApplicationDbContext _mockContext;
    private readonly CreateBookCommandHandler _sut;

    public CreateBookCommandHandlerTests()
    {
        _mockContext = Substitute.For<IApplicationDbContext>();
        _sut = new CreateBookCommandHandler(_mockContext);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenValidCommandProvided()
    {
        // Arrange
        var command = CreateTestCommand();
        var books = new List<Book>().AsQueryable().BuildMockDbSet();

        _mockContext.Books.Returns(books);
        _mockContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        var book = result.Value;
        book.Isbn.Should().Be(command.Isbn);
        book.Title.Should().Be(command.Title);
        book.Author.Should().Be(command.Author);
        book.Publisher.Should().Be(command.Publisher);
        book.PublicationYear.Should().Be(command.PublicationYear);
        book.Genre.Should().Be(command.Genre);
        book.TotalCopies.Should().Be(command.TotalCopies);
    }

    [Fact]
    public async Task Handle_ShouldAddBookToContext_WhenValidCommandProvided()
    {
        // Arrange
        var command = CreateTestCommand();
        var books = new List<Book>().AsQueryable().BuildMockDbSet();
        
        _mockContext.Books.Returns(books);
        _mockContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        books.Received(1).Add(Arg.Is<Book>(b => 
            b.Isbn == command.Isbn && 
            b.Title == command.Title));
        await _mockContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenBookWithSameIsbnExists()
    {
        // Arrange
        var command = CreateTestCommand();
        var existingBook = new Book(command.Isbn, "Existing Title", "Existing Author", 
                                   "Existing Publisher", 2000, "Fiction", 2);
        var books = new List<Book> { existingBook }.AsQueryable().BuildMockDbSet();
        
        _mockContext.Books.Returns(books);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Book.IsbnAlreadyExists");
        result.FirstError.Description.Should().Be("A book with this ISBN already exists");
    }

    [Fact]
    public async Task Handle_ShouldNotAddOrSave_WhenBookWithSameIsbnExists()
    {
        // Arrange
        var command = CreateTestCommand();
        var existingBook = new Book(command.Isbn, "Existing Title", "Existing Author", 
                                   "Existing Publisher", 2000, "Fiction", 2);
        var books = new List<Book> { existingBook }.AsQueryable().BuildMockDbSet();
        
        _mockContext.Books.Returns(books);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        books.DidNotReceive().Add(Arg.Any<Book>());
        await _mockContext.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    // Helper methods
    private static CreateBookCommand CreateTestCommand(
        string isbn = "978-0-547-92822-7",
        string title = "The Hobbit",
        string author = "J.R.R. Tolkien",
        string publisher = "Houghton Mifflin",
        int publicationYear = 1937,
        string genre = "Fantasy",
        int totalCopies = 3)
    {
        return new CreateBookCommand(isbn, title, author, publisher, publicationYear, genre, totalCopies);
    }
}
