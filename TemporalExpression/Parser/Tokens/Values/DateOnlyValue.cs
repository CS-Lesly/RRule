using System.Globalization;

namespace TemporalExpression.Parser.Tokens.Values;

public class DateOnlyValue : ParsableExpression
{
    public DateOnly? Value { get; private set; }

    public override ParseResult Parse(ref TokenStream stream)
    {
        if (stream.ConsumeWhile(char.IsDigit, out string? stringValue))
        {
            if (DateOnly.TryParseExact(stringValue, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                Value = result;
                return ParseResult.Parsed(this);
            }
            else
            {
                return ParseResult.Fail($"Invalid date format: '{stringValue}'", stream);
            }
        }

        return ParseResult.Fail("Expected a date value", stream);
    }
}