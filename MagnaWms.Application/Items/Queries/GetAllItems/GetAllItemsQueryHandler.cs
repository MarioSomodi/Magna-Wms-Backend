using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Items.Repository;
using MagnaWms.Contracts;
using MagnaWms.Contracts.Errors;
using MagnaWms.Domain.ItemAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Items.Queries.GetAllItems;

public sealed class GetAllItemsQueryHandler
    : IRequestHandler<GetAllItemsQuery, Result<IReadOnlyList<ItemDto>>>
{
    private readonly IItemRepository _itemRepository;
    private readonly IMapper _mapper;

    public GetAllItemsQueryHandler(IItemRepository itemRepository, IMapper mapper)
    {
        _itemRepository = itemRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<ItemDto>>> Handle(
        GetAllItemsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Item> items = await _itemRepository.GetAllAsync(cancellationToken);

        if (items.Count == 0)
        {
            return Result<IReadOnlyList<ItemDto>>.Failure(
                new Error(ErrorCode.NotFound, "No items found."));
        }

        IReadOnlyList<ItemDto> dtoList = _mapper.Map<IReadOnlyList<ItemDto>>(items);
        return Result<IReadOnlyList<ItemDto>>.Success(dtoList);
    }
}
