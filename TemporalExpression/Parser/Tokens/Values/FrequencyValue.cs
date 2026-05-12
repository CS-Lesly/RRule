namespace TemporalExpression.Parser.Tokens.Values;

public class FrequencyValue : ParsableExpression
{
    public Frequency? Value { get; private set; }

    public override ParseResult Parse(ref TokenStream stream)
    {
        int start = stream.Position;

        foreach (Frequency frequency in Enum.GetValues<Frequency>())
        {
            if (stream.MatchAndConsume(frequency.ToString()))
            {
                Value = frequency;
                return ParseResult.Parsed(this);
            }
        }

        stream.Seek(start);

        return ParseResult.Fail("Unknown frequency", stream);
    }
}