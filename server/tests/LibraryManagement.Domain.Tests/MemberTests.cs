using System;
using ErrorOr;
using FluentAssertions;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Events;

namespace LibraryManagement.Domain.Tests.Unit;

public class MemberTests
{
    [Fact]
    public void Constructor_ShouldCreateMemberWithValidProperties_WhenValidParametersProvided()
    {
        // Arrange
        var membershipNumber = "MEM001";
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@email.com";
        var phone = "+1234567890";
        var address = "123 Main St";
        var membershipType = MembershipType.Adult;

        // Act
        var member = new Member(membershipNumber, firstName, lastName, email, phone, address, membershipType);

        // Assert
        member.MembershipNumber.Should().Be(membershipNumber);
        member.FirstName.Should().Be(firstName);
        member.LastName.Should().Be(lastName);
        member.Email.Should().Be(email);
        member.Phone.Should().Be(phone);
        member.Address.Should().Be(address);
        member.MembershipType.Should().Be(membershipType);
        member.IsActive.Should().BeTrue();
        member.ActiveBorrowingsCount.Should().Be(0);
        member.RegistrationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_ShouldRaiseMemberRegisteredEvent_WhenMemberIsCreated()
    {
        // Arrange
        var membershipNumber = "MEM001";
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@email.com";
        var phone = "+1234567890";
        var address = "123 Main St";
        var membershipType = MembershipType.Adult;

        // Act
        var member = new Member(membershipNumber, firstName, lastName, email, phone, address, membershipType);

        // Assert
        member.DomainEvents.Should().HaveCount(1);
        var domainEvent = member.DomainEvents.First().Should().BeOfType<MemberRegisteredEvent>().Subject;
        domainEvent.MemberId.Should().Be(member.Id);
        domainEvent.MembershipNumber.Should().Be(membershipNumber);
        domainEvent.Email.Should().Be(email);
        domainEvent.FullName.Should().Be($"{firstName} {lastName}");
        domainEvent.RegistrationDate.Should().Be(member.RegistrationDate);
    }

    [Fact]
    public void FullName_ShouldReturnConcatenatedFirstAndLastName_WhenCalled()
    {
        // Arrange
        var member = CreateTestMember(firstName: "John", lastName: "Doe");

        // Act
        var fullName = member.FullName;

        // Assert
        fullName.Should().Be("John Doe");
    }

    [Theory]
    [InlineData(MembershipType.Student, 3)]
    [InlineData(MembershipType.Adult, 5)]
    [InlineData(MembershipType.Senior, 5)]
    [InlineData(MembershipType.Staff, 10)]
    public void GetBorrowingLimit_ShouldReturnCorrectLimit_WhenMembershipTypeProvided(MembershipType membershipType, int expectedLimit)
    {
        // Arrange
        var member = CreateTestMember(membershipType: membershipType);

        // Act
        var limit = member.GetBorrowingLimit();

        // Assert
        limit.Should().Be(expectedLimit);
    }

    [Theory]
    [InlineData(MembershipType.Student, 14)]
    [InlineData(MembershipType.Adult, 21)]
    [InlineData(MembershipType.Senior, 21)]
    [InlineData(MembershipType.Staff, 30)]
    public void GetLoanPeriodDays_ShouldReturnCorrectPeriod_WhenMembershipTypeProvided(MembershipType membershipType, int expectedDays)
    {
        // Arrange
        var member = CreateTestMember(membershipType: membershipType);

        // Act
        var loanPeriod = member.GetLoanPeriodDays();

        // Assert
        loanPeriod.Should().Be(expectedDays);
    }

    [Fact]
    public void CanBorrow_ShouldReturnSuccess_WhenMemberIsActiveAndUnderLimit()
    {
        // Arrange
        var member = CreateTestMember(membershipType: MembershipType.Adult);

        // Act
        var result = member.CanBorrow();

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<Success>();
    }

    [Fact]
    public void CanBorrow_ShouldReturnError_WhenMemberIsNotActive()
    {
        // Arrange
        var member = CreateTestMember();
        member.Deactivate();

        // Act
        var result = member.CanBorrow();

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Member.NotActive");
        result.FirstError.Description.Should().Be("Member account is not active");
    }

     [Fact]
    public void CanBorrow_ShouldReturnError_WhenMemberAtBorrowingLimit()
    {
        // Arrange
        var member = CreateTestMember(membershipType: MembershipType.Student); // Limit: 3
        
        // Simulate reaching the borrowing limit
        member.IncrementActiveBorrowings();
        member.IncrementActiveBorrowings();
        member.IncrementActiveBorrowings(); // Now at limit of 3

        // Act
        var result = member.CanBorrow();

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Member.BorrowingLimitExceeded");
        result.FirstError.Description.Should().Be("Member has reached borrowing limit of 3");
    }

     [Fact]
    public void IncrementActiveBorrowings_ShouldIncreaseCount_WhenCalled()
    {
        // Arrange
        var member = CreateTestMember();
        var initialCount = member.ActiveBorrowingsCount;

        // Act
        member.IncrementActiveBorrowings();

        // Assert
        member.ActiveBorrowingsCount.Should().Be(initialCount + 1);
    }

    [Fact]
    public void DecrementActiveBorrowings_ShouldDecreaseCount_WhenCountIsGreaterThanZero()
    {
        // Arrange
        var member = CreateTestMember();
        member.IncrementActiveBorrowings(); // Make it 1
        var countBeforeDecrement = member.ActiveBorrowingsCount;

        // Act
        member.DecrementActiveBorrowings();

        // Assert
        member.ActiveBorrowingsCount.Should().Be(countBeforeDecrement - 1);
    }

    [Fact]
    public void DecrementActiveBorrowings_ShouldNotGoNegative_WhenCountIsZero()
    {
        // Arrange
        var member = CreateTestMember(); // Count is 0 by default

        // Act
        member.DecrementActiveBorrowings();

        // Assert
        member.ActiveBorrowingsCount.Should().Be(0);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse_WhenCalled()
    {
        // Arrange
        var member = CreateTestMember(); // IsActive is true by default

        // Act
        member.Deactivate();

        // Assert
        member.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue_WhenCalled()
    {
        // Arrange
        var member = CreateTestMember();
        member.Deactivate();

        // Act
        member.Activate();

        // Assert
        member.IsActive.Should().BeTrue();
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
