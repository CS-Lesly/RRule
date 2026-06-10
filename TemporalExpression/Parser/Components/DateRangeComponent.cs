using static TemporalExpression.Parser.Literals;

namespace TemporalExpression.Parser.Components;

public class DateRangeComponent : CompositeComponent
{
    public override Component Composition
        => (DateTimeOrDate | new DurationComponent()) + SLASH
         + (DateTimeOrDate | new DurationComponent());

    private static Component DateTimeOrDate // a DateOnly is also the first part of a DateTime, therefore we must try to parse a DateTime first
        => new DateTimeComponent() | new DateOnlyComponent();

    public override Token? OnParsed(IReadOnlyList<Token> tokens, ref InputStream stream)
    {
        if (Composition is SequenceComponent sequence && sequence.Components.Length != tokens.Count)
        {
            stream.Fail("Invalid date-range format expected date, date-time or duration for begin and end of date-range are required");
            return null;
        }

        var result = new DateRange();
        switch (tokens[0].Value)
        {
            case DateTime dateTime:
                result.Start = dateTime;
                break;
            case DateOnly dateOnly:
                result.Start = dateOnly.ToDateTime(TimeOnly.MinValue);
                break;
            case ExtendedTimeSpan extendedTimeSpan:
                result.From = extendedTimeSpan;
                break;
            default:
                stream.Fail("Invalid date-range format expected date, date-time or duration for begin of date-range");
                return null;
        }
        switch (tokens[2].Value)
        {
            case DateTime dateTime:
                result.End = dateTime;
                break;
            case DateOnly dateOnly:
                result.End = dateOnly.ToDateTime(TimeOnly.MinValue);
                break;
            case ExtendedTimeSpan extendedTimeSpan:
                result.Until = extendedTimeSpan;
                break;
            default:
                stream.Fail("Invalid date-range format expected date, date-time or duration for end of date-range");
                return null;
        }

        return stream.Parsed(result, tokens);
    }
}