namespace TemporalExpression.Parser.Tokens.Values;

public class FrequencyValue : ParsableExpression
{
    public Frequency? Value { get; private set; }

    public override ParseResult Parse(ref TokenStream stream)
    {
        int start = stream.Position;

        Frequency result;
        if (stream.MatchAndConsume("DAILY"))
        {
            result = Frequency.Daily;
        }
        else if (stream.MatchAndConsume("WEEKLY"))
        {
            result = Frequency.Weekly;
        }
        else if (stream.MatchAndConsume("MONTHLY"))
        {
            result = Frequency.Monthly;
        }
        // "QUARTERLY" | "SEMESTERLY" TODO
        else if (stream.MatchAndConsume("YEARLY"))
        {
            result = Frequency.Yearly;
        }
        else
        {
            stream.Seek(start);

            return ParseResult.Fail("Unknown frequency", stream);
        }

        Value = result;
        return ParseResult.Parsed(this);
    }
}