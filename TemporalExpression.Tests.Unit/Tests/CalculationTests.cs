using TemporalExpression.Parser;

namespace TemporalExpression.Tests;

[TestFixture]
public class CalculationTests
{
    [Test]
    public void Todo()
    {
        var startDate = new DateTime(2026,  1,  1);
        var endDate   = new DateTime(2026, 12, 31);

        var inputString = "RRULE:FREQ=DAILY";

        var temporalExpression = TemporalExpressionParser.Parse(inputString, out var parseError);

        Assert.That(parseError, Is.Null);

        var result = temporalExpression!.ToDateTimes(startDate, endDate);

        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Count(), Is.EqualTo(365));
            Assert.That(result.Take(3), Is.EquivalentTo(
            [
                new DateTime(2026,  1,  1),
                new DateTime(2026,  1,  2),
                new DateTime(2026,  1,  3),
            ]));
            Assert.That(result.Skip(362).Take(3), Is.EquivalentTo(
            [
                new DateTime(2026, 12, 29),
                new DateTime(2026, 12, 30),
                new DateTime(2026, 12, 31),
            ]));
        }
    }

    [Test]
    public void DateRangeTodo()
    {
        var startDate = new DateTime(2026,  1,  1);
        var endDate   = new DateTime(2026, 12, 31);

        var dateRange = new DateRange()
        {
            Start = startDate,
            End = endDate
        };

        var result = dateRange.ToDateTimePoints(startDate, endDate);

        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Count(), Is.EqualTo(365));
            Assert.That(result.Take(3), Is.EquivalentTo(
            [
                DateTimePoint.FromDateTime(new DateTime(2026,  1,  1)),
                DateTimePoint.FromDateTime(new DateTime(2026,  1,  2)),
                DateTimePoint.FromDateTime(new DateTime(2026,  1,  3)),
            ]));
            Assert.That(result.Skip(362).Take(3), Is.EquivalentTo(
            [
                DateTimePoint.FromDateTime(new DateTime(2026, 12, 29)),
                DateTimePoint.FromDateTime(new DateTime(2026, 12, 30)),
                DateTimePoint.FromDateTime(new DateTime(2026, 12, 31)),
            ]));
        }
    }
}