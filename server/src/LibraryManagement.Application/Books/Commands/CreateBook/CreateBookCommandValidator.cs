using FluentValidation;

namespace LibraryManagement.Application.Books.Commands.CreateBook;

public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.Isbn)
            .NotEmpty().WithMessage("ISBN is required")
            .Matches(@"^[\d-]+$").WithMessage("ISBN should only contain numbers and dashes")
            .Length(10, 17).WithMessage("ISBN must be between 10 and 17 characters");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("Author is required")
            .MaximumLength(100).WithMessage("Author cannot exceed 100 characters");

        RuleFor(x => x.Publisher)
            .NotEmpty().WithMessage("Publisher is required")
            .MaximumLength(100).WithMessage("Publisher cannot exceed 100 characters");

        RuleFor(x => x.PublicationYear)
            .GreaterThan(1000).WithMessage("Publication year must be greater than 1000")
            .LessThanOrEqualTo(DateTime.UtcNow.Year).WithMessage("Publication year cannot be in the future");

        RuleFor(x => x.Genre)
            .NotEmpty().WithMessage("Genre is required")
            .MaximumLength(50).WithMessage("Genre cannot exceed 50 characters");

        RuleFor(x => x.TotalCopies)
            .GreaterThan(0).WithMessage("Total copies must be greater than 0")
            .LessThanOrEqualTo(1000).WithMessage("Total copies cannot exceed 1000");
    }
}