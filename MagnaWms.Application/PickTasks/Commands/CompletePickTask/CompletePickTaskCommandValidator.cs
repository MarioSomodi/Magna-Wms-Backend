using FluentValidation;

namespace MagnaWms.Application.PickTasks.Commands.CompletePickTask;

public sealed class CompletePickTaskCommandValidator : AbstractValidator<CompletePickTaskCommand>
{
    public CompletePickTaskCommandValidator() => RuleFor(x => x.TaskId).GreaterThan(0);
}
