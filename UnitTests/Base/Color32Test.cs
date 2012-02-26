using NUnit.Framework;
using SharpKml.Base;

namespace UnitTests.Base
{
    [TestFixture]
    public class Color32Test
    {
        private const int OpaqueWhite = unchecked((int)0xFFFFFFFF);
        private const int OpaqueBlack = unchecked((int)0xFF000000);
        private const int OpaqueBlue = unchecked((int)0xFFFF0000);
        private const int OpaqueGreen = unchecked((int)0xFF00FF00);
        private const int OpaqueRed = unchecked((int)0xFF0000FF);

        [Test]
        public void TestConstruction()
        {
            // Verify construction from an int.
            Color32 color = new Color32(OpaqueBlack);
            Assert.That(color.Abgr, Is.EqualTo(OpaqueBlack));

            // Verify construction from a bunch of RGBA bytes.
            color = new Color32(0xff, 0x00, 0x00, 0xff);
            Assert.That(color.Abgr, Is.EqualTo(OpaqueRed));

            // Verify Clone
            Color32 other = new Color32(OpaqueBlue);
            color = other.Clone();
            Assert.That(color.Abgr, Is.EqualTo(OpaqueBlue));

            // Verify parse from a string using mixed case.
            color = Color32.Parse("ff0000FF");
            Assert.That(color.Abgr, Is.EqualTo(OpaqueRed));


            // Verify correct behaviour with poorly formed string data.
            //
            // Any string supplied that is less than 8 chars is filled from the front
            // with zeros (and will thus be completely transparent).

            // An fully empty string initalizes to all zeroes (transparent black).
            color = Color32.Parse(string.Empty);
            Assert.That(color.ToString(), Is.EqualTo("00000000"));

            color = Color32.Parse("ffffff");
            Assert.That(color.ToString(), Is.EqualTo("00ffffff"));

            color = Color32.Parse("ff");
            Assert.That(color.ToString(), Is.EqualTo("000000ff"));

            // Only the first eight chars are used for construction from string. Extra
            // chars at the end of the input string are ignored.
            color = Color32.Parse("aabbccddee");
            Assert.That(color.ToString(), Is.EqualTo("aabbccdd"));

            // The input string here has two valid hex values in the first eight chars.
            // ( the "a" and "c" in "Not a c") and those are the only chars that
            // won't be replaced with zeroes.
            color = Color32.Parse("Not a color value");
            Assert.That(color.ToString(), Is.EqualTo("0000a0c0"));
        }

        [Test]
        public void TestGetSet()
        {
            // Verify getters of default state.
            Color32 color = new Color32();
            byte value = 0x00;
            Assert.That(color.Alpha, Is.EqualTo(value));
            Assert.That(color.Blue, Is.EqualTo(value));
            Assert.That(color.Green, Is.EqualTo(value));
            Assert.That(color.Red, Is.EqualTo(value));

            // Verify getters of newly set state.
            value = 0xAB;
            color = new Color32(value, value, value, value);
            Assert.That(color.Alpha, Is.EqualTo(value));
            Assert.That(color.Blue, Is.EqualTo(value));
            Assert.That(color.Green, Is.EqualTo(value));
            Assert.That(color.Red, Is.EqualTo(value));

            // Verify ABGR and ARGB.
            color = new Color32(OpaqueRed);
            Assert.That(color.Abgr, Is.EqualTo(OpaqueRed));
            Assert.That(color.Argb, Is.EqualTo(OpaqueBlue)); // Red and blue have switched?
        }

        [Test]
        public void TestOperators()
        {
            Color32 black = new Color32(OpaqueBlack);
            Color32 green = new Color32(OpaqueGreen);

            // Inequality operator.
            Assert.True(black != green);
            Assert.True(green != black);
            Assert.True(black != null);

            // Equality operator.
            Assert.True(black == new Color32(OpaqueBlack));
            Assert.True(green == new Color32(OpaqueGreen));

            // Greater-than operator.
            Assert.True(green > black);
            Assert.False(black > green);

            // Less-than operator.
            Assert.True(black < green);
            Assert.False(green < black);
        }
    }
}
