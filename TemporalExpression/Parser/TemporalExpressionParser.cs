using TemporalExpression.Parser.Components;

namespace TemporalExpression.Parser;

public class TemporalExpressionParser
{
    public static TemporalExpression? Parse(string input, out string? errorMessage)
    {
        var stream = new InputStream(input.AsSpan());
        var token = new TemporalExpressionComponent().Parse(ref stream);
        if (token is not null)
        {
            errorMessage = null;
            return token.ValueAs<TemporalExpression>();
        }
        else
        {
            errorMessage = stream.ErrorMessage ?? string.Format("Unknown parsing error (Ln {line}, Col {column})", stream.GetLocation()); // TODO
            return null;
        }
    }
}