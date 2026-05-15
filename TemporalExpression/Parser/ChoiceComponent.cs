namespace TemporalExpression.Parser;

public class ChoiceComponent(params Component[] options) : Component
{
    public Component[] Options { get; init; } = [.. options.SelectMany(option => option is ChoiceComponent choice ? choice.Options : [option])];
}