namespace TemporalExpression.Parser;

public abstract class CompositeComponent : ParsableComponent
{
    public abstract Component Composition { get; }
    public abstract Token? OnParsed(TokenList tokens, ref InputStream stream);

    public sealed override Token? Parse(ref InputStream stream)
    {
        var tokens = new TokenList();
        var result = ExecuteMatch(Composition, tokens, ref stream);
        if (stream.ErrorMessage is not null)
        {
            stream.ErrorMessage = $"{DisplayName} > {stream.ErrorMessage}";
            return null;
        }

        return OnParsed(tokens, ref stream);
    }

    private static Token? ExecuteMatch(Component component, TokenList tokens, ref InputStream stream) => component switch
    {
        Literal  literal =>  HandleLiteral( literal, tokens, ref stream),
        Choice   choice   => HandleChoice(  choice,   tokens, ref stream),
        Optional optional => HandleOptional(optional, tokens, ref stream),
        Repeat   repeat   => HandleRepeat(  repeat,   tokens, ref stream),
        Sequence sequence => HandleSequence(sequence, tokens, ref stream),

        ParsableComponent parsableComponent => HandleParsableComponent(parsableComponent, tokens, ref stream),

        _ => null,
    };

    private static Token? HandleLiteral(Literal literal, TokenList tokens, ref InputStream stream)
    {
        if (stream.MatchAndConsume(literal.Value))
        {
            tokens.Add(stream.Parsed(literal.Value, startPosition: stream.Position - literal.Value.Length));
            return tokens.Last();
        }
        
        stream.Fail($"Expected literal `{literal.Value}`");
        return null;
    }

    private static Token? HandleChoice(Choice choice, TokenList tokens, ref InputStream stream)
    {
        int startPosition = stream.Position;

        foreach (var option in choice.Options)
        {
            var result = ExecuteMatch(option, tokens, ref stream);
            if (result is not null)
            {
                return result;
            }

            stream.ErrorMessage = null;
            stream.Seek(startPosition); // backtrack
        }

        stream.Fail($"Expected {string.Join(" or ", choice.Options.Select(option => option.DisplayName))}");
        return null;
    }

    private static Token? HandleOptional(Optional optional, TokenList tokens, ref InputStream stream)
    {
        var result = ExecuteMatch(optional.Component, tokens, ref stream);
        if (result is not null || stream.ErrorMessage is not null)
        {
            return result;
        }

        return null;
    }

    private static Token? HandleRepeat(Repeat repeat, TokenList tokens, ref InputStream stream)
    {
        int counter = 0;
        while (true)
        {
            int startPosition = stream.Position;

            var result = ExecuteMatch(repeat.Component, tokens, ref stream);
            if (result is not null)
            {
                ++counter;
                continue;
            }

            // TODO DEBUG
            // If we hit a Hard Error (e.g., malformed syntax inside the repeat)
            // we must propagate it.
            if (stream.ErrorMessage is not null)
            {
                return null;
            }

            // If it's a Soft Mismatch, it means the "repeat" is done.
            // We break the loop and return a Success result.
            stream.Seek(startPosition); 
            break; 
        }

        if (counter < repeat.Minimum)
        {
            stream.Fail($"Expected at least {repeat.Minimum} repetitions, but got {counter}");
            return null;
        }
        else if (repeat.Maximum.HasValue
             && (repeat.Maximum.Value < counter))
        {
            stream.Fail($"Expected at most {repeat.Maximum.Value} repetitions, but got {counter}");
            return null;
        }

        return tokens.Last();
    }

    private static Token? HandleSequence(Sequence sequence, TokenList tokens, ref InputStream stream)
    {
        int startPosition = stream.Position;

        // We use a local bucket so we don't pollute the main one if the sequence fails
        var newTokens = new TokenList(); 

        for (int i = 0; i < sequence.Components.Length; ++i) 
        {
            var result = ExecuteMatch(sequence.Components[i], newTokens, ref stream);
            if (result is null)
            {
                if (i == 0) 
                {
                    stream.ErrorMessage = null;
                    stream.Seek(startPosition); // backtrack

                    return null; // only if the VERY FIRST element failed
                }

                if (stream.ErrorMessage is null)
                {
                    stream.Fail($"Expected {sequence.Components[i]} at {stream.Position}");
                }
                return null;
            }
        }

        tokens.AddRange(newTokens);
        return tokens.Last();
    }

    private static Token? HandleParsableComponent(ParsableComponent parsableComponent, TokenList tokens, ref InputStream stream)
    {
        var result = parsableComponent.Parse(ref stream);
        if (result is not null)
        {
            tokens.Add(result);
        }

        return result;
    }
}