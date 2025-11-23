namespace MagnaWms.Contracts;

public sealed record ExecutePutawayRequest(
    decimal Quantity,
    long DestinationLocationId
);
