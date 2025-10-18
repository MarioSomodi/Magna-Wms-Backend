namespace MagnaWms.Domain.LocationAggregate;

public static class LocationTypes
{
    public const string Rack = "RACK";
    public const string Bin = "BIN";
    public const string Floor = "FLOOR";
    public const string Stage = "STAGE";

    public static readonly string[] All = { Rack, Bin, Floor, Stage };
}
