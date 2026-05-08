namespace TemporalExpression;

public enum Frequency { Daily, Weekly, Monthly, Yearly };

public record RecurrenceRule : TemporalComponent, ITemporalExpression
{
    public Frequency Frequency { get; set; }

    public IEnumerable<DateTimePoint> ToDateTimePoints(DateTime startDate, DateTime endDate)
    {
        yield return DateTimePoint.FromDateTime(startDate);

        for (var nextDate = startDate.AddDays(1); nextDate <= endDate; nextDate = nextDate.AddDays(1)) // TODO respect frequency and other rule parameters
        {
            yield return DateTimePoint.FromDateTime(nextDate);
        }
    }
}