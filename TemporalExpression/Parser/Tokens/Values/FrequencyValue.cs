namespace TemporalExpression.Parser.Tokens.Values;

public class FrequencyValue : ParsableExpression
{
    public Frequency? Value { get; private set; }

    public override ParseResult Parse(ref TokenStream stream)
    {
        if (stream.ConsumeWhile(char.IsLetter, out string? stringValue))
        {
            foreach (Frequency frequency in Enum.GetValues<Frequency>())
            {
                if (frequency.ToString().Equals(stringValue, StringComparison.OrdinalIgnoreCase))
                {
                    Value = frequency;
                    return ParseResult.Parsed(this);
                }
            }

            return ParseResult.Fail($"Invalid frequency: '{stringValue}'", stream);
        }

        return ParseResult.Fail("Expected a frequency value", stream);
    }
}