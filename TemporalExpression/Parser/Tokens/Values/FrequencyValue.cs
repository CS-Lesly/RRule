namespace TemporalExpression.Parser.Tokens.Values;

public class FrequencyValue : ParsableExpression
{
    public override ParseResult Parse(ref TokenStream stream)
    {
        // Peek at the key to decide which token to use
        int start = stream.Position;
        if (stream.MatchAndConsume("DAILY"))
        {
            return ParseResult.Parsed(Frequency.Daily);
        }
        if (stream.MatchAndConsume("WEEKLY"))
        {
            return ParseResult.Parsed(Frequency.Weekly);
        }
        if (stream.MatchAndConsume("MONTHLY"))
        {
            return ParseResult.Parsed(Frequency.Monthly);
        }
        if (stream.MatchAndConsume("YEARLY"))
        {
            return ParseResult.Parsed(Frequency.Yearly);
        }

        stream.Seek(start);

        return ParseResult.Fail("Unknown frequency", stream);
    }
}