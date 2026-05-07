namespace TemporalExpression.Parser;

public abstract class Expression
{
    public static implicit operator Expression(char value) => new Terminal(value.ToString());
    public static implicit operator Expression(string value) => new Terminal(value);

    public static Expression operator +(Expression first, Expression second) => new Sequence(first, second);
    public static Expression operator |(Expression first, Expression second) => new Choice(first, second);

    public Expression Optional() => new Optional(this);

    public Expression ZeroOrMore() => new Repeat(this, minimum: 0);
    public Expression OneOrMore()  => new Repeat(this, minimum: 1);
}

public class Terminal(string value) : Expression
{
    public string Value { get; init; } = value;

    public static Terminal COLON     => new(":");
    public static Terminal SEMICOLON => new(";");
    public static Terminal COMMA     => new(",");
    public static Terminal EQUALS    => new("=");
    public static Terminal NEWLINE   => new("\r\n");
}

public class Sequence(params Expression[] expressions) : Expression
{
    public Expression[] Expressions { get; init; } = expressions;
}

public class Choice(params Expression[] options) : Expression
{
    public Expression[] Options { get; init; } = options;
}

public class Optional(Expression expression) : Expression
{
    public Expression Expression { get; init; } = expression;
}

public class Repeat(Expression expression, int minimum = 0, int? maximum = null) : Expression
{
    public Expression Expression { get; init; } = expression;

    public int Minimum { get; init; } = minimum;
    public int? Maximum { get; init; } = maximum;
}