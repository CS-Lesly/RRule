using System.Text.RegularExpressions;
using static TemporalExpression.Parser.Terminal;

namespace TemporalExpression.Parser.Tokens.Values;

public partial class DurationValue : ParsableExpression
{
    public ExtendedTimeSpan? Value { get; private set; }

    [GeneratedRegex(@"^P(?:(?<Years>\d+)Y)?(?:(?<Months>\d+)M)?(?:(?<Weeks>\d+)W)?(?:(?<Days>\d+)D)?(?:T(?:(?<Hours>\d+)H)?(?:(?<Minutes>\d+)M)?)?$", RegexOptions.IgnoreCase)]
    private static partial Regex UnsignedDurationRegex();

    public override ParseResult Parse(ref TokenStream stream)
    {
            bool isNegative = false;
            if (stream.MatchAndConsume(PLUS))
            {
            }
            else if (stream.MatchAndConsume(MINUS))
            {
                isNegative = true;
            }

        if (!stream.ConsumeWhile(char.IsLetterOrDigit, out string? stringValue))
        {
            return ParseResult.Fail("Expected a duration value", stream);
        }

        var match = UnsignedDurationRegex().Match(stringValue!);
        if (!match.Success)
        {
            return ParseResult.Fail($"Invalid duration format: '{stringValue}'", stream);
        }

        Value = new ExtendedTimeSpan(isNegative)
            .WithYears(GetGroupValue(match, "Years"))
            .WithMonths(GetGroupValue(match, "Months"))
            .WithWeeks(GetGroupValue(match, "Weeks"))
            .WithDays(GetGroupValue(match, "Days"))
            .WithHours(GetGroupValue(match, "Hours"))
            .WithMinutes(GetGroupValue(match, "Minutes"));

        return ParseResult.Parsed(this);
    }

    private static int GetGroupValue(Match match, string groupName) 
        => match.Groups[groupName].Success
        ? int.Parse(match.Groups[groupName].Value)
        : 0;
}