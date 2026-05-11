//using System.Globalization;

namespace TemporalExpression.Parser.Tokens.Values;

public class DurationValue : ParsableExpression
{
    public ExtendedTimeSpan? Value { get; private set; }

    public override ParseResult Parse(ref TokenStream stream)
    {
        // TODO

        return ParseResult.Fail("Expected a duration value", stream);
    }
}