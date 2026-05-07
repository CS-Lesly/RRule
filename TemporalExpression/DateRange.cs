namespace TemporalExpression;

public class DateRange : ITemporalExpression
{
    protected DateTime _start; // TODO nullable
  //protected ExtendedTimeSpan? _from;

    protected DateTime? _end;
  //protected ExtendedTimeSpan? _until;

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