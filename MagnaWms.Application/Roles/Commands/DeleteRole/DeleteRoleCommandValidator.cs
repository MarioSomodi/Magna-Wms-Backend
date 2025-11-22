using FluentValidation;

namespace MagnaWms.Application.Roles.Commands.DeleteRole;
public class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
{
    public DeleteRoleCommandValidator() => RuleFor(x => x.RoleId)
            .GreaterThan(0)
            .WithMessage("Role ID must be greater than zero.");
}
