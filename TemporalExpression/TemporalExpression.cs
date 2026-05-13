namespace TemporalExpression;

public class TemporalExpression : ITemporalExpression
{
    public List<TemporalComponent> Inclusions { get; set; } = [];
    public List<TemporalComponent> Exclusions { get; set; } = [];

    public IEnumerable<DateTime> ToDateTimes(DateTime startDate, DateTime endDate)
    {
        return ToDateTimePoints(startDate, endDate)
            .Select(datetimepoint => datetimepoint.ToDateTime())
            .Distinct();
    }

    public IEnumerable<DateTimePoint> ToDateTimePoints(DateTime startDate, DateTime endDate)
    {
        var inclusions = new List<DateTimePoint>(Inclusions.SelectMany(inclusion => ExpandRuleComponent(inclusion, startDate, endDate)));

        inclusions = [.. inclusions.Distinct()];
        if (inclusions.Count == 0)
        {
            return [];
        }

        var exclusions = new List<DateTimePoint>(Exclusions.SelectMany(exclusion => ExpandRuleComponent(exclusion, startDate, endDate)));
        if (exclusions.Count > 0)
        {
            inclusions = Exclude(inclusions, exclusions);
            if (inclusions.Count == 0)
            {
                return [];
            }
        }

        inclusions.Sort();

        return inclusions;
    }

    private static IEnumerable<DateTimePoint> ExpandRuleComponent(TemporalComponent temporalComponent, DateTime startDate, DateTime endDate)
        => temporalComponent switch
        {
            DateTimeTemporalComponent dateTimeComponent => [DateTimePoint.FromDateTime(dateTimeComponent.DateTime)],
            DateOnlyTemporalComponent dateOnlyComponent => [new DateTimePoint(dateOnlyComponent.Date)],

            DateRange dateRange => dateRange.ToDateTimePoints(startDate, endDate),

            RecurrenceRule recurrenceRule => recurrenceRule.ToDateTimePoints(startDate, endDate),

            _ => throw new ArgumentException($"Unhandled temporal component type {temporalComponent.GetType().Name}", nameof(temporalComponent))
        };

    private static List<DateTimePoint> Exclude(IEnumerable<DateTimePoint> items, IEnumerable<DateTimePoint> itemsToExclude)
    {
        // 1. Pre-process the excludes list into a fast-lookup Dictionary
        // Key: Date, Value: (Is it an all-day exclusion?, Set of specific times to exclude)
        var excludeMap = itemsToExclude
            .GroupBy(e => e.Date)
            .ToDictionary(
                group => group.Key,
                group => (
                    IsAllDay: group.Any(item => !item.Time.HasValue),
                    Times: new HashSet<TimeSpan>(group.Where(item => item.Time.HasValue).Select(item => item.Time!.Value))
                )
            );

        // 2. Perform the filter using O(1) lookups
        return [.. items.Where(result =>
        {
            // If the date isn't in the exclude map at all, keep the result
            if (!excludeMap.TryGetValue(result.Date, out var exclusion))
            {
                return true;
            }

            // Rule: If an 'All Day' exclusion exists for this date, drop the result
            if (exclusion.IsAllDay)
            {
                return false;
            }

            // Rule: If the result has a time, check if that specific time is excluded
            if (result.Time.HasValue && exclusion.Times.Contains(result.Time.Value))
            {
                return false;
            }

            // Otherwise, keep it
            return true;
        })];
    }
}