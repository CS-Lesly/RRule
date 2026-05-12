using System.Diagnostics.CodeAnalysis;
using TemporalExpression.Parser.Tokens.Values;
using static TemporalExpression.Parser.Terminal;

namespace TemporalExpression.Parser.Tokens;

public class PeriodToken : GrammarToken<PeriodToken>
{
    public override Expression Syntax
        => (new DateTimeValue() | new DateOnlyValue() | new DurationValue()) + SLASH
         + (new DateTimeValue() | new DateOnlyValue() | new DurationValue());

    public DateRange? DateRange { get; private set; }

    public override ParseResult OnParsed([DisallowNull]PeriodToken periodToken, List<object> tokens, ref TokenStream stream)
    {
        var dateRange = new DateRange();
        // TODO

        periodToken.DateRange = dateRange;
        return ParseResult.Parsed(periodToken);
    }
}