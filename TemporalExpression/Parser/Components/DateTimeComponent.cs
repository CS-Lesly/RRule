using System.Globalization;

namespace TemporalExpression.Parser.Components;

public class DateTimeComponent : Component
{
    public override string DisplayName => "a valid date-time (yyyyMMddTHHmm)";

    public override Token? Parse(ref InputStream stream)
    {
        int startPosition = stream.Position;

        if (stream.ConsumeWhile(char.IsLetterOrDigit, out string? stringValue))
        {
            if (DateTime.TryParseExact(stringValue, "yyyyMMddTHHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                return stream.Parsed(result, startPosition);
            }
            else
            {
                stream.Fail($"Invalid date-time format: '{stringValue}'");
                return null;
            }
        }

        return null;
    }
}