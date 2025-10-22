using FluentValidation;

namespace MagnaWms.Application.UnitOfMeasures.Queries.GetUnitOfMeasureById;
public class GetUnitOfMeasureByIdQueryValidator : AbstractValidator<GetUnitOfMeasureByIdQuery>
{
    public GetUnitOfMeasureByIdQueryValidator() => RuleFor(x => x.UnitOfMeasureId)
            .GreaterThan(0)
            .WithMessage("Warehouse ID must be greater than zero.");
}
