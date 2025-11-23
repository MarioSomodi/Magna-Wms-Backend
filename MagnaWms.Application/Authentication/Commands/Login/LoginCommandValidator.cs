using FluentValidation;

namespace MagnaWms.Application.Authentication.Commands.Login;
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Password)
            .NotNull()
            .NotEmpty()
            .WithMessage("Password must be supplied.");
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Email must be supplied and valid.");
    }
}
