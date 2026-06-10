namespace TemporalExpression.Parser;

public sealed class ChoiceComponent(params Component[] options) : Component
{
    public Component[] Options { get; init; } = [.. options.SelectMany(option => option is ChoiceComponent choice ? choice.Options : [option])];

    public override Token? Parse(ref InputStream stream)
    {
        var startPosition = stream.Position;

        foreach (var option in Options)
        {
            var token = option.Parse(ref stream);
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
        }

        stream.ErrorMessage = $"{DisplayName} > Expected one of the options";

        return null;
    }
}