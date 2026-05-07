namespace TemporalExpression;

public sealed class RDateExpression : DateExpression;
public sealed class ExDateExpression : DateExpression;

public abstract class DateExpression : ITemporalExpression
{
    protected List<DateRange> _dateRanges = [];

    public IEnumerable<DateTimePoint> ToDateTimePoints(DateTime startDate, DateTime endDate)
        => _dateRanges.SelectMany(dateRange => dateRange.ToDateTimePoints(startDate, endDate));
}