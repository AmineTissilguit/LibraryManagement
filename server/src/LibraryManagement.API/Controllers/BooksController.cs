using LibraryManagement.API.Extensions;
using LibraryManagement.Application.Books.Commands.CreateBook;
using LibraryManagement.Application.Books.Queries.GetAllBooks;
using LibraryManagement.Application.Books.Queries.GetBookById;
using LibraryManagement.Application.Books.Queries.SearchBooks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ISender _sender;

        public BooksController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var query = new GetAllBooksQuery();
            var result = await _sender.Send(query);

            return result.Match(
                books => Ok(books),
                errors => errors.ToProblemDetails(this));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var query = new GetBookByIdQuery(id);
            var result = await _sender.Send(query);

            return result.Match(
                book => Ok(book),
                errors => errors.ToProblemDetails(this));
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchBooks([FromQuery] string searchTerm)
        {
            var query = new SearchBooksQuery(searchTerm);
            var result = await _sender.Send(query);

            return result.Match(
                books => Ok(books),
                errors => errors.ToProblemDetails(this));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] CreateBookCommand command)
        {
            var result = await _sender.Send(command);

            return result.Match(
                book => CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book),
                errors => errors.ToProblemDetails(this));
        }
    }
}
