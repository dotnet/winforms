using Xunit.Abstractions;

namespace System.Windows.Forms.UITests
{
    public class ApplicationVisualStylesOffTests : ControlTestBase
    {
        public ApplicationVisualStylesOffTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper, enableVisualStyles: false)
        {
        }

#if VISUAL_STYLES_OFF

        [Fact]
        public void Application_VisualStylesOff_EnableVisualStyles_Success()
        {
            Assert.False(Application.UseVisualStyles);
            Assert.False(Application.RenderWithVisualStyles);
        }

#endif

    }
}
