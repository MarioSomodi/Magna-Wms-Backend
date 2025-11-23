namespace MagnaWms.Contracts.Putaway;

public sealed record ExecutePutawayRequest(
    decimal Quantity,
    long DestinationLocationId
);
