using static TemporalExpression.Parser.Terminal;

namespace TemporalExpression.Parser.Tokens.Values;

public class IntegersValue(string parameterName, bool signed, uint minimum = uint.MinValue, uint maximum = uint.MaxValue) : ParsableExpression
{
    public string ParameterName { get; init; } = parameterName;
    public bool Signed { get; init; } = signed;
    public uint Minimum { get; init; } = minimum;
    public uint Maximum { get; init; } = maximum;

    public int[]? Value { get; private set; }

    public override ParseResult Parse(ref TokenStream stream)
    {
        List<int> result = [];

        do
        {
            int factor = 1;
            if (Signed)
            {
                if (stream.MatchAndConsume(PLUS))
                {
                }
                else if (stream.MatchAndConsume(MINUS))
                {
                    factor = -1;
                }
            }
            else if (stream.MatchAndConsume(PLUS) || stream.MatchAndConsume(MINUS))
            {
                return ParseResult.Fail($"Unexpected sign for '{ParameterName}'", stream);
            }

            var parsedUnsignedInteger = new UnsignedIntegerValue().Parse(ref stream);
            if (!parsedUnsignedInteger.IsParsed)
            {
                return ParseResult.Fail($"Expected an integer value for parameter '{ParameterName}'", stream);
            }

            var unsignedInteger = (int)parsedUnsignedInteger.Value!;
            if (unsignedInteger < Minimum || Maximum < unsignedInteger)
            {
                return ParseResult.Fail($"Value {unsignedInteger} is out of range; it must be between {Minimum} and {Maximum}", stream);
            }

            result.Add(unsignedInteger * factor);
        }
        while (stream.MatchAndConsume(COMMA));

        Value = [.. result];
        return ParseResult.Parsed(this);
    }
}