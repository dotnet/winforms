using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ApplicationTests
    {
        [Fact]
        public void Application_EnsureVisualStylesAreEnabled()
        {
            Application.EnableVisualStyles();
            Assert.True(Application.UseVisualStyles, "New Visual Styles will not be applied on Winforms app. This is a high priority bug and must be looked into");            
        }
    }
}
