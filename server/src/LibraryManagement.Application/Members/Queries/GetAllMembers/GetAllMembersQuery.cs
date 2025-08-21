using ErrorOr;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.Members.Queries.Common;
using LibraryManagement.Domain.Entities;
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
                m.FullName,
                m.Email,
                m.Phone,
                m.Address,
                m.MembershipType,
                m.RegistrationDate,
                m.IsActive,
                m.ActiveBorrowingsCount,
                m.GetBorrowingLimit()
            ))
            .OrderBy(m => m.LastName)
            .ThenBy(m => m.FirstName)
            .ToListAsync(cancellationToken);

        return members;
    }
}