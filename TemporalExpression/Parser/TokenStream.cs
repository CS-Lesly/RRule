namespace TemporalExpression.Parser;

public ref struct TokenStream
{
    private const char CR = '\r';
    private const char NL = '\n';
    private const char SPACE = ' ';
    private const char HTAB = '\t';

    private readonly ReadOnlySpan<char> _input;
    public int Position { get; private set; } = 0;

    public TokenStream(ReadOnlySpan<char> input)
    {
        _input = input;
    }

    public readonly (int Line, int Column) GetLocation()
    {
        int line = 1, lastNewline = -1;
        for (int i = 0; i < Position; ++i)
        {
            if (_input[i] == NL)
            {
                ++line;
                lastNewline = i;
            }
        }
        return (line, Position - lastNewline);
    }

    private readonly int SkipFolds(int position)
    {
        int foldLength;
        while ((foldLength = GetFoldLength(position)) > 0)
        {
            position += foldLength;
        }
        return position;
    }

    private readonly int GetFoldLength(int position)
        => ((position + 2 < _input.Length)
        && (_input[position] == CR)
        && (_input[position + 1] == NL)
        && (_input[position + 2] == SPACE || _input[position + 2] == HTAB))
        ? 3 // \r\n followed by space or tab is a fold, so skip all three characters
        : 0;

    public bool MatchAndConsume(Terminal expected) => MatchAndConsume(expected.Value);

    public bool MatchAndConsume(string expected)
    {
        int temporaryPosition = Position;
        
        foreach (char c in expected)
        {
            if (temporaryPosition >= _input.Length || char.ToUpperInvariant(_input[temporaryPosition]) != char.ToUpperInvariant(c))
            {
                return false;
            }
            ++temporaryPosition;

            temporaryPosition = SkipFolds(temporaryPosition);
        }
        
        Position = temporaryPosition;
        
        return true;
    }

    public bool ConsumeWhile(Predicate<char> condition, out string? captured)
    {
        int temporaryPosition = Position;
        while (temporaryPosition < _input.Length && condition(_input[temporaryPosition]))
        {
            temporaryPosition = SkipFolds(temporaryPosition);
        }

        if (temporaryPosition == Position)
        {
            captured = null;
            return false;
        }
        else
        {
            captured = _input[Position..temporaryPosition].ToString();
        }

        Position = temporaryPosition;

        return true;
    }

    public void Seek(int position) => Position = position;
}