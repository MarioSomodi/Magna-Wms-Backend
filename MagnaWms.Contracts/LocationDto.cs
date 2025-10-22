﻿namespace MagnaWms.Contracts;
public sealed record LocationDto(
    long LocationId,
    long WarehouseId,
    string Code,
    string Type,
    int MaxQty,
    bool IsActive,
    DateTime CreatedUtc,
    DateTime UpdatedUtc);
