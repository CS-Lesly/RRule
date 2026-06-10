namespace TemporalExpression.Parser;

public sealed class LiteralComponent(string value) : Component, IEquatable<LiteralComponent>, IEquatable<string>
{
    public string Value { get; init; } = value;

    public override string DisplayName => $"`{Value}`";

    public override Token? Parse(ref InputStream stream)
    {
        if (stream.MatchAndConsume(Value))
        {
            return new(Value);
        }

        stream.Fail($"Expected literal `{Value}`");
        return null;
    }

    public static bool operator ==(LiteralComponent? left, LiteralComponent? right)
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
    public static bool operator !=(LiteralComponent? left, LiteralComponent? right) => !(left == right);

    public static bool operator ==(LiteralComponent? left, string? right)
    {
        if (left is null)
        {
            return right is null;
        }

        return string.Equals(left.Value, right, StringComparison.OrdinalIgnoreCase);
    }
    public static bool operator !=(LiteralComponent? left, string? right) => !(left == right);

    public static bool operator ==(string? left, LiteralComponent? right) => right == left;
    public static bool operator !=(string? left, LiteralComponent? right) => !(right == left);

    public bool Equals(LiteralComponent? other)
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
        LiteralComponent otherLiteral => Equals(otherLiteral),
        string otherString   => Equals(otherString),
        _                    => false,
    };

    public override int GetHashCode() => Value?.ToUpperInvariant().GetHashCode() ?? 0;
}