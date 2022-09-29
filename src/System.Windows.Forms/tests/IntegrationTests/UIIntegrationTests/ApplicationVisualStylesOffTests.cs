using Xunit;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests
{
    public class ApplicationVisualStylesOffTests : ControlTestBase
    {
        public ApplicationVisualStylesOffTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper, enableVisualStyles: false)
        {
        }

        [Fact]
        public void Application_VisualStylesOff_EnableVisualStyles_Success()
        {
            // Typically, EnableVisualStyles() is the first line in the Main function.
            Application.EnableVisualStyles();
            Assert.True(Application.UseVisualStyles, "New Visual Styles will not be applied on Winforms app. This is a high priority bug and must be looked into");
            Assert.True(Application.RenderWithVisualStyles);
        }
    }
}
