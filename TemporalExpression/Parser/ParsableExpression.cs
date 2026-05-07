namespace TemporalExpression.Parser;

public abstract class ParsableExpression : Expression
{
    public abstract ParseResult Parse(ref TokenStream stream);
}