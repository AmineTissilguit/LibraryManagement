using ErrorOr;
using FluentAssertions;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Events;

namespace LibraryManagement.Domain.Tests.Unit;

public class BorrowingTransactionTests
{
    [Fact]
    public void Create_ShouldReturnSuccessWithValidTransaction_WhenBookAvailableAndMemberCanBorrow()
    {
        // Arrange
        var book = CreateTestBook();
        var member = CreateTestMember();

        // Act
        var result = BorrowingTransaction.Create(book, member);

        // Assert
        result.IsError.Should().BeFalse();
        var transaction = result.Value;
        transaction.BookId.Should().Be(book.Id);
        transaction.MemberId.Should().Be(member.Id);
        transaction.Status.Should().Be(BorrowingStatus.Active);
        transaction.FineAmount.Should().Be(0);
        transaction.BorrowDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        transaction.DueDate.Should().Be(transaction.BorrowDate.AddDays(member.GetLoanPeriodDays()));
        transaction.ReturnDate.Should().BeNull();
    }

    [Fact]
    public void Create_ShouldRaiseBookBorrowedEvent_WhenTransactionCreated()
    {
        // Arrange
        var book = CreateTestBook();
        var member = CreateTestMember();

        // Act
        var result = BorrowingTransaction.Create(book, member);

        // Assert
        result.IsError.Should().BeFalse();
        var transaction = result.Value;
        transaction.DomainEvents.Should().HaveCount(1);
        var domainEvent = transaction.DomainEvents.First().Should().BeOfType<BookBorrowedEvent>().Subject;
        domainEvent.BookId.Should().Be(book.Id);
        domainEvent.MemberId.Should().Be(member.Id);
        domainEvent.TransactionId.Should().Be(transaction.Id);
        domainEvent.BorrowDate.Should().Be(transaction.BorrowDate);
        domainEvent.DueDate.Should().Be(transaction.DueDate);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenMemberCannotBorrow()
    {
        // Arrange
        var book = CreateTestBook();
        var member = CreateTestMember();
        member.Deactivate(); // Make member unable to borrow

        // Act
        var result = BorrowingTransaction.Create(book, member);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Member.NotActive");
    }

    [Fact]
    public void Create_ShouldReturnError_WhenBookNotAvailable()
    {
        // Arrange
        var book = CreateTestBook(totalCopies: 1);
        book.BorrowCopy(); // Make book unavailable
        var member = CreateTestMember();

        // Act
        var result = BorrowingTransaction.Create(book, member);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("BorrowingTransaction.BookNotAvailable");
        result.FirstError.Description.Should().Be("Book is not available for borrowing");
    }

    [Fact]
    public void IsOverdue_ShouldReturnFalse_WhenTransactionNotOverdue()
    {
        // Arrange
        var book = CreateTestBook();
        var member = CreateTestMember();
        var result = BorrowingTransaction.Create(book, member);
        var transaction = result.Value;

        // Act
        var isOverdue = transaction.IsOverdue();

        // Assert
        isOverdue.Should().BeFalse();
    }

    [Fact]
    public void Return_ShouldReturnSuccess_WhenTransactionIsActive()
    {
        // Arrange
        var book = CreateTestBook();
        var member = CreateTestMember();
        var result = BorrowingTransaction.Create(book, member);
        var transaction = result.Value;

        // Act
        var returnResult = transaction.Return();

        // Assert
        returnResult.IsError.Should().BeFalse();
        returnResult.Value.Should().BeOfType<Success>();
        transaction.Status.Should().Be(BorrowingStatus.Returned);
        transaction.ReturnDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        transaction.FineAmount.Should().Be(0); // Not overdue, no fine
    }

    [Fact]
    public void Return_ShouldRaiseBookReturnedEvent_WhenReturned()
    {
        // Arrange
        var book = CreateTestBook();
        var member = CreateTestMember();
        var result = BorrowingTransaction.Create(book, member);
        var transaction = result.Value;
        transaction.ClearDomainEvents(); // Clear creation event

        // Act
        var returnResult = transaction.Return();

        // Assert
        returnResult.IsError.Should().BeFalse();
        transaction.DomainEvents.Should().HaveCount(1);
        var domainEvent = transaction.DomainEvents.First().Should().BeOfType<BookReturnedEvent>().Subject;
        domainEvent.BookId.Should().Be(transaction.BookId);
        domainEvent.MemberId.Should().Be(transaction.MemberId);
        domainEvent.TransactionId.Should().Be(transaction.Id);
        domainEvent.ReturnDate.Should().Be(transaction.ReturnDate.Value);
        domainEvent.FineAmount.Should().Be(transaction.FineAmount);
        domainEvent.WasOverdue.Should().BeFalse();
    }

    [Fact]
    public void Return_ShouldReturnError_WhenTransactionNotActive()
    {
        // Arrange
        var book = CreateTestBook();
        var member = CreateTestMember();
        var result = BorrowingTransaction.Create(book, member);
        var transaction = result.Value;
        transaction.Return(); // Return once

        // Act
        var secondReturnResult = transaction.Return(); // Try to return again

        // Assert
        secondReturnResult.IsError.Should().BeTrue();
        secondReturnResult.FirstError.Code.Should().Be("BorrowingTransaction.NotActive");
        secondReturnResult.FirstError.Description.Should().Be("Transaction is not in active status");
    }

    [Fact]
    public void MarkAsOverdue_ShouldNotUpdateStatus_WhenNotPastDueDate()
    {
        // Arrange
        var book = CreateTestBook();
        var member = CreateTestMember();
        var result = BorrowingTransaction.Create(book, member);
        var transaction = result.Value;

        // Act
        transaction.MarkAsOverdue();

        // Assert
        transaction.Status.Should().Be(BorrowingStatus.Active);
    }

    [Fact]
    public void MarkAsOverdue_ShouldNotUpdateStatus_WhenNotActive()
    {
        // Arrange
        var book = CreateTestBook();
        var member = CreateTestMember();
        var result = BorrowingTransaction.Create(book, member);
        var transaction = result.Value;
        transaction.Return(); // Make it not active

        // Act
        transaction.MarkAsOverdue();

        // Assert
        transaction.Status.Should().Be(BorrowingStatus.Returned);
    }

    [Fact]
    public void GetOverdueDays_ShouldReturnZero_WhenNotOverdue()
    {
        // Arrange
        var book = CreateTestBook();
        var member = CreateTestMember();
        var result = BorrowingTransaction.Create(book, member);
        var transaction = result.Value;

        // Act
        var overdueDays = transaction.GetOverdueDays();

        // Assert
        overdueDays.Should().Be(0);
    }

    [Theory]
    [InlineData(MembershipType.Student, 14)]
    [InlineData(MembershipType.Adult, 21)]
    [InlineData(MembershipType.Senior, 21)]
    [InlineData(MembershipType.Staff, 30)]
    public void Create_ShouldSetCorrectDueDate_WhenMembershipTypeProvided(MembershipType membershipType, int expectedLoanDays)
    {
        // Arrange
        var book = CreateTestBook();
        var member = CreateTestMember(membershipType: membershipType);

        // Act
        var result = BorrowingTransaction.Create(book, member);

        // Assert
        result.IsError.Should().BeFalse();
        var transaction = result.Value;
        var expectedDueDate = transaction.BorrowDate.AddDays(expectedLoanDays);
        transaction.DueDate.Should().Be(expectedDueDate);
    }

    // Helper methods
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