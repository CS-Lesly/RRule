namespace TemporalExpression;

public record DateRange : TemporalComponent, ITemporalExpression
{
    public DateTime? Start;
    public ExtendedTimeSpan? From;

    public DateTime? End;
    public ExtendedTimeSpan? Until;

    public IEnumerable<DateTimePoint> ToDateTimePoints(DateTime startDate, DateTime endDate)
    {
        var start = Start ?? From!.CalculateEndDate(startDate);
        start = start < startDate ? startDate : start;

        yield return DateTimePoint.FromDateTime(start);

        if (End.HasValue)
        {
            for (var nextDate = start.AddDays(1); nextDate <= End.Value && nextDate <= endDate; nextDate = nextDate.AddDays(1))
            {
                yield return DateTimePoint.FromDateTime(nextDate);
            }
        }
        else
        {
            for (var nextDate = start.AddDays(1); nextDate <= Until!.CalculateEndDate(startDate) && nextDate <= endDate; nextDate = nextDate.AddDays(1))
            {
                yield return DateTimePoint.FromDateTime(nextDate);
            }
        }
    }
}