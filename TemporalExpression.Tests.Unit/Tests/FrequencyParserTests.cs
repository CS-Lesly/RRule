using TemporalExpression.Parser;
using TemporalExpression.Parser.Tokens.Values;

namespace TemporalExpression.Tests;

[TestFixture]
public class FrequencyParserTests
{
    [Test]
    [TestCaseSource(nameof(GetFrequencyTestSets))]
    public void TestFrequencyParsing(string input, FrequencyResult expected)
    {
        var inputStream = new TokenStream(input);
        var parseResult = new FrequencyValue().Parse(ref inputStream);

        if (expected.IsSuccess)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(parseResult.Error, Is.Null);
                Assert.That(parseResult.Value, Is.Not.Null);
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(parseResult.Value, Is.InstanceOf<FrequencyValue>());
                Assert.That(((FrequencyValue)parseResult.Value!).Value, Is.Not.Null);
            }

            Assert.That(((FrequencyValue)parseResult.Value!).Value, Is.EqualTo(expected.Frequency));
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
        yield return new TestCaseData(string.Empty, new FrequencyResult(IsSuccess: false))
            .SetName("Invalid empty frequency")
            .SetCategory("Invalid data");
        yield return new TestCaseData(";", new FrequencyResult(IsSuccess: false))
            .SetName("Invalid frequency (no frequency, only a suffix)")
            .SetCategory("Invalid data");
    }
}