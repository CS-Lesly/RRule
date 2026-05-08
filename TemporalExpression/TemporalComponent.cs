namespace TemporalExpression;

public abstract record TemporalComponent;

public record DateTimeComponent(DateTime DateTime) : TemporalComponent;

public record DateOnlyComponent(DateOnly Date) : TemporalComponent;