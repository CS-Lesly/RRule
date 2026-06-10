namespace TemporalExpression.Parser.Components;

public class IntervalComponent : UnsignedIntegerComponent;

public class UnsignedIntegerComponent : Component
{
    public override Token? Parse(ref InputStream stream)
    {
        if (stream.ConsumeWhile(char.IsDigit, out string? stringValue))
        {
            if (uint.TryParse(stringValue, out var result))
            {
                return new(result);
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