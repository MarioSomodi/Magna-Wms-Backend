using FluentValidation;

namespace MagnaWms.Application.Warehouses.Command.DeactivateWarehouse;
public sealed class DeactivateWarehouseCommandValidator
    : AbstractValidator<DeactivateWarehouseCommand>
{
    public DeactivateWarehouseCommandValidator()
        => RuleFor(x => x.WarehouseId).GreaterThan(0);
}
