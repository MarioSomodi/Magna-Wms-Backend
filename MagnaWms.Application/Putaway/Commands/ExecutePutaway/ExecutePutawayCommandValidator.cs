using FluentValidation;

namespace MagnaWms.Application.Putaway.Commands.ExecutePutaway;
public sealed class ExecutePutawayCommandValidator : AbstractValidator<ExecutePutawayCommand>
{
    public ExecutePutawayCommandValidator()
    {
        RuleFor(x => x.TaskId).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.DestinationLocationId).GreaterThan(0);
    }
}
