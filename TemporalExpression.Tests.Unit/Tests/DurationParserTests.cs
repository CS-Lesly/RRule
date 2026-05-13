using TemporalExpression.Parser;
using TemporalExpression.Parser.Components;

namespace TemporalExpression.Tests;

[TestFixture]
public class DurationParserTests
{
    [Test]
    [TestCaseSource(nameof(GetDurationTestSets))]
    public void TestDurationParsing(string input, DurationResult expected)
    {
        var stream = new InputStream(input);
        var token = new DurationComponent().Parse(ref stream);

        if (expected.IsSuccess)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(stream.ErrorMessage, Is.Null);
                Assert.That(token, Is.Not.Null);
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(token.Value, Is.InstanceOf<ExtendedTimeSpan>());
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(((ExtendedTimeSpan)token.Value).Years,   Is.EqualTo(expected.Years));
                Assert.That(((ExtendedTimeSpan)token.Value).Months,  Is.EqualTo(expected.Months));
                Assert.That(((ExtendedTimeSpan)token.Value).Weeks,   Is.EqualTo(expected.Weeks));
                Assert.That(((ExtendedTimeSpan)token.Value).Days,    Is.EqualTo(expected.Days));
                Assert.That(((ExtendedTimeSpan)token.Value).Hours,   Is.EqualTo(expected.Hours));
                Assert.That(((ExtendedTimeSpan)token.Value).Minutes, Is.EqualTo(expected.Minutes));
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

    public record struct DurationResult(bool IsSuccess, int Years = default, int Months = default, int Weeks = default, int Days = default, int Hours = default, int Minutes = default);

    private static IEnumerable<TestCaseData> GetDurationTestSets()
    {
        yield return new TestCaseData("P1Y2M3DT4H5M", new DurationResult(IsSuccess: true, 1, 2, 0, 3, 4, 5))
            .SetName("Valid duration with years, months, days, hours and minutes")
            .SetCategory("Valid data");
        yield return new TestCaseData("+P3W", new DurationResult(IsSuccess: true, 0, 0, 3, 0, 0, 0))
            .SetName("Valid duration with weeks and a leading plus sign")
            .SetCategory("Valid data");
        yield return new TestCaseData("PT10M", new DurationResult(IsSuccess: true, 0, 0, 0, 0, 0, 10))
            .SetName("Valid duration with minutes")
            .SetCategory("Valid data");
        yield return new TestCaseData("P12Y12M12DT12H12M", new DurationResult(IsSuccess: true, 12, 12, 0, 12, 12, 12))
            .SetName("Valid duration with years, months, days, hours and minutes, using multiple digits for each component")
            .SetCategory("Valid data");
        yield return new TestCaseData("P1Y2M3W4DT5H6M", new DurationResult(IsSuccess: true, 1, 2, 3, 4, 5, 6))
            .SetName("Valid duration with years, months, weeks, days, hours and minutes")
            .SetCategory("Valid data");

        yield return new TestCaseData("1Y2M3DT4H5M", new DurationResult(IsSuccess: false))
            .SetName("Invalid duration with missing P prefix")
            .SetCategory("Invalid data");
        yield return new TestCaseData("-P1Y2M3D4H5M", new DurationResult(IsSuccess: false))
            .SetName("Invalid duration with missing T separator between date and time components")
            .SetCategory("Invalid data");
        yield return new TestCaseData("+P3W1Y", new DurationResult(IsSuccess: false))
            .SetName("Invalid duration with components in wrong order")
            .SetCategory("Invalid data");
        yield return new TestCaseData("PT10M9M", new DurationResult(IsSuccess: false))
            .SetName("Invalid duration with repeating time components")
            .SetCategory("Invalid data");
    }
}