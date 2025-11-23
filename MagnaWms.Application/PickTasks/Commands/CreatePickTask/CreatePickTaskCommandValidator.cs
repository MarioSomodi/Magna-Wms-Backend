using FluentValidation;

namespace MagnaWms.Application.PickTasks.Commands.CreatePickTask;

public sealed class CreatePickTaskCommandValidator : AbstractValidator<CreatePickTaskCommand>
{
    public CreatePickTaskCommandValidator() => RuleFor(x => x.SalesOrderId).GreaterThan(0);
}
