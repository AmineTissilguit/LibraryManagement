using LibraryManagement.API.Extensions;
using LibraryManagement.Application.BorrowingTransactions.Commands.BorrowBook;
using LibraryManagement.Application.BorrowingTransactions.Commands.ReturnBook;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowingTransactionsController : ControllerBase
    {
        private readonly ISender _sender;

        public BorrowingTransactionsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("borrow")]
        public async Task<IActionResult> BorrowBook([FromBody] BorrowBookCommand command)
        {
            var result = await _sender.Send(command);

            return result.Match(
                transaction => Ok(new
                {
                    TransactionId = transaction.Id,
                    transaction.BookId,
                    transaction.MemberId,
                    transaction.BorrowDate,
                    transaction.DueDate,
                    Status = transaction.Status.ToString(),
                    Message = "Book borrowed successfully"
                }),
                errors => errors.ToProblemDetails(this));
        }

        [HttpPost("return/{transactionId}")]
        public async Task<IActionResult> ReturnBook(int transactionId)
        {
            var command = new ReturnBookCommand(transactionId);
            var result = await _sender.Send(command);

            return result.Match(
                transaction => Ok(new
                {
                    TransactionId = transaction.Id,
                    transaction.BookId,
                    transaction.MemberId,
                    transaction.BorrowDate,
                    transaction.DueDate,
                    transaction.ReturnDate,
                    Status = transaction.Status.ToString(),
                    transaction.FineAmount,
                    Message = transaction.FineAmount > 0
                        ? $"Book returned with fine: {transaction.FineAmount:C}"
                        : "Book returned successfully"
                }),
                errors => errors.ToProblemDetails(this));
        }
    }
}
