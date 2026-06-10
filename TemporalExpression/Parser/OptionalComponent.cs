namespace TemporalExpression.Parser;

public sealed class OptionalComponent(Component component) : Component
{
    public Component Component { get; init; } = component;

    public override Token? Parse(ref InputStream stream)
    {
        var startPosition = stream.Position;

        var token = Component.Parse(ref stream);
        if (token is not null)
        {
            return token;
        }

        if (stream.ErrorMessage is not null)
        {
            stream.ErrorMessage = $"{DisplayName} > {stream.ErrorMessage}";
            return null;
        }

        stream.Seek(startPosition);

        return Token.Empty();
    }
}