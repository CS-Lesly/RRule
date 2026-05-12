using TemporalExpression.Parser;
using TemporalExpression.Parser.Tokens.Values;

namespace TemporalExpression.Tests;

[TestFixture]
public class ParserTests
{
    [Test]
    [TestCaseSource(nameof(GetDurationTestSets))]
    public void TestDurationParsing(string input, DurationResult expected)
    {
        var inputStream = new TokenStream(input);
        var parseResult = new DurationValue().Parse(ref inputStream);

        if (expected.IsSuccess)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parseResult.Error, Is.Null);
                Assert.That(parseResult.Value, Is.Not.Null);
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(parseResult.Value, Is.InstanceOf<DurationValue>());
                Assert.That(((DurationValue)parseResult.Value!).Value, Is.Not.Null);
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(((DurationValue)parseResult.Value!).Value!.IsNegative, Is.EqualTo(expected.IsNegative));
                Assert.That(((DurationValue)parseResult.Value!).Value!.Years,      Is.EqualTo(expected.Years));
                Assert.That(((DurationValue)parseResult.Value!).Value!.Months,     Is.EqualTo(expected.Months));
                Assert.That(((DurationValue)parseResult.Value!).Value!.Weeks,      Is.EqualTo(expected.Weeks));
                Assert.That(((DurationValue)parseResult.Value!).Value!.Days,       Is.EqualTo(expected.Days));
                Assert.That(((DurationValue)parseResult.Value!).Value!.Hours,      Is.EqualTo(expected.Hours));
                Assert.That(((DurationValue)parseResult.Value!).Value!.Minutes,    Is.EqualTo(expected.Minutes));
            }
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

    public record struct DurationResult(bool IsSuccess, bool IsNegative = default, int Years = default, int Months = default, int Weeks = default, int Days = default, int Hours = default, int Minutes = default);

    private static IEnumerable<TestCaseData> GetDurationTestSets()
    {
        yield return new TestCaseData("P1Y2M3DT4H5M", new DurationResult(IsSuccess: true, false, 1, 2, 0, 3, 4, 5))
            .SetName("Valid duration with years, months, days, hours and minutes")
            .SetCategory("Valid data");
        yield return new TestCaseData("-P1Y2M3DT4H5M", new DurationResult(IsSuccess: true, true, 1, 2, 0, 3, 4, 5))
            .SetName("Valid negative duration")
            .SetCategory("Valid data");
        yield return new TestCaseData("+P3W", new DurationResult(IsSuccess: true, false, 0, 0, 3, 0, 0, 0))
            .SetName("Valid duration with weeks and a leading plus sign")
            .SetCategory("Valid data");
        yield return new TestCaseData("PT10M", new DurationResult(IsSuccess: true, false, 0, 0, 0, 0, 0, 10))
            .SetName("Valid duration with minutes")
            .SetCategory("Valid data");
        yield return new TestCaseData("P12Y12M12DT12H12M", new DurationResult(IsSuccess: true, false, 12, 12, 0, 12, 12, 12))
            .SetName("Valid duration with years, months, days, hours and minutes, using multiple digits for each component")
            .SetCategory("Valid data");
        yield return new TestCaseData("P1Y2M3W4DT5H6M", new DurationResult(IsSuccess: true, false, 1, 2, 3, 4, 5, 6))
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