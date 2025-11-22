using FluentValidation;

namespace MagnaWms.Application.Roles.Commands.CreateRole;
public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty()
            .WithMessage("Role name must be supplied.");
        RuleFor(x => x.PermissionKeys)
             .NotNull()
             .NotEmpty()
             .WithMessage("Cannot add role without permissions.");
    }
}
