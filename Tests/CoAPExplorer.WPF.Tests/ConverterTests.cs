using System;
using System.Globalization;
using CoAPExplorer.WPF.Converters;
using NUnit.Framework;


namespace CoAPExplorer.WPF.Tests
{
    [TestFixture]
    public class ConverterTests
    {
        [TestCase(new byte[] { 0x12, 0x34 }, 2, ExpectedResult = "12 34")]
        [TestCase(new byte[] { 0x12, 0x34 }, 4, ExpectedResult = "12 34 ")]
        [TestCase(new byte[] { 0x1}, 4, ExpectedResult = "1")]
        [TestCase(new byte[] { 0x12 }, 4, ExpectedResult = "12 ")]
        [TestCase(new byte[] { }, 4, ExpectedResult = "")]
        [TestCase(new byte[] { 0x0 }, 1, ExpectedResult = "0")]
        [TestCase(new byte[] { 0x15, 0x1F }, 1, ExpectedResult = "15")]
        public string HexToAsciiConverter_Convert(byte[] data, int maxBytes)
        {
            var converter = new HextoAsciiConverter();

            return converter.Convert(data, typeof(string), maxBytes, CultureInfo.CurrentCulture);
        }

        [TestCase("12 34", 2, ExpectedResult = new byte[] { 0x12, 0x34 })]
        [TestCase("12 34 ", 4, ExpectedResult = new byte[] { 0x12, 0x34 })]
        [TestCase("1", 4, ExpectedResult = new byte[] { 0x1 })]
        [TestCase("12 ", 4, ExpectedResult = new byte[] { 0x12 })]
        [TestCase("", 4, ExpectedResult = new byte[] { })]
        [TestCase("0", 1, ExpectedResult = new byte[] { 0x0 })]
        [TestCase("15 1F", 1, ExpectedResult = new byte[] { 0x15 })]
        public byte[] HexToAsciiConverter_ConvertBack(string data, int maxBytes)
        {
            var converter = new HextoAsciiConverter();

            return converter.ConvertBack(data, typeof(string), maxBytes, CultureInfo.CurrentCulture);
        }
    }
}
