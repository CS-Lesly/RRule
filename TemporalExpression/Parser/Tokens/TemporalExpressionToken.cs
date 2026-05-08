using System.Diagnostics.CodeAnalysis;
using static TemporalExpression.Parser.Terminal;

namespace TemporalExpression.Parser.Tokens;

public class TemporalExpressionToken : GrammarToken<TemporalExpressionToken>
{
    public override Expression Syntax
        => new RRule() + (NEWLINE + (new RRule() | new ExRule() | new RDate() | new ExDate())).ZeroOrMore();

    public TemporalExpression? TemporalExpression { get; private set; }

    public override ParseResult OnParsed([DisallowNull]TemporalExpressionToken temporalExpressionToken, List<object> tokens, ref TokenStream stream)
    {
        var temporalExpression = new TemporalExpression
        {
            Inclusions = [
                .. tokens.OfType<RRule>().Where(rrule => rrule.RecurrenceRule is not null).Select(rrule => rrule.RecurrenceRule!),
                .. tokens.OfType<RDate>().Where(rdate => rdate.TemporalValues is not null).SelectMany(rdate => rdate.TemporalValues!),
            ],
            Exclusions = [
                .. tokens.OfType<ExRule>().Where(exrule => exrule.RecurrenceRule is not null).Select(exrule => exrule.RecurrenceRule!),
                .. tokens.OfType<ExDate>().Where(exdate => exdate.TemporalValues is not null).SelectMany(exdate => exdate.TemporalValues!),
            ],
        };

        temporalExpressionToken.TemporalExpression = temporalExpression;
        return ParseResult.Parsed(temporalExpressionToken);
    }
}