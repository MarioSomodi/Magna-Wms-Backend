using FluentValidation;

namespace MagnaWms.Application.Putaway.Commands.CreatePutawayTask;

public sealed class CreatePutawayTaskCommandValidator : AbstractValidator<CreatePutawayTaskCommand>
{
    public CreatePutawayTaskCommandValidator()
    {
        RuleFor(x => x.ReceiptId).GreaterThan(0);
        RuleFor(x => x.ReceiptLineId).GreaterThan(0);
        RuleFor(x => x.QuantityToPutaway).GreaterThan(0);
    }
}
