namespace UnitTests.IntegrationTests
{
    using NUnit.Framework;
    using SharpKml.Dom.GX;

    public sealed class SimpleArrayDataTests : DomSerialization
    {
        [Test]
        public void TestSerialization()
        {
            const string Kml =
@"<gx:SimpleArrayData xmlns:gx=""http://www.google.com/kml/ext/2.2"" name=""cadence"">
  <gx:value>86</gx:value>
  <gx:value>103</gx:value>
  <gx:value>108</gx:value>
  <gx:value>113</gx:value>
  <gx:value>113</gx:value>
</gx:SimpleArrayData>";

            SimpleArrayData parsed = this.Parse<SimpleArrayData>(Kml);

            Assert.That(parsed.Name, Is.EqualTo("cadence"));
            Assert.That(parsed.Values, Is.EqualTo(new[] { "86", "103", "108", "113", "113" }));
        }
    }
}
