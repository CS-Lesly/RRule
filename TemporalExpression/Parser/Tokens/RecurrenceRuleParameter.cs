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

        stream.Seek(start);

        return ParseResult.Fail("Expected a Rule parameter", stream);
    }
}