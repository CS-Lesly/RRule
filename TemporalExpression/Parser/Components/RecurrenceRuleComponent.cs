using static TemporalExpression.Parser.Literals;

namespace TemporalExpression.Parser.Components;

public sealed class RRule  : RecurrenceRuleComponent { protected override string Prefix => "RRULE";  }
public sealed class ExRule : RecurrenceRuleComponent { protected override string Prefix => "EXRULE"; }

public abstract class RecurrenceRuleComponent : CompositeComponent
{
    protected abstract string Prefix { get; }

    public override Component Composition
        => Prefix + COLON + Parameter + (SEMICOLON + Parameter).ZeroOrMore();

    private static Component Parameter
        => ("FREQ"       + EQUALS + new FrequencyComponent())
         | ("INTERVAL"   + EQUALS + new IntervalComponent())
         | ("BYDAY"      + EQUALS + new ByWeekdaysComponent())
         | ("BYMONTHDAY" + EQUALS + new IntegersComponent("BYMONTHDAY", signed: true,  minimum: 1, maximum:  31))
         | ("BYYEARDAY"  + EQUALS + new IntegersComponent("BYYEARDAY",  signed: true,  minimum: 1, maximum: 366))
         | ("BYWEEKNO"   + EQUALS + new IntegersComponent("BYWEEKNO",   signed: true,  minimum: 1, maximum:  53))
         | ("BYMONTH"    + EQUALS + new IntegersComponent("BYMONTH",    signed: false, minimum: 1, maximum:  12))
         | ("BYSETPOS"   + EQUALS + new IntegersComponent("BYSETPOS",   signed: true,  minimum: 1, maximum: 366))
         | ("WKST"       + EQUALS + new DayOfWeekComponent())
         | ("BYHOUR"     + EQUALS + new IntegersComponent("BYHOUR",     signed: false, minimum: 0, maximum:  23))
         | ("BYMINUTE"   + EQUALS + new IntegersComponent("BYMINUTE",   signed: false, minimum: 0, maximum:  59))
         ;

    public override Token? OnParsed(TokenList tokens, ref InputStream stream)
    {
        RecurrenceRule result;
        try
        {
            result = new RecurrenceRule
            {
                Frequency = tokens.Single(token => token.Value is Frequency).ValueAs<Frequency>(),
            };
 /* TODO
             if (tokens.OfType<IntervalComponent>().FirstOrDefault() is var intervalValue && intervalValue?.Value is var interval && interval is not null)
            {
                result.Interval = (uint)interval;
            }
            if (tokens.OfType<ByWeekdaysComponent>().SingleOrDefault() is var byWeekdayValue && byWeekdayValue?.Value is var byWeekday && byWeekday is not null)
            {
                result.ByWeekday = byWeekday;
            }
            if (tokens.OfType<IntegersComponent>().SingleOrDefault(value => value.ParameterName == "BYMONTHDAY") is var byMonthdayValue && byMonthdayValue?.Value is var byMonthday && byMonthday is not null)
            {
                result.ByMonthday = byMonthday;
            }
            if (tokens.OfType<IntegersComponent>().SingleOrDefault(value => value.ParameterName == "BYYEARDAY") is var byYeardayValue && byYeardayValue?.Value is var byYearday && byYearday is not null)
            {
                result.ByYearday = byYearday;
            }
            if (tokens.OfType<IntegersComponent>().SingleOrDefault(value => value.ParameterName == "BYWEEKNO") is var byWeekNumberValue && byWeekNumberValue?.Value is var byWeekNumber && byWeekNumber is not null)
            {
                result.ByWeekNumber = byWeekNumber;
            }
            if (tokens.OfType<IntegersComponent>().SingleOrDefault(value => value.ParameterName == "BYMONTH") is var byMonthValue && byMonthValue?.Value is var byMonth && byMonth is not null)
            {
                result.ByMonth = byMonth.Select(i => (uint)i).ToArray();
            }
            if (tokens.OfType<IntegersComponent>().SingleOrDefault(value => value.ParameterName == "BYSETPOS") is var bySetPositionValue && bySetPositionValue?.Value is var bySetPosition && bySetPosition is not null)
            {
                result.BySetPosition = bySetPosition;
            }
            if (tokens.OfType<DayOfWeekComponent>().SingleOrDefault() is var weekStartValue && weekStartValue?.Value is var weekStart && weekStart is not null)
            {
                result.WeekStart = (DayOfWeek)weekStart;
            }
            if (tokens.OfType<IntegersComponent>().SingleOrDefault(value => value.ParameterName == "BYHOUR") is var byHourValue && byHourValue?.Value is var byHour && byHour is not null)
            {
                result.ByHour = [.. byHour.Select(i => (uint)i)]; ;
            }
            if (tokens.OfType<IntegersComponent>().SingleOrDefault(value => value.ParameterName == "BYMINUTE") is var byMinuteValue && byMinuteValue?.Value is var byMinute && byMinute is not null)
            {
                result.ByMinute = [.. byMinute.Select(i => (uint)i)]; ;
            }
        */
        }
        catch (InvalidOperationException)
        {
            stream.Fail("Missing required FREQ parameter in RRULE");
            return null;
        }

        return stream.Parsed(result, tokens, target: Prefix);
    }
}