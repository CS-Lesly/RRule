using System.Text.RegularExpressions;
using static TemporalExpression.Parser.Literals;

namespace TemporalExpression.Parser.Components;

public partial class DurationComponent : ParsableComponent // ExtendedTimeSpanComponent?
{
    public override string DisplayName => "a duration";

    [GeneratedRegex(@"^P(?:(?<Years>\d+)Y)?(?:(?<Months>\d+)M)?(?:(?<Weeks>\d+)W)?(?:(?<Days>\d+)D)?(?:T(?:(?<Hours>\d+)H)?(?:(?<Minutes>\d+)M)?)?$", RegexOptions.IgnoreCase)]
    private static partial Regex UnsignedDurationRegex();

    public override Token? Parse(ref InputStream stream)
    {
        int startPosition = stream.Position;

        if (stream.MatchAndConsume(PLUS))
        {
        }
        else if (stream.MatchAndConsume(MINUS))
        {
            stream.Fail($"Unexpected minus-sign for duration");
            return null;
        }

        if (!stream.ConsumeWhile(char.IsLetterOrDigit, out string? stringValue))
        {
            stream.Fail("Expected a duration value");
            return null;
        }

        var match = UnsignedDurationRegex().Match(stringValue!);
        if (!match.Success)
        {
            stream.Fail($"Invalid duration format: '{stringValue}'");
            return null;
        }

        return stream.Parsed(result: new ExtendedTimeSpan()
            .WithYears(GetGroupValue(match, "Years"))
            .WithMonths(GetGroupValue(match, "Months"))
            .WithWeeks(GetGroupValue(match, "Weeks"))
            .WithDays(GetGroupValue(match, "Days"))
            .WithHours(GetGroupValue(match, "Hours"))
            .WithMinutes(GetGroupValue(match, "Minutes"))
            , startPosition);
    }

    private static int GetGroupValue(Match match, string groupName) 
        => match.Groups[groupName].Success
        ? int.Parse(match.Groups[groupName].Value)
        : 0;
}