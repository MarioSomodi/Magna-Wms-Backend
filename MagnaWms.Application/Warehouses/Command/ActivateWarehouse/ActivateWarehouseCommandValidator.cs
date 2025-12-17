using FluentValidation;

namespace MagnaWms.Application.Warehouses.Command.ActivateWarehouse;
public sealed class ActivateWarehouseCommandValidator
    : AbstractValidator<ActivateWarehouseCommand>
{
    public ActivateWarehouseCommandValidator()
        => RuleFor(x => x.WarehouseId).GreaterThan(0);
}
