namespace TemporalExpression.Parser;

public class RepetitionComponent(Component component, int minimumCount = 0, int? maximumCount = null) : Component
{
    public Component Component { get; init; } = component;

    public int MinimumCount { get; init; } = minimumCount;
    public int? MaximumCount { get; init; } = maximumCount;
}