using static TemporalExpression.Parser.Terminal;

namespace TemporalExpression.Parser.Components;

public sealed class RDate  : DateComponent { protected override string Prefix => "RDATE";  }
public sealed class ExDate : DateComponent { protected override string Prefix => "EXDATE"; }

public abstract class DateComponent : CompositeComponent
{
    protected abstract string Prefix { get; }

    public override Component Composition
        => Prefix +
        (((SEMICOLON + "VALUE" + EQUALS + "DATE-TIME").Optional() + COLON + new DateTimeComponent()  + (COMMA + new DateTimeComponent()).ZeroOrMore())
        | (SEMICOLON + "VALUE" + EQUALS + "DATE"                  + COLON + new DateOnlyComponent()  + (COMMA + new DateOnlyComponent()).ZeroOrMore())
         // Note that according to RFC 5545, Section 3.8.5.2, the PERIOD data type is specifically allowed for RDATE (Recurrence Date-Times) properties
        | (SEMICOLON + "VALUE" + EQUALS + "PERIOD"                + COLON + new DateRangeComponent() + (COMMA + new DateRangeComponent()).ZeroOrMore()));

    public override Token? OnParsed(TokenList tokens, ref InputStream stream)
    {
        var result = new List<TemporalComponent>();
        foreach (var token in tokens)
        {
            switch (token.Value)
            {
                case DateTime dateTime:
                    result.Add(new DateTimeTemporalComponent(dateTime));
                    break;
                case DateOnly dateOnly:
                    result.Add(new DateOnlyTemporalComponent(dateOnly));
                    break;
                case DateRange dateRange:
                    result.Add(dateRange);
                    break;
                default:
                    stream.Fail($"Unexpected subtoken type: {token.GetType().Name}");
                    return null;
            }
        }   

        return stream.Parsed(result, tokens, target: Prefix);
    }
}