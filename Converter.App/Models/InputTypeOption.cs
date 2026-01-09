namespace Converter.App.Models;

public sealed class InputTypeOption
{
    public InputTypeOption(string display, InputType type)
    {
        Display = display;
        Type = type;
    }

    public string Display { get; }

    public InputType Type { get; }
}
