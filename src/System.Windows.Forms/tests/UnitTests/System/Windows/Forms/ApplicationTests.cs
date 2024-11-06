// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms.VisualStyles;
using Microsoft.DotNet.RemoteExecutor;
using Microsoft.Win32;

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
                    Assert.Equal(expectedLcid, PInvokeCore.GetThreadLocale());

                    // Set same.
                    Application.CurrentCulture = value;
                    Assert.Same(value, Application.CurrentCulture);
                    Assert.Same(value, Thread.CurrentThread.CurrentCulture);
                    Assert.Same(value, CultureInfo.CurrentCulture);
                    Assert.Equal(expectedLcid, PInvokeCore.GetThreadLocale());
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
        // This is not a recommended scenario per
        // https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.application.enablevisualstyles
        // EnableVisualStyles should be executed before any control-related code is.
        RemoteExecutor.Invoke(() =>
        {
            Assert.False(Application.UseVisualStyles);
            Assert.False(Application.RenderWithVisualStyles);

            Application.EnableVisualStyles();
            Assert.True(Application.UseVisualStyles, "New Visual Styles will not be applied on WinForms app. This is a high priority bug and must be looked into");
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

#pragma warning disable SYSLIB5002 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    [Fact]
    public void Application_SetColorMode_PlausibilityTests()
    {
        if (SystemInformation.HighContrast)
        {
            // We don't run this test in HighContrast mode.
            return;
        }

        SystemColorMode systemColorMode = Application.SystemColorMode;

        Application.SetColorMode(SystemColorMode.Classic);
        Assert.False(Application.IsDarkModeEnabled);
        Assert.Equal(SystemColorMode.Classic, Application.ColorMode);
        Assert.False(SystemColors.UseAlternativeColorSet);

        Application.SetColorMode(SystemColorMode.Dark);
        Assert.True(Application.IsDarkModeEnabled);
        Assert.Equal(SystemColorMode.Dark, Application.ColorMode);
        Assert.True(SystemColors.UseAlternativeColorSet);

        Application.SetColorMode(SystemColorMode.System);
        Assert.False(Application.IsDarkModeEnabled ^ systemColorMode == SystemColorMode.Dark);
        Assert.Equal(SystemColorMode.System, Application.ColorMode);
        Assert.False(SystemColors.UseAlternativeColorSet ^ systemColorMode == SystemColorMode.Dark);
    }
#pragma warning restore WFO5001
#pragma warning restore SYSLIB5002

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

        Font font = new(new FontFamily("Arial"), 12f);
        Font scaled = new(new FontFamily("Arial"), 16f);
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
    public void Application_SetDefaultFont_SetNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("font", () => Application.SetDefaultFont(null));
    }

    [WinFormsFact]
    public void Application_SetDefaultFont_AfterHandleCreated_InvalidOperationException()
    {
        using Control control = new();
        NativeWindow window = new();
        window.AssignHandle(control.Handle);

        Assert.Throws<InvalidOperationException>(() => Application.SetDefaultFont(SystemFonts.CaptionFont));
    }

    [WinFormsFact]
    public void Application_SetDefaultFont_SystemFont()
    {
        var applicationTestAccessor = typeof(Application).TestAccessor().Dynamic;
        Font font = applicationTestAccessor.s_defaultFont;
        font.Should().BeNull();
        font = applicationTestAccessor.s_defaultFontScaled;
        font.Should().BeNull();

        // This a unholy, but generally at this stage NativeWindow.AnyHandleCreated=true,
        // And we won't be able to set the font, unless we flip the bit
        var nativeWindowTestAccessor = typeof(NativeWindow).TestAccessor().Dynamic;
        bool currentAnyHandleCreated = nativeWindowTestAccessor.t_anyHandleCreated;
        try
        {
            nativeWindowTestAccessor.t_anyHandleCreated = false;
            using Font sysFont = SystemFonts.CaptionFont;
            sysFont.IsSystemFont.Should().BeTrue();
            Application.SetDefaultFont(sysFont);
            font = applicationTestAccessor.s_defaultFontScaled;
            font.Should().BeNull();
            // Because we set default font to system font, then in this test it must not be changed,
            // unless, of course, after calling SystemFonts.CaptionFont and this check
            // HKCU\Software\Microsoft\Accessibility\TextScaleFactor is not changed
            Application.DefaultFont.Should().BeSameAs(sysFont);

            // create fake system font
            using Font fakeSysFont = sysFont.WithSize(sysFont.Size * 1.25f);
            // set IsSystemFont flag
            fakeSysFont.TestAccessor().Dynamic.SetSystemFontName(sysFont.SystemFontName);
            fakeSysFont.IsSystemFont.Should().BeTrue();
            Application.SetDefaultFont(fakeSysFont);
            font = applicationTestAccessor.s_defaultFontScaled;
            font.Should().BeNull();
            Application.DefaultFont.Should().NotBe(fakeSysFont, "Because we got a new real system font.");
            Application.DefaultFont.Should().NotBeSameAs(sysFont, "Because we got a new system font.");
            Application.DefaultFont.Should().Be(sysFont, "Because the new system font is the same as our original system font.");
        }
        finally
        {
            // Flip the bit back
            nativeWindowTestAccessor.t_anyHandleCreated = currentAnyHandleCreated;
            applicationTestAccessor.s_defaultFont?.Dispose();
            applicationTestAccessor.s_defaultFont = null;
        }
    }

    [WinFormsFact]
    public void Application_SetDefaultFont_NonSystemFont()
    {
        var applicationTestAccessor = typeof(Application).TestAccessor().Dynamic;
        Font font = applicationTestAccessor.s_defaultFont;
        font.Should().BeNull();
        font = applicationTestAccessor.s_defaultFontScaled;
        font.Should().BeNull();

        using Font customFont = new(new FontFamily("Arial"), 12f);
        customFont.IsSystemFont.Should().BeFalse();

        // This a unholy, but generally at this stage NativeWindow.AnyHandleCreated=true,
        // And we won't be able to set the font, unless we flip the bit
        var nativeWindowTestAccessor = typeof(NativeWindow).TestAccessor().Dynamic;
        bool currentAnyHandleCreated = nativeWindowTestAccessor.t_anyHandleCreated;
        try
        {
            nativeWindowTestAccessor.t_anyHandleCreated = false;
            Application.SetDefaultFont(customFont);
            font = applicationTestAccessor.s_defaultFontScaled;
            if (!OsVersion.IsWindows10_1507OrGreater())
            {
                font.Should().BeNull();
                Application.DefaultFont.Should().BeSameAs(customFont);
                return;
            }

            // Retrieve the text scale factor, which is set via Settings > Display > Make Text Bigger.
            using RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Accessibility");
            int textScale = (int)(key?.GetValue("TextScaleFactor", 100) ?? 100);
            if (textScale == 100) // Application.DefaultFont must be the same
            {
                font.Should().BeNull("Because TextScaleFactor == 100.");
                Application.DefaultFont.Should().BeSameAs(customFont, "Because TextScaleFactor == 100.");
            }
            else // Application.DefaultFont must be a new scaled font
            {
                font.Should().NotBeNull("Because TextScaleFactor != 100.");
                Application.DefaultFont.Should().NotBe(customFont, "Because textScaleFactor != 100 and we got a new scaled font.");
            }
        }
        finally
        {
            // Flip the bit back
            nativeWindowTestAccessor.t_anyHandleCreated = currentAnyHandleCreated;
            applicationTestAccessor.s_defaultFont?.Dispose();
            applicationTestAccessor.s_defaultFont = null;
            applicationTestAccessor.s_defaultFontScaled?.Dispose();
            applicationTestAccessor.s_defaultFontScaled = null;
        }
    }

    [WinFormsTheory]
    [InvalidEnumData<HighDpiMode>]
    public void Application_SetHighDpiMode_SetInvalidValue_ThrowsInvalidEnumArgumentException(HighDpiMode value)
    {
        Assert.Throws<InvalidEnumArgumentException>("highDpiMode", () => Application.SetHighDpiMode(value));
    }

    /// <summary>
    ///  Test <see cref="Application.Exit()"/> fire Closing events in correct order for MDI windows.
    /// </summary>
    /// <param name="mainMDIFormCountParam">Count of MdiContainers. If == 1 then main form is MdiContainer.</param>
    /// <param name="childFormCountParam">Count of MDI child.</param>
    [WinFormsTheory]
    [InlineData(1, 0)]
    [InlineData(1, 1)]
    [InlineData(1, 3)]
    [InlineData(2, 0)]
    [InlineData(2, 1)]
    [InlineData(2, 3)]
    [InlineData(3, 0)]
    [InlineData(3, 1)]
    [InlineData(3, 3)]
    public void Application_Exit_MDIFormClosing_Order(int mainMDIFormCountParam, int childFormCountParam)
    {
        Assert.True(mainMDIFormCountParam > 0);
        Assert.True(childFormCountParam > -1);
        Assert.True(mainMDIFormCountParam < 10);  // to not flood
        Assert.True(childFormCountParam < 10); // to not flood

        RemoteExecutor.Invoke((mainMDIFormCountString, childFormCountString) =>
        {
            int mainMDIFormCount = int.Parse(mainMDIFormCountString);
            int childFormCount = int.Parse(childFormCountString);

            Application.EnableVisualStyles();
            using Form mainForm = new Form();
            Assert.Empty(Application.OpenForms);
            Dictionary<object, int> formClosingProcessed = new(mainMDIFormCount);
            Dictionary<object, int> formClosedProcessed = new(mainMDIFormCount);
            bool exitCalled = false;

            if (mainMDIFormCount == 1) // main form is MdiContainer
            {
                AddMDI(mainForm);
            }
            else
            {
                mainForm.Show();
                for (int i = 0; i < mainMDIFormCount; i++)
                {
                    AddMDI(new Form());
                }
            }

            try
            {
                Application.Exit();
            }
            finally
            {
                exitCalled = true;
            }

            Assert.Equal(mainMDIFormCount + mainMDIFormCount * childFormCount, formClosingProcessed.Values.Sum());
            Assert.Equal(mainMDIFormCount + mainMDIFormCount * childFormCount, formClosedProcessed.Values.Sum());

            void AddMDI(Form mdiParent)
            {
                formClosingProcessed[mdiParent] = 0;
                formClosedProcessed[mdiParent] = 0;
                mdiParent.IsMdiContainer = true;
                mdiParent.FormClosing += (object sender, FormClosingEventArgs e) =>
                {
                    if (exitCalled)
                        return;

                    Assert.Equal(childFormCount, formClosingProcessed[sender]++);
                };

                mdiParent.FormClosed += (object sender, FormClosedEventArgs e) =>
                {
                    if (exitCalled)
                        return;

                    Assert.Equal(childFormCount, formClosedProcessed[sender]++);
                };

                mdiParent.Show();
                for (int i = 0; i < childFormCount; i++)
                {
                    var child = new Form
                    {
                        MdiParent = mdiParent
                    };

                    child.FormClosing += (object sender, FormClosingEventArgs e) =>
                    {
                        if (exitCalled)
                            return;

                        formClosingProcessed[(sender as Form).MdiParent]++;
                    };

                    child.FormClosed += (object sender, FormClosedEventArgs e) =>
                    {
                        if (exitCalled)
                            return;

                        formClosedProcessed[(sender as Form).MdiParent]++;
                    };

                    child.Show();
                }
            }
        }, mainMDIFormCountParam.ToString(), childFormCountParam.ToString()).Dispose();
    }

    /// <summary>
    ///  Test <see cref="Application.Exit()"/> fire Closing events in which we close existing and open new forms.
    /// </summary>
    /// <param name="childFormCountParam">Count of child forms.</param>
    /// <param name="removedFormCountParam">
    ///  Count of forms that we will remove during last form <see cref="Form.OnFormClosing(FormClosingEventArgs)"/>.
    /// </param>
    /// <param name="addFormCountParam">
    ///  Count of forms that we will add during last form <see cref="Form.OnFormClosing(FormClosingEventArgs)"/>.
    /// </param>
    /// <param name="cancelParam">If set to <see langword="true" /> we will cancel application exit process.</param>
    [WinFormsTheory]
    [InlineData(0, 0, 0, false)]
    [InlineData(0, 0, 1, false)]
    [InlineData(1, 1, 0, false)]
    [InlineData(1, 1, 1, false)]
    [InlineData(1, 1, 2, false)]
    [InlineData(2, 0, 0, false)]
    [InlineData(2, 0, 1, false)]
    [InlineData(2, 1, 1, false)]
    [InlineData(2, 2, 2, false)]
    [InlineData(2, 1, 4, false)]
    [InlineData(5, 4, 3, false)]
    [InlineData(5, 4, 5, false)]
    [InlineData(5, 5, 5, false)]
    [InlineData(0, 0, 0, true)]
    [InlineData(0, 0, 1, true)]
    [InlineData(1, 1, 0, true)]
    [InlineData(1, 1, 1, true)]
    [InlineData(1, 1, 2, true)]
    [InlineData(2, 0, 0, true)]
    [InlineData(2, 0, 1, true)]
    [InlineData(2, 1, 1, true)]
    [InlineData(2, 2, 2, true)]
    [InlineData(2, 1, 4, true)]
    [InlineData(5, 4, 3, true)]
    [InlineData(5, 4, 5, true)]
    [InlineData(5, 5, 5, true)]
    public void Application_Exit_OpenForms_Show_Close(int childFormCountParam, int removedFormCountParam, int addFormCountParam, bool cancelParam)
    {
        Assert.True(childFormCountParam > -1);
        Assert.True(removedFormCountParam > -1);
        Assert.True(addFormCountParam > -1);
        Assert.True(childFormCountParam < 10); // to not flood
        Assert.True(addFormCountParam < 10);  // to not flood
        Assert.True(removedFormCountParam <= childFormCountParam);
        Assert.True(childFormCountParam > 0 || removedFormCountParam == 0);

        RemoteExecutor.Invoke((childFormCountString, removedFormCountString, addFormCountString, cancelString) =>
        {
            int childFormCount = int.Parse(childFormCountString);
            int removedFormCount = int.Parse(removedFormCountString);
            int addFormCount = int.Parse(addFormCountString);
            bool cancel = bool.Parse(cancelString);

            Application.EnableVisualStyles();
            using Form mainForm = new Form();
            Assert.Empty(Application.OpenForms);
            int formClosingProcessed = 0;
            Form lastChild = null;
            bool exitCalled = false;
            bool lastChildProcessed = false;

            mainForm.FormClosing += (object sender, FormClosingEventArgs e) =>
            {
                if (exitCalled)
                    return;

                Assert.Equal(childFormCount + (childFormCount > 0 ? addFormCount : 0), formClosingProcessed);
                formClosingProcessed++;
                if (childFormCount == 0) // no child forms
                {
                    for (int j = 0; j < addFormCount; j++)
                    {
                        AddForm();
                    }

                    if (cancel)
                    {
                        e.Cancel = cancel;
                        formClosingProcessed--; // not count on e.Cancel == true
                    }
                }
            };

            mainForm.Show();
            for (int i = 0; i < childFormCount; i++)
            {
                Form other = new Form() { Text = "Child" };
                other.Show(mainForm);
                if (i == childFormCount - 1)
                    lastChild = other;

                other.FormClosing += (object sender, FormClosingEventArgs e) =>
                {
                    if (exitCalled)
                        return;

                    formClosingProcessed++;
                    if (sender == lastChild)
                    {
                        if (!lastChildProcessed)
                        {
                            lastChildProcessed = true;
                            if (removedFormCount > 0)
                            {
                                Random rnd = new Random();
                                for (int j = 0; j < removedFormCount; j++)
                                {
                                    Application.OpenForms[rnd.Next(1, Application.OpenForms.Count - 1)].Close();
                                }
                            }

                            for (int j = 0; j < addFormCount; j++)
                            {
                                AddForm();
                            }

                            if (cancel)
                            {
                                e.Cancel = cancel;
                                formClosingProcessed--; // not count on e.Cancel == true
                            }
                        }
                        else if (!cancel)
                        {
                            formClosingProcessed--; // need to compensate 2-d call on last child form
                        }
                    }
                };
            }

            try
            {
                Application.Exit();
            }
            finally
            {
                exitCalled = true;
            }

            if (!cancel)
            {
                Assert.Empty(Application.OpenForms);
                Assert.Equal(1 + childFormCount + addFormCount, formClosingProcessed);
            }
            else
            {
                Assert.Equal(1 + childFormCount + addFormCount - removedFormCount, Application.OpenForms.Count);
                Assert.Equal(removedFormCount, formClosingProcessed);
            }

            void AddForm()
            {
                Form add = new Form() { Text = "Add" };
                add.FormClosing += (object sender, FormClosingEventArgs e) =>
                {
                    if (exitCalled)
                        return;

                    formClosingProcessed++;
                };

                add.Show(mainForm);
            }
        }, childFormCountParam.ToString(), removedFormCountParam.ToString(), addFormCountParam.ToString(), cancelParam.ToString()).Dispose();
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
