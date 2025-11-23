using FluentValidation;

namespace MagnaWms.Application.PickTasks.Commands.StartPickTask;

public sealed class StartPickTaskCommandValidator : AbstractValidator<StartPickTaskCommand>
{
    public StartPickTaskCommandValidator() => RuleFor(x => x.TaskId).GreaterThan(0);
}
