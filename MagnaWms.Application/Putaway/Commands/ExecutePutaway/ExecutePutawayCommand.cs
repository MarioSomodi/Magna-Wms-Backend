using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts;
using MediatR;

namespace MagnaWms.Application.Putaway.Commands.ExecutePutaway;
public sealed record ExecutePutawayCommand(
    long TaskId,
    decimal Quantity,
    long DestinationLocationId
) : IRequest<Result<PutawayTaskDto>>;
