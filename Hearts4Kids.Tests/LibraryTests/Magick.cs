using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hearts4Kids.Tests.Samples.MagickNET;

namespace Hearts4Kids.Tests.LibraryTests
{
    [TestClass]
    public class Magick
    {
        [TestMethod]
        public void TestLog()
        {
            DetailedDebugInformationSamples.ReadImage();
        }
    }
}
