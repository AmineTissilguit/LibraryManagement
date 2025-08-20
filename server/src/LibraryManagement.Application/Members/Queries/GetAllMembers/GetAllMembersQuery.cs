using ErrorOr;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Members.Queries.GetAllMembers;

public record GetAllMembersQuery() : IRequest<ErrorOr<List<Member>>>;

public class GetAllMembersQueryHandler : IRequestHandler<GetAllMembersQuery, ErrorOr<List<Member>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllMembersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<List<Member>>> Handle(GetAllMembersQuery request, CancellationToken cancellationToken)
    {
        var members = await _context.Members
            .AsNoTracking()
            .Where(m => m.IsActive)
            .OrderBy(m => m.LastName)
            .ThenBy(m => m.FirstName)
            .ToListAsync(cancellationToken);

        return members;
    }
}