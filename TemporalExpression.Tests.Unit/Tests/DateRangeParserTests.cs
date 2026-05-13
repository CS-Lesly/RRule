using TemporalExpression.Parser;
using TemporalExpression.Parser.Components;

namespace TemporalExpression.Tests;

[TestFixture]
public class DateRangeParserTests
{
    [Test]
    [TestCaseSource(nameof(GetDateRangeTestSets))]
    public void TestDateRangeParsing(string input, DateRangeResult expected)
    {
        var stream = new InputStream(input);
        var token = new DateRangeComponent().Parse(ref stream);

        if (expected.IsSuccess)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(stream.ErrorMessage, Is.Null);
                Assert.That(token, Is.Not.Null);
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(token.Value, Is.InstanceOf<DateRange>());
                Assert.That(token.Value, Is.EqualTo(expected.DateRange));
            }
        }
        else
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(stream.ErrorMessage, Is.Not.Null);
                Assert.That(token, Is.Null);
            }
        }
    }

    public record struct DateRangeResult(bool IsSuccess, DateRange? DateRange = default);

    private static IEnumerable<TestCaseData> GetDateRangeTestSets()
    {
        yield return new TestCaseData("20260101/P3W", new DateRangeResult(IsSuccess: true, new DateRange()
            {
                Start = new DateTime(2026, 1, 1),
                Until = new ExtendedTimeSpan().WithWeeks(3),
            }))
            .SetName("Valid range: start date and duration")
            .SetCategory("Valid data");
        yield return new TestCaseData("20260101T1200/+P3W", new DateRangeResult(IsSuccess: true, new DateRange()
            {
                Start = new DateTime(2026, 1, 1, 12, 0, 0),
                Until = new ExtendedTimeSpan().WithWeeks(3),
            }))
            .SetName("Valid range: start date-time and positive duration")
            .SetCategory("Valid data");
        yield return new TestCaseData("20260101/20261231", new DateRangeResult(IsSuccess: true, new DateRange()
            {
                Start = new DateTime(2026,  1,  1),
                End   = new DateTime(2026, 12, 31),
            }))
            .SetName("Valid range: start date and end date")
            .SetCategory("Valid data");
        yield return new TestCaseData("P7M/P1Y", new DateRangeResult(IsSuccess: true, new DateRange()
            {
                From  = new ExtendedTimeSpan().WithMonths(7),
                Until = new ExtendedTimeSpan().WithYears(1),
            }))
            .SetName("Valid range: start date and end date")
            .SetCategory("Valid data");

        yield return new TestCaseData("20260101P3W", new DateRangeResult(IsSuccess: false))
            .SetName("Invalid range: not a valid range (slash is missing)")
            .SetCategory("Invalid data");
        yield return new TestCaseData("20260101/-P3W", new DateRangeResult(IsSuccess: false))
            .SetName("Invalid range: negative duration not allowed")
            .SetCategory("Invalid data");
    }
}