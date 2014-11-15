using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using NUnit.Framework;
using SharpKml.Base;

namespace UnitTests.Dom
{
    [TestFixture]
    public sealed class PointTest
    {
        [Test]
        public void TestParseAltitudeMode()
        {
            PartialTrustTestHelper.Run<TestParseAltitudeModeClass>();
        }

        public class TestParseAltitudeModeClass : PartialTrustTest
        {
            public override void Run()
            {
                const string PointKml =
@"<Point>
    <coordinates>-90.86948943473118,48.25450093195546</coordinates>
  </Point>";

                Parser parser = new Parser();
                parser.ParseString(PointKml, namespaces: false);
                Assert.That(parser.Root, Is.Not.Null);
            }
        }
    }
}
