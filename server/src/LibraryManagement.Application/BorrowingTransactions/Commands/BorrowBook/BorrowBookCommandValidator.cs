using FluentValidation;

namespace LibraryManagement.Application.BorrowingTransactions.Commands.BorrowBook;

public class BorrowBookCommandValidator : AbstractValidator<BorrowBookCommand>
{
    public BorrowBookCommandValidator()
    {
        RuleFor(x => x.BookId)
            .GreaterThan(0).WithMessage("Book ID must be greater than 0");

        RuleFor(x => x.MemberId)
            .GreaterThan(0).WithMessage("Member ID must be greater than 0");
    }
}