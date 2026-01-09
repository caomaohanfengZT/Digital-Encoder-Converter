using System.Globalization;
using System.Numerics;
using System.Text;
using Converter.App.Models;

namespace Converter.App.Services;

public sealed class EncodingConverter
{
    public ConversionResult Convert(string input, InputType inputType)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return Error("请输入需要转换的内容。");
        }

        string trimmed = input.Trim();
        if (!TryParseInput(trimmed, inputType, out BigInteger value, out int width, out string error))
        {
            return Error(error);
        }

        if (!TryFormatSignMagnitude(value, width, out string originalBits, out error))
        {
            return Error(error);
        }

        if (!TryFormatOnesComplement(value, width, out string onesBits, out error))
        {
            return Error(error);
        }

        if (!TryFormatTwosComplement(value, width, out string twosBits, out error))
        {
            return Error(error);
        }

        return new ConversionResult
        {
            DecimalValue = value.ToString(CultureInfo.InvariantCulture),
            OriginalBinary = originalBits,
            OriginalHex = ToHex(originalBits),
            OnesBinary = onesBits,
            OnesHex = ToHex(onesBits),
            TwosBinary = twosBits,
            TwosHex = ToHex(twosBits)
        };
    }

    private static ConversionResult Error(string message)
    {
        return new ConversionResult
        {
            HasError = true,
            ErrorMessage = message
        };
    }

    private static bool TryParseInput(string input, InputType inputType, out BigInteger value, out int width, out string error)
    {
        value = BigInteger.Zero;
        width = 0;
        error = string.Empty;

        switch (inputType)
        {
            case InputType.OriginalDecimal:
                if (!BigInteger.TryParse(input, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value))
                {
                    error = "十进制输入无效。";
                    return false;
                }

                width = GetWidthForDecimal(value);
                return true;
            case InputType.OriginalBinary:
                if (!TryParseBinary(input, out string originalBits, out error))
                {
                    return false;
                }

                width = originalBits.Length;
                value = ParseSignMagnitude(originalBits);
                return true;
            case InputType.OriginalHex:
                if (!TryParseHex(input, out string originalHexBits, out error))
                {
                    return false;
                }

                width = originalHexBits.Length;
                value = ParseSignMagnitude(originalHexBits);
                return true;
            case InputType.OnesBinary:
                if (!TryParseBinary(input, out string onesBits, out error))
                {
                    return false;
                }

                width = onesBits.Length;
                value = ParseOnesComplement(onesBits);
                return true;
            case InputType.OnesHex:
                if (!TryParseHex(input, out string onesHexBits, out error))
                {
                    return false;
                }

                width = onesHexBits.Length;
                value = ParseOnesComplement(onesHexBits);
                return true;
            case InputType.TwosBinary:
                if (!TryParseBinary(input, out string twosBits, out error))
                {
                    return false;
                }

                width = twosBits.Length;
                value = ParseTwosComplement(twosBits);
                return true;
            case InputType.TwosHex:
                if (!TryParseHex(input, out string twosHexBits, out error))
                {
                    return false;
                }

                width = twosHexBits.Length;
                value = ParseTwosComplement(twosHexBits);
                return true;
            default:
                error = "不支持的输入类型。";
                return false;
        }
    }

    private static bool TryParseBinary(string input, out string bits, out string error)
    {
        bits = string.Empty;
        error = string.Empty;

        if (input.Length == 0)
        {
            error = "二进制输入为空。";
            return false;
        }

        foreach (char c in input)
        {
            if (c is not ('0' or '1'))
            {
                error = "二进制输入只能包含 0 或 1。";
                return false;
            }
        }

        bits = input;
        return true;
    }

    private static bool TryParseHex(string input, out string bits, out string error)
    {
        bits = string.Empty;
        error = string.Empty;

        if (input.Length == 0)
        {
            error = "十六进制输入为空。";
            return false;
        }

        var builder = new StringBuilder(input.Length * 4);
        foreach (char c in input)
        {
            int value = HexValue(c);
            if (value < 0)
            {
                error = "十六进制输入只能包含 0-9 或 A-F。";
                return false;
            }

            builder.Append(System.Convert.ToString(value, 2).PadLeft(4, '0'));
        }

        bits = builder.ToString();
        return true;
    }

    private static int HexValue(char c)
    {
        if (c is >= '0' and <= '9')
        {
            return c - '0';
        }

        if (c is >= 'A' and <= 'F')
        {
            return c - 'A' + 10;
        }

        if (c is >= 'a' and <= 'f')
        {
            return c - 'a' + 10;
        }

        return -1;
    }

    private static BigInteger ParseSignMagnitude(string bits)
    {
        if (bits.Length == 0)
        {
            return BigInteger.Zero;
        }

        bool negative = bits[0] == '1';
        string magnitudeBits = bits.Substring(1);
        BigInteger magnitude = ParseBinary(magnitudeBits);
        if (magnitude.IsZero)
        {
            return BigInteger.Zero;
        }

        return negative ? -magnitude : magnitude;
    }

    private static BigInteger ParseOnesComplement(string bits)
    {
        if (bits.Length == 0)
        {
            return BigInteger.Zero;
        }

        if (bits[0] == '0')
        {
            return ParseBinary(bits);
        }

        string inverted = InvertBits(bits);
        BigInteger magnitude = ParseBinary(inverted);
        if (magnitude.IsZero)
        {
            return BigInteger.Zero;
        }

        return -magnitude;
    }

    private static BigInteger ParseTwosComplement(string bits)
    {
        if (bits.Length == 0)
        {
            return BigInteger.Zero;
        }

        if (bits[0] == '0')
        {
            return ParseBinary(bits);
        }

        string inverted = InvertBits(bits);
        BigInteger magnitude = ParseBinary(inverted) + BigInteger.One;
        if (magnitude.IsZero)
        {
            return BigInteger.Zero;
        }

        return -magnitude;
    }

    private static bool TryFormatSignMagnitude(BigInteger value, int width, out string bits, out string error)
    {
        bits = string.Empty;
        error = string.Empty;

        if (width < 1)
        {
            error = "位宽必须至少为 1。";
            return false;
        }

        BigInteger magnitude = BigInteger.Abs(value);
        if (GetMagnitudeBitLength(magnitude) > width - 1)
        {
            error = $"原码位宽为 {width} 时数值超出范围。";
            return false;
        }

        string magnitudeBits = magnitude.IsZero
            ? new string('0', width - 1)
            : ToBinary(magnitude).PadLeft(width - 1, '0');

        char signBit = value.Sign < 0 ? '1' : '0';
        bits = signBit + magnitudeBits;
        return true;
    }

    private static bool TryFormatOnesComplement(BigInteger value, int width, out string bits, out string error)
    {
        bits = string.Empty;
        error = string.Empty;

        BigInteger magnitude = BigInteger.Abs(value);
        if (GetMagnitudeBitLength(magnitude) > width - 1)
        {
            error = $"反码位宽为 {width} 时数值超出范围。";
            return false;
        }

        string magnitudeBits = magnitude.IsZero
            ? new string('0', width - 1)
            : ToBinary(magnitude).PadLeft(width - 1, '0');
        string positiveBits = "0" + magnitudeBits;

        if (value.Sign < 0)
        {
            bits = InvertBits(positiveBits);
        }
        else
        {
            bits = positiveBits;
        }

        return true;
    }

    private static bool TryFormatTwosComplement(BigInteger value, int width, out string bits, out string error)
    {
        bits = string.Empty;
        error = string.Empty;

        if (!IsInTwosRange(value, width))
        {
            error = $"补码位宽为 {width} 时数值超出范围。";
            return false;
        }

        if (value.Sign >= 0)
        {
            bits = ToBinary(value).PadLeft(width, '0');
            return true;
        }

        BigInteger modulus = BigInteger.One << width;
        BigInteger unsignedValue = modulus + value;
        bits = ToBinary(unsignedValue).PadLeft(width, '0');
        return true;
    }

    private static bool IsInTwosRange(BigInteger value, int width)
    {
        BigInteger min = -(BigInteger.One << (width - 1));
        BigInteger max = (BigInteger.One << (width - 1)) - BigInteger.One;
        return value >= min && value <= max;
    }

    private static int GetWidthForDecimal(BigInteger value)
    {
        BigInteger magnitude = BigInteger.Abs(value);
        int magBits = GetBitLength(magnitude);
        int minWidth = 1 + magBits;
        int width = RoundUpToMultipleOf4(Math.Max(minWidth, 4));

        while (!IsInTwosRange(value, width))
        {
            width += 4;
        }

        return width;
    }

    private static int RoundUpToMultipleOf4(int width)
    {
        return ((width + 3) / 4) * 4;
    }

    private static int GetBitLength(BigInteger value)
    {
        if (value.Sign < 0)
        {
            value = BigInteger.Abs(value);
        }

        if (value.IsZero)
        {
            return 1;
        }

        return ToBinary(value).Length;
    }

    private static int GetMagnitudeBitLength(BigInteger magnitude)
    {
        return magnitude.IsZero ? 0 : GetBitLength(magnitude);
    }

    private static BigInteger ParseBinary(string bits)
    {
        BigInteger value = BigInteger.Zero;
        foreach (char c in bits)
        {
            value <<= 1;
            if (c == '1')
            {
                value += BigInteger.One;
            }
        }

        return value;
    }

    private static string ToBinary(BigInteger value)
    {
        if (value.IsZero)
        {
            return "0";
        }

        var builder = new StringBuilder();
        BigInteger remaining = value;
        while (remaining > 0)
        {
            builder.Insert(0, (remaining & BigInteger.One) == BigInteger.One ? '1' : '0');
            remaining >>= 1;
        }

        return builder.ToString();
    }

    private static string InvertBits(string bits)
    {
        var builder = new StringBuilder(bits.Length);
        foreach (char c in bits)
        {
            builder.Append(c == '1' ? '0' : '1');
        }

        return builder.ToString();
    }

    private static string ToHex(string bits)
    {
        string padded = PadToNibble(bits);
        var builder = new StringBuilder(padded.Length / 4);
        for (int i = 0; i < padded.Length; i += 4)
        {
            string nibble = padded.Substring(i, 4);
            int value = System.Convert.ToInt32(nibble, 2);
            builder.Append(value.ToString("X"));
        }

        return builder.ToString();
    }

    private static string PadToNibble(string bits)
    {
        if (bits.Length % 4 == 0)
        {
            return bits;
        }

        int target = ((bits.Length + 3) / 4) * 4;
        char pad = bits.Length > 0 && bits[0] == '1' ? '1' : '0';
        return bits.PadLeft(target, pad);
    }
}
