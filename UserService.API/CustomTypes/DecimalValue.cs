namespace UserService.API.CustomTypes;

public partial class DecimalValue
{
    private const decimal NanoFactor = 1_000_000_000;
    private long _units { get; set; }
    private long _nanos { get; set; }

    public DecimalValue(long units, int nanos)
    {
        _units = units;
        _nanos = nanos;
    }

    public static implicit operator decimal(DecimalValue grpcDecimal)
    {
        return grpcDecimal._units + grpcDecimal._nanos / NanoFactor;
    }

    public static implicit operator DecimalValue(decimal value)
    {
        var units = decimal.ToInt64(value);
        var nanos = decimal.ToInt32((value - units) * NanoFactor);
        return new DecimalValue(units, nanos);
    }
}