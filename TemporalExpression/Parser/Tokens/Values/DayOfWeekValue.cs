namespace TemporalExpression.Parser.Tokens.Values;

public class DayOfWeekValue : ParsableExpression
{
    public DayOfWeek? Value { get; private set; }
    
    public override ParseResult Parse(ref TokenStream stream)
    {
        if (stream.ConsumeWhile(char.IsLetter, out string? stringValue))
        {
            foreach (DayOfWeek dayOfWeek in Enum.GetValues<DayOfWeek>())
            {
                if (dayOfWeek.ToString()[..2].Equals(stringValue, StringComparison.OrdinalIgnoreCase))
                {
                    Value = dayOfWeek;
                    return ParseResult.Parsed(this);
                }
            }

            return ParseResult.Fail($"Invalid weekday: '{stringValue}'", stream);
        }

        return ParseResult.Fail("Expected a weekday value", stream);
    }
}