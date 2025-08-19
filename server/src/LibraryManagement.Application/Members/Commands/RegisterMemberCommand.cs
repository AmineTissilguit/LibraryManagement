using ErrorOr;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Members.Commands;

public record RegisterMemberCommand(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string Address,
    MembershipType MembershipType
) : IRequest<ErrorOr<Member>>;

public class RegisterMemberCommandHandler : IRequestHandler<RegisterMemberCommand, ErrorOr<Member>>
{
    private readonly IApplicationDbContext _context;

    public RegisterMemberCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<Member>> Handle(RegisterMemberCommand request, CancellationToken cancellationToken)
    {
        var existingMember = await _context.Members
            .FirstOrDefaultAsync(m => m.Email == request.Email, cancellationToken);

        if (existingMember is not null)
        {
            return Error.Conflict("Member.EmailAlreadyExists", "A member with this email already exists");
        }

        var membershipNumber = await GenerateMembershipNumberAsync(cancellationToken);

        var member = new Member(
            membershipNumber,
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.Address,
            request.MembershipType);

        _context.Members.Add(member);
        await _context.SaveChangesAsync(cancellationToken);

        return member;
    }

    private async Task<string> GenerateMembershipNumberAsync(CancellationToken cancellationToken)
    {
        var year = DateTime.UtcNow.Year;
        var count = await _context.Members.CountAsync(cancellationToken) + 1;
        return $"MEM{year}{count:D4}";
    }
}