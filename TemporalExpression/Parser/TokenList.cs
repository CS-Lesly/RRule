namespace TemporalExpression.Parser;

public class TokenList : List<Token>
{
    public IEnumerable<T> FindMany<T>(string target)
        => this.Where(token => (token.Value is IEnumerable<T>) && (token.Target == target)).SelectMany(token => token.ValueAs<IEnumerable<T>>());
    
    public IEnumerable<T> Find<T>(string target)
        => this.Where(token => (token.Value is T) && (token.Target == target)).Select(token => token.ValueAs<T>());
}