namespace TemporalExpression.Parser;

public readonly record struct ParseResult
{
    public bool Success { get; init; } = false;

    public object? Value { get; init; }
    public ParseError? Error { get; init; }

    private ParseResult(bool success, object? value = null, ParseError? error = null)
    {
        Success = success;
        Value = value;
        Error = error;
    }

    public static ParseResult Ok(object value) => new(success: true, value);
    public static ParseResult Fail(string parseError, TokenStream stream) => new(success: false, error: new ParseError(parseError, stream));

    public T ValueAs<T>() => (T)Value!;

    public bool ValueIs<T>() => Value is T;
}