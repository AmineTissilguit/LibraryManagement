using ErrorOr;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.Members.Queries.Common;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Members.Queries.GetAllMembers;

public record GetAllMembersQuery() : IRequest<ErrorOr<List<MemberDto>>>;

public class GetAllMembersQueryHandler : IRequestHandler<GetAllMembersQuery, ErrorOr<List<MemberDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllMembersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<List<MemberDto>>> Handle(GetAllMembersQuery request, CancellationToken cancellationToken)
    {
        var members = await _context.Members
        .AsNoTracking()
        .Where(m => m.IsActive)
        .Select(m => new MemberDto(
            m.Id,
            m.MembershipNumber,
            m.FirstName,
            m.LastName,
            m.FirstName + " " + m.LastName,
            m.Email,
            m.Phone,
            m.Address,
            m.MembershipType,
            m.RegistrationDate,
            m.IsActive,
            m.ActiveBorrowingsCount,
            m.MembershipType == MembershipType.Student ? 3 :
            m.MembershipType == MembershipType.Adult ? 5 :
            m.MembershipType == MembershipType.Senior ? 5 :
            m.MembershipType == MembershipType.Staff ? 10 : 5
            ))
        .ToListAsync(cancellationToken);

        return members;
    }
}