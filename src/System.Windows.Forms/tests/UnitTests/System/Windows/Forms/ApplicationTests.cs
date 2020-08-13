// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows.Forms.VisualStyles;
using Microsoft.DotNet.RemoteExecutor;
using WinForms.Common.Tests;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ApplicationTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void Application_CurrentCulture_Get_ReturnsExpected()
        {
            Assert.Same(Thread.CurrentThread.CurrentCulture, Application.CurrentCulture);
        }

        public static IEnumerable<object[]> CurrentCulture_Set_TestData()
        {
            yield return new object[] { CultureInfo.InvariantCulture, 0x7Fu };
            yield return new object[] { new CultureInfo("en"), 0x9u };
            yield return new object[] { new CultureInfo("fr-FR"), 0x40Cu };
            yield return new object[] { new CultureInfo("en-DK"), 0xC00u };
            yield return new object[] { new CultureInfo("haw"), 0x00000075u };
            yield return new object[] { new CultureInfo("en-US"), 0x00000409u };
            yield return new object[] { new CultureInfo("de-DE_phoneb"), 0x00010407u };
            yield return new object[] { new CustomLCIDCultureInfo(10), 0x409u };
            yield return new object[] { new CustomLCIDCultureInfo(0), 0x409u };
            yield return new object[] { new CustomLCIDCultureInfo(-1), 0x409u };
        }

        [WinFormsFact(Skip = "Crash with AbandonedMutexException. See: https://github.com/dotnet/arcade/issues/5325")]
        public void Application_CurrentCulture_Set_GetReturnsExpected()
        {
            RemoteExecutor.Invoke(() =>
            {
                foreach (object[] testData in CurrentCulture_Set_TestData())
                {
                    CultureInfo value = (CultureInfo)testData[0];
                    uint expectedLcid = (uint)testData[1];

                    CultureInfo oldValue = Application.CurrentCulture;
                    try
                    {
                        Application.CurrentCulture = value;
                        Assert.Same(value, Application.CurrentCulture);
                        Assert.Same(value, Thread.CurrentThread.CurrentCulture);
                        Assert.Same(value, CultureInfo.CurrentCulture);
                        Assert.Equal(expectedLcid, Kernel32.GetThreadLocale());

                        // Set same.
                        Application.CurrentCulture = value;
                        Assert.Same(value, Application.CurrentCulture);
                        Assert.Same(value, Thread.CurrentThread.CurrentCulture);
                        Assert.Same(value, CultureInfo.CurrentCulture);
                        Assert.Equal(expectedLcid, Kernel32.GetThreadLocale());
                    }
                    finally
                    {
                        Application.CurrentCulture = oldValue;
                    }
                }
            }).Dispose();
        }

        [WinFormsFact]
        public void Application_CurrentCulture_SetNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("value", () => Application.CurrentCulture = null);
        }

        [WinFormsFact(Skip = "Crash with AbandonedMutexException. See: https://github.com/dotnet/arcade/issues/5325")]
        public void Application_EnableVisualStyles_InvokeBeforeGettingRenderWithVisualStyles_Success()
        {
            RemoteExecutor.Invoke(() =>
            {
                Application.EnableVisualStyles();
                Assert.True(Application.UseVisualStyles);
                Assert.True(Application.RenderWithVisualStyles);
            }).Dispose();
        }

        [WinFormsFact(Skip = "Crash with AbandonedMutexException. See: https://github.com/dotnet/arcade/issues/5325")]
        public void Application_EnableVisualStyles_InvokeAfterGettingRenderWithVisualStyles_Success()
        {
            // This is not a recommended scenario per https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.application.enablevisualstyles
            // EnableVisualStyles should be executed before any control-related code is.
            RemoteExecutor.Invoke(() =>
            {
                Assert.False(Application.UseVisualStyles);
                Assert.False(Application.RenderWithVisualStyles);

                Application.EnableVisualStyles();
                Assert.True(Application.UseVisualStyles, "New Visual Styles will not be applied on Winforms app. This is a high priority bug and must be looked into");
                Assert.True(Application.RenderWithVisualStyles);
            }).Dispose();
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

        [WinFormsTheory(Skip = "Crash with AbandonedMutexException. See: https://github.com/dotnet/arcade/issues/5325")]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(VisualStyleState))]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(VisualStyleState))]
        public void Application_VisualStyleState_Set_ReturnsExpected(VisualStyleState valueParam)
        {
            // This needs to be in RemoteExecutor.Invoke because changing Application.VisualStyleState
            // sends WM_THEMECHANGED to all controls, which can cause a deadlock if another test fails.
            RemoteExecutor.Invoke((valueString) =>
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
        }

        private class CustomLCIDCultureInfo : CultureInfo
        {
            private readonly int _lcid;

            public CustomLCIDCultureInfo(int lcid) : base("en-US")
            {
                _lcid = lcid;
            }

            public override int LCID => _lcid;
        }
    }
}
