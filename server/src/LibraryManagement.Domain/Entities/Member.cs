using System;
using ErrorOr;
using LibraryManagement.Domain.Common;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Domain.Entities;

public class Member : AggregateRoot
{
    public string MembershipNumber { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }
    public string Address { get; private set; }
    public MembershipType MembershipType { get; private set; }
    public DateTime RegistrationDate { get; private set; }
    public bool IsActive { get; private set; }
    public int ActiveBorrowingsCount { get; private set; }

    private Member() { } // EF Core constructor

    public Member(string membershipNumber, string firstName, string lastName, 
                  string email, string phone, string address, MembershipType membershipType)
    {
        MembershipNumber = membershipNumber;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        Address = address;
        MembershipType = membershipType;
        RegistrationDate = DateTime.UtcNow;
        IsActive = true;
        ActiveBorrowingsCount = 0;
    }

    public string FullName => $"{FirstName} {LastName}";

    public int GetBorrowingLimit() => MembershipType switch
    {
        MembershipType.Student => 3,
        MembershipType.Adult => 5,
        MembershipType.Senior => 5,
        MembershipType.Staff => 10,
        _ => 5
    };

    public int GetLoanPeriodDays() => MembershipType switch
    {
        MembershipType.Student => 14,
        MembershipType.Adult => 21,
        MembershipType.Senior => 21,
        MembershipType.Staff => 30,
        _ => 21
    };

    public ErrorOr<Success> CanBorrow()
    {
        if (!IsActive)
            return Error.Forbidden("Member.NotActive", "Member account is not active");

        if (ActiveBorrowingsCount >= GetBorrowingLimit())
            return Error.Conflict("Member.BorrowingLimitExceeded", 
                                $"Member has reached borrowing limit of {GetBorrowingLimit()}");

        return Result.Success;
    }

    public void IncrementActiveBorrowings()
    {
        ActiveBorrowingsCount++;
    }

    public void DecrementActiveBorrowings()
    {
        if (ActiveBorrowingsCount > 0)
            ActiveBorrowingsCount--;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}