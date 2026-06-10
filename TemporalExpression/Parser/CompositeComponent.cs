namespace TemporalExpression.Parser;

public abstract class CompositeComponent : Component
{
    public abstract Component Composition { get; }
    public abstract Token? OnParsed(IReadOnlyList<Token> tokens, ref InputStream stream);

    public sealed override Token? Parse(ref InputStream stream)
    {
        var startPosition = stream.Position;

        var result = Composition.Parse(ref stream);
        if (result is null)
        {
            if (stream.ErrorMessage is not null)
            {
                stream.ErrorMessage = $"{DisplayName} > {stream.ErrorMessage}";
            }
            else
            {
                stream.Seek(startPosition);
            }

            return null;
        }

        return OnParsed(result.SubTokens, ref stream);
    }
}