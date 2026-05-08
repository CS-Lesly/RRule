using System.Diagnostics.CodeAnalysis;

namespace TemporalExpression.Parser;

public abstract class GrammarToken<TToken> : ParsableExpression where TToken : GrammarToken<TToken>, new()
{
    public abstract Expression Syntax { get; }
    public abstract ParseResult OnParsed([DisallowNull]TToken node, List<object> tokens, ref TokenStream stream);

    public sealed override ParseResult Parse(ref TokenStream stream)
    {
        int startPos = stream.Position;

        var tokens = new List<object>();
        var result = GrammarToken<TToken>.ExecuteMatch(Syntax, tokens, ref stream);
        if (result.IsParsed)
        {
            return OnParsed(new TToken(), tokens, ref stream);
        }
        if (result.Error.HasValue)
        {
            return result;
        }

        stream.Seek(startPos);

        return ParseResult.Retry;
    }

    private static ParseResult ExecuteMatch(Expression expression, List<object> tokens, ref TokenStream stream) => expression switch
    {
        Terminal terminal => stream.MatchAndConsume(terminal.Value)
            ? ParseResult.Continue
            : ParseResult.Fail($"Expected terminal `{terminal.Value}`", stream),

        Choice   choice   => HandleChoice(  choice,   tokens, ref stream),
        Optional optional => HandleOptional(optional, tokens, ref stream),
        Repeat   repeat   => HandleRepeat(  repeat,   tokens, ref stream),
        Sequence sequence => HandleSequence(sequence, tokens, ref stream),

        ParsableExpression subExpression => HandleSubExpression(subExpression, tokens, ref stream),

        _ => ParseResult.Fail("Syntax error", stream),
    };

    private static ParseResult HandleChoice(Choice choice, List<object> tokens, ref TokenStream stream)
    {
        int startPosition = stream.Position;

        foreach (var option in choice.Options)
        {
            stream.Seek(startPosition);

            var result = ExecuteMatch(option, tokens, ref stream);
            if (result.IsParsed || result.Error.HasValue)
            {
                return result;
            }
        }

        return ParseResult.Fail("Syntax error, no matching option found", stream);
    }

    private static ParseResult HandleOptional(Optional optional, List<object> tokens, ref TokenStream stream)
    {
        var result = ExecuteMatch(optional.Expression, tokens, ref stream);
        if (result.IsParsed || result.Error.HasValue)
        {
            return result;
        }

        return ParseResult.Continue; // TODO Retry?
    }

    private static ParseResult HandleRepeat(Repeat repeat, List<object> tokens, ref TokenStream stream)
    {
        int counter = 0;
        while (true)
        {
            int startPosition = stream.Position;

            var result = ExecuteMatch(repeat.Expression, tokens, ref stream);
            if (result.IsParsed)
            {
                ++counter;
                continue;
            }

            // TODO DEBUG
            // If we hit a Hard Error (e.g., malformed syntax inside the repeat)
            // we must propagate it.
            if (result.Error.HasValue)
            {
                return result;
            }

            // If it's a Soft Mismatch, it means the "repeat" is done.
            // We break the loop and return a Success result.
            stream.Seek(startPosition); 
            break; 
        }

        if (counter < repeat.Minimum)
        {
            return ParseResult.Fail($"Expected at least {repeat.Minimum} repetitions, but got {counter}", stream);
        }
        else if (repeat.Maximum.HasValue
             && (repeat.Maximum.Value < counter))
        {
            return ParseResult.Fail($"Expected at most {repeat.Maximum.Value} repetitions, but got {counter}", stream);
        }

        return ParseResult.Continue;
    }

    private static ParseResult HandleSequence(Sequence sequence, List<object> tokens, ref TokenStream stream)
    {
        int startPosition = stream.Position;

        // We use a local bucket so we don't pollute the main one if the sequence fails
        var newTokens = new List<object>(); 

        for (int i = 0; i < sequence.Expressions.Length; i++) 
        {
            var result = ExecuteMatch(sequence.Expressions[i], newTokens, ref stream);
            if (!result.IsParsed)
            {
                if (i == 0) 
                {
                    stream.Seek(startPosition);

                    return ParseResult.Retry; // only if the VERY FIRST element failed
                }

                return result.Error.HasValue
                    ? result
                    : ParseResult.Fail($"Expected {sequence.Expressions[i]} at {stream.Position}", stream);
            }
        }

        tokens.AddRange(newTokens);

        return ParseResult.Continue;
    }

    private static ParseResult HandleSubExpression(ParsableExpression expression, List<object> tokens, ref TokenStream stream)
    {
        var result = expression.Parse(ref stream);
        if (result.IsParsed
        &&  result.Value is not null)
        {
            tokens.Add(result.Value);
        }

        return result;
    }
}