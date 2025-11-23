using FluentValidation;

namespace MagnaWms.Application.PickTasks.Queries.GetPickTask;
public sealed class GetPickTaskQueryValidator : AbstractValidator<GetPickTaskQuery>
{
    public GetPickTaskQueryValidator() => RuleFor(x => x.TaskId).GreaterThan(0);
}
