using System.Diagnostics.CodeAnalysis;
using TemporalExpression.Parser.Tokens.Values;
using static TemporalExpression.Parser.Terminal;

namespace TemporalExpression.Parser.Tokens;

public class DateRangeToken : GrammarToken<DateRangeToken>
{
    public override Expression Syntax
        => (DateTimeOrDate | new DurationValue()) + SLASH
         + (DateTimeOrDate | new DurationValue());

    private static Expression DateTimeOrDate // a DateOnly is also the first part of a DateTime, therefore we must try to parse a DateTime first
        => new DateTimeValue() | new DateOnlyValue();

    public DateRange? Value { get; private set; }

    public override ParseResult OnParsed([DisallowNull]DateRangeToken dateRange, List<object> tokens, ref TokenStream stream)
    {
        if (tokens.Count != 2)
        {
            return ParseResult.Fail("Invalid date-range format expected date, date-time or duration for begin and end of date-range are required", stream);
        }

        var result = new DateRange();
        switch (tokens[0])
        {
            case DateTimeValue dateTime:
                result.Start = dateTime.Value;
                break;
            case DateOnlyValue dateOnly:
                result.Start = dateOnly.Value!.Value.ToDateTime(TimeOnly.MinValue);
                break;
            case DurationValue extendedTimeSpan:
                result.From = extendedTimeSpan.Value;
                break;
            default:
                return ParseResult.Fail("Invalid date-range format expected date, date-time or duration for begin of date-range", stream);
        }
        switch (tokens[1])
        {
            case DateTimeValue dateTime:
                result.End = dateTime.Value;
                break;
            case DateOnlyValue dateOnly:
                result.End = dateOnly.Value!.Value.ToDateTime(TimeOnly.MinValue);
                break;
            case DurationValue extendedTimeSpan:
                result.Until = extendedTimeSpan.Value;
                break;
            default:
                return ParseResult.Fail("Invalid date-range format expected date, date-time or duration for end of date-range", stream);
        }

        dateRange.Value = result;
        return ParseResult.Parsed(dateRange);
    }
}