using System.Diagnostics.CodeAnalysis;
using TemporalExpression.Parser.Tokens.Values;
using static TemporalExpression.Parser.Terminal;

namespace TemporalExpression.Parser.Tokens;

public sealed class RDate  : DateToken<RDate>  { protected override string Prefix => "RDATE";  }
public sealed class ExDate : DateToken<ExDate> { protected override string Prefix => "EXDATE"; }

public abstract class DateToken<T> : GrammarToken<T> where T : DateToken<T>, new()
{
    protected abstract string Prefix { get; }

    public override Expression Syntax
        => Prefix +
        (((SEMICOLON + "VALUE" + EQUALS + "DATE-TIME").Optional() + COLON + new DateTimeValue()  + (COMMA + new DateTimeValue()).ZeroOrMore())
        | (SEMICOLON + "VALUE" + EQUALS + "DATE"                  + COLON + new DateOnlyValue()  + (COMMA + new DateOnlyValue()).ZeroOrMore())
         // Note that according to RFC 5545, Section 3.8.5.2, the PERIOD data type is specifically allowed for RDATE (Recurrence Date-Times) properties
        | (SEMICOLON + "VALUE" + EQUALS + "PERIOD"                + COLON + new DateRangeToken() + (COMMA + new DateRangeToken()).ZeroOrMore()));

    public List<TemporalComponent>? TemporalValues { get; private set; }

    public override ParseResult OnParsed([DisallowNull]T dateToken, List<object> tokens, ref TokenStream stream)
    {
        var temporalValues = new List<TemporalComponent>();
        foreach (var token in tokens)
        {
            switch (token)
            {
                case DateTime dateTime:
                    temporalValues.Add(new DateTimeComponent(dateTime));
                    break;
                case DateOnly dateOnly:
                    temporalValues.Add(new DateOnlyComponent(dateOnly));
                    break;
                case DateRange dateRange:
                    temporalValues.Add(dateRange);
                    break;
                default:
                    return ParseResult.Fail($"Unexpected subtoken type: {token.GetType().Name}", stream);
            }
        }   

        dateToken.TemporalValues = temporalValues;
        return ParseResult.Parsed(dateToken);
    }
}