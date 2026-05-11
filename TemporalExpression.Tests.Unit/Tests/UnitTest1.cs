using TemporalExpression.Parser;
using TemporalExpression.Parser.Tokens.Values;

namespace TemporalExpression.Tests;

public class Tests
{
    [Test]
    public void CorrectDurationValues()
    {
        (string Input, bool IsNegative, int Years, int Months, int Weeks, int Days, int Hours, int Minutes)[] durationTestSets =
        [
            ("P1Y2M3DT4H5M",   false, 1, 2, 0, 3,  4,  5),
            ("-P1Y2M3DT4H5M",  true,  1, 2, 0, 3,  4,  5),
            ("+P3W",           false, 0, 0, 3, 0,  0,  0),
            ("PT10M",          false, 0, 0, 0, 0,  0, 10),
            ("P2Y6M5DT12H35M", false, 2, 6, 0, 5, 12, 35),
            ("P1Y2M3W4DT5H6M", false, 1, 2, 3, 4,  5,  6),
        ];

        foreach (var (input, isNegative, years, months, weeks, days, hours, minutes) in durationTestSets)
        {
            var inputStream = new TokenStream(input);
            var parseResult = new DurationValue().Parse(ref inputStream);

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
                Assert.That(((DurationValue)parseResult.Value!).Value!.IsNegative, Is.EqualTo(isNegative));
                Assert.That(((DurationValue)parseResult.Value!).Value!.Years, Is.EqualTo(years));
                Assert.That(((DurationValue)parseResult.Value!).Value!.Months, Is.EqualTo(months));
                Assert.That(((DurationValue)parseResult.Value!).Value!.Weeks, Is.EqualTo(weeks));
                Assert.That(((DurationValue)parseResult.Value!).Value!.Days, Is.EqualTo(days));
                Assert.That(((DurationValue)parseResult.Value!).Value!.Hours, Is.EqualTo(hours));
                Assert.That(((DurationValue)parseResult.Value!).Value!.Minutes, Is.EqualTo(minutes));
            }
        }
    }

    [Test]
    public void ErroneousDurationValues()
    {
        string[] durationTestSets =
        [
            "1Y2M3DT4H5M",
            "-P1Y2M3D4H5M",
            "+P3W1Y",
            "PT10M9M",
        ];

        foreach (var durationTestSet in durationTestSets)
        {
            var inputStream = new TokenStream(durationTestSet);
            var parseResult = new DurationValue().Parse(ref inputStream);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(parseResult.Error, Is.Not.Null);
                Assert.That(parseResult.Value, Is.Null);
            }
        }
    }
}