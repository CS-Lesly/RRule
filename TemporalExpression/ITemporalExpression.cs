namespace TemporalExpression;

public interface ITemporalExpression
{
    public IEnumerable<DateTimePoint> ToDateTimePoints(DateTime startDate, DateTime endDate);
}