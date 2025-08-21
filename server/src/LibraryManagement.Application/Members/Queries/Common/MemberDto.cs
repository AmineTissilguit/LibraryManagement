using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.Members.Queries.Common;

public record MemberDto(
    int Id,
    string MembershipNumber,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string Phone,
    string Address,
    MembershipType MembershipType,
    DateTime RegistrationDate,
    bool IsActive,
    int ActiveBorrowingsCount,
    int BorrowingLimit
);
