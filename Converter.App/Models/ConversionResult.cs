namespace Converter.App.Models;

public sealed class ConversionResult
{
    public bool HasError { get; init; }

    public string ErrorMessage { get; init; } = string.Empty;

    public string DecimalValue { get; init; } = string.Empty;

    public string OriginalBinary { get; init; } = string.Empty;

    public string OriginalHex { get; init; } = string.Empty;

    public string OnesBinary { get; init; } = string.Empty;

    public string OnesHex { get; init; } = string.Empty;

    public string TwosBinary { get; init; } = string.Empty;

    public string TwosHex { get; init; } = string.Empty;
}
