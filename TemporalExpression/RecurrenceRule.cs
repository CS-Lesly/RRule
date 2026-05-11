namespace TemporalExpression;

public enum Frequency { Daily, Weekly, Monthly, Yearly };

public record RecurrenceRule : TemporalComponent, ITemporalExpression
{
    public Frequency Frequency { get; set; }
    public uint Interval { get; set; } = 1;

    public (int, DayOfWeek)[] ByWeekday { get; set; } = [];
    public int[] ByMonthday { get; set; } = [];
    public int[] ByYearday { get; set; } = [];
    public int[] ByWeekNumber { get; set; } = [];
    public uint[] ByMonth { get; set; } = [];
    public int[] BySetPosition { get; set; } = [];
    public DayOfWeek WeekStart { get; set; } = DayOfWeek.Monday;

    public uint[] ByHour { get; set; } = [];
    public uint[] ByMinute { get; set; } = [];

    public IEnumerable<DateTimePoint> ToDateTimePoints(DateTime startDate, DateTime endDate)
    {
        yield return DateTimePoint.FromDateTime(startDate);

        for (var nextDate = startDate.AddDays(Interval); nextDate <= endDate; nextDate = nextDate.AddDays(Interval)) // TODO respect frequency and other rule parameters
        {
            yield return DateTimePoint.FromDateTime(nextDate);
        }
    }
}