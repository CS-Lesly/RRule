namespace TemporalExpression.Parser.Tokens.Values;

public class DayOfWeekValue : ParsableExpression
{
    public DayOfWeek? Value { get; private set; }
    
    public override ParseResult Parse(ref TokenStream stream)
    {
        int start = stream.Position;

        foreach (DayOfWeek dayOfWeek in Enum.GetValues<DayOfWeek>())
        {
            if (stream.MatchAndConsume(dayOfWeek.ToString()[..2]))
            {
                Value = dayOfWeek;
                return ParseResult.Parsed(this);
            }
        }

        stream.Seek(start);

        return ParseResult.Fail("Unknown weekday", stream);
    }
}