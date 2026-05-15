using static TemporalExpression.Parser.Literals;

namespace TemporalExpression.Parser.Components;

public class TemporalExpressionComponent : CompositeComponent
{
    public override Component Composition
        => new RRule() + (NEWLINE + (new RRule() | new ExRule() | new RDate() | new ExDate())).ZeroOrMore();

    public override Token? OnParsed(TokenList tokens, ref InputStream stream)
    {
        var result = new TemporalExpression
        {
            Inclusions = [
                .. tokens.Find<RecurrenceRule>("RRULE"),
                .. tokens.FindMany<TemporalComponent>("RDATE"),
            ],
            Exclusions = [
                .. tokens.Find<RecurrenceRule>("EXRULE"),
                .. tokens.FindMany<TemporalComponent>("EXDATE"),
            ],
        };

        return stream.Parsed(result, tokens);
    }
}