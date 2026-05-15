namespace TemporalExpression.Parser;

public abstract class Component(string? name = null)
{
    public string? Name { get; init; } = name;
    public virtual string DisplayName => GetType().Name;

    //public Component As(string name)
    //{ TODO
    //    Name = name;
    //    return this;
    //}

    public static implicit operator Component(char value)   => new Literal(value.ToString());
    public static implicit operator Component(string value) => new Literal(value);

    public static Component operator +(Component firstComponent, Component secondComponent) => new Sequence(firstComponent, secondComponent);
    public static Component operator |(Component firstOption,    Component secondOption)    => new Choice(  firstOption,    secondOption);

    public Component Optional() => new Optional(this);

    public Component ZeroOrMore() => new Repeat(this, minimum: 0);
    public Component OneOrMore()  => new Repeat(this, minimum: 1);
}

public class Sequence(params Component[] components) : Component
{
    public Component[] Components { get; init; } = [.. components.SelectMany(component => component is Sequence sequence ? sequence.Components : [component])];
}

public class Choice(params Component[] options) : Component
{
    public Component[] Options { get; init; } = [.. options.SelectMany(option => option is Choice choice ? choice.Options : [option])];
}

public class Optional(Component component) : Component
{
    public Component Component { get; init; } = component;
}

public class Repeat(Component component, int minimum = 0, int? maximum = null) : Component
{
    public Component Component { get; init; } = component;

    public int Minimum { get; init; } = minimum;
    public int? Maximum { get; init; } = maximum;
}