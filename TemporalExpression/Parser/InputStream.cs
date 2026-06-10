namespace TemporalExpression.Parser;

public ref struct InputStream
{
    private const char CR = '\r';
    private const char NL = '\n';
    private const char SPACE = ' ';
    private const char HTAB = '\t';

    public ReadOnlySpan<char> Input { get; init; }
    public int Position { get; private set; } = 0;

    public InputStream(ReadOnlySpan<char> input)
    {
        Input = input;
    }

    public readonly (int Line, int Column) GetLocation()
    {
        int line = 1, lastNewline = -1;
        for (int i = 0; i < Position; ++i)
        {
            if (Input[i] == NL)
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
        => ((position + 2 < Input.Length)
        && (Input[position] == CR)
        && (Input[position + 1] == NL)
        && (Input[position + 2] == SPACE || Input[position + 2] == HTAB))
        ? 3 // \r\n followed by space or tab is a fold, so skip all three characters
        : 0;

    public bool MatchAndConsume(LiteralComponent expected) => MatchAndConsume(expected.Value);

    public bool MatchAndConsume(string expected)
    {
        int currentPosition = Position;
        
        foreach (char c in expected)
        {
            if (currentPosition >= Input.Length || char.ToUpperInvariant(Input[currentPosition]) != char.ToUpperInvariant(c))
            {
                return false;
            }
            ++currentPosition;

            currentPosition = SkipFolds(currentPosition);
        }
        
        Position = currentPosition;
        
        return true;
    }

    public bool ConsumeWhile(Predicate<char> condition, out string? captured)
    {
        int currentPosition = Position;
        while (currentPosition < Input.Length && condition(Input[currentPosition]))
        {
            currentPosition = SkipFolds(++currentPosition);
        }

        if (currentPosition == Position)
        {
            captured = null;
            return false;
        }
        else
        {
            captured = Input[Position..currentPosition].ToString();
        }

        Position = currentPosition;

        return true;
    }

    public void Seek(int position) => Position = position;

    public string? ErrorMessage { get; set; }

    public void Fail(string errorMessage)
    {
        var (line, column) = GetLocation();

        ErrorMessage = $"{errorMessage} (Ln {line}, Col {column})";
    }
}