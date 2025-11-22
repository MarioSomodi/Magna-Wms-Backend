using FluentValidation;

namespace MagnaWms.Application.Roles.Commands.UpdateRolePermissions;
public class UpdateRolePermissionsCommandValidator : AbstractValidator<UpdateRolePermissionsCommand>
{
    public UpdateRolePermissionsCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .GreaterThan(0)
            .WithMessage("Role ID must be greater than zero.");
        RuleFor(x => x.PermissionKeys)
             .NotNull()
             .NotEmpty()
             .WithMessage("Cannot remove all permissions from role.");
    }
}
