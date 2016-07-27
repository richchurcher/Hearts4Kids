using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hearts4Kids.Tests.LibraryTests
{
    [TestClass]
    public class SanitizerTests
    {
        [TestMethod]
        public void TestXSSSanitizer()
        {
            var san = new Ganss.XSS.HtmlSanitizer();
            const string benign = "<p>benign string</p>";
            Assert.AreEqual(benign,san.Sanitize(benign));
        }
    }
}
