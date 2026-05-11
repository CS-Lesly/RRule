using System.Diagnostics.CodeAnalysis;
using TemporalExpression.Parser.Tokens.Values;
using static TemporalExpression.Parser.Terminal;

namespace TemporalExpression.Parser.Tokens;

public sealed class RRule  : RecurrenceRuleToken<RRule>  { protected override string Prefix => "RRULE";  }
public sealed class ExRule : RecurrenceRuleToken<ExRule> { protected override string Prefix => "EXRULE"; }

public abstract class RecurrenceRuleToken<T> : GrammarToken<T> where T : RecurrenceRuleToken<T>, new()
{
    protected abstract string Prefix { get; }

    public override Expression Syntax
        => Prefix + COLON + new RecurrenceRuleParameter() + (SEMICOLON + new RecurrenceRuleParameter()).ZeroOrMore();

    public RecurrenceRule? RecurrenceRule { get; private set; }

    public override ParseResult OnParsed([DisallowNull]T recurrenceRuleToken, List<object> tokens, ref TokenStream stream)
    {
        RecurrenceRule recurrenceRule;
        try
        {
            recurrenceRule = new RecurrenceRule
            {
                Frequency = tokens.OfType<FrequencyValue>().Single().Value ?? throw new InvalidOperationException("Missing required FREQ parameter in RRULE"),
            };

            if (tokens.OfType<IntervalValue>().FirstOrDefault() is var intervalValue && intervalValue?.Value is var interval && interval is not null)
            {
                recurrenceRule.Interval = (uint)interval;
            }
            if (tokens.OfType<ByWeekdaysValue>().SingleOrDefault() is var byWeekdayValue && byWeekdayValue?.Value is var byWeekday && byWeekday is not null)
            {
                recurrenceRule.ByWeekday = byWeekday;
            }
            if (tokens.OfType<IntegersValue>().SingleOrDefault(value => value.ParameterName == "BYMONTHDAY") is var byMonthdayValue && byMonthdayValue?.Value is var byMonthday && byMonthday is not null)
            {
                recurrenceRule.ByMonthday = byMonthday;
            }
            if (tokens.OfType<IntegersValue>().SingleOrDefault(value => value.ParameterName == "BYYEARDAY") is var byYeardayValue && byYeardayValue?.Value is var byYearday && byYearday is not null)
            {
                recurrenceRule.ByYearday = byYearday;
            }
            if (tokens.OfType<IntegersValue>().SingleOrDefault(value => value.ParameterName == "BYWEEKNO") is var byWeekNumberValue && byWeekNumberValue?.Value is var byWeekNumber && byWeekNumber is not null)
            {
                recurrenceRule.ByWeekNumber = byWeekNumber;
            }
            if (tokens.OfType<IntegersValue>().SingleOrDefault(value => value.ParameterName == "BYMONTH") is var byMonthValue && byMonthValue?.Value is var byMonth && byMonth is not null)
            {
                recurrenceRule.ByMonth = byMonth.Select(i => (uint)i).ToArray();
            }
            if (tokens.OfType<IntegersValue>().SingleOrDefault(value => value.ParameterName == "BYSETPOS") is var bySetPositionValue && bySetPositionValue?.Value is var bySetPosition && bySetPosition is not null)
            {
                recurrenceRule.BySetPosition = bySetPosition;
            }
            if (tokens.OfType<DayOfWeekValue>().SingleOrDefault() is var weekStartValue && weekStartValue?.Value is var weekStart && weekStart is not null)
            {
                recurrenceRule.WeekStart = (DayOfWeek)weekStart;
            }
            if (tokens.OfType<IntegersValue>().SingleOrDefault(value => value.ParameterName == "BYHOUR") is var byHourValue && byHourValue?.Value is var byHour && byHour is not null)
            {
                recurrenceRule.ByHour = [.. byHour.Select(i => (uint)i)]; ;
            }
            if (tokens.OfType<IntegersValue>().SingleOrDefault(value => value.ParameterName == "BYMINUTE") is var byMinuteValue && byMinuteValue?.Value is var byMinute && byMinute is not null)
            {
                recurrenceRule.ByMinute = [.. byMinute.Select(i => (uint)i)]; ;
            }
        }
        catch (InvalidOperationException)
        {
            return ParseResult.Fail("Missing required FREQ parameter in RRULE", stream);
        }

        recurrenceRuleToken.RecurrenceRule = recurrenceRule;
        return ParseResult.Parsed(recurrenceRuleToken);
    }
}