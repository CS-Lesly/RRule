namespace TemporalExpression.Parser.Components;

public class IntervalComponent : UnsignedIntegerComponent;

public class UnsignedIntegerComponent : Component
{
    public override Token? Parse(ref InputStream stream)
    {
        int startPosition = stream.Position;

        if (stream.ConsumeWhile(char.IsDigit, out string? stringValue))
        {
            if (uint.TryParse(stringValue, out var result))
            {
                return stream.Parsed(result, startPosition);
            }
            else
            {
                stream.Fail($"Invalid integer format: '{stringValue}'");
                return null;
            }
        }

        return null;
    }
}