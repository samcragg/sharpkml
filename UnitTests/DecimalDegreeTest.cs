namespace UnitTests
{
    using System.Globalization;
    using NUnit.Framework;
    using SharpKml;

    [TestFixture]
    public class DecimalDegreeTest
    {
        public sealed class ParseTests : DecimalDegreeTest
        {
            private const string Zeros292 = "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
            private const string Zeros323 = Zeros292 + "0000000000000000000000000000000";

            [TestCase("+0", 0.0)]
            [TestCase("+0." + Zeros323 + "49406564584124654", double.Epsilon)]
            [TestCase("+17976931348623157" + Zeros292, double.MaxValue)]
            [TestCase("-0", -0.0)]
            [TestCase("-0." + Zeros323 + "49406564584124654", -double.Epsilon)]
            [TestCase("-17976931348623157" + Zeros292, double.MinValue)]
            public void ShouldParseMaximumRanges(string value, double expected)
            {
                int index = 0;
                bool result = DecimalDegree.Parse(value, ref index, out double parsed);

                Assert.That(result, Is.True);
                Assert.That(index, Is.EqualTo(value.Length));
                Assert.That(parsed, Is.EqualTo(expected));
            }

            [TestCase("1", 1.0)]
            [TestCase(".2", 0.2)]
            [TestCase("1e1", 10.0)]
            [TestCase(".2e1", 2.0)]
            [TestCase("1.2e1", 12.0)]
            [TestCase("+1", 1.0)]
            [TestCase("+.2", 0.2)]
            [TestCase("+1e1", 10.0)]
            [TestCase("+.2e1", 2.0)]
            [TestCase("+1.2e1", 12.0)]
            [TestCase("+1e+1", 10.0)]
            [TestCase("+.2e+1", 2.0)]
            [TestCase("+1.2e+1", 12.0)]
            [TestCase("010.0", 10.0)]
            [TestCase("0.010", 0.01)]
            [TestCase("10.01", 10.01)]
            public void ShouldParseValidFormats(string value, double expected)
            {
                int index = 0;
                bool result = DecimalDegree.Parse(value, ref index, out double parsed);

                Assert.That(result, Is.True);
                Assert.That(index, Is.EqualTo(value.Length));
                Assert.That(parsed, Is.EqualTo(expected));
            }

            [Test]
            public void ShouldReadAsMuchAsPossible()
            {
                int index = 1;
                bool result = DecimalDegree.Parse(",123,", ref index, out double parsed);

                Assert.That(result, Is.True);
                Assert.That(index, Is.EqualTo(4));
                Assert.That(parsed, Is.EqualTo(123));
            }

            [TestCase("123.456789012345678")]
            [TestCase("123456789012345678")]
            [TestCase("123456789012345678.901")]
            [TestCase("-123.456789012345678")]
            [TestCase("-123456789012345678")]
            [TestCase("-123456789012345678.901")]
            public void ShouldTruncateInsignificantDigits(string value)
            {
                int index = 0;
                bool result = DecimalDegree.Parse(value, ref index, out double parsed);

                Assert.That(result, Is.True);
                Assert.That(parsed, Is.EqualTo(double.Parse(value, NumberFormatInfo.InvariantInfo)));
            }
        }
    }
}
