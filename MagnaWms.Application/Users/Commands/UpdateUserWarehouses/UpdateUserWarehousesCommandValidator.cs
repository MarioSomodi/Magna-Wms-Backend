using FluentValidation;

namespace MagnaWms.Application.Users.Commands.UpdateUserWarehouses;
public class UpdateUserWarehousesCommandValidator : AbstractValidator<UpdateUserWarehousesCommand>
{
    public UpdateUserWarehousesCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than zero.");
        RuleFor(x => x.WarehouseIds)
             .NotNull()
             .NotEmpty()
             .WithMessage("Cannot remove all warehouses from user.");
    }
}
