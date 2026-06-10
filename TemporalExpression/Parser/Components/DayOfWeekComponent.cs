namespace TemporalExpression.Parser.Components;

public class DayOfWeekComponent : Component
{
    public override Token? Parse(ref InputStream stream)
    {
        int startPosition = stream.Position;

        if (stream.ConsumeWhile(char.IsLetter, out string? stringValue))
        {
            foreach (DayOfWeek dayOfWeek in Enum.GetValues<DayOfWeek>())
            {
                if (dayOfWeek.ToString()[..2].Equals(stringValue, StringComparison.OrdinalIgnoreCase))
                {
                    return stream.Parsed(result: dayOfWeek, startPosition);
                }
            }

            stream.Fail($"Invalid weekday: '{stringValue}'");
            return null;
        }

        return null;
    }
}