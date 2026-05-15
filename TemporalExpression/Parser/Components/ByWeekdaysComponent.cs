using static TemporalExpression.Parser.Literal;

namespace TemporalExpression.Parser.Components;

public class ByWeekdaysComponent : ParsableComponent // TODO Tokenize?
{
    public override Token? Parse(ref InputStream stream)
    {
        int startPosition = stream.Position;

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

            var parsedUnsignedInteger = new UnsignedIntegerComponent().Parse(ref stream);
            if (stream.ErrorMessage is not null)
            {
                return null;
            }

            var unsignedInteger = parsedUnsignedInteger?.ValueAs<int>() ?? 1;
            if (unsignedInteger < 1 || 53 < unsignedInteger)
            {
                stream.Fail($"Value {unsignedInteger} is out of range; it must be between 1 and 53");
                return null;
            }

            var parsedDayOfWeek = new DayOfWeekComponent().Parse(ref stream);
            if (parsedDayOfWeek is null)
            {
                stream.Fail($"Expected a weekday value for parameter 'BYDAY'");
                return null;
            }

            result.Add((unsignedInteger * factor, (DayOfWeek)parsedDayOfWeek.Value));
        }
        while (stream.MatchAndConsume(COMMA));

        return stream.Parsed(result.ToArray(), startPosition);
    }
}