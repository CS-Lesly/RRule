namespace TemporalExpression.Parser;

public class Literal(string value) : Component, IEquatable<Literal>, IEquatable<string>
{
    public string Value { get; init; } = value;

    public override string DisplayName => $"`{Value}`";

    public static Literal COLON     => new(":");
    public static Literal SEMICOLON => new(";");
    public static Literal COMMA     => new(",");
    public static Literal EQUALS    => new("=");
    public static Literal NEWLINE   => new("\r\n");
    public static Literal PLUS      => new("+");
    public static Literal MINUS     => new("-");
    public static Literal SLASH     => new("/");

    public static bool operator ==(Literal? left, Literal? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return string.Equals(left.Value, right.Value, StringComparison.OrdinalIgnoreCase);
    }
    public static bool operator !=(Literal? left, Literal? right) => !(left == right);

    public static bool operator ==(Literal? left, string? right)
    {
        if (left is null)
        {
            return right is null;
        }

        return string.Equals(left.Value, right, StringComparison.OrdinalIgnoreCase);
    }
    public static bool operator !=(Literal? left, string? right) => !(left == right);

    public static bool operator ==(string? left, Literal? right) => right == left;
    public static bool operator !=(string? left, Literal? right) => !(right == left);

    public bool Equals(Literal? other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (other is null)
        {
            return false;
        }

        return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public bool Equals(string? other) => string.Equals(Value, other, StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object? obj) => obj switch
    {
        Literal otherLiteral => Equals(otherLiteral),
        string otherString   => Equals(otherString),
        _                    => false,
    };

    public override int GetHashCode() => Value?.ToUpperInvariant().GetHashCode() ?? 0;
}