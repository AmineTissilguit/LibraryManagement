using System;
using FluentAssertions;
using LibraryManagement.Application.BorrowingTransactions.Commands.BorrowBook;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;
using NSubstitute;
using Xunit.Abstractions;

namespace LibraryManagement.Application.Tests.Unit;

public class BorrowBookCommandHandlerTests
{
    private readonly IApplicationDbContext _mockContext;
    private readonly BorrowBookCommandHandler _sut;
    private readonly ITestOutputHelper _output;


    public BorrowBookCommandHandlerTests(ITestOutputHelper output)
    {
        _output = output;
        _mockContext = Substitute.For<IApplicationDbContext>();
        _sut = new BorrowBookCommandHandler(_mockContext);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenValidCommandProvided()
    {
        // Arrange
        var book = CreateTestBook();
        var member = CreateTestMember();
        var command = new BorrowBookCommand(book.Id, member.Id);
        var transactions = new List<BorrowingTransaction>().AsQueryable().BuildMockDbSet();

        SetupMockContext(book, member, transactions);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        var transaction = result.Value;
        transaction.BookId.Should().Be(book.Id);
        transaction.MemberId.Should().Be(member.Id);
        transaction.Status.Should().Be(BorrowingStatus.Active);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenBookNotFound()
    {
        // Arrange
        var command = new BorrowBookCommand(999, 1);
        var books = new List<Book>().AsQueryable().BuildMockDbSet();
        var members = new List<Member> { CreateTestMember() }.AsQueryable().BuildMockDbSet();
        var transactions = new List<BorrowingTransaction>().AsQueryable().BuildMockDbSet();

        _mockContext.Books.Returns(books);
        _mockContext.Members.Returns(members);
        _mockContext.BorrowingTransactions.Returns(transactions);
        _mockContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Book.NotFound");
        result.FirstError.Description.Should().Be($"Book with ID {command.BookId} was not found");
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenMemberNotFound()
    {
        // Arrange
        var book = CreateTestBook();
        var command = new BorrowBookCommand(book.Id, 999);
        
        var books = new List<Book> { book }.AsQueryable().BuildMockDbSet();
        var members = new List<Member>().AsQueryable().BuildMockDbSet();
        var transactions = new List<BorrowingTransaction>().AsQueryable().BuildMockDbSet();

        _mockContext.Books.Returns(books);
        _mockContext.Members.Returns(members);
        _mockContext.BorrowingTransactions.Returns(transactions);
        _mockContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Member.NotFound");
        result.FirstError.Description.Should().Be($"Member with ID {command.MemberId} was not found");
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenMemberAlreadyBorrowedSameBook()
    {
        // Arrange
        var book = CreateTestBook();
        var member = CreateTestMember();
        var command = new BorrowBookCommand(book.Id, member.Id);
        var existingTransaction = BorrowingTransaction.Create(book, member).Value;
        var transactions = new List<BorrowingTransaction> { existingTransaction }.AsQueryable().BuildMockDbSet();

        SetupMockContext(book, member, transactions);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("BorrowingTransaction.AlreadyBorrowed");
        result.FirstError.Description.Should().Be("Member has already borrowed this book");
    }

    
    // Helper methods
    private void SetupMockContext(Book book, Member member, DbSet<BorrowingTransaction> transactions)
    {
        var books = new List<Book> { book }.AsQueryable().BuildMockDbSet();
        var members = new List<Member> { member }.AsQueryable().BuildMockDbSet();

        _mockContext.Books.Returns(books);
        _mockContext.Members.Returns(members);
        _mockContext.BorrowingTransactions.Returns(transactions);
        _mockContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);
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

    private static Member CreateTestMember(
        string membershipNumber = "MEM001",
        string firstName = "John",
        string lastName = "Doe",
        string email = "john.doe@email.com",
        string phone = "+1234567890",
        string address = "123 Main St",
        MembershipType membershipType = MembershipType.Adult)
    {
        return new Member(membershipNumber, firstName, lastName, email, phone, address, membershipType);
    }
}
