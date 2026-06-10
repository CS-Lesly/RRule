namespace TemporalExpression.Parser;

public sealed class SequenceComponent(params Component[] components) : Component
{
    public Component[] Components { get; init; } = [.. components.SelectMany(component => component is SequenceComponent sequence ? sequence.Components : [component])];

    public override Token? Parse(ref InputStream stream)
    {
        var startPosition = stream.Position;

        var subTokens = new List<Token>();
        foreach (var component in Components)
        {
            var token = component.Parse(ref stream);
            if (token is null)
            {
                if (stream.ErrorMessage is not null)
                {
                    stream.ErrorMessage = $"{DisplayName} > {stream.ErrorMessage}";
                    return null;
                }

                stream.Seek(startPosition);
                return null;
            }

            subTokens.Add(token);
        }

        return new(subTokens);
    }
}