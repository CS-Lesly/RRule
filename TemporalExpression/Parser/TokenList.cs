namespace TemporalExpression.Parser;

public static class TokenListExtensions
{
    public static IEnumerable<T> GetResultsAfterLiteral<T>(this IEnumerable<Token> tokens, params LiteralComponent[] literals)
        => StreamResultsAfterLiteral<T>(tokens, literals);

    public static bool TryGetSingleResultAfterLiteral<T>(this IEnumerable<Token> tokens, LiteralComponent[] literals, out T? result)
    {
        result = default;
        using var enumerator = StreamResultsAfterLiteral<T>(tokens, literals).GetEnumerator();

        if (!enumerator.MoveNext())
        {
            return false;
        }

        result = enumerator.Current;
        if (enumerator.MoveNext()) // Duplicated
        {
            result = default;
            return false;
        }

        return true;
    }

    public static bool TryGetOptionalResultAfterLiteral<T>(this IEnumerable<Token> tokens, LiteralComponent[] literals, out T? result)
    {
        result = default;
        using var enumerator = StreamResultsAfterLiteral<T>(tokens, literals).GetEnumerator();

        if (!enumerator.MoveNext())
        {
            return true;
        }

        result = enumerator.Current;
        if (enumerator.MoveNext())
        {
            result = default;
            return false;
        }

        return true;
    }

    // TODO this need to be improved, first find all use cases, some literals will we a sequence while others might be 'nested'
    private static IEnumerable<T> StreamResultsAfterLiteral<T>(this IEnumerable<Token> tokens, params LiteralComponent[] literals)
    {
        var remainder = FlattenMany(tokens).ToList();

        if (literals is null || literals.Length == 0)
        {
            for (int index = 0; index < remainder.Count; ++index)
            {
                if (remainder[index].Value is T result)
                {
                    yield return result;
                }
            }

            yield break;
        }

        var currentPosition = 0;
        while (currentPosition < remainder.Count)
        {
            // Try to match literals one-by-one starting at current position
            var matchAll = true;
            for (int literalIndex = 0; literalIndex < literals.Length; ++literalIndex)
            {
                var index = currentPosition + literalIndex;
                if (index >= remainder.Count)
                {
                    matchAll = false;
                    break;
                }

                var node = remainder[index];
                if (node.Value is not string s || !string.Equals(s, literals[literalIndex].Value, StringComparison.OrdinalIgnoreCase))
                {
                    matchAll = false;
                    break;
                }
            }

            if (!matchAll)
            {
                ++currentPosition; // advance to try matching at next position
                continue;
            }

            // All literals matched consecutively; search forward for the first T
            var search = currentPosition + literals.Length;
            var found = false;
            while (search < remainder.Count)
            {
                if (remainder[search].Value is T foundValue)
                {
                    yield return foundValue;
                    currentPosition = search + 1; // continue after the found token
                    found = true;
                    break;
                }

                search++;
            }

            if (!found)
            { // no T found after this matched literal sequence; nothing more to yield
                yield break;
            }
        }
    }

    private static IEnumerable<Token> FlattenMany(IEnumerable<Token> tokens)
    {
        var stack = new Stack<Token>();
        foreach (var token in tokens.Reverse())
        {
            stack.Push(token);
        }

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            yield return current;

            if (current.SubTokens is not null)
            {
                foreach (var subToken in current.SubTokens.Reverse())
                {
                    stack.Push(subToken);
                }
            }
        }
    }
}