using static TemporalExpression.Parser.Terminal;

namespace TemporalExpression.Parser.Tokens.Values;

public class ByWeekdaysValue : ParsableExpression
{
    public (int, DayOfWeek)[]? Value { get; private set; }

    public override ParseResult Parse(ref TokenStream stream)
    {
        List<(int, DayOfWeek)> result = [];

        do
        {
            int factor = 1;
            if (stream.MatchAndConsume(PLUS))
            {
            }
            else if (stream.MatchAndConsume(MINUS))
            {
                factor = -1;
            }

            var parsedUnsignedInteger = new UnsignedIntegerValue().Parse(ref stream);
            if (parsedUnsignedInteger.Error.HasValue)
            {
                return parsedUnsignedInteger;
            }

            var unsignedInteger = parsedUnsignedInteger.IsParsed
                ? (int)parsedUnsignedInteger.Value!
                : 1;
            if (unsignedInteger < 1 || 53 < unsignedInteger)
            {
                return ParseResult.Fail($"Value {unsignedInteger} is out of range; it must be between 1 and 53", stream);
            }

            var parsedDayOfWeek = new DayOfWeekValue().Parse(ref stream);
            if (!parsedDayOfWeek.IsParsed)
            {
                return ParseResult.Fail($"Expected a weekday value for parameter 'BYDAY'", stream);
            }

            result.Add((unsignedInteger * factor, (DayOfWeek)parsedDayOfWeek.Value!));
        }
        while (stream.MatchAndConsume(COMMA));

        Value = [.. result];
        return ParseResult.Parsed(this);
    }
}