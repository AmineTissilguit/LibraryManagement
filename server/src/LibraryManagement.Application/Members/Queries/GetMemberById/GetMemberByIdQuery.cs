using ErrorOr;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Members.Queries.GetMemberById;

public record GetMemberByIdQuery(int Id) : IRequest<ErrorOr<Member>>;

public class GetMemberByIdQueryHandler : IRequestHandler<GetMemberByIdQuery, ErrorOr<Member>>
{
    private readonly IApplicationDbContext _context;

    public GetMemberByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<Member>> Handle(GetMemberByIdQuery request, CancellationToken cancellationToken)
    {
        var member = await _context.Members
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (member is null)
        {
            return Error.NotFound("Member.NotFound", $"Member with ID {request.Id} was not found");
        }

        return member;
    }
}