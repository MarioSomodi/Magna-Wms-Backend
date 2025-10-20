using MagnaWms.Domain.Core.Exceptions;
using MagnaWms.Domain.Core.Primitives;

namespace MagnaWms.Domain.UnitOfMeasureAggregate;

public sealed class UnitOfMeasure : AggregateRoot
{
    private UnitOfMeasure() { }

    public UnitOfMeasure(string symbol, string name)
    {
        Symbol = string.IsNullOrWhiteSpace(symbol)
            ? throw new DomainException("UOM symbol cannot be empty.")
            : symbol.Trim().ToLowerInvariant();

        Name = string.IsNullOrWhiteSpace(name)
            ? throw new DomainException("UOM name cannot be empty.")
            : name.Trim();
    }

    public string Symbol { get; private set; } = default!;
    public string Name { get; private set; } = default!;
}
