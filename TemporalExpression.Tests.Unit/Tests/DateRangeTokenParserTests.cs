using TemporalExpression.Parser;
using TemporalExpression.Parser.Tokens;

namespace TemporalExpression.Tests;

[TestFixture]
public class DateRangeTokenParserTests
{
    [Test]
    [TestCaseSource(nameof(GetDateRangeTokenTestSets))]
    public void TestDateRangeTokenParsing(string input, DateRangeTokenResult expected)
    {
        var inputStream = new TokenStream(input);
        var parseResult = new DateRangeToken().Parse(ref inputStream);

        if (expected.IsSuccess)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parseResult.Error, Is.Null);
                Assert.That(parseResult.Value, Is.Not.Null);
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(parseResult.Value, Is.InstanceOf<DateRangeToken>());
                Assert.That(((DateRangeToken)parseResult.Value!).Value, Is.Not.Null);
            }

            Assert.That(((DateRangeToken)parseResult.Value!).Value, Is.EqualTo(expected.DateRange));
        }
        else
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parseResult.Error, Is.Not.Null);
                Assert.That(parseResult.Value, Is.Null);
            }
        }
    }

    public record struct DateRangeTokenResult(bool IsSuccess, DateRange? DateRange = default);

    private static IEnumerable<TestCaseData> GetDateRangeTokenTestSets()
    {
        yield return new TestCaseData("20260101/P3W", new DateRangeTokenResult(IsSuccess: true, new DateRange()
            {
                Start = new DateTime(2026, 1, 1),
                Until = new ExtendedTimeSpan().WithWeeks(3),
            }))
            .SetName("Valid range: start date and duration")
            .SetCategory("Valid data");
        yield return new TestCaseData("20260101T1200/+P3W", new DateRangeTokenResult(IsSuccess: true, new DateRange()
            {
                Start = new DateTime(2026, 1, 1, 12, 0, 0),
                Until = new ExtendedTimeSpan().WithWeeks(3),
            }))
            .SetName("Valid range: start date-time and positive duration")
            .SetCategory("Valid data");
        yield return new TestCaseData("20260101/20261231", new DateRangeTokenResult(IsSuccess: true, new DateRange()
            {
                Start = new DateTime(2026,  1,  1),
                End   = new DateTime(2026, 12, 31),
            }))
            .SetName("Valid range: start date and end date")
            .SetCategory("Valid data");
        yield return new TestCaseData("P7M/P1Y", new DateRangeTokenResult(IsSuccess: true, new DateRange()
            {
                From  = new ExtendedTimeSpan().WithMonths(7),
                Until = new ExtendedTimeSpan().WithYears(1),
            }))
            .SetName("Valid range: start date and end date")
            .SetCategory("Valid data");

        yield return new TestCaseData("20260101P3W", new DateRangeTokenResult(IsSuccess: false))
            .SetName("Invalid range: not a valid range (slash is missing)")
            .SetCategory("Invalid data");
        yield return new TestCaseData("20260101/-P3W", new DateRangeTokenResult(IsSuccess: false))
            .SetName("Invalid range: negative duration not allowed")
            .SetCategory("Invalid data");
    }
}