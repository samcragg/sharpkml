using NUnit.Framework;
using SharpKml.Base;

namespace NUintTests.Base
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
            Assert.AreEqual(OpaqueBlack, color.Abgr);

            // Verify construction from a bunch of RGBA bytes.
            color = new Color32(0xff, 0x00, 0x00, 0xff);
            Assert.AreEqual(OpaqueRed, color.Abgr);

            // Verify Clone
            Color32 other = new Color32(OpaqueBlue);
            color = other.Clone();
            Assert.AreEqual(OpaqueBlue, color.Abgr);

            // Verify parse from a string using mixed case.
            color = Color32.Parse("ff0000FF");
            Assert.AreEqual(OpaqueRed, color.Abgr);


            // Verify correct behaviour with poorly formed string data.
            //
            // Any string supplied that is less than 8 chars is filled from the front
            // with zeros (and will thus be completely transparent).

            // An fully empty string initalizes to all zeroes (transparent black).
            color = Color32.Parse(string.Empty);
            Assert.AreEqual("00000000", color.ToString());

            color = Color32.Parse("ffffff");
            Assert.AreEqual("00ffffff", color.ToString());

            color = Color32.Parse("ff");
            Assert.AreEqual("000000ff", color.ToString());

            // Only the first eight chars are used for construction from string. Extra
            // chars at the end of the input string are ignored.
            color = Color32.Parse("aabbccddee");
            Assert.AreEqual("aabbccdd", color.ToString());

            // The input string here has two valid hex values in the first eight chars.
            // ( the "a" and "c" in "Not a c") and those are the only chars that
            // won't be replaced with zeroes.
            color = Color32.Parse("Not a color value");
            Assert.AreEqual("0000a0c0", color.ToString());
        }

        [Test]
        public void TestGetSet()
        {
            // Verify getters of default state.
            Color32 color = new Color32();
            byte value = 0x00;
            Assert.AreEqual(value, color.Alpha);
            Assert.AreEqual(value, color.Blue);
            Assert.AreEqual(value, color.Green);
            Assert.AreEqual(value, color.Red);

            // Verify getters of newly set state.
            value = 0xAB;
            color = new Color32(value, value, value, value);
            ////color.Alpha = value;
            ////color.Blue = value;
            ////color.Green = value;
            ////color.Red = value;
            Assert.AreEqual(value, color.Alpha);
            Assert.AreEqual(value, color.Blue);
            Assert.AreEqual(value, color.Green);
            Assert.AreEqual(value, color.Red);

            // Verify ABGR and ARGB.
            color = new Color32(OpaqueRed);
            Assert.AreEqual(OpaqueRed, color.Abgr);
            Assert.AreEqual(OpaqueBlue, color.Argb); // Red and blue have switched?
        }

        [Test]
        public void TestOperators()
        {
            Color32 black = new Color32(OpaqueBlack);
            Color32 green = new Color32(OpaqueGreen);

            // Inequality operator.
            Assert.IsTrue(black != green);
            Assert.IsTrue(green != black);
            Assert.IsTrue(black != null);

            // Equality operator.
            Assert.IsTrue(black == new Color32(OpaqueBlack));
            Assert.IsTrue(green == new Color32(OpaqueGreen));

            // Greater-than operator.
            Assert.IsTrue(green > black);

            // Less-than operator.
            Assert.IsTrue(black < green);
        }
    }
}
