using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Items;
using MediatR;

namespace MagnaWms.Application.Items.Queries.GetAllItems;

public sealed record GetAllItemsQuery : IRequest<Result<IReadOnlyList<ItemDto>>>;
