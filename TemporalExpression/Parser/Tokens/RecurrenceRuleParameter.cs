using TemporalExpression.Parser.Tokens.Values;

namespace TemporalExpression.Parser.Tokens;

public class RecurrenceRuleParameter : ParsableExpression
{
    public override ParseResult Parse(ref TokenStream stream)
    {
        // Peek at the key to decide which token to use
        int start = stream.Position;
        if (stream.MatchAndConsume("FREQ="))
        {
            return new FrequencyValue().Parse(ref stream);
        }
        if (stream.MatchAndConsume("INTERVAL="))
        {
            return new IntervalValue().Parse(ref stream);
        }
        if (stream.MatchAndConsume("BYDAY="))
        {
            return new ByWeekdaysValue().Parse(ref stream);
        }
        if (stream.MatchAndConsume("BYMONTHDAY="))
        {
            return new IntegersValue("BYMONTHDAY", signed: true, minimum: 1, maximum: 31).Parse(ref stream);
        }
        if (stream.MatchAndConsume("BYYEARDAY="))
        {
            return new IntegersValue("BYYEARDAY", signed: true, minimum: 1, maximum: 366).Parse(ref stream);
        }
        if (stream.MatchAndConsume("BYWEEKNO="))
        {
            return new IntegersValue("BYWEEKNO", signed: true, minimum: 1, maximum: 53).Parse(ref stream);
        }
        if (stream.MatchAndConsume("BYMONTH="))
        {
            return new IntegersValue("BYMONTH", signed: false, minimum: 1, maximum: 12).Parse(ref stream);
        }
        if (stream.MatchAndConsume("BYSETPOS="))
        {
            return new IntegersValue("BYSETPOS", signed: true, minimum: 1, maximum: 366).Parse(ref stream);
        }
        if (stream.MatchAndConsume("WKST="))
        {
            return new DayOfWeekValue().Parse(ref stream);
        }
        if (stream.MatchAndConsume("BYHOUR="))
        {
            return new IntegersValue("BYHOUR", signed: false, minimum: 0, maximum: 23).Parse(ref stream);
        }
        if (stream.MatchAndConsume("BYMINUTE="))
        {
            return new IntegersValue("BYMINUTE", signed: false, minimum: 0, maximum: 59).Parse(ref stream);
        }

        stream.Seek(start);

        return ParseResult.Fail("Expected a Rule parameter", stream);
    }
}