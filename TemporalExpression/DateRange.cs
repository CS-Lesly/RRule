namespace TemporalExpression;

public record DateRange : TemporalComponent, ITemporalExpression
{
    public DateTime? Start;
    public ExtendedTimeSpan? From;

    public DateTime? End;
    public ExtendedTimeSpan? Until;

    public IEnumerable<DateTimePoint> ToDateTimePoints(DateTime startDate, DateTime endDate)
    {
        yield return DateTimePoint.FromDateTime(_start);

        if (_end.HasValue)
        {
            for (var nextDate = _start.AddDays(1); nextDate <= _end.Value; nextDate = nextDate.AddDays(1))
            {
                yield return DateTimePoint.FromDateTime(nextDate);
            }
        }
    }
}