using FluentValidation;

namespace MagnaWms.Application.Warehouses.Command.CreateWarehouse;
public sealed class CreateWarehouseCommandValidator
    : AbstractValidator<CreateWarehouseCommand>
{
    public CreateWarehouseCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(32);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}
