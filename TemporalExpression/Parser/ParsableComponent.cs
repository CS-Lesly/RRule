namespace TemporalExpression.Parser;

public abstract class ParsableComponent : Component
{
    public abstract Token? Parse(ref InputStream stream);
}