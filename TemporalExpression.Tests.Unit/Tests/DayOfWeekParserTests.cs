using TemporalExpression.Parser;
using TemporalExpression.Parser.Tokens.Values;

namespace TemporalExpression.Tests;

[TestFixture]
public class DayOfWeekParserTests
{
    [Test]
    [TestCaseSource(nameof(GetDayOfWeekTestSets))]
    public void TestDayOfWeekParsing(string input, DayOfWeekResult expected)
    {
        var inputStream = new TokenStream(input);
        var parseResult = new DayOfWeekValue().Parse(ref inputStream);

        if (expected.IsSuccess)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parseResult.Error, Is.Null);
                Assert.That(parseResult.Value, Is.Not.Null);
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(parseResult.Value, Is.InstanceOf<DayOfWeekValue>());
                Assert.That(((DayOfWeekValue)parseResult.Value!).Value, Is.Not.Null);
            }

            Assert.That(((DayOfWeekValue)parseResult.Value!).Value, Is.EqualTo(expected.DayOfWeek));
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

    public record struct DayOfWeekResult(bool IsSuccess, DayOfWeek DayOfWeek = default);

    private static IEnumerable<TestCaseData> GetDayOfWeekTestSets()
    {
        yield return new TestCaseData("mo", new DayOfWeekResult(IsSuccess: true, DayOfWeek.Monday))
            .SetName("Valid Monday (lowercase)")
            .SetCategory("Valid data");
        yield return new TestCaseData("Tu", new DayOfWeekResult(IsSuccess: true, DayOfWeek.Tuesday))
            .SetName("Valid Tuesday (mixedcase)")
            .SetCategory("Valid data");
        yield return new TestCaseData("WE", new DayOfWeekResult(IsSuccess: true, DayOfWeek.Wednesday))
            .SetName("Valid Wednesday")
            .SetCategory("Valid data");
        yield return new TestCaseData("TH", new DayOfWeekResult(IsSuccess: true, DayOfWeek.Thursday))
            .SetName("Valid Thursday")
            .SetCategory("Valid data");
        yield return new TestCaseData("FR", new DayOfWeekResult(IsSuccess: true, DayOfWeek.Friday))
            .SetName("Valid Friday")
            .SetCategory("Valid data");
        yield return new TestCaseData("SA", new DayOfWeekResult(IsSuccess: true, DayOfWeek.Saturday))
            .SetName("Valid Saturday")
            .SetCategory("Valid data");
        yield return new TestCaseData("SU", new DayOfWeekResult(IsSuccess: true, DayOfWeek.Sunday))
            .SetName("Valid Sunday")
            .SetCategory("Valid data");

        yield return new TestCaseData("MON", new DayOfWeekResult(IsSuccess: false))
            .SetName("Invalid day of the week (not abbreviated to two characters)")
            .SetCategory("Invalid data");
        yield return new TestCaseData("MONDAY", new DayOfWeekResult(IsSuccess: false))
            .SetName("Invalid day of the week (not abbreviated)")
            .SetCategory("Invalid data");
        yield return new TestCaseData(string.Empty, new DayOfWeekResult(IsSuccess: false))
            .SetName("Invalid empty day of the week")
            .SetCategory("Invalid data");
        yield return new TestCaseData(";", new DayOfWeekResult(IsSuccess: false))
            .SetName("Invalid day of the week (no day, only a suffix)")
            .SetCategory("Invalid data");
    }
}