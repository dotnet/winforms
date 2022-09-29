using Xunit;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests
{
    public class ListViewInsertionMarkVisualStylesOffTests : ControlTestBase
    {
        public ListViewInsertionMarkVisualStylesOffTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper, enableVisualStyles: false)
        {
        }

        [WinFormsFact]
        public unsafe void ListViewInsertionMark_VisualStylesOff_NotAvailable()
        {
            Assert.False(Application.UseVisualStyles);
        }
    }
}
