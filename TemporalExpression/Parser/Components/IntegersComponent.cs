using static TemporalExpression.Parser.Literal;

namespace TemporalExpression.Parser.Components;

public class IntegersComponent(string target, bool signed, uint minimum = uint.MinValue, uint maximum = uint.MaxValue) : ParsableComponent // TODO Tokenize?
{
    public string Target { get; init; } = target;
    public bool Signed { get; init; } = signed;
    public uint Minimum { get; init; } = minimum;
    public uint Maximum { get; init; } = maximum;

    public override Token? Parse(ref InputStream stream)
    {
        int startPosition = stream.Position;

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
                stream.Fail($"Unexpected sign for '{Target}'");
                return null;
            }

            var parsedUnsignedInteger = new UnsignedIntegerComponent().Parse(ref stream);
            if (parsedUnsignedInteger is null)
            {
                stream.Fail($"Expected an integer value for parameter '{Target}'");
                return null;
            }

            var unsignedInteger = parsedUnsignedInteger.ValueAs<int>();
            if (unsignedInteger < Minimum || Maximum < unsignedInteger)
            {
                stream.Fail($"Value {unsignedInteger} is out of range; it must be between {Minimum} and {Maximum}");
                return null;
            }

            result.Add(unsignedInteger * factor);
        }
        while (stream.MatchAndConsume(COMMA));

        return stream.Parsed(result.ToArray(), startPosition, Target);
    }
}