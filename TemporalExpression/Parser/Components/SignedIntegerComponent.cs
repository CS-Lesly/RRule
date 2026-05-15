using static TemporalExpression.Parser.Literal;

namespace TemporalExpression.Parser.Components;

// TODO
public class SignedIntegerComponent : ParsableComponent
{
    public override Token? Parse(ref InputStream stream)
    {
        int startPosition = stream.Position;

        int factor = 1;
        if (stream.MatchAndConsume(PLUS))
        {
        }
        else if (stream.MatchAndConsume(MINUS))
        {
            factor = -1;
        }

        if (stream.ConsumeWhile(char.IsDigit, out string? stringValue))
        {
            if (int.TryParse(stringValue, out var result))
            {
                return stream.Parsed(result * factor, startPosition);
            }
            else
            {
                stream.Fail($"Invalid integer format: '{stringValue}'");
                return null;
            }
        }

        // TODO? stream.Seek(startPosition);

        return null;
    }
}