// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.VisualStyles;
using Microsoft.DotNet.RemoteExecutor;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ApplicationTests
    {
        [WinFormsFact]
        public void Application_EnableVisualStyles_InvokeBeforeGettingRenderWithVisualStyles_Success()
        {
            RemoteExecutor.Invoke(() =>
            {
                Application.EnableVisualStyles();
                Assert.True(Application.UseVisualStyles);
                Assert.True(Application.RenderWithVisualStyles);
            }).Dispose();
        }

        [Fact]
        public void Application_OpenForms_Get_ReturnsExpected()
        {
            FormCollection forms = Application.OpenForms;
            Assert.Same(forms, Application.OpenForms);
        }

        [Fact]
        public void Application_VisualStyleState_Get_ReturnsExpected()
        {
            VisualStyleState state = Application.VisualStyleState;
            Assert.True(Enum.IsDefined(typeof(VisualStyleState), state));
            Assert.Equal(state, Application.VisualStyleState);
        }

        [Theory(Skip = "Deadlock, see: https://github.com/dotnet/winforms/issues/2192")]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(VisualStyleState))]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(VisualStyleState))]
        public void Application_VisualStyleState_Set_ReturnsExpected(VisualStyleState value)
        {
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
