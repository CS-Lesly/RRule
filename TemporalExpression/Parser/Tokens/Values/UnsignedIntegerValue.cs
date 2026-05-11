namespace TemporalExpression.Parser.Tokens.Values;

public class IntervalValue : UnsignedIntegerValue;

public class UnsignedIntegerValue : ParsableExpression
{
    public uint? Value { get; private set; }

    public override ParseResult Parse(ref TokenStream stream)
    {
        if (stream.ConsumeWhile(char.IsDigit, out string? stringValue))
        {
            if (uint.TryParse(stringValue, out var result))
            {
                Value = result;
                return ParseResult.Parsed(this);
            }
            else
            {
                return ParseResult.Fail($"Invalid integer format: '{stringValue}'", stream);
            }
        }

        return ParseResult.Fail("Expected an integer value", stream);
    }
}