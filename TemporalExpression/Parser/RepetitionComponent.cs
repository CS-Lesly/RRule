namespace TemporalExpression.Parser;

public sealed class RepetitionComponent(Component component, int minimumCount = 0, int? maximumCount = null) : Component
{
    public Component Component { get; init; } = component;

    public int MinimumCount { get; init; } = minimumCount;
    public int? MaximumCount { get; init; } = maximumCount;

    public override Token? Parse(ref InputStream stream)
    {
        var startPosition = stream.Position;

        var subTokens = new List<Token>();
        while (subTokens.Count < MaximumCount)
        {
            var currentPosition = stream.Position;

            var token = Component.Parse(ref stream);
            if (token is null)
            {
                if (stream.ErrorMessage is not null)
                {
                    stream.ErrorMessage = $"{DisplayName} > {stream.ErrorMessage}";
                    return null;
                }

                stream.Seek(currentPosition);
                break;
            }

            if (stream.Position == currentPosition) // Avoid infinite loops if the sub-token matches zero characters
            {
                subTokens.Add(token);
                break; 
            }

            subTokens.Add(token);
        }

        if (subTokens.Count < MinimumCount)
        {
            stream.Seek(startPosition);
            return null;
        }

        return new Token(subTokens);
    }
}