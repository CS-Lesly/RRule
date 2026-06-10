using System.Globalization;

namespace TemporalExpression.Parser.Components;

public class DateOnlyComponent : Component
{
    public override string DisplayName => "a valid date (yyyyMMdd)";

    public override Token? Parse(ref InputStream stream)
    {
        if (stream.ConsumeWhile(char.IsDigit, out string? stringValue))
        {
            if (DateOnly.TryParseExact(stringValue, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                return new(result);
            }
            else
            {
                stream.Fail($"Invalid date format: '{stringValue}'");
                return null;
            }
        }

        return null;
    }
}