// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.TestUtilities;
using System.Windows.Forms.VisualStyles;
using Xunit;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests
{
    public class ApplicationVisualStylesOnTests : ControlTestBase
    {
        public ApplicationVisualStylesOnTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper, enableVisualStyles: true)
        {
        }

        [WinFormsFact]
        public void Application_VisualStylesOn_GetRenderWithVisualStyles_Success()
        {
            Assert.True(Application.UseVisualStyles);
            Assert.True(Application.RenderWithVisualStyles);
        }

        [WinFormsFact]
        public void Application_EnableVisualStyles_InvokeAfterGettingRenderWithVisualStyles_Success()
        {
            Assert.True(Application.UseVisualStyles);
            Assert.True(Application.RenderWithVisualStyles);

            Application.EnableVisualStyles();
            Assert.True(Application.UseVisualStyles, "New Visual Styles will not be applied on Winforms app. This is a high priority bug and must be looked into");
            Assert.True(Application.RenderWithVisualStyles);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(VisualStyleState))]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(VisualStyleState))]
        public void Application_VisualStyleState_Set_ReturnsExpected(VisualStyleState value)
        {
            // This needs to be in UIIntegration because changing Application.VisualStyleState
            // sends WM_THEMECHANGED to all controls, which can cause a deadlock if another test fails.
            VisualStyleState state = Application.VisualStyleState;
            try
            {
                Application.VisualStyleState = value;
                Assert.Equal(value, Application.VisualStyleState);
            }
            finally
            {
                Application.VisualStyleState = state;
            }
        }
    }
}
