namespace TemporalExpression;

public record ExtendedTimeSpan()
{
    public int Years { get; private set; }

    public ExtendedTimeSpan WithYears(int years)
    {
        Years = Math.Abs(years);
        return this;
    }

    public int Months { get; private set; }

    public ExtendedTimeSpan WithMonths(int months)
    {
        Months = Math.Abs(months);
        return this;
    }

    public int Weeks { get; private set; }

    public ExtendedTimeSpan WithWeeks(int weeks)
    {
        Weeks = Math.Abs(weeks);
        return this;
    }

    public int Days { get; private set; }

    public ExtendedTimeSpan WithDays(int days)
    {
        Days = Math.Abs(days);
        return this;
    }

    public int Hours { get; private set; }

    public ExtendedTimeSpan WithHours(int hours)
    {
        Hours = Math.Abs(hours);
        return this;
    }

    public int Minutes { get; private set; }

    public ExtendedTimeSpan WithMinutes(int minutes)
    {
        Minutes = Math.Abs(minutes);
        return this;
    }

    public DateTime CalculateEndDate(DateTime startDate)
        => startDate
            .AddYears(Years)
            .AddMonths(Months)
            .AddDays((Weeks * 7) + Days)
            .AddHours(Hours)
            .AddMinutes(Minutes);

    public DateTimePoint ApplyTo(DateTimePoint start)
    {
        // 1. Convert to DateTime to perform the complex calendar math
        DateTime startDateTime = start.ToDateTime();

        // 2. Use your existing logic to shift the date
        DateTime endDateTime = CalculateEndDate(startDateTime);

        // 3. Return a new Point
        // If the original point had no time, we might want to keep the result as "All Day"
        return new(
            DateOnly.FromDateTime(endDateTime),
            start.Time.HasValue ? endDateTime.TimeOfDay : null
        );
    }
}