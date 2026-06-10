using static TemporalExpression.Parser.Literals;

namespace TemporalExpression.Parser.Components;

public class TemporalExpressionComponent : CompositeComponent
{
    public override Component Composition
        =>  "RRULE"  + new RecurrenceRuleComponent()
        + (NEWLINE +
          (("RRULE"  + new RecurrenceRuleComponent())
         | ("EXRULE" + new RecurrenceRuleComponent())
         | ("RDATE"  + new DateComponent())
         | ("EXDATE" + new DateComponent())
        )).ZeroOrMore();

    public override Token? OnParsed(IReadOnlyList<Token> tokens, ref InputStream stream) => new(
        new TemporalExpression
        {
            Inclusions = [
                .. tokens.GetResultsAfterLiteral<RecurrenceRule>(new LiteralComponent("RRULE")),
                .. tokens.GetResultsAfterLiteral<TemporalComponent>(new LiteralComponent("RDATE")),
            ],
            Exclusions = [
                .. tokens.GetResultsAfterLiteral<RecurrenceRule>(new LiteralComponent("EXRULE")),
                .. tokens.GetResultsAfterLiteral<TemporalComponent>(new LiteralComponent("EXDATE")),
            ],
        });
}