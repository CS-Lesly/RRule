using static TemporalExpression.Parser.Literals;

namespace TemporalExpression.Parser.Components;

// TODO
public class SignedIntegerComponent : Component
{
    public override Token? Parse(ref InputStream stream)
    {
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
                return new(result * factor);
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