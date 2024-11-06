// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Tests;

public class FontDialogTests
{
    [WinFormsFact]
    public void FontDialog_Ctor_Default()
    {
        using SubFontDialog dialog = new();
        Assert.True(dialog.AllowScriptChange);
        Assert.True(dialog.AllowSimulations);
        Assert.True(dialog.AllowVectorFonts);
        Assert.True(dialog.AllowVerticalFonts);
        Assert.True(dialog.CanRaiseEvents);
        Assert.Equal(Color.Black, dialog.Color);
        Assert.Null(dialog.Container);
        Assert.False(dialog.DesignMode);
        Assert.NotNull(dialog.Events);
        Assert.Same(dialog.Events, dialog.Events);
        Assert.False(dialog.FixedPitchOnly);
        Assert.Equal(Control.DefaultFont, dialog.Font);
        Assert.False(dialog.FontMustExist);
        Assert.Equal(0, dialog.MaxSize);
        Assert.Equal(0, dialog.MinSize);
        Assert.Equal(0x40101, dialog.Options);
        Assert.False(dialog.ScriptsOnly);
        Assert.False(dialog.ShowApply);
        Assert.False(dialog.ShowColor);
        Assert.True(dialog.ShowEffects);
        Assert.False(dialog.ShowHelp);
        Assert.Null(dialog.Site);
        Assert.Null(dialog.Tag);
    }

    [WinFormsFact]
    public void FontDialog_Ctor_Default_OverriddenReset()
    {
        using EmptyResetFontDialog dialog = new();
        Assert.True(dialog.AllowScriptChange);
        Assert.True(dialog.AllowSimulations);
        Assert.True(dialog.AllowVectorFonts);
        Assert.True(dialog.CanRaiseEvents);
        Assert.True(dialog.AllowVerticalFonts);
        Assert.True(dialog.CanRaiseEvents);
        Assert.Equal(Color.Empty, dialog.Color);
        Assert.Null(dialog.Container);
        Assert.False(dialog.DesignMode);
        Assert.NotNull(dialog.Events);
        Assert.Same(dialog.Events, dialog.Events);
        Assert.False(dialog.FixedPitchOnly);
        Assert.Equal(Control.DefaultFont, dialog.Font);
        Assert.False(dialog.FontMustExist);
        Assert.Equal(0, dialog.MaxSize);
        Assert.Equal(0, dialog.MinSize);
        Assert.Equal(0, dialog.Options);
        Assert.False(dialog.ScriptsOnly);
        Assert.False(dialog.ShowApply);
        Assert.False(dialog.ShowColor);
        Assert.False(dialog.ShowEffects);
        Assert.False(dialog.ShowHelp);
        Assert.Null(dialog.Site);
        Assert.Null(dialog.Tag);
    }

    [WinFormsTheory]
    [InlineData(true, 0x40101, 0x440101)]
    [InlineData(false, 0x440101, 0x40101)]
    public void FontDialog_AllowScriptChange_Set_GetReturnsExpected(bool value, int expectedOptions, int expectedOptionsAfter)
    {
        using SubFontDialog dialog = new()
        {
            AllowScriptChange = value
        };
        Assert.Equal(value, dialog.AllowScriptChange);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set same.
        dialog.AllowScriptChange = value;
        Assert.Equal(value, dialog.AllowScriptChange);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set different.
        dialog.AllowScriptChange = !value;
        Assert.Equal(!value, dialog.AllowScriptChange);
        Assert.Equal(expectedOptionsAfter, dialog.Options);
    }

    [WinFormsTheory]
    [InlineData(true, 0x40101, 0x41101)]
    [InlineData(false, 0x41101, 0x40101)]
    public void FontDialog_AllowSimulations_Set_GetReturnsExpected(bool value, int expectedOptions, int expectedOptionsAfter)
    {
        using SubFontDialog dialog = new()
        {
            AllowSimulations = value
        };
        Assert.Equal(value, dialog.AllowSimulations);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set same.
        dialog.AllowSimulations = value;
        Assert.Equal(value, dialog.AllowSimulations);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set different.
        dialog.AllowSimulations = !value;
        Assert.Equal(!value, dialog.AllowSimulations);
        Assert.Equal(expectedOptionsAfter, dialog.Options);
    }

    [WinFormsTheory]
    [InlineData(true, 0x40101, 0x40901)]
    [InlineData(false, 0x40901, 0x40101)]
    public void FontDialog_AllowVectorFonts_Set_GetReturnsExpected(bool value, int expectedOptions, int expectedOptionsAfter)
    {
        using SubFontDialog dialog = new()
        {
            AllowVectorFonts = value
        };
        Assert.Equal(value, dialog.AllowVectorFonts);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set same.
        dialog.AllowVectorFonts = value;
        Assert.Equal(value, dialog.AllowVectorFonts);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set different.
        dialog.AllowVectorFonts = !value;
        Assert.Equal(!value, dialog.AllowVectorFonts);
        Assert.Equal(expectedOptionsAfter, dialog.Options);
    }

    [WinFormsTheory]
    [InlineData(true, 0x40101, 0x1040101)]
    [InlineData(false, 0x1040101, 0x40101)]
    public void FontDialog_AllowVerticalFonts_Set_GetReturnsExpected(bool value, int expectedOptions, int expectedOptionsAfter)
    {
        using SubFontDialog dialog = new()
        {
            AllowVerticalFonts = value
        };
        Assert.Equal(value, dialog.AllowVerticalFonts);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set same.
        dialog.AllowVerticalFonts = value;
        Assert.Equal(value, dialog.AllowVerticalFonts);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set different.
        dialog.AllowVerticalFonts = !value;
        Assert.Equal(!value, dialog.AllowVerticalFonts);
        Assert.Equal(expectedOptionsAfter, dialog.Options);
    }

    public static IEnumerable<object[]> Color_Set_TestData()
    {
        yield return new object[] { Color.Empty, Color.Black };
        yield return new object[] { Color.Black, Color.Black };
        yield return new object[] { Color.Red, Color.Red };
        yield return new object[] { Color.FromArgb(1, 2, 3, 4), Color.FromArgb(1, 2, 3, 4) };
        yield return new object[] { SystemColors.ControlText, SystemColors.ControlText };
    }

    [WinFormsTheory]
    [MemberData(nameof(Color_Set_TestData))]
    public void FontDialog_Color_Set_GetReturnsExpected(Color value, Color expected)
    {
        using FontDialog dialog = new()
        {
            Color = value
        };
        Assert.Equal(expected, dialog.Color);

        // Set same.
        dialog.Color = value;
        Assert.Equal(expected, dialog.Color);
    }

    [WinFormsTheory]
    [MemberData(nameof(Color_Set_TestData))]
    public void FontDialog_Color_SetWithCustomOldValue_GetReturnsExpected(Color value, Color expected)
    {
        using FontDialog dialog = new()
        {
            Color = Color.Yellow
        };

        dialog.Color = value;
        Assert.Equal(expected, dialog.Color);

        // Set same.
        dialog.Color = value;
        Assert.Equal(expected, dialog.Color);
    }

    [WinFormsTheory]
    [InlineData(true, 0x44101, 0x40101)]
    [InlineData(false, 0x40101, 0x44101)]
    public void FontDialog_FixedPitchOnly_Set_GetReturnsExpected(bool value, int expectedOptions, int expectedOptionsAfter)
    {
        using SubFontDialog dialog = new()
        {
            FixedPitchOnly = value
        };
        Assert.Equal(value, dialog.FixedPitchOnly);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set same.
        dialog.FixedPitchOnly = value;
        Assert.Equal(value, dialog.FixedPitchOnly);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set different.
        dialog.FixedPitchOnly = !value;
        Assert.Equal(!value, dialog.FixedPitchOnly);
        Assert.Equal(expectedOptionsAfter, dialog.Options);
    }

    [WinFormsFact]
    public void FontDialog_Font_GetWithSmallMinSize_ReturnsExpected()
    {
        using Font font = new("Arial", 8.25f);
        using FontDialog dialog = new()
        {
            MinSize = 1,
            Font = font
        };
        Assert.Same(font, dialog.Font);
    }

    [WinFormsFact]
    public void FontDialog_Font_GetWithLargeMinSize_ReturnsExpected()
    {
        using Font font = new("Arial", 8.25f);
        using FontDialog dialog = new()
        {
            MinSize = 100,
            Font = font
        };
        Font result = dialog.Font;
        Assert.NotSame(result, dialog.Font);
        Assert.NotSame(font, dialog.Font);
        Assert.Equal(font.FontFamily, result.FontFamily);
        Assert.Equal(100, result.SizeInPoints);
        Assert.Equal(font.Style, result.Style);
        Assert.Equal(GraphicsUnit.Point, result.Unit);
    }

    [WinFormsFact]
    public void FontDialog_Font_GetWithSmallMaxSize_ReturnsExpected()
    {
        using Font font = new("Arial", 8.25f);
        using FontDialog dialog = new()
        {
            MaxSize = 1,
            Font = font
        };
        Font result = dialog.Font;
        Assert.NotSame(result, dialog.Font);
        Assert.NotSame(font, dialog.Font);
        Assert.Equal(font.FontFamily, result.FontFamily);
        Assert.Equal(1, result.SizeInPoints);
        Assert.Equal(font.Style, result.Style);
        Assert.Equal(GraphicsUnit.Point, result.Unit);
    }

    [WinFormsFact]
    public void FontDialog_Font_GetWithLargeMaxSize_ReturnsExpected()
    {
        using Font font = new("Arial", 8.25f);
        using FontDialog dialog = new()
        {
            MaxSize = 100,
            Font = font
        };
        Assert.Same(font, dialog.Font);
    }

    public static IEnumerable<object[]> Font_Set_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { SystemFonts.MenuFont };

        Font font = new("Arial", 8.25f);
        yield return new object[] { font };
    }

    [WinFormsTheory]
    [MemberData(nameof(Font_Set_TestData))]
    public void FontDialog_Font_Set_GetReturnsExpected(Font value)
    {
        using FontDialog dialog = new()
        {
            Font = value
        };
        Assert.Equal(value ?? Control.DefaultFont, dialog.Font);

        // Set same.
        dialog.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, dialog.Font);
    }

    [WinFormsTheory]
    [MemberData(nameof(Font_Set_TestData))]
    public void FontDialog_Font_SetWithCustomOldValue_GetReturnsExpected(Font value)
    {
        using Font font = new("Arial", 8.25f);
        using FontDialog dialog = new()
        {
            Font = font
        };

        dialog.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, dialog.Font);

        // Set same.
        dialog.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, dialog.Font);
    }

    [WinFormsFact]
    public void FontDialog_Font_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(FontDialog))[nameof(FontDialog.Font)];
        using FontDialog dialog = new();
        Assert.False(property.CanResetValue(dialog));

        using Font font = new("Arial", 8.25f);
        dialog.Font = font;
        Assert.Same(font, dialog.Font);
        Assert.True(property.CanResetValue(dialog));

        property.ResetValue(dialog);
        Assert.Equal(Control.DefaultFont, dialog.Font);
        Assert.False(property.CanResetValue(dialog));
    }

    [WinFormsFact]
    public void FontDialog_Font_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(FontDialog))[nameof(FontDialog.Font)];
        using FontDialog dialog = new();
        Assert.False(property.ShouldSerializeValue(dialog));

        using Font font = new("Arial", 8.25f);
        dialog.Font = font;
        Assert.Same(font, dialog.Font);
        Assert.True(property.ShouldSerializeValue(dialog));

        property.ResetValue(dialog);
        Assert.Equal(Control.DefaultFont, dialog.Font);
        Assert.False(property.ShouldSerializeValue(dialog));
    }

    [WinFormsTheory]
    [InlineData(true, 0x50101, 0x40101)]
    [InlineData(false, 0x40101, 0x50101)]
    public void FontDialog_FontMustExist_Set_GetReturnsExpected(bool value, int expectedOptions, int expectedOptionsAfter)
    {
        using SubFontDialog dialog = new()
        {
            FontMustExist = value
        };
        Assert.Equal(value, dialog.FontMustExist);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set same.
        dialog.FontMustExist = value;
        Assert.Equal(value, dialog.FontMustExist);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set different.
        dialog.FontMustExist = !value;
        Assert.Equal(!value, dialog.FontMustExist);
        Assert.Equal(expectedOptionsAfter, dialog.Options);
    }

    [WinFormsTheory]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(int.MaxValue, int.MaxValue)]
    public void FontDialog_MinSize_Set_GetReturnsExpected(int value, int expected)
    {
        using FontDialog dialog = new()
        {
            MinSize = value
        };
        Assert.Equal(expected, dialog.MinSize);
        Assert.Equal(0, dialog.MaxSize);

        // Set same.
        dialog.MinSize = value;
        Assert.Equal(expected, dialog.MinSize);
        Assert.Equal(0, dialog.MaxSize);
    }

    [WinFormsTheory]
    [InlineData(-1, 0, 10)]
    [InlineData(0, 0, 10)]
    [InlineData(1, 1, 10)]
    [InlineData(10, 10, 10)]
    [InlineData(int.MaxValue, int.MaxValue, int.MaxValue)]
    public void FontDialog_MinSize_SetWithMaxSize_GetReturnsExpected(int value, int expected, int expectedMaxSize)
    {
        using FontDialog dialog = new()
        {
            MaxSize = 10,
            MinSize = value
        };
        Assert.Equal(expected, dialog.MinSize);
        Assert.Equal(expectedMaxSize, dialog.MaxSize);

        // Set same.
        dialog.MinSize = value;
        Assert.Equal(expected, dialog.MinSize);
        Assert.Equal(expectedMaxSize, dialog.MaxSize);
    }

    [WinFormsTheory]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(int.MaxValue, int.MaxValue)]
    public void FontDialog_MaxSize_Set_GetReturnsExpected(int value, int expected)
    {
        using FontDialog dialog = new()
        {
            MaxSize = value
        };
        Assert.Equal(0, dialog.MinSize);
        Assert.Equal(expected, dialog.MaxSize);

        // Set same.
        dialog.MaxSize = value;
        Assert.Equal(0, dialog.MinSize);
        Assert.Equal(expected, dialog.MaxSize);
    }

    [WinFormsTheory]
    [InlineData(-1, 0, 10)]
    [InlineData(0, 0, 10)]
    [InlineData(1, 1, 1)]
    [InlineData(10, 10, 10)]
    [InlineData(int.MaxValue, int.MaxValue, 10)]
    public void FontDialog_MaxSize_SetWithMinSize_GetReturnsExpected(int value, int expected, int expectedMinSize)
    {
        using FontDialog dialog = new()
        {
            MinSize = 10,
            MaxSize = value
        };
        Assert.Equal(expectedMinSize, dialog.MinSize);
        Assert.Equal(expected, dialog.MaxSize);

        // Set same.
        dialog.MaxSize = value;
        Assert.Equal(expectedMinSize, dialog.MinSize);
        Assert.Equal(expected, dialog.MaxSize);
    }

    [WinFormsTheory]
    [InlineData(true, 0x40501, 0x40101)]
    [InlineData(false, 0x40101, 0x40501)]
    public void FontDialog_ScriptsOnly_Set_GetReturnsExpected(bool value, int expectedOptions, int expectedOptionsAfter)
    {
        using SubFontDialog dialog = new()
        {
            ScriptsOnly = value
        };
        Assert.Equal(value, dialog.ScriptsOnly);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set same.
        dialog.ScriptsOnly = value;
        Assert.Equal(value, dialog.ScriptsOnly);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set different.
        dialog.ScriptsOnly = !value;
        Assert.Equal(!value, dialog.ScriptsOnly);
        Assert.Equal(expectedOptionsAfter, dialog.Options);
    }

    [WinFormsTheory]
    [InlineData(true, 0x40301, 0x40101)]
    [InlineData(false, 0x40101, 0x40301)]
    public void FontDialog_ShowApply_Set_GetReturnsExpected(bool value, int expectedOptions, int expectedOptionsAfter)
    {
        using SubFontDialog dialog = new()
        {
            ShowApply = value
        };
        Assert.Equal(value, dialog.ShowApply);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set same.
        dialog.ShowApply = value;
        Assert.Equal(value, dialog.ShowApply);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set different.
        dialog.ShowApply = !value;
        Assert.Equal(!value, dialog.ShowApply);
        Assert.Equal(expectedOptionsAfter, dialog.Options);
    }

    [WinFormsTheory]
    [BoolData]
    public void FontDialog_ShowColor_Set_GetReturnsExpected(bool value)
    {
        using FontDialog dialog = new()
        {
            ShowColor = value
        };
        Assert.Equal(value, dialog.ShowColor);

        // Set same.
        dialog.ShowColor = value;
        Assert.Equal(value, dialog.ShowColor);

        // Set different.
        dialog.ShowColor = !value;
        Assert.Equal(!value, dialog.ShowColor);
    }

    [WinFormsTheory]
    [InlineData(true, 0x40101, 0x40001)]
    [InlineData(false, 0x40001, 0x40101)]
    public void FontDialog_ShowEffects_Set_GetReturnsExpected(bool value, int expectedOptions, int expectedOptionsAfter)
    {
        using SubFontDialog dialog = new()
        {
            ShowEffects = value
        };
        Assert.Equal(value, dialog.ShowEffects);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set same.
        dialog.ShowEffects = value;
        Assert.Equal(value, dialog.ShowEffects);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set different.
        dialog.ShowEffects = !value;
        Assert.Equal(!value, dialog.ShowEffects);
        Assert.Equal(expectedOptionsAfter, dialog.Options);
    }

    [WinFormsTheory]
    [InlineData(true, 0x40105, 0x40101)]
    [InlineData(false, 0x40101, 0x40105)]
    public void FontDialog_ShowHelp_Set_GetReturnsExpected(bool value, int expectedOptions, int expectedOptionsAfter)
    {
        using SubFontDialog dialog = new()
        {
            ShowHelp = value
        };
        Assert.Equal(value, dialog.ShowHelp);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set same.
        dialog.ShowHelp = value;
        Assert.Equal(value, dialog.ShowHelp);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set different.
        dialog.ShowHelp = !value;
        Assert.Equal(!value, dialog.ShowHelp);
        Assert.Equal(expectedOptionsAfter, dialog.Options);
    }

    [WinFormsFact]
    public void FontDialog_Reset_Invoke_Success()
    {
        using Font font = new("Arial", 8.25f);
        using SubFontDialog dialog = new()
        {
            AllowScriptChange = false,
            AllowSimulations = false,
            AllowVectorFonts = false,
            AllowVerticalFonts = false,
            FixedPitchOnly = true,
            Font = font,
            FontMustExist = true,
            MaxSize = 10,
            MinSize = 5,
            ScriptsOnly = true,
            ShowApply = true,
            ShowColor = true,
            ShowEffects = false,
            ShowHelp = true,
            Tag = "Tag",
        };

        dialog.Reset();
        Assert.True(dialog.AllowScriptChange);
        Assert.True(dialog.AllowSimulations);
        Assert.True(dialog.AllowVectorFonts);
        Assert.True(dialog.AllowVerticalFonts);
        Assert.True(dialog.CanRaiseEvents);
        Assert.Equal(Color.Black, dialog.Color);
        Assert.Null(dialog.Container);
        Assert.False(dialog.DesignMode);
        Assert.NotNull(dialog.Events);
        Assert.Same(dialog.Events, dialog.Events);
        Assert.False(dialog.FixedPitchOnly);
        Assert.Equal(Control.DefaultFont, dialog.Font);
        Assert.False(dialog.FontMustExist);
        Assert.Equal(0, dialog.MaxSize);
        Assert.Equal(0, dialog.MinSize);
        Assert.Equal(0x40101, dialog.Options);
        Assert.False(dialog.ScriptsOnly);
        Assert.False(dialog.ShowApply);
        Assert.False(dialog.ShowColor);
        Assert.True(dialog.ShowEffects);
        Assert.False(dialog.ShowHelp);
        Assert.Null(dialog.Site);
        Assert.Equal("Tag", dialog.Tag);
    }

    public static IEnumerable<object[]> HookProc_TestData()
    {
        foreach (bool showColor in new bool[] { true, false })
        {
            yield return new object[] { showColor, (int)PInvokeCore.WM_INITDIALOG, IntPtr.Zero };
            yield return new object[] { showColor, (int)PInvokeCore.WM_SETFOCUS, IntPtr.Zero };
            yield return new object[] { showColor, (int)PInvokeCore.WM_COMMAND, IntPtr.Zero };

            const uint CDM_SETDEFAULTFOCUS = (int)PInvokeCore.WM_USER + 0x51;
            yield return new object[] { showColor, (int)CDM_SETDEFAULTFOCUS, IntPtr.Zero };

            yield return new object[] { showColor, 0, IntPtr.Zero };
            yield return new object[] { showColor, -1, IntPtr.Zero };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(HookProc_TestData))]
    public void FontDialog_HookProc_Invoke_ReturnsZero(bool showColor, int msg, IntPtr wparam)
    {
        using SubFontDialog dialog = new()
        {
            ShowColor = showColor
        };
        int applyCallCount = 0;
        dialog.Apply += (sender, e) => applyCallCount++;
        Assert.Equal(IntPtr.Zero, dialog.HookProc(IntPtr.Zero, msg, wparam, IntPtr.Zero));
        Assert.Equal(0, applyCallCount);
    }

    public static IEnumerable<object[]> HookProc_WithHWnd_TestData()
    {
        foreach (bool showColor in new bool[] { true, false })
        {
            yield return new object[] { showColor, (int)PInvokeCore.WM_INITDIALOG, IntPtr.Zero, 0 };
            yield return new object[] { showColor, (int)PInvokeCore.WM_SETFOCUS, IntPtr.Zero, 0 };
            yield return new object[] { showColor, (int)PInvokeCore.WM_COMMAND, IntPtr.Zero, 0 };
            yield return new object[] { showColor, (int)PInvokeCore.WM_COMMAND, (IntPtr)0x402, 1 };

            const uint CDM_SETDEFAULTFOCUS = (int)PInvokeCore.WM_USER + 0x51;
            yield return new object[] { showColor, (int)CDM_SETDEFAULTFOCUS, IntPtr.Zero, 0 };

            yield return new object[] { showColor, 0, IntPtr.Zero, 0 };
            yield return new object[] { showColor, -1, IntPtr.Zero, 0 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(HookProc_WithHWnd_TestData))]
    public void FontDialog_HookProc_InvokeWithHWnd_ReturnsZero(bool showColor, int msg, IntPtr wparam, int expectedApplyCallCount)
    {
        using CustomClass control = new();
        using SubFontDialog dialog = new()
        {
            ShowColor = showColor
        };
        int applyCallCount = 0;
        dialog.Apply += (sender, e) => applyCallCount++;
        Assert.Equal(IntPtr.Zero, dialog.HookProc(control.Handle, msg, wparam, IntPtr.Zero));
        Assert.Equal(expectedApplyCallCount, applyCallCount);
    }

    private class CustomClass : Control
    {
        protected override unsafe void WndProc(ref Message m)
        {
            if (m.Msg == (int)PInvokeCore.WM_CHOOSEFONT_GETLOGFONT)
            {
                using Font font = new("Arial", 8.25f);
                LOGFONTW* pLogfont = (LOGFONTW*)m.LParam;
                object lf = default(LOGFONTW);
                font.ToLogFont(lf);
                *pLogfont = (LOGFONTW)lf;
            }

            base.WndProc(ref m);
        }
    }

    [WinFormsTheory]
    [BoolData]
    public void FontDialog_HookProc_Invoke_InvalidCommandHWnd(bool showColor)
    {
        using SubFontDialog dialog = new()
        {
            ShowColor = showColor
        };
        int applyCallCount = 0;
        dialog.Apply += (sender, e) => applyCallCount++;
        Assert.Throws<ArgumentException>(() => dialog.HookProc(IntPtr.Zero, (int)PInvokeCore.WM_COMMAND, 0x402, IntPtr.Zero));
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void FontDialog_OnApply_Invoke_CallsApply(EventArgs eventArgs)
    {
        using SubFontDialog dialog = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(dialog, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        dialog.Apply += handler;
        dialog.OnApply(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        dialog.Apply -= handler;
        dialog.OnApply(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void FontDialog_ToString_Invoke_ReturnsExpected()
    {
        using SubFontDialog dialog = new();
        Assert.Equal($"System.Windows.Forms.Tests.FontDialogTests+SubFontDialog,  Font: {Control.DefaultFont}", dialog.ToString());
    }

    private class SubFontDialog : FontDialog
    {
        public new bool CanRaiseEvents => base.CanRaiseEvents;

        public new bool DesignMode => base.DesignMode;

        public new EventHandlerList Events => base.Events;

        public new int Options => base.Options;

        public new IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam) => base.HookProc(hWnd, msg, wparam, lparam);

        public new bool RunDialog(IntPtr hWndOwner) => base.RunDialog(hWndOwner);

        public new void OnApply(EventArgs e) => base.OnApply(e);
    }

    private class EmptyResetFontDialog : SubFontDialog
    {
        public override void Reset()
        {
        }
    }
}
