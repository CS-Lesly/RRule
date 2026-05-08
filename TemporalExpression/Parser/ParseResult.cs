namespace TemporalExpression.Parser;

public readonly record struct ParseResult
{
    public bool IsParsed { get; init; } = false;

    public object? Value { get; init; }
    public ParseError? Error { get; init; }

    private ParseResult(bool isParsed = false, object? value = default, ParseError? error = null)
    {
        IsParsed = isParsed;
        Value = value;
        Error = error;
    }

    public static ParseResult Parsed(object value) => new(isParsed: true, value);
    public static ParseResult Fail(string parseError, TokenStream stream) => new(error: new ParseError(parseError, stream));

    public static ParseResult Continue => new(isParsed: true);
    public static ParseResult Retry    => new();
}