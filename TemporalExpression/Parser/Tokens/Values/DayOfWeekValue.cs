namespace TemporalExpression.Parser.Tokens.Values;

public class DayOfWeekValue : ParsableExpression
{
    public DayOfWeek? Value { get; private set; }
    
    public override ParseResult Parse(ref TokenStream stream)
    {
        int start = stream.Position;

        DayOfWeek result;
        if (stream.MatchAndConsume("MO"))
        {
            result = DayOfWeek.Monday;
        }
        else if (stream.MatchAndConsume("TU"))
        {
            result = DayOfWeek.Tuesday;
        }
        else if (stream.MatchAndConsume("WE"))
        {
            result = DayOfWeek.Wednesday;
        }
        else if (stream.MatchAndConsume("TH"))
        {
            result = DayOfWeek.Thursday;
        }
        else if (stream.MatchAndConsume("FR"))
        {
            result = DayOfWeek.Friday;
        }
        else if (stream.MatchAndConsume("SA"))
        {
            result = DayOfWeek.Saturday;
        }
        else if (stream.MatchAndConsume("SU"))
        {
            result = DayOfWeek.Sunday;
        }
        else
        {
            stream.Seek(start);

            return ParseResult.Fail("Unknown weekday", stream);
        }

        Value = result;
        return ParseResult.Parsed(this);
    }
}