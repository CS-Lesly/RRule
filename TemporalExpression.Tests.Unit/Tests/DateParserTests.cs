using TemporalExpression.Parser;
using TemporalExpression.Parser.Components;

namespace TemporalExpression.Tests;

[TestFixture]
public class DateParserTests
{
    [Test]
    [TestCaseSource(nameof(GetDateOnlyTestSets))]
    public void TestDateOnlyParsing(string input, DateOnlyResult expected)
    {
        var stream = new InputStream(input);
        var token = new DateOnlyComponent().Parse(ref stream);

        if (expected.IsSuccess)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(stream.ErrorMessage, Is.Null);
                Assert.That(token, Is.Not.Null);
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(token.Value, Is.InstanceOf<DateOnly>());
                Assert.That(token.Value, Is.EqualTo(expected.DateOnly));
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

    public record struct DateOnlyResult(bool IsSuccess, DateOnly DateOnly = default);

    private static IEnumerable<TestCaseData> GetDateOnlyTestSets()
    {
        yield return new TestCaseData("20260101", new DateOnlyResult(IsSuccess: true, new DateOnly(2026, 1, 1)))
            .SetName("Valid date")
            .SetCategory("Valid data");

        yield return new TestCaseData("260101", new DateOnlyResult(IsSuccess: false))
            .SetName("Invalid date: missing century digits")
            .SetCategory("Invalid data");
        yield return new TestCaseData("20269901", new DateOnlyResult(IsSuccess: false))
            .SetName("Invalid date: date part includes invalid month")
            .SetCategory("Invalid data");
    }

    [Test]
    [TestCaseSource(nameof(GetDateTimeTestSets))]
    public void TestDateTimeParsing(string input, DateTimeResult expected)
    {
        var stream = new InputStream(input);
        var token = new DateTimeComponent().Parse(ref stream);

        if (expected.IsSuccess)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(stream.ErrorMessage, Is.Null);
                Assert.That(token, Is.Not.Null);
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(token.Value, Is.InstanceOf<DateTime>());
                Assert.That(token.Value, Is.EqualTo(expected.DateTime));
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

    public record struct DateTimeResult(bool IsSuccess, DateTime DateTime = default);

    private static IEnumerable<TestCaseData> GetDateTimeTestSets()
    {
        yield return new TestCaseData("20260101T0000", new DateTimeResult(IsSuccess: true, new DateTime(2026, 1, 1)))
            .SetName("Valid date-time with only years, months and days")
            .SetCategory("Valid data");
        yield return new TestCaseData("20260101T1234", new DateTimeResult(IsSuccess: true, new DateTime(2026, 1, 1, 12, 34, 0)))
            .SetName("Valid date-time (with time part but not including seconds)")
            .SetCategory("Valid data");

        yield return new TestCaseData("260101T1234", new DateTimeResult(IsSuccess: false))
            .SetName("Invalid date-time: missing century digits")
            .SetCategory("Invalid data");
        yield return new TestCaseData("20260101T123456", new DateTimeResult(IsSuccess: false))
            .SetName("Invalid date-time: includes seconds")
            .SetCategory("Invalid data");
        yield return new TestCaseData("202601011234", new DateTimeResult(IsSuccess: false))
            .SetName("Invalid date-time with missing T separator between date and time components")
            .SetCategory("Invalid data");
        yield return new TestCaseData("20269901T1234", new DateTimeResult(IsSuccess: false))
            .SetName("Invalid date-time: date part includes invalid month")
            .SetCategory("Invalid data");
    }
}