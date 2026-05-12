using System.Diagnostics.CodeAnalysis;
using TemporalExpression.Parser.Tokens.Values;
using static TemporalExpression.Parser.Terminal;

namespace TemporalExpression.Parser.Tokens;

public class DateRangeToken : GrammarToken<DateRangeToken>
{
    public override Expression Syntax
        => (new DateOnlyValue() | new DateTimeValue() | new DurationValue()) + SLASH
         + (new DateOnlyValue() | new DateTimeValue() | new DurationValue());

    public DateRange? Value { get; private set; }

    public override ParseResult OnParsed([DisallowNull]DateRangeToken dateRange, List<object> tokens, ref TokenStream stream)
    {
        var result = new DateRange();
        switch (tokens[0])
        {
            case DateOnly dateOnly:
                result.Start = dateOnly.ToDateTime(TimeOnly.MinValue);
                break;
            case DateTime dateTime:
                result.Start = dateTime;
                break;
            case ExtendedTimeSpan extendedTimeSpan:
                result.From = extendedTimeSpan;
                break;
            default:
                return ParseResult.Fail("Invalid date range format expected date-time or duration for begin of date range", stream);
        }
        switch (tokens[1])
        {
            case DateOnly dateOnly:
                result.End = dateOnly.ToDateTime(TimeOnly.MinValue);
                break;
            case DateTime dateTime:
                result.End = dateTime;
                break;
            case ExtendedTimeSpan extendedTimeSpan:
                result.Until = extendedTimeSpan;
                break;
            default:
                return ParseResult.Fail("Invalid date range format expected date-time or duration for end of date range", stream);
        }

        dateRange.Value = result;
        return ParseResult.Parsed(dateRange);
    }
}