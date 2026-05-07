namespace TemporalExpression.Parser;

public readonly struct ParseError
{
    private string Message { get; init; }
    
    private int Position { get; init; }
    private int Line { get; init; }
    private int Column { get; init; }

    public ParseError(string message, TokenStream stream)
    {
        Message = message;
        Position = stream.Position;

        var location = stream.GetLocation();
        Line = location.Line;
        Column = location.Column;
    }   

    public override string ToString()
        => $"{Message} (Ln {Line}, Col {Column})";
}