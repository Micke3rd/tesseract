using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Tesseract.Tests
{
	[TestClass]
	public class AnalyseResultTests: TesseractTestBase
	{
		private string ResultsDirectory
		{
			get { return TestResultPath(@"Analysis\"); }
		}

		private const string ExampleImagePath = @"Ocr\phototest.tif";

		#region Setup\TearDown

		private TesseractEngine engine;

		[TestCleanup]
		public void Dispose()
		{
			if (engine != null)
			{
				engine.Dispose();
				engine = null;
			}
		}

		[TestInitialize]
		public void Init()
		{
			if (!Directory.Exists(ResultsDirectory)) Directory.CreateDirectory(ResultsDirectory);

			engine = CreateEngine("osd");
		}

		#endregion Setup\TearDown

		#region Tests

		[DataTestMethod]
		[DataRow(null)]
		[DataRow(90f)]
		[DataRow(180f)]
		public void AnalyseLayout_RotatedImage(float? angle)
		{
			var exampleImagePath = TestFilePath("Ocr/phototest.tif");
			using (var img = LoadTestImage(ExampleImagePath))
			{
				using (var rotatedImage = angle.HasValue ? img.Rotate(MathHelper.ToRadians(angle.Value)) : img.Clone())
				{
					rotatedImage.Save(TestResultRunFile(string.Format(@"AnalyseResult\AnalyseLayout_RotateImage_{0}.png",angle)));

					engine.DefaultPageSegMode = PageSegMode.AutoOsd;
					using (var page = engine.Process(rotatedImage))
					{
						using (var pageLayout = page.GetIterator())
						{
							pageLayout.Begin();
							do
							{
								var result = pageLayout.GetProperties();
								Orientation orient;
								float deskew;

								ExpectedOrientation(angle.HasValue ? angle.Value : 0,out orient,out deskew);
								Assert.AreEqual(result.Orientation,orient);

								if (angle.HasValue)
								{
									if (angle == 180f)
									{
										// This isn't correct...
										Assert.AreEqual(result.WritingDirection,WritingDirection.LeftToRight);
										Assert.AreEqual(result.TextLineOrder,TextLineOrder.TopToBottom);
									}
									else if (angle == 90f)
									{
										Assert.AreEqual(result.WritingDirection,WritingDirection.LeftToRight);
										Assert.AreEqual(result.TextLineOrder,TextLineOrder.TopToBottom);
									}
									else
									{
										Assert.Fail("Angle not supported.");
									}
								}
								else
								{
									Assert.AreEqual(result.WritingDirection,WritingDirection.LeftToRight);
									Assert.AreEqual(result.TextLineOrder,TextLineOrder.TopToBottom);
								}
							} while (pageLayout.Next(PageIteratorLevel.Block));
						}
					}
				}
			}
		}

		//[TestMethod]
		//public void CanDetectOrientationForMode(
		//	//[Values(PageSegMode.Auto,
		//	//    PageSegMode.AutoOnly,
		//	//    PageSegMode.AutoOsd,
		//	//    PageSegMode.CircleWord,
		//	//    PageSegMode.OsdOnly,
		//	//    PageSegMode.SingleBlock,
		//	//    PageSegMode.SingleBlockVertText,
		//	//    PageSegMode.SingleChar,
		//	//    PageSegMode.SingleColumn,
		//	//    PageSegMode.SingleLine,
		//	//    PageSegMode.SingleWord)]
		//	//PageSegMode pageSegMode
		//	)
		//{
		//	foreach (var pageSegMode in new[] {
		//	PageSegMode.Auto,
		//		PageSegMode.AutoOnly,
		//		PageSegMode.AutoOsd,
		//		PageSegMode.CircleWord,
		//		PageSegMode.OsdOnly,
		//		PageSegMode.SingleBlock,
		//		PageSegMode.SingleBlockVertText,
		//		PageSegMode.SingleChar,
		//		PageSegMode.SingleColumn,
		//		PageSegMode.SingleLine,
		//		PageSegMode.SingleWord
		//	})
		//		using (var img = LoadTestImage(ExampleImagePath))
		//		{
		//			using (var rotatedPix = img.Rotate((float)Math.PI))
		//			{
		//				using (var page = engine.Process(rotatedPix,pageSegMode))
		//				{
		//					int orientation;
		//					float confidence;
		//					string scriptName;
		//					float scriptConfidence;

		//					page.DetectBestOrientationAndScript(out orientation,out confidence,out scriptName,out scriptConfidence);

		//					Assert.AreEqual(orientation,180);
		//					Assert.AreEqual(scriptName,"Latin");
		//				}
		//			}
		//		}
		//}

		//[DataTestMethod]
		//[DataRow(0)]
		//[DataRow(90)]
		//[DataRow(180)]
		//[DataRow(270)]
		//public void DetectOrientation_Degrees_RotatedImage(int expectedOrientation)
		//{
		//	using (var img = LoadTestImage(ExampleImagePath))
		//	{
		//		using (var rotatedPix = img.Rotate((float)expectedOrientation / 360 * (float)Math.PI * 2))
		//		{
		//			using (var page = engine.Process(rotatedPix,PageSegMode.OsdOnly))
		//			{

		//				int orientation;
		//				float confidence;
		//				string scriptName;
		//				float scriptConfidence;

		//				page.DetectBestOrientationAndScript(out orientation,out confidence,out scriptName,out scriptConfidence);

		//				Assert.AreEqual(orientation,expectedOrientation);
		//				Assert.AreEqual(scriptName,"Latin");
		//			}
		//		}
		//	}
		//}

		//[DataTestMethod]
		//[DataRow(0)]
		//[DataRow(90)]
		//[DataRow(180)]
		//[DataRow(270)]
		//public void DetectOrientation_Legacy_RotatedImage(int expectedOrientationDegrees)
		//{
		//	using (var img = LoadTestImage(ExampleImagePath))
		//	{
		//		using (var rotatedPix = img.Rotate((float)expectedOrientationDegrees / 360 * (float)Math.PI * 2))
		//		{
		//			using (var page = engine.Process(rotatedPix,PageSegMode.OsdOnly))
		//			{
		//				Orientation orientation;
		//				float confidence;

		//				page.DetectBestOrientation(out orientation,out confidence);

		//				Orientation expectedOrientation;
		//				float expectedDeskew;
		//				ExpectedOrientation(expectedOrientationDegrees,out expectedOrientation,out expectedDeskew);

		//				Assert.AreEqual(orientation,expectedOrientation);
		//			}
		//		}
		//	}
		//}


		[TestMethod]
		public void GetImage(
			//[Values(PageIteratorLevel.Block, PageIteratorLevel.Para, PageIteratorLevel.TextLine, PageIteratorLevel.Word, PageIteratorLevel.Symbol)] PageIteratorLevel level,
			//[Values(0, 3)] int padding
			)
		{
			foreach (var level in new[] { PageIteratorLevel.Block, PageIteratorLevel.Para, PageIteratorLevel.TextLine, PageIteratorLevel.Word,
				PageIteratorLevel.Symbol })
				foreach (var padding in new[] { 0,3 })

					using (var img = LoadTestImage(ExampleImagePath))
					{
						using (var page = engine.Process(img))
						{
							using (var pageLayout = page.GetIterator())
							{
								pageLayout.Begin();
								// get symbol
								int x, y;
								using (var elementImg = pageLayout.GetImage(level,padding,out x,out y))
								{
									var elementImgFilename = string.Format(@"AnalyseResult\GetImage\ResultIterator_Image_{0}_{1}_at_({2},{3}).png",level,padding,x,y);

									// TODO: Ensure generated pix is equal to expected pix, only saving it if it's not.
									var destFilename = TestResultRunFile(elementImgFilename);
									elementImg.Save(destFilename,ImageFormat.Png);
								}
							}
						}
					}
		}

		#endregion Tests

		#region Helpers


		private void ExpectedOrientation(float rotation,out Orientation orientation,out float deskew)
		{
			rotation = rotation % 360f;
			rotation = rotation < 0 ? rotation + 360 : rotation;

			if (rotation >= 315 || rotation < 45)
			{
				orientation = Orientation.PageUp;
				deskew = -rotation;
			}
			else if (rotation >= 45 && rotation < 135)
			{
				orientation = Orientation.PageRight;
				deskew = 90 - rotation;
			}
			else if (rotation >= 135 && rotation < 225)
			{
				orientation = Orientation.PageDown;
				deskew = 180 - rotation;
			}
			else if (rotation >= 225 && rotation < 315)
			{
				orientation = Orientation.PageLeft;
				deskew = 270 - rotation;
			}
			else
			{
				throw new ArgumentOutOfRangeException(nameof(rotation));
			}
		}

		private Pix LoadTestImage(string path)
		{
			var fullExampleImagePath = TestFilePath(path);
			return Pix.LoadFromFile(fullExampleImagePath);
		}

		#endregion
	}
}