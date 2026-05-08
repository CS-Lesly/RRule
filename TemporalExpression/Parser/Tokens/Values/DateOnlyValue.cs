using System.Globalization;

namespace TemporalExpression.Parser.Tokens.Values;

public class DateOnlyValue : ParsableExpression
{
    public override ParseResult Parse(ref TokenStream stream)
    {
        if (stream.ConsumeWhile(char.IsLetterOrDigit, out string? stringValue))
        {
            if (DateOnly.TryParseExact(stringValue, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                return ParseResult.Parsed(result);
            }
            else
            {
                return ParseResult.Fail($"Invalid date format: '{stringValue}'", stream);
            }
        }

        return ParseResult.Fail("Expected a date value", stream);
    }
}