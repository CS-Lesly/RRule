using System.Diagnostics.CodeAnalysis;

namespace TemporalExpression.Parser;

public abstract class GrammarToken<T> : ParsableExpression where T : new()
{
    public abstract Expression Syntax { get; }
    public virtual ParseResult OnParsed([DisallowNull]T node, List<object> children, ref TokenStream stream)
        => ParseResult.Ok(node);

    public sealed override ParseResult Parse(ref TokenStream stream)
    {
        int startPos = stream.Position;

        var node = new T();
        var children = new List<object>();
        var result = GrammarToken<T>.ExecuteMatch(Syntax, children, ref stream);
        if (result.Success)
        {
            return OnParsed(node, children, ref stream);
        }
        if (result.Error.HasValue)
        {
            return result;
        }

        stream.Seek(startPos);
        return new(); // soft mismatch
    }

    private static ParseResult ExecuteMatch(Expression expression, List<object> children, ref TokenStream stream) => expression switch
    {
        Terminal terminal => stream.MatchAndConsume(terminal.Value)
            ? ParseResult.Ok(terminal.Value)
            : ParseResult.Fail($"Expected terminal `{terminal.Value}`", stream),

        Choice   choice   => HandleChoice(  choice,   children, ref stream),
        Optional optional => HandleOptional(optional, children, ref stream),
        Repeat   repeat   => HandleRepeat(  repeat,   children, ref stream),
        Sequence sequence => HandleSequence(sequence, children, ref stream),

        ParsableExpression subExpression => HandleSubExpression(subExpression, children, ref stream),

        _ => ParseResult.Fail("Syntax error", stream),
    };

    private static ParseResult HandleChoice(Choice choice, List<object> children, ref TokenStream stream)
    {
        int startPosition = stream.Position;

        foreach (var option in choice.Options)
        {
            stream.Seek(startPosition);

            var result = ExecuteMatch(option, children, ref stream);
            if (result.Success || result.Error.HasValue)
            {
                return result;
            }
        }

        return ParseResult.Fail("Syntax error, no matching option found", stream);
    }

    private static ParseResult HandleOptional(Optional optional, List<object> children, ref TokenStream stream)
    {
        ExecuteMatch(optional.Expression, children, ref stream);

        return ParseResult.Ok(children);
    }

    private static ParseResult HandleRepeat(Repeat repeat, List<object> children, ref TokenStream stream)
    {
        int counter = 0;
        while (true)
        {
            int startPosition = stream.Position;

            var result = ExecuteMatch(repeat.Expression, children, ref stream);
            if (result.Success)
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

        return ParseResult.Ok(children);
    }

    private static ParseResult HandleSequence(Sequence sequence, List<object> children, ref TokenStream stream)
    {
        int startPosition = stream.Position;
        // We use a local bucket so we don't pollute the main one if the sequence fails
        var newChildren = new List<object>(); 

        for (int i = 0; i < sequence.Expressions.Length; i++) 
        {
            var result = ExecuteMatch(sequence.Expressions[i], newChildren, ref stream);
            if (!result.Success)
            {
                if (i == 0) 
                {
                    stream.Seek(startPosition);

                    return new(); // If the VERY FIRST element failed, it's just a soft mismatch
                }

                return result.Error.HasValue
                    ? result
                    : ParseResult.Fail($"Expected {sequence.Expressions[i]} at {stream.Position}", stream);
            }
        }

        children.AddRange(newChildren);
        return ParseResult.Ok(children);
    }

    private static ParseResult HandleSubExpression(ParsableExpression expression, List<object> children, ref TokenStream stream)
    {
        var result = expression.Parse(ref stream);
        if (result.Success
        &&  result.Value is not null)
        {
            children.Add(result.Value);
        }

        return result;
    }
}