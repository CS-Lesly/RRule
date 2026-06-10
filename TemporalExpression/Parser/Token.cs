namespace TemporalExpression.Parser;

public sealed class Token
{
    // Concrete value (Only set on leaf nodes like Literal)
    public object? Value { get; init; }
    public T? ValueAs<T>() => (T?)Value;

    // Nested sub-tokens (Populated by structural nodes like Sequence/Repetition)
    public IReadOnlyList<Token> SubTokens { get; init; }

    // A fast convenience flag to determine if this node is a branch or a leaf
    public bool IsLeaf => SubTokens.Count == 0;

    // Constructor for Leaf Nodes (e.g., used by Literal)
    public Token(object value)
    {
        Value = value;
        SubTokens = [];
    }

    // Constructor for Structural Parent Nodes (e.g., used by Sequence and Repetition)
    public Token(IEnumerable<Token> subTokens)
    {
        Value = null;
        SubTokens = subTokens.ToList().AsReadOnly();
    }

    // Static factory for empty optional placeholders
    public static Token Empty() => new(string.Empty);
}