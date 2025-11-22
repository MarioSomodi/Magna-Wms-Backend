using FluentValidation;

namespace MagnaWms.Application.Users.Commands.UpdateUserActiveStatus;

public sealed class UpdateUserActiveStatusCommandValidator
    : AbstractValidator<UpdateUserActiveStatusCommand>
{
    public UpdateUserActiveStatusCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than zero.");
        RuleFor(x => x.IsActive)
        .NotNull()
        .WithMessage("Active status must be suppplied.");
    }
}
