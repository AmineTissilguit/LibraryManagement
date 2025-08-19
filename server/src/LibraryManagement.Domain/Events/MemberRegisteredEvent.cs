using System;

namespace LibraryManagement.Domain.Events;

public class MemberRegisteredEvent : BaseDomainEvent
{
    public int MemberId { get; }
    public string MembershipNumber { get; }
    public string Email { get; }
    public string FullName { get; }
    public DateTime RegistrationDate { get; }

    public MemberRegisteredEvent(int memberId, string membershipNumber, 
                               string email, string fullName, DateTime registrationDate)
    {
        MemberId = memberId;
        MembershipNumber = membershipNumber;
        Email = email;
        FullName = fullName;
        RegistrationDate = registrationDate;
    }
}
