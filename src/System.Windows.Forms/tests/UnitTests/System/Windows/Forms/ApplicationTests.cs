// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms.VisualStyles;
using Microsoft.DotNet.RemoteExecutor;

namespace System.Windows.Forms.Tests;

public class ApplicationTests
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
                    Assert.Equal(expectedLcid, PInvoke.GetThreadLocale());

                    // Set same.
                    Application.CurrentCulture = value;
                    Assert.Same(value, Application.CurrentCulture);
                    Assert.Same(value, Thread.CurrentThread.CurrentCulture);
                    Assert.Same(value, CultureInfo.CurrentCulture);
                    Assert.Equal(expectedLcid, PInvoke.GetThreadLocale());
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
        Assert.True(Enum.IsDefined(state));
        Assert.Equal(state, Application.VisualStyleState);
    }

    [WinFormsTheory(Skip = "Crash with AbandonedMutexException. See: https://github.com/dotnet/arcade/issues/5325")]
    [EnumData<VisualStyleState>]
    [InvalidEnumData<VisualStyleState>]
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

    [Fact]
    public void Application_EnableVisualStyles_ManifestResourceExists()
    {
        // Check to make sure the manifest we use for single file publishing is present
        using Stream stream = typeof(Application).Module.Assembly.GetManifestResourceStream(
            "System.Windows.Forms.XPThemes.manifest");
        Assert.NotNull(stream);
    }

    [WinFormsFact]
    public void Application_DefaultFont_ReturnsNull_IfNoFontSet()
    {
        var applicationTestAccessor = typeof(Application).TestAccessor().Dynamic;
        Assert.Null(applicationTestAccessor.s_defaultFont);
        Assert.Null(applicationTestAccessor.s_defaultFontScaled);
        Assert.Null(Application.DefaultFont);
    }

    [WinFormsFact]
    public void Application_DefaultFont_Returns_DefaultFont_IfNotScaled()
    {
        var applicationTestAccessor = typeof(Application).TestAccessor().Dynamic;
        Assert.Null(applicationTestAccessor.s_defaultFont);
        Assert.Null(applicationTestAccessor.s_defaultFontScaled);

        Font customFont = (Font)SystemFonts.CaptionFont.Clone();
        try
        {
            applicationTestAccessor.s_defaultFont = customFont;

            AreFontEqual(customFont, Application.DefaultFont);
        }
        finally
        {
            customFont.Dispose();
            applicationTestAccessor.s_defaultFont = null;
            applicationTestAccessor.s_defaultFontScaled?.Dispose();
            applicationTestAccessor.s_defaultFontScaled = null;
        }
    }

    [WinFormsFact]
    public void Application_DefaultFont_Returns_ScaledDefaultFont_IfScaled()
    {
        var applicationTestAccessor = typeof(Application).TestAccessor().Dynamic;
        Assert.Null(applicationTestAccessor.s_defaultFont);
        Assert.Null(applicationTestAccessor.s_defaultFontScaled);

        Font font = new Font(new FontFamily("Arial"), 12f);
        Font scaled = new Font(new FontFamily("Arial"), 16f);
        try
        {
            applicationTestAccessor.s_defaultFont = font;
            applicationTestAccessor.s_defaultFontScaled = scaled;

            AreFontEqual(scaled, Application.DefaultFont);
        }
        finally
        {
            applicationTestAccessor.s_defaultFont = null;
            applicationTestAccessor.s_defaultFontScaled = null;
            font.Dispose();
            scaled.Dispose();
        }
    }

    [WinFormsFact]
    public void Application_ScaleDefaultFont_Disposes_ScaleDefaultFont_IfExists()
    {
        var applicationTestAccessor = typeof(Application).TestAccessor().Dynamic;
        Assert.Null(applicationTestAccessor.s_defaultFont);
        Assert.Null(applicationTestAccessor.s_defaultFontScaled);

        Font font = new Font(new FontFamily("Arial"), 12f);
        Font scaled = new Font(new FontFamily("Arial"), 16f);
        try
        {
            applicationTestAccessor.s_defaultFont = font;
            applicationTestAccessor.s_defaultFontScaled = scaled;

            AreFontEqual(scaled, Application.DefaultFont);

            Application.ScaleDefaultFont(DpiHelper.MinTextScaleFactorValue);

            // The font is not scaled at 100% (factor=1.0)
            Assert.Null(applicationTestAccessor.s_defaultFontScaled);
        }
        finally
        {
            applicationTestAccessor.s_defaultFont = null;
            font.Dispose();
            scaled.Dispose();
        }
    }

    [WinFormsFact]
    public void Application_ScaleDefaultFont_Should_Not_Rescale_IfTextScaleFactorLessThan1()
    {
        var applicationTestAccessor = typeof(Application).TestAccessor().Dynamic;
        Assert.Null(applicationTestAccessor.s_defaultFont);
        Assert.Null(applicationTestAccessor.s_defaultFontScaled);

        Font font = new Font(new FontFamily("Arial"), 12f);
        try
        {
            applicationTestAccessor.s_defaultFont = font;

            Application.ScaleDefaultFont(0f);

            // The font is not scaled at 100% (factor=1.0)
            Assert.Null(applicationTestAccessor.s_defaultFontScaled);
        }
        finally
        {
            applicationTestAccessor.s_defaultFont = null;
            font.Dispose();
        }
    }

    [WinFormsFact]
    public void Application_ScaleDefaultFont_Should_Rescale_IfTextScaleFactorGreaterThan1()
    {
        var applicationTestAccessor = typeof(Application).TestAccessor().Dynamic;
        Assert.Null(applicationTestAccessor.s_defaultFont);
        Assert.Null(applicationTestAccessor.s_defaultFontScaled);

        Font font = new Font(new FontFamily("Arial"), 12f);
        try
        {
            applicationTestAccessor.s_defaultFont = font;

            Application.ScaleDefaultFont(2.0f);

            Assert.NotNull(applicationTestAccessor.s_defaultFontScaled);
            Assert.Equal(12f * 2, applicationTestAccessor.s_defaultFontScaled.SizeInPoints);
        }
        finally
        {
            applicationTestAccessor.s_defaultFont = null;
            applicationTestAccessor.s_defaultFontScaled.Dispose();
            applicationTestAccessor.s_defaultFontScaled = null;
            font.Dispose();
        }
    }

    [WinFormsFact]
    public void Application_ScaleDefaultFont_Should_Cap_Rescale_ToTextScaleFactor225()
    {
        var applicationTestAccessor = typeof(Application).TestAccessor().Dynamic;
        Assert.Null(applicationTestAccessor.s_defaultFont);
        Assert.Null(applicationTestAccessor.s_defaultFontScaled);

        Font font = new Font(new FontFamily("Arial"), 12f);
        try
        {
            applicationTestAccessor.s_defaultFont = font;

            Application.ScaleDefaultFont(4.0f);

            Assert.NotNull(applicationTestAccessor.s_defaultFontScaled);
            Assert.Equal(12f * DpiHelper.MaxTextScaleFactorValue, applicationTestAccessor.s_defaultFontScaled.SizeInPoints);
        }
        finally
        {
            applicationTestAccessor.s_defaultFont = null;
            applicationTestAccessor.s_defaultFontScaled.Dispose();
            applicationTestAccessor.s_defaultFontScaled = null;
            font.Dispose();
        }
    }

    [WinFormsFact]
    public void Application_ScaleDefaultFont_Should_Rescale_FontCorrectly()
    {
        var applicationTestAccessor = typeof(Application).TestAccessor().Dynamic;
        Assert.Null(applicationTestAccessor.s_defaultFont);
        Assert.Null(applicationTestAccessor.s_defaultFontScaled);

        Font font = new Font(new FontFamily("Arial"), 12f, FontStyle.Italic | FontStyle.Underline, GraphicsUnit.Pixel);
        Font scaled = new Font(new FontFamily("Arial"), 18f, FontStyle.Italic | FontStyle.Underline, GraphicsUnit.Pixel);
        try
        {
            applicationTestAccessor.s_defaultFont = font;

            Application.ScaleDefaultFont(1.5f);

            Assert.NotNull(applicationTestAccessor.s_defaultFontScaled);
            AreFontEqual(scaled, applicationTestAccessor.s_defaultFontScaled);
        }
        finally
        {
            applicationTestAccessor.s_defaultFont = null;
            applicationTestAccessor.s_defaultFontScaled = null;
            font.Dispose();
            scaled.Dispose();
        }
    }

    [WinFormsFact]
    public void Application_SetDefaultFont_SetNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("font", () => Application.SetDefaultFont(null));
    }

    [WinFormsFact]
    public void Application_SetDefaultFont_AfterHandleCreated_InvalidOperationException()
    {
        using var control = new Control();
        var window = new NativeWindow();
        window.AssignHandle(control.Handle);

        Assert.Throws<InvalidOperationException>(() => Application.SetDefaultFont(SystemFonts.CaptionFont));
    }

    [WinFormsFact]
    public void Application_SetDefaultFont_MustCloneSystemFont()
    {
        var applicationTestAccessor = typeof(Application).TestAccessor().Dynamic;
        Assert.Null(applicationTestAccessor.s_defaultFont);
        Assert.Null(applicationTestAccessor.s_defaultFontScaled);

        Assert.True(SystemFonts.CaptionFont.IsSystemFont);

        // This a unholy, but generally at this stage NativeWindow.AnyHandleCreated=true,
        // And we won't be able to set the font, unless we flip the bit
        var nativeWindowTestAccessor = typeof(NativeWindow).TestAccessor().Dynamic;
        bool currentAnyHandleCreated = nativeWindowTestAccessor.t_anyHandleCreated;

        try
        {
            nativeWindowTestAccessor.t_anyHandleCreated = false;

            Application.SetDefaultFont(SystemFonts.CaptionFont);

            Assert.False(applicationTestAccessor.s_defaultFont.IsSystemFont);
        }
        finally
        {
            // Flip the bit back
            nativeWindowTestAccessor.t_anyHandleCreated = currentAnyHandleCreated;

            applicationTestAccessor.s_defaultFont.Dispose();
            applicationTestAccessor.s_defaultFontScaled?.Dispose();
            applicationTestAccessor.s_defaultFont = null;
            applicationTestAccessor.s_defaultFontScaled = null;
        }
    }

    [WinFormsTheory]
    [InvalidEnumData<HighDpiMode>]
    public void Application_SetHighDpiMode_SetInvalidValue_ThrowsInvalidEnumArgumentException(HighDpiMode value)
    {
        Assert.Throws<InvalidEnumArgumentException>("highDpiMode", () => Application.SetHighDpiMode(value));
    }

    private static void AreFontEqual(Font expected, Font actual)
    {
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.SizeInPoints, actual.SizeInPoints);
        Assert.Equal(expected.GdiCharSet, actual.GdiCharSet);
        Assert.Equal(expected.Style, actual.Style);
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
