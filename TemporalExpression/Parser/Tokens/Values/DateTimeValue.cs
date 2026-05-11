using System.Globalization;

namespace TemporalExpression.Parser.Tokens.Values;

public class DateTimeValue : ParsableExpression
{
    public DateTime? Value { get; private set; }

    public override ParseResult Parse(ref TokenStream stream)
    {
        if (stream.ConsumeWhile(char.IsLetterOrDigit, out string? stringValue))
        {
            if (DateTime.TryParseExact(stringValue, "yyyyMMddTHHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                Value = result;
                return ParseResult.Parsed(this);
            }
            else
            {
                return ParseResult.Fail($"Invalid date-time format: '{stringValue}'", stream);
            }
        }

        return ParseResult.Fail("Expected a date-time value", stream);
    }
}