// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.VisualStyles;
using Microsoft.DotNet.RemoteExecutor;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ApplicationTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void Application_EnableVisualStyles_InvokeBeforeGettingRenderWithVisualStyles_Success()
        {
            using RemoteInvokeHandle invokerHandle = RemoteExecutor.Invoke(() =>
            {
                Application.EnableVisualStyles();
                Assert.True(Application.UseVisualStyles);
                Assert.True(Application.RenderWithVisualStyles);
            });

            // verify the remote process succeeded
            Assert.Equal(0, invokerHandle.ExitCode);
        }

        [WinFormsFact]
        public void Application_EnableVisualStyles_InvokeAfterGettingRenderWithVisualStyles_Success()
        {
            // This is not a recommended scenario per https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.application.enablevisualstyles
            // EnableVisualStyles should be executed before any control-related code is.
            using RemoteInvokeHandle invokerHandle = RemoteExecutor.Invoke(() =>
            {
                Assert.False(Application.UseVisualStyles);
                Assert.False(Application.RenderWithVisualStyles);

                Application.EnableVisualStyles();
                Assert.True(Application.UseVisualStyles, "New Visual Styles will not be applied on Winforms app. This is a high priority bug and must be looked into");
                Assert.True(Application.RenderWithVisualStyles);
            });

            // verify the remote process succeeded
            Assert.Equal(0, invokerHandle.ExitCode);
        }

        [WinFormsFact]
        public void Application_OpenForms_Get_ReturnsExpected()
        {
            FormCollection forms = Application.OpenForms;
            Assert.Same(forms, Application.OpenForms);
        }

        [WinFormsFact]
        public void Application_VisualStyleState_Get_ReturnsExpected()
        {
            VisualStyleState state = Application.VisualStyleState;
            Assert.True(Enum.IsDefined(typeof(VisualStyleState), state));
            Assert.Equal(state, Application.VisualStyleState);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(VisualStyleState))]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(VisualStyleState))]
        public void Application_VisualStyleState_Set_ReturnsExpected(VisualStyleState valueParam)
        {
            // This needs to be in RemoteExecutor.Invoke because changing Application.VisualStyleState
            // sends WM_THEMECHANGED to all controls, which can cause a deadlock if another test fails.
            using RemoteInvokeHandle invokerHandle = RemoteExecutor.Invoke((valueString) =>
            {
                VisualStyleState value = Enum.Parse<VisualStyleState>(valueString);
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
            }, valueParam.ToString());

            // verify the remote process succeeded
            Assert.Equal(0, invokerHandle.ExitCode);
        }
    }
}
