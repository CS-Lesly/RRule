using System.Globalization;

namespace TemporalExpression.Parser.Components;

public class DateOnlyComponent : ParsableComponent
{
    public override string DisplayName => "a valid date (yyyyMMdd)";

    public override Token? Parse(ref InputStream stream)
    {
        int startPosition = stream.Position;

        if (stream.ConsumeWhile(char.IsDigit, out string? stringValue))
        {
            if (DateOnly.TryParseExact(stringValue, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                return stream.Parsed(result, startPosition);
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