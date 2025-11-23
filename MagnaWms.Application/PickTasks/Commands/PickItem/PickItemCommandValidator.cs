using FluentValidation;

namespace MagnaWms.Application.PickTasks.Commands.PickItem;

public sealed class PickItemCommandValidator : AbstractValidator<PickItemCommand>
{
    public PickItemCommandValidator()
    {
        RuleFor(x => x.TaskId).GreaterThan(0);
        RuleFor(x => x.LineId).GreaterThan(0);
        RuleFor(x => x.QuantityPicked).GreaterThan(0);
    }
}
