namespace TemporalExpression;

public abstract record TemporalComponent;

public record DateTimeTemporalComponent(DateTime DateTime) : TemporalComponent;

public record DateOnlyTemporalComponent(DateOnly Date) : TemporalComponent;