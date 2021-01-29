using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tesseract.Tests
{
	[TestClass]
	public class BaseApiTests: TesseractTestBase
	{
		[TestMethod]
		public void GetVersion_Is500()
		{
			using (var engine = CreateEngine())
			{
				var version = TesseractEngine.Version;
				Assert.IsTrue(version.StartsWith("5.0.0"));
			}
		}
	}
}