using FluentValidation;

namespace LibraryManagement.Application.Members.Commands.RegisterMember;

public class RegisterMemberCommandValidator : AbstractValidator<RegisterMemberCommand>
{
    public RegisterMemberCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone is required")
            .Matches(@"^(\+212|0)[567]\d{8}$").WithMessage("Invalid Moroccan phone number. Use format: +212612345678 or 0612345678");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(200).WithMessage("Address cannot exceed 200 characters");

        RuleFor(x => x.MembershipType)
            .IsInEnum().WithMessage("Invalid membership type");
    }
}
