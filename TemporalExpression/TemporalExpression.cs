namespace TemporalExpression;

public class TemporalExpression : ITemporalExpression
{
    public List<RRuleExpression> RRules { get; set; } = [];
    public List<ExRuleExpression> ExRules { get; set; } = [];

    public List<RDateExpression> RDates { get; set; } = [];
    public List<ExDateExpression> ExDates { get; set; } = [];

    public IEnumerable<DateTime> ToDateTimes(DateTime startDate, DateTime endDate)
    {
        return ToDateTimePoints(startDate, endDate)
            .Select(datetimepoint => datetimepoint.ToDateTime())
            .Distinct();
    }

    public IEnumerable<DateTimePoint> ToDateTimePoints(DateTime startDate, DateTime endDate)
    {
        IEnumerable<DateTimePoint> rRules = [.. RRules.SelectMany(rule => rule.ToDateTimePoints(startDate, endDate))];
        IEnumerable<DateTimePoint> rDates = [.. RDates.SelectMany(date => date.ToDateTimePoints(startDate, endDate))];

        var result = rRules.Union(rDates).Distinct().ToList();
        if (result.Count == 0)
        {
            return result;
        }

        foreach (var exDate in ExDates)
        {
            result = Exclude(result, exDate.ToDateTimePoints(startDate, endDate));
            if (result.Count == 0)
            {
                return result;
            }
        }
        foreach (var exRule in ExRules)
        {
            result = Exclude(result, exRule.ToDateTimePoints(startDate, endDate));
            if (result.Count == 0)
            {
                return result;
            }
        }

        result.Sort();

        return result;
    }

    public static List<DateTimePoint> Exclude(List<DateTimePoint> items, IEnumerable<DateTimePoint> itemsToExclude)
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