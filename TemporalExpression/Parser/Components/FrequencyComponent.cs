namespace TemporalExpression.Parser.Components;

public class FrequencyComponent : Component
{
    public override Token? Parse(ref InputStream stream)
    {
        if (stream.ConsumeWhile(char.IsLetter, out string? stringValue))
        {
            foreach (Frequency frequency in Enum.GetValues<Frequency>())
            {
                if (frequency.ToString().Equals(stringValue, StringComparison.OrdinalIgnoreCase))
                {
                    return new(frequency);
                }
            }

            stream.Fail($"Invalid frequency: '{stringValue}'");
            return null;
        }

        return null;
    }
}