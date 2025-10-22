﻿namespace MagnaWms.Contracts;

public sealed record WarehouseDto(
    long Id,
    string Code,
    string Name,
    string TimeZone,
    bool IsActive,
    DateTime CreatedUtc,
    DateTime UpdatedUtc);
