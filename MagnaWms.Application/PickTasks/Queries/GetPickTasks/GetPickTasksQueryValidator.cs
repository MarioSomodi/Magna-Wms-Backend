using FluentValidation;

namespace MagnaWms.Application.PickTasks.Queries.GetPickTasks;
public sealed class GetPickTasksQueryValidator : AbstractValidator<GetPickTasksQuery>
{
    public GetPickTasksQueryValidator() => RuleFor(x => x.WarehouseId).GreaterThan(0);
}
