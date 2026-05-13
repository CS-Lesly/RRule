using TemporalExpression.Parser;
using TemporalExpression.Parser.Components;

namespace TemporalExpression.Tests;

[TestFixture]
public class FrequencyParserTests
{
    [Test]
    [TestCaseSource(nameof(GetFrequencyTestSets))]
    public void TestFrequencyParsing(string input, FrequencyResult expected)
    {
        var stream = new InputStream(input);
        var token = new FrequencyComponent().Parse(ref stream);

        if (expected.IsSuccess)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(stream.ErrorMessage, Is.Null);
                Assert.That(token, Is.Not.Null);
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(token.Value, Is.InstanceOf<Frequency>());
                Assert.That(token.Value, Is.EqualTo(expected.Frequency));
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

    public record struct FrequencyResult(bool IsSuccess, Frequency Frequency = default);

    private static IEnumerable<TestCaseData> GetFrequencyTestSets()
    {
        yield return new TestCaseData("DAILY", new FrequencyResult(IsSuccess: true, Frequency.Daily))
            .SetName("Valid daily frequency")
            .SetCategory("Valid data");
        yield return new TestCaseData("WEEKLY", new FrequencyResult(IsSuccess: true, Frequency.Weekly))
            .SetName("Valid weekly frequency")
            .SetCategory("Valid data");
        yield return new TestCaseData("MONTHLY", new FrequencyResult(IsSuccess: true, Frequency.Monthly))
            .SetName("Valid monthly frequency")
            .SetCategory("Valid data");
        yield return new TestCaseData("QUARTERLY", new FrequencyResult(IsSuccess: true, Frequency.Quarterly))
            .SetName("Valid quarterly frequency")
            .SetCategory("Valid data");
        yield return new TestCaseData("SEMESTERLY", new FrequencyResult(IsSuccess: true, Frequency.Semesterly))
            .SetName("Valid semesterly frequency")
            .SetCategory("Valid data");
        yield return new TestCaseData("YEARLY", new FrequencyResult(IsSuccess: true, Frequency.Yearly))
            .SetName("Valid yearly frequency")
            .SetCategory("Valid data");

        yield return new TestCaseData("LESLY", new FrequencyResult(IsSuccess: false))
            .SetName("Invalid frequency")
            .SetCategory("Invalid data");
        // TODO?
        //yield return new TestCaseData(string.Empty, new FrequencyResult(IsSuccess: false))
        //    .SetName("Invalid empty frequency")
        //    .SetCategory("Invalid data");
        //yield return new TestCaseData(";", new FrequencyResult(IsSuccess: false))
        //    .SetName("Invalid frequency (no frequency, only a suffix)")
        //    .SetCategory("Invalid data");
    }
}