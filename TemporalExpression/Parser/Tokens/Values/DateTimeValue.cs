using System.Globalization;

namespace TemporalExpression.Parser.Tokens.Values;

public class DateTimeValue : ParsableExpression
{
    public override ParseResult Parse(ref TokenStream stream)
    {
        if (stream.ConsumeWhile(char.IsLetterOrDigit, out string? stringValue))
        {
            if (DateTime.TryParseExact(stringValue, "yyyyMMddTHHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                return ParseResult.Parsed(result);
            }
            else
            {
                return ParseResult.Fail($"Invalid date-time format: '{stringValue}'", stream);
            }
        }

        return ParseResult.Fail("Expected a date-time value", stream);
    }
}