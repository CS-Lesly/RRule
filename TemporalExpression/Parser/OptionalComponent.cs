namespace TemporalExpression.Parser;

public class OptionalComponent(Component component) : Component
{
    public Component Component { get; init; } = component;
}