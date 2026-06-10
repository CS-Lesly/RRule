namespace TemporalExpression.Parser.Components;

public class FrequencyComponent : Component
{
    public override Token? Parse(ref InputStream stream)
    {
        int startPosition = stream.Position;

        if (stream.ConsumeWhile(char.IsLetter, out string? stringValue))
        {
            foreach (Frequency frequency in Enum.GetValues<Frequency>())
            {
                if (frequency.ToString().Equals(stringValue, StringComparison.OrdinalIgnoreCase))
                {
                    return stream.Parsed(result: frequency, startPosition);
                }
            }

            stream.Fail($"Invalid frequency: '{stringValue}'");
            return null;
        }

        return null;
    }
}