using FluentValidation;

namespace MagnaWms.Application.Warehouses.Command.UpdateWarehouse;
public sealed class UpdateWarehouseCommandValidator
    : AbstractValidator<UpdateWarehouseCommand>
{
    public UpdateWarehouseCommandValidator()
    {
        RuleFor(x => x.WarehouseId)
            .GreaterThan(0);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}
