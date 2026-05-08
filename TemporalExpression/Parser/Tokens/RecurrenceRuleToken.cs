using System.Diagnostics.CodeAnalysis;
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
                Frequency = tokens.OfType<Frequency>().Single()
        // TODO Interval  = tokens.OfType<IntervalData>().FirstOrDefault()?.Value ?? 1;
            };
        }
        catch (InvalidOperationException)
        {
            return ParseResult.Fail("Missing required FREQ parameter in RRULE", stream);
        }

        recurrenceRuleToken.RecurrenceRule = recurrenceRule;
        return ParseResult.Parsed(recurrenceRuleToken);
    }
}