using TemporalExpression.Parser.Tokens;

namespace TemporalExpression.Parser;

public class TemporalExpressionParser
{
    public static TemporalExpression? Parse(string input, out ParseError? result)
    {
        var stream = new TokenStream(input.AsSpan());
        var parseResult = new TemporalExpressionToken().Parse(ref stream);
        if (parseResult.IsParsed)
        {
            result = null;
            return ((TemporalExpressionToken)parseResult.Value!).TemporalExpression;
        }
        else
        {
            result = parseResult.Error ?? new ParseError("Unknown parsing error", stream);
            return null;
        }
    }
}