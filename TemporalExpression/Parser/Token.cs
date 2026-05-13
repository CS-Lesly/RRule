namespace TemporalExpression.Parser;

public class Token
{
    public string? Target { get; init; }

  // TODO public InputStream Stream { get; init; }
    public int StartPosition { get; init; }
    public int EndPosition { get; init; }

  // TODO public ReadOnlySpan<char> GetRawString() => Stream.Input[StartPosition..EndPosition];

    public required object Value { get; init; }
    public T ValueAs<T>() => (T)Value;

    public Token(InputStream stream, int startPosition)
    {
        StartPosition = startPosition;
        EndPosition = stream.Position;
    }
}