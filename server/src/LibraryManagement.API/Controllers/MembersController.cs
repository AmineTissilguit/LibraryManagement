using LibraryManagement.API.Extensions;
using LibraryManagement.Application.Members.Commands.RegisterMember;
using LibraryManagement.Application.Members.Queries.GetMemberById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly ISender _sender;

        public MembersController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMemberById(int id)
        {
            var query = new GetMemberByIdQuery(id);
            var result = await _sender.Send(query);

            return result.Match(
                member => Ok(member),
                errors => errors.ToProblemDetails(this));
        }

        [HttpPost]
        public async Task<IActionResult> RegisterMember([FromBody] RegisterMemberCommand   command)
        {
            var result = await _sender.Send(command);

            return result.Match(
                member => CreatedAtAction(nameof(GetMemberById), new { id = member.Id },    member),
                errors => errors.ToProblemDetails(this));
        }
    }
}
