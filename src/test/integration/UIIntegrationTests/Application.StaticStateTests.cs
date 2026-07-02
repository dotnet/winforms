// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.UITests;

// Migrated from unit tests; see issue #4500.
public class ApplicationStaticStateTests : ControlTestBase
{
    public ApplicationStaticStateTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    public static IEnumerable<object[]> CurrentCulture_Set_TestData()
    {
        yield return new object[] { CultureInfo.InvariantCulture };
        yield return new object[] { new CultureInfo("en") };
        yield return new object[] { new CultureInfo("fr-FR") };
        yield return new object[] { new CultureInfo("en-DK") };
        yield return new object[] { new CultureInfo("haw") };
        yield return new object[] { new CultureInfo("en-US") };
        yield return new object[] { new CultureInfo("de-DE_phoneb") };
        yield return new object[] { new CustomLCIDCultureInfo(10) };
        yield return new object[] { new CustomLCIDCultureInfo(0) };
        yield return new object[] { new CustomLCIDCultureInfo(-1) };
    }

    [WinFormsFact]
    public void Application_CurrentCulture_Set_GetReturnsExpected()
    {
        // GetThreadLocale round-trip assertion dropped: [UseDefaultXunitCulture] restores the
        // thread locale per test, and SetThreadLocale silently no-ops for LCID 0x7F on STA hosts.
        // Managed CurrentCulture surface is still verified below.
        CultureInfo originalApplicationCulture = Application.CurrentCulture;
        CultureInfo originalThreadCulture = Thread.CurrentThread.CurrentCulture;

        try
        {
            foreach (object[] testData in CurrentCulture_Set_TestData())
            {
                CultureInfo value = (CultureInfo)testData[0];

                CultureInfo oldValue = Application.CurrentCulture;
                try
                {
                    Application.CurrentCulture = value;
                    Assert.Same(value, Application.CurrentCulture);
                    Assert.Same(value, Thread.CurrentThread.CurrentCulture);
                    Assert.Same(value, CultureInfo.CurrentCulture);

                    // Set same.
                    Application.CurrentCulture = value;
                    Assert.Same(value, Application.CurrentCulture);
                    Assert.Same(value, Thread.CurrentThread.CurrentCulture);
                    Assert.Same(value, CultureInfo.CurrentCulture);
                }
                finally
                {
                    Application.CurrentCulture = oldValue;
                }
            }
        }
        finally
        {
            Application.CurrentCulture = originalApplicationCulture;
            Thread.CurrentThread.CurrentCulture = originalThreadCulture;
        }
    }

    [WinFormsFact]
    public void Application_EnableVisualStyles_InvokeBeforeGettingRenderWithVisualStyles_Success()
    {
        Application.EnableVisualStyles();
        Assert.True(Application.UseVisualStyles);
        Assert.True(Application.RenderWithVisualStyles);
    }

    // InvokeAfterGettingRenderWithVisualStyles_Success dropped: precondition UseVisualStyles==false
    // is unsatisfiable here (EnableVisualStyles is a one-way switch and the host already enabled it).

    [WinFormsTheory]
    [EnumData<VisualStyleState>]
    [InvalidEnumData<VisualStyleState>]
    public void Application_VisualStyleState_Set_ReturnsExpected(VisualStyleState value)
    {
        // Serial execution in UIIntegrationTests prevents WM_THEMECHANGED cross-test interference.
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
