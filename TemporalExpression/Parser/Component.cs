namespace TemporalExpression.Parser;

public abstract class Component(string? name = null)
{
    public string? Name { get; init; } = name;
    public virtual string DisplayName => GetType().Name;

    public abstract Token? Parse(ref InputStream stream);

    //public Component As(string name)
    //{ TODO
    //    Name = name;
    //    return this;
    //}

    public static implicit operator Component(char value)   => new LiteralComponent(value.ToString());
    public static implicit operator Component(string value) => new LiteralComponent(value);

    public static Component operator +(Component firstComponent, Component secondComponent) => new SequenceComponent(firstComponent, secondComponent);
    public static Component operator |(Component firstOption,    Component secondOption)    => new ChoiceComponent(  firstOption,    secondOption);

    public Component Optional() => new OptionalComponent(this);

    public Component ZeroOrMore() => new RepetitionComponent(this, minimumCount: 0);
    public Component OneOrMore()  => new RepetitionComponent(this, minimumCount: 1);
}