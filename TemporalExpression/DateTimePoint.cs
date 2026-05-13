namespace TemporalExpression;

public readonly struct DateTimePoint(DateOnly date, TimeSpan? time = null) : IComparable<DateTimePoint>
{
    public DateOnly Date { get; } = date;
    public TimeSpan? Time { get; } = time;

    public static DateTimePoint FromDateTime(DateTime dateTime)
        => new(new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day), dateTime.TimeOfDay);

    public DateTime ToDateTime() => Date.ToDateTime(TimeOnly.MinValue).Add(Time ?? TimeSpan.Zero);

    public int CompareTo(DateTimePoint other)
    {
        // 1. Compare Dates first
        int dateComparison = Date.CompareTo(other.Date);
        if (dateComparison != 0)
        {
            return dateComparison;
        }

        // 2. If Dates are equal, compare Times
        // We treat null (All Day) as earlier than any specific time (e.g., 00:00)
        if (Time == other.Time)
        {
            return 0;
        }
        if (Time is null)
        {
            return -1;
        }
        if (other.Time is null)
        {
            return 1;
        }

        return Time.Value.CompareTo(other.Time.Value);
    }

    // TODO still needed?
    public bool IsExcludedBy(DateTimePoint excludeEntry)
    {
        // Must always be on the same day
        if (Date != excludeEntry.Date)
        {
            return false;
        }

        // Rule: If exclude entry is 'All Day', ignore time comparison
        if (!excludeEntry.Time.HasValue)
        {
            return true;
        }

        // Rule: If exclude entry has a time, match must be exact
        return Time == excludeEntry.Time;
    }
}