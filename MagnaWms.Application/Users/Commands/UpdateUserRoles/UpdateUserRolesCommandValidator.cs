using FluentValidation;

namespace MagnaWms.Application.Users.Commands.UpdateUserRoles;

public class UpdateUserRolesCommandValidator : AbstractValidator<UpdateUserRolesCommand>
{
    public UpdateUserRolesCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than zero.");
        RuleFor(x => x.RoleIds)
             .NotNull()
             .NotEmpty()
             .WithMessage("Cannot remove all roles from user.");
    }
}
