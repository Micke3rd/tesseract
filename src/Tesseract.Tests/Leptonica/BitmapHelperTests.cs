using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tesseract.Tests.Leptonica
{
	[TestClass]
	public class BitmapHelperTests
	{
		[TestMethod]
		public void ConvertRgb555ToPixColor()
		{
			ushort originalVal = 0x39EC;
			var convertedValue = BitmapHelper.ConvertRgb555ToRGBA(originalVal);
			Assert.AreEqual(convertedValue,0x737B63FF);
		}

		[DataTestMethod]
		[DataRow(0xB9EC,0x737B63FF)]
		[DataRow(0x39EC,0x737B6300)]
		public void ConvertArgb555ToPixColor(int originalVal,int expectedVal)
		{
			var convertedValue = BitmapHelper.ConvertArgb1555ToRGBA((ushort)originalVal);
			Assert.AreEqual(convertedValue,(uint)expectedVal);
		}

		[TestMethod]
		public void ConvertRgb565ToPixColor()
		{
			ushort originalVal = 0x73CC;
			var convertedValue = BitmapHelper.ConvertRgb565ToRGBA(originalVal);
			Assert.AreEqual(convertedValue,0x737963FF);
		}
	}
}
