namespace TemporalExpression.Parser.Components;

public class DayOfWeekComponent : Component
{
    public override Token? Parse(ref InputStream stream)
    {
        if (stream.ConsumeWhile(char.IsLetter, out string? stringValue))
        {
            foreach (DayOfWeek dayOfWeek in Enum.GetValues<DayOfWeek>())
            {
                if (dayOfWeek.ToString()[..2].Equals(stringValue, StringComparison.OrdinalIgnoreCase))
                {
                    return new(dayOfWeek);
                }
            }

            stream.Fail($"Invalid weekday: '{stringValue}'");
            return null;
        }

        return null;
    }
}