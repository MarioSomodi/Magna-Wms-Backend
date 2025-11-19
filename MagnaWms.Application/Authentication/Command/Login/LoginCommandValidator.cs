using FluentValidation;
using MagnaWms.Application.Authentication.Command.Login;

namespace MagnaWms.Application.Authentication.Command.Login;
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
