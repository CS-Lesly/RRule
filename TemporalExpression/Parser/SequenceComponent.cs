namespace TemporalExpression.Parser;

public class SequenceComponent(params Component[] components) : Component
{
    public Component[] Components { get; init; } = [.. components.SelectMany(component => component is SequenceComponent sequence ? sequence.Components : [component])];
}