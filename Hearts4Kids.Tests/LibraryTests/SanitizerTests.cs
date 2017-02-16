using Xunit;

namespace Hearts4Kids.Tests.LibraryTests
{
    public class SanitizerTests
    {
        [Fact]
        public void TestXSSSanitizer()
        {
            var san = new Ganss.XSS.HtmlSanitizer();
            const string benign = "<p>benign string</p>";
            Assert.Equal(benign,san.Sanitize(benign));
        }
    }
}
