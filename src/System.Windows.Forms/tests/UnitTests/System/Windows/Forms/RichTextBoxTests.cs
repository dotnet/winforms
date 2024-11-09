// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms.Automation;
using System.Windows.Forms.TestUtilities;
using Moq;
using Moq.Protected;
using Windows.Win32.System.Ole;
using Windows.Win32.UI.Controls.RichEdit;
using static Interop.Richedit;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests;

public partial class RichTextBoxTests
{
    private static readonly int s_preferredHeight = Control.DefaultFont.Height + SystemInformation.BorderSize.Height * 4 + 3;

    [WinFormsFact]
    public void RichTextBox_Ctor_Default()
    {
        using SubRichTextBox control = new();
        Assert.False(control.AcceptsTab);
        Assert.Null(control.AccessibleDefaultActionDescription);
        Assert.Null(control.AccessibleDescription);
        Assert.Null(control.AccessibleName);
        Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
        Assert.False(control.AllowDrop);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        Assert.False(control.AutoSize);
        Assert.False(control.AutoWordSelection);
        Assert.Equal(SystemColors.Window, control.BackColor);
        Assert.Null(control.BackgroundImage);
        Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
        Assert.Null(control.BindingContext);
        Assert.Equal(BorderStyle.Fixed3D, control.BorderStyle);
        Assert.Equal(96, control.Bottom);
        Assert.Equal(new Rectangle(0, 0, 100, 96), control.Bounds);
        Assert.Equal(0, control.BulletIndent);
        Assert.True(control.CanEnableIme);
        Assert.False(control.CanFocus);
        Assert.True(control.CanRaiseEvents);
        Assert.False(control.CanRedo);
        Assert.True(control.CanSelect);
        Assert.False(control.CanUndo);
        Assert.False(control.Capture);
        Assert.True(control.CausesValidation);
        Assert.Equal(new Size(96, 92), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 96, 92), control.ClientRectangle);
        Assert.Null(control.Container);
        Assert.False(control.ContainsFocus);
        Assert.Null(control.ContextMenuStrip);
        Assert.Empty(control.Controls);
        Assert.Same(control.Controls, control.Controls);
        Assert.False(control.Created);
        Assert.Same(Cursors.IBeam, control.Cursor);
        Assert.Same(Cursors.IBeam, control.DefaultCursor);
        Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
        Assert.Equal(new Padding(3), control.DefaultMargin);
        Assert.Equal(Size.Empty, control.DefaultMaximumSize);
        Assert.Equal(Size.Empty, control.DefaultMinimumSize);
        Assert.Equal(Padding.Empty, control.DefaultPadding);
        Assert.Equal(new Size(100, 96), control.DefaultSize);
        Assert.False(control.DesignMode);
        Assert.True(control.DetectUrls);
        Assert.Equal(new Rectangle(0, 0, 96, 92), control.DisplayRectangle);
        Assert.Equal(DockStyle.None, control.Dock);
        Assert.False(control.DoubleBuffered);
        Assert.False(control.EnableAutoDragDrop);
        Assert.True(control.Enabled);
        Assert.False(control.EnableAutoDragDrop);
        Assert.NotNull(control.Events);
        Assert.Same(control.Events, control.Events);
        Assert.False(control.Focused);
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.Equal(SystemColors.WindowText, control.ForeColor);
        Assert.False(control.HasChildren);
        Assert.Equal(96, control.Height);
        Assert.True(control.HideSelection);
        Assert.Equal(ImeMode.NoControl, control.ImeMode);
        Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
        Assert.False(control.IsAccessible);
        Assert.False(control.IsMirrored);
        Assert.Equal(RichTextBoxLanguageOptions.AutoFont | RichTextBoxLanguageOptions.DualFont, control.LanguageOption);
        Assert.NotNull(control.LayoutEngine);
        Assert.Same(control.LayoutEngine, control.LayoutEngine);
        Assert.Equal(0, control.Left);
        Assert.Empty(control.Lines);
        Assert.Equal(Point.Empty, control.Location);
        Assert.Equal(new Padding(3), control.Margin);
        Assert.Equal(Size.Empty, control.MaximumSize);
        Assert.Equal(int.MaxValue, control.MaxLength);
        Assert.Equal(Size.Empty, control.MinimumSize);
        Assert.False(control.Modified);
        Assert.True(control.Multiline);
        Assert.Equal(Padding.Empty, control.Padding);
        Assert.Null(control.Parent);
        Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
        Assert.True(control.PreferredSize.Width > 0);
        Assert.True(control.PreferredSize.Height > 0);
        Assert.Equal(control.PreferredSize.Height, control.PreferredHeight);
        Assert.Empty(control.RedoActionName);
        Assert.False(control.ReadOnly);
        Assert.False(control.RecreatingHandle);
        Assert.Null(control.Region);
        Assert.False(control.ResizeRedraw);
        Assert.Equal(0, control.RightMargin);
        Assert.True(control.RichTextShortcutsEnabled);
        Assert.Equal(100, control.Right);
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.Null(control.Rtf);
        Assert.Equal(RichTextBoxScrollBars.Both, control.ScrollBars);
        Assert.Equal(Color.Empty, control.SelectionBackColor);
        Assert.False(control.ShowSelectionMargin);
        Assert.True(control.ShowFocusCues);
        Assert.True(control.ShowKeyboardCues);
        Assert.Null(control.Site);
        Assert.Equal(new Size(100, 96), control.Size);
        Assert.Equal(0, control.TabIndex);
        Assert.True(control.TabStop);
        Assert.Empty(control.Text);
        Assert.Equal(0, control.Top);
        Assert.Null(control.TopLevelControl);
        Assert.Empty(control.UndoActionName);
        Assert.False(control.UseWaitCursor);
        Assert.True(control.Visible);
        Assert.Equal(100, control.Width);
        Assert.True(control.WordWrap);
        Assert.Equal(1.0f, control.ZoomFactor);

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_CreateParams_GetDefault_ReturnsExpected()
    {
        using SubRichTextBox control = new();
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("RICHEDIT50W", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0x200, createParams.ExStyle);
        Assert.Equal(96, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(0x56210044, createParams.Style);
        Assert.Equal(100, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(BorderStyle.None, 0x56210044, 0)]
    [InlineData(BorderStyle.Fixed3D, 0x56210044, 0x200)]
    [InlineData(BorderStyle.FixedSingle, 0x56210044, 0x200)]
    public void RichTextBox_CreateParams_GetBorderStyle_ReturnsExpected(BorderStyle borderStyle, int expectedStyle, int expectedExStyle)
    {
        using SubRichTextBox control = new()
        {
            BorderStyle = borderStyle
        };

        CreateParams createParams = control.CreateParams;
        Assert.Equal("RICHEDIT50W", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(expectedExStyle, createParams.ExStyle);
        Assert.Equal(control.Height, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(expectedStyle, createParams.Style);
        Assert.Equal(100, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, true, RichTextBoxScrollBars.Both, 0x56210044)]
    [InlineData(true, true, RichTextBoxScrollBars.ForcedBoth, 0x56212044)]
    [InlineData(true, true, RichTextBoxScrollBars.ForcedHorizontal, 0x56010044)]
    [InlineData(true, true, RichTextBoxScrollBars.ForcedVertical, 0x56212044)]
    [InlineData(true, true, RichTextBoxScrollBars.Horizontal, 0x56010044)]
    [InlineData(true, true, RichTextBoxScrollBars.None, 0x56010044)]
    [InlineData(true, true, RichTextBoxScrollBars.Vertical, 0x56210044)]
    [InlineData(true, false, RichTextBoxScrollBars.Both, 0x563100C4)]
    [InlineData(true, false, RichTextBoxScrollBars.ForcedBoth, 0x563120C4)]
    [InlineData(true, false, RichTextBoxScrollBars.ForcedHorizontal, 0x561120C4)]
    [InlineData(true, false, RichTextBoxScrollBars.ForcedVertical, 0x562120C4)]
    [InlineData(true, false, RichTextBoxScrollBars.Horizontal, 0x561100C4)]
    [InlineData(true, false, RichTextBoxScrollBars.None, 0x560100C4)]
    [InlineData(true, false, RichTextBoxScrollBars.Vertical, 0x562100C4)]
    [InlineData(false, true, RichTextBoxScrollBars.Both, 0x560100C0)]
    [InlineData(false, true, RichTextBoxScrollBars.ForcedBoth, 0x560100C0)]
    [InlineData(false, true, RichTextBoxScrollBars.ForcedHorizontal, 0x560100C0)]
    [InlineData(false, true, RichTextBoxScrollBars.ForcedVertical, 0x560100C0)]
    [InlineData(false, true, RichTextBoxScrollBars.Horizontal, 0x560100C0)]
    [InlineData(false, true, RichTextBoxScrollBars.None, 0x560100C0)]
    [InlineData(false, true, RichTextBoxScrollBars.Vertical, 0x560100C0)]
    [InlineData(false, false, RichTextBoxScrollBars.Both, 0x560100C0)]
    [InlineData(false, false, RichTextBoxScrollBars.ForcedBoth, 0x560100C0)]
    [InlineData(false, false, RichTextBoxScrollBars.ForcedHorizontal, 0x560100C0)]
    [InlineData(false, false, RichTextBoxScrollBars.ForcedVertical, 0x560100C0)]
    [InlineData(false, false, RichTextBoxScrollBars.Horizontal, 0x560100C0)]
    [InlineData(false, false, RichTextBoxScrollBars.None, 0x560100C0)]
    [InlineData(false, false, RichTextBoxScrollBars.Vertical, 0x560100C0)]
    public void RichTextBox_CreateParams_GetMultiline_ReturnsExpected(bool multiline, bool wordWrap, RichTextBoxScrollBars scrollBars, int expectedStyle)
    {
        using SubRichTextBox control = new()
        {
            Multiline = multiline,
            WordWrap = wordWrap,
            ScrollBars = scrollBars
        };

        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("RICHEDIT50W", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0x200, createParams.ExStyle);
        Assert.Equal(96, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(expectedStyle, createParams.Style);
        Assert.Equal(100, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void RichTextBox_AllowDrop_Set_GetReturnsExpected(bool value)
    {
        using RichTextBox control = new()
        {
            AllowDrop = value
        };
        Assert.Equal(value, control.AllowDrop);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AllowDrop = value;
        Assert.Equal(value, control.AllowDrop);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.AllowDrop = value;
        Assert.Equal(value, control.AllowDrop);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void RichTextBox_AllowDrop_SetWithHandle_GetReturnsExpected(bool value)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.AllowDrop = value;
        Assert.Equal(value, control.AllowDrop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.AllowDrop = value;
        Assert.Equal(value, control.AllowDrop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.AllowDrop = value;
        Assert.Equal(value, control.AllowDrop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void RichTextBox_AllowDrop_SetWithHandleAlreadyRegistered_GetReturnsExpected(bool value)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        DropTargetMock dropTarget = new();
        Assert.Equal(ApartmentState.STA, Application.OleRequired());
        Assert.Equal(HRESULT.DRAGDROP_E_ALREADYREGISTERED, PInvoke.RegisterDragDrop(control, dropTarget));

        control.AllowDrop = value;
        Assert.Equal(value, control.AllowDrop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.AllowDrop = value;
        Assert.Equal(value, control.AllowDrop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.AllowDrop = value;
        Assert.Equal(value, control.AllowDrop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [Theory]
    [BoolData]
    public void RichTextBox_AllowDrop_SetWithHandleNonSTAThread_GetReturnsExpected(bool value)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.AllowDrop = value;
        Assert.Equal(value, control.AllowDrop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.AllowDrop = value;
        Assert.Equal(value, control.AllowDrop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.AllowDrop = value;
        Assert.Equal(value, control.AllowDrop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void RichTextBox_AutoSize_Set_GetReturnsExpected(bool value)
    {
        using SubRichTextBox control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.Equal(96, control.Height);
        Assert.False(control.GetStyle(ControlStyles.FixedHeight));
        Assert.True(control.Multiline);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.Equal(96, control.Height);
        Assert.False(control.GetStyle(ControlStyles.FixedHeight));
        Assert.True(control.Multiline);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.AutoSize = !value;
        Assert.Equal(!value, control.AutoSize);
        Assert.Equal(96, control.Height);
        Assert.False(control.GetStyle(ControlStyles.FixedHeight));
        Assert.True(control.Multiline);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> AutoSize_SetNotMultiline_TestData()
    {
        yield return new object[] { true, s_preferredHeight, 96, 1 };
        yield return new object[] { false, 96, s_preferredHeight, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(AutoSize_SetNotMultiline_TestData))]
    public void RichTextBox_AutoSize_SetNotMultiline_GetReturnsExpected(bool value, int expectedHeight, int expectedHeight2, int expectedLayoutCallCount)
    {
        using SubRichTextBox control = new()
        {
            Multiline = false
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(value, control.GetStyle(ControlStyles.FixedHeight));
        Assert.False(control.Multiline);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(value, control.GetStyle(ControlStyles.FixedHeight));
        Assert.False(control.Multiline);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.AutoSize = !value;
        Assert.Equal(!value, control.AutoSize);
        Assert.Equal(expectedHeight2, control.Height);
        Assert.Equal(!value, control.GetStyle(ControlStyles.FixedHeight));
        Assert.False(control.Multiline);
        Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void RichTextBox_AutoSize_SetWithParent_GetReturnsExpected(bool value)
    {
        using Control parent = new();
        using SubRichTextBox control = new()
        {
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);
            Assert.Equal(96, control.Height);
            Assert.False(control.GetStyle(ControlStyles.FixedHeight));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);
            Assert.Equal(96, control.Height);
            Assert.False(control.GetStyle(ControlStyles.FixedHeight));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set different.
            control.AutoSize = !value;
            Assert.Equal(!value, control.AutoSize);
            Assert.Equal(96, control.Height);
            Assert.False(control.GetStyle(ControlStyles.FixedHeight));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void RichTextBox_AutoSize_SetWithHandler_CallsAutoSizeChanged()
    {
        using RichTextBox control = new()
        {
            AutoSize = true
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.AutoSizeChanged += handler;

        // Set different.
        control.AutoSize = false;
        Assert.False(control.AutoSize);
        Assert.Equal(1, callCount);

        // Set same.
        control.AutoSize = false;
        Assert.False(control.AutoSize);
        Assert.Equal(1, callCount);

        // Set different.
        control.AutoSize = true;
        Assert.True(control.AutoSize);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.AutoSizeChanged -= handler;
        control.AutoSize = false;
        Assert.False(control.AutoSize);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void RichTextBox_AutoWordSelection_GetWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.False(control.AutoWordSelection);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call EM_SETOPTIONS.
        PInvokeCore.SendMessage(control, PInvokeCore.EM_SETOPTIONS, (WPARAM)(int)PInvoke.ECOOP_OR, (LPARAM)(int)PInvoke.ECO_AUTOWORDSELECTION);
        Assert.False(control.AutoWordSelection);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void RichTextBox_AutoWordSelection_Set_GetReturnsExpected(bool value)
    {
        using RichTextBox control = new()
        {
            AutoWordSelection = value
        };
        Assert.Equal(value, control.AutoWordSelection);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AutoWordSelection = value;
        Assert.Equal(value, control.AutoWordSelection);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.AutoWordSelection = !value;
        Assert.Equal(!value, control.AutoWordSelection);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void RichTextBox_AutoWordSelection_SetWithHandle_GetReturnsExpected(bool value)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.AutoWordSelection = value;
        Assert.Equal(value, control.AutoWordSelection);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.AutoWordSelection = value;
        Assert.Equal(value, control.AutoWordSelection);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.AutoWordSelection = !value;
        Assert.Equal(!value, control.AutoWordSelection);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(true, 65)]
    [InlineData(false, 64)]
    public void RichTextBox_AutoWordSelection_GetOptions_Success(bool value, int expected)
    {
        using RichTextBox control = new();

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.AutoWordSelection = value;
        Assert.Equal(expected, (int)PInvokeCore.SendMessage(control, PInvokeCore.EM_GETOPTIONS));
    }

    public static IEnumerable<object[]> BackColor_Set_TestData()
    {
        yield return new object[] { Color.Empty, SystemColors.Window };
        yield return new object[] { Color.Red, Color.Red };
    }

    [WinFormsTheory]
    [MemberData(nameof(BackColor_Set_TestData))]
    public void RichTextBox_BackColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using RichTextBox control = new()
        {
            BackColor = value
        };
        Assert.Equal(expected, control.BackColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> BackColor_SetWithHandle_TestData()
    {
        yield return new object[] { Color.Empty, Control.DefaultBackColor, 0 };
        yield return new object[] { Color.Red, Color.Red, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(BackColor_SetWithHandle_TestData))]
    public void Control_BackColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_BackColor_SetWithHandler_CallsBackColorChanged()
    {
        using RichTextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.BackColorChanged += handler;

        // Set different.
        control.BackColor = Color.Red;
        Assert.Equal(Color.Red, control.BackColor);
        Assert.Equal(1, callCount);

        // Set same.
        control.BackColor = Color.Red;
        Assert.Equal(Color.Red, control.BackColor);
        Assert.Equal(1, callCount);

        // Set different.
        control.BackColor = Color.Empty;
        Assert.Equal(SystemColors.Window, control.BackColor);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.BackColorChanged -= handler;
        control.BackColor = Color.Red;
        Assert.Equal(Color.Red, control.BackColor);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetImageTheoryData))]
    public void RichTextBox_BackgroundImage_Set_GetReturnsExpected(Image value)
    {
        using RichTextBox control = new()
        {
            BackgroundImage = value
        };
        Assert.Same(value, control.BackgroundImage);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BackgroundImage = value;
        Assert.Same(value, control.BackgroundImage);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_BackgroundImage_SetWithHandler_CallsBackgroundImageChanged()
    {
        using RichTextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.BackgroundImageChanged += handler;

        // Set different.
        using Bitmap image1 = new(10, 10);
        control.BackgroundImage = image1;
        Assert.Same(image1, control.BackgroundImage);
        Assert.Equal(1, callCount);

        // Set same.
        control.BackgroundImage = image1;
        Assert.Same(image1, control.BackgroundImage);
        Assert.Equal(1, callCount);

        // Set different.
        using Bitmap image2 = new(10, 10);
        control.BackgroundImage = image2;
        Assert.Same(image2, control.BackgroundImage);
        Assert.Equal(2, callCount);

        // Set null.
        control.BackgroundImage = null;
        Assert.Null(control.BackgroundImage);
        Assert.Equal(3, callCount);

        // Remove handler.
        control.BackgroundImageChanged -= handler;
        control.BackgroundImage = image1;
        Assert.Same(image1, control.BackgroundImage);
        Assert.Equal(3, callCount);
    }

    [WinFormsTheory]
    [EnumData<ImageLayout>]
    public void RichTextBox_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
    {
        using SubRichTextBox control = new()
        {
            BackgroundImageLayout = value
        };
        Assert.Equal(value, control.BackgroundImageLayout);
        Assert.False(control.DoubleBuffered);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BackgroundImageLayout = value;
        Assert.Equal(value, control.BackgroundImageLayout);
        Assert.False(control.DoubleBuffered);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_BackgroundImageLayout_SetWithHandler_CallsBackgroundImageLayoutChanged()
    {
        using RichTextBox control = new();
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        }

        control.BackgroundImageLayoutChanged += handler;

        // Set different.
        control.BackgroundImageLayout = ImageLayout.Center;
        Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
        Assert.Equal(1, callCount);

        // Set same.
        control.BackgroundImageLayout = ImageLayout.Center;
        Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
        Assert.Equal(1, callCount);

        // Set different.
        control.BackgroundImageLayout = ImageLayout.Stretch;
        Assert.Equal(ImageLayout.Stretch, control.BackgroundImageLayout);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.BackgroundImageLayoutChanged -= handler;
        control.BackgroundImageLayout = ImageLayout.Center;
        Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2000)]
    [InlineData(2001)]
    [InlineData(int.MaxValue)]
    public void RichTextBox_BulletIndent_Set_GetReturnsExpected(int value)
    {
        using RichTextBox control = new()
        {
            BulletIndent = value
        };
        Assert.Equal(value, control.BulletIndent);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BulletIndent = value;
        Assert.Equal(value, control.BulletIndent);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2000)]
    [InlineData(2001)]
    [InlineData(int.MaxValue)]
    public void RichTextBox_BulletIndent_SetWithHandle_GetReturnsExpected(int value)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.BulletIndent = value;
        Assert.Equal(value, control.BulletIndent);
        Assert.Equal(0, control.SelectionHangingIndent);
        Assert.False(control.SelectionBullet);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.BulletIndent = value;
        Assert.Equal(value, control.BulletIndent);
        Assert.Equal(0, control.SelectionHangingIndent);
        Assert.False(control.SelectionBullet);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2000, 2000)]
    [InlineData(2001, 2001)]
    [InlineData(int.MaxValue, 0)]
    public void RichTextBox_BulletIndent_SetSelectionBulletWithHandle_GetReturnsExpected(int value, int selectionHangingIndent)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        control.SelectionBullet = true;

        control.BulletIndent = value;
        Assert.Equal(value, control.BulletIndent);
        Assert.Equal(selectionHangingIndent, control.SelectionHangingIndent);
        Assert.True(control.SelectionBullet);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.BulletIndent = value;
        Assert.Equal(value, control.BulletIndent);
        Assert.Equal(selectionHangingIndent, control.SelectionHangingIndent);
        Assert.True(control.SelectionBullet);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    public void RichTextBox_BulletIndent_SetInvalidValue_ThrowsArgumentOutOfRangeException(int value)
    {
        using RichTextBox control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.BulletIndent = value);
    }

    [WinFormsFact]
    public void RichTextBox_CanRedo_GetWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.False(control.CanRedo);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> CanRedo_CustomCanRedo_TestData()
    {
        yield return new object[] { IntPtr.Zero, false };
        yield return new object[] { (IntPtr)1, true };
    }

    [WinFormsTheory]
    [MemberData(nameof(CanRedo_CustomCanRedo_TestData))]
    public void RichTextBox_CanRedo_CustomCanRedo_ReturnsExpected(IntPtr result, bool expected)
    {
        using CustomCanRedoRichTextBox control = new()
        {
            Result = result
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(expected, control.CanRedo);
    }

    private class CustomCanRedoRichTextBox : RichTextBox
    {
        public IntPtr Result { get; set; }

        protected override unsafe void WndProc(ref Message m)
        {
            if (m.Msg == (int)PInvokeCore.EM_CANREDO)
            {
                m.Result = Result;
                return;
            }

            base.WndProc(ref m);
        }
    }

    [WinFormsFact]
    public void RichTextBox_CanRedo_GetCantCreateHandle_GetReturnsExpected()
    {
        using CantCreateHandleRichTextBox control = new();
        Assert.False(control.CanRedo);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_CanRedo_GetDisposed_ThrowsObjectDisposedException()
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.False(control.CanRedo);
    }

    [WinFormsFact]
    public void RichTextBox_CanUndo_GetWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.False(control.CanUndo);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> CanUndo_CustomCanUndo_TestData()
    {
        yield return new object[] { IntPtr.Zero, false };
        yield return new object[] { (IntPtr)1, true };
    }

    [WinFormsTheory]
    [MemberData(nameof(CanUndo_CustomCanUndo_TestData))]
    public void RichTextBox_CanUndo_CustomCanUndo_ReturnsExpected(IntPtr result, bool expected)
    {
        using CustomCanUndoRichTextBox control = new()
        {
            Result = result
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(expected, control.CanUndo);
    }

    private class CustomCanUndoRichTextBox : RichTextBox
    {
        public IntPtr Result { get; set; }

        protected override unsafe void WndProc(ref Message m)
        {
            if (m.Msg == (int)PInvokeCore.EM_CANUNDO)
            {
                m.Result = Result;
                return;
            }

            base.WndProc(ref m);
        }
    }

    [WinFormsFact]
    public void RichTextBox_CanUndo_GetCantCreateHandle_GetReturnsExpected()
    {
        using CantCreateHandleRichTextBox control = new();
        Assert.False(control.CanUndo);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_CanUndo_GetDisposed_ThrowsObjectDisposedException()
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.False(control.CanUndo);
    }

    [WinFormsFact]
    public void RichTextBox_DetectUrls_GetWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.True(control.DetectUrls);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call EM_AUTOURLDETECT.
        PInvokeCore.SendMessage(control, PInvokeCore.EM_AUTOURLDETECT, 0);
        Assert.True(control.DetectUrls);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void RichTextBox_DetectUrls_Set_GetReturnsExpected(bool value)
    {
        using RichTextBox control = new()
        {
            DetectUrls = value
        };
        Assert.Equal(value, control.DetectUrls);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.DetectUrls = value;
        Assert.Equal(value, control.DetectUrls);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.DetectUrls = !value;
        Assert.Equal(!value, control.DetectUrls);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 0)]
    [InlineData(false, 1)]
    public void RichTextBox_DetectUrls_SetWithHandle_GetReturnsExpected(bool value, int expectedCreatedCallCount)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.DetectUrls = value;
        Assert.Equal(value, control.DetectUrls);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        control.DetectUrls = value;
        Assert.Equal(value, control.DetectUrls);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set different.
        control.DetectUrls = !value;
        Assert.Equal(!value, control.DetectUrls);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount + 1, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void RichTextBox_DetectUrls_GetAutoUrlDetect_Success(bool value, int expected)
    {
        using RichTextBox control = new();

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.DetectUrls = value;
        Assert.Equal(expected, (int)PInvokeCore.SendMessage(control, PInvokeCore.EM_GETAUTOURLDETECT));
    }

    [WinFormsTheory]
    [BoolData]
    public void RichTextBox_EnableAutoDragDrop_Set_GetReturnsExpected(bool value)
    {
        using RichTextBox control = new()
        {
            EnableAutoDragDrop = value
        };
        Assert.Equal(value, control.EnableAutoDragDrop);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.EnableAutoDragDrop = value;
        Assert.Equal(value, control.EnableAutoDragDrop);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.EnableAutoDragDrop = value;
        Assert.Equal(value, control.EnableAutoDragDrop);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void RichTextBox_EnableAutoDragDrop_SetWithHandle_GetReturnsExpected(bool value)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.EnableAutoDragDrop = value;
        Assert.Equal(value, control.EnableAutoDragDrop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.EnableAutoDragDrop = value;
        Assert.Equal(value, control.EnableAutoDragDrop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.EnableAutoDragDrop = value;
        Assert.Equal(value, control.EnableAutoDragDrop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void RichTextBox_EnableAutoDragDrop_SetWithHandleAlreadyRegistered_GetReturnsExpected(bool value)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        DropTargetMock dropTarget = new();
        Assert.Equal(ApartmentState.STA, Application.OleRequired());
        Assert.Equal(HRESULT.DRAGDROP_E_ALREADYREGISTERED, PInvoke.RegisterDragDrop(control, dropTarget));

        control.EnableAutoDragDrop = value;
        Assert.Equal(value, control.EnableAutoDragDrop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.EnableAutoDragDrop = value;
        Assert.Equal(value, control.EnableAutoDragDrop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.EnableAutoDragDrop = value;
        Assert.Equal(value, control.EnableAutoDragDrop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [Theory]
    [BoolData]
    public void RichTextBox_EnableAutoDragDrop_SetWithHandleNonSTAThread_GetReturnsExpected(bool value)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.EnableAutoDragDrop = value;
        Assert.Equal(value, control.EnableAutoDragDrop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.EnableAutoDragDrop = value;
        Assert.Equal(value, control.EnableAutoDragDrop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.EnableAutoDragDrop = value;
        Assert.Equal(value, control.EnableAutoDragDrop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetFontTheoryData))]
    public void RichTextBox_Font_Set_GetReturnsExpected(Font value)
    {
        using SubRichTextBox control = new()
        {
            Font = value
        };
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetFontTheoryData))]
    public void RichTextBox_Font_SetWithText_GetReturnsExpected(Font value)
    {
        using SubRichTextBox control = new()
        {
            Text = "text",
            Font = value
        };
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetFontTheoryData))]
    public void RichTextBox_Font_SetWithNonNullOldValue_GetReturnsExpected(Font value)
    {
        using Font oldValue = new("Arial", 1);
        using SubRichTextBox control = new()
        {
            Font = oldValue
        };

        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetFontTheoryData))]
    public void RichTextBox_Font_SetWithNonNullOldValueWithText_GetReturnsExpected(Font value)
    {
        using Font oldValue = new("Arial", 1);
        using SubRichTextBox control = new()
        {
            Font = oldValue,
            Text = "text"
        };

        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Font_SetWithHandle_TestData()
    {
        foreach (bool userPaint in new bool[] { true, false })
        {
            yield return new object[] { userPaint, new Font("Arial", 8.25f), 1 };
            yield return new object[] { userPaint, null, 0 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Font_SetWithHandle_TestData))]
    public void RichTextBox_Font_SetWithHandle_GetReturnsExpected(bool userPaint, Font value, int expectedInvalidatedCallCount)
    {
        using SubRichTextBox control = new();
        control.SetStyle(ControlStyles.UserPaint, userPaint);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(userPaint, control.GetStyle(ControlStyles.UserPaint));
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        // Set different.
        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Font_SetWithHandle_TestData))]
    public void RichTextBox_Font_SetWithTextWithHandle_GetReturnsExpected(bool userPaint, Font value, int expectedInvalidatedCallCount)
    {
        using SubRichTextBox control = new()
        {
            Text = "text"
        };
        control.SetStyle(ControlStyles.UserPaint, userPaint);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(userPaint, control.GetStyle(ControlStyles.UserPaint));
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        // Set different.
        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> Font_SetWithNonNullOldValueWithHandle_TestData()
    {
        foreach (bool userPaint in new bool[] { true, false })
        {
            yield return new object[] { userPaint, new Font("Arial", 8.25f) };
            yield return new object[] { userPaint, null };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Font_SetWithNonNullOldValueWithHandle_TestData))]
    public void RichTextBox_Font_SetWithNonNullOldValueWithHandle_GetReturnsExpected(bool userPaint, Font value)
    {
        using Font oldValue = new("Arial", 1);
        using SubRichTextBox control = new()
        {
            Font = oldValue
        };
        control.SetStyle(ControlStyles.UserPaint, userPaint);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(userPaint, control.GetStyle(ControlStyles.UserPaint));
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        // Set different.
        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Font_SetWithNonNullOldValueWithHandle_TestData))]
    public void RichTextBox_Font_SetWithNonNullOldValueWithTextWithHandle_GetReturnsExpected(bool userPaint, Font value)
    {
        using Font oldValue = new("Arial", 1);
        using SubRichTextBox control = new()
        {
            Font = oldValue,
            Text = "text"
        };
        control.SetStyle(ControlStyles.UserPaint, userPaint);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(userPaint, control.GetStyle(ControlStyles.UserPaint));
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        // Set different.
        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> Font_GetCharFormat_TestData()
    {
        yield return new object[] { "Arial", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 1, 165, 0 };
        yield return new object[] { "Arial", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 1, 165, CFE_EFFECTS.CFE_BOLD };
        yield return new object[] { "Arial", 8.25f, FontStyle.Italic, GraphicsUnit.Point, 1, 165, CFE_EFFECTS.CFE_ITALIC };
        yield return new object[] { "Arial", 8.25f, FontStyle.Strikeout, GraphicsUnit.Point, 1, 165, CFE_EFFECTS.CFE_STRIKEOUT };
        yield return new object[] { "Arial", 8.25f, FontStyle.Underline, GraphicsUnit.Point, 1, 165, CFE_EFFECTS.CFE_UNDERLINE };
        yield return new object[] { "Arial", 8.25f, FontStyle.Bold | FontStyle.Italic | FontStyle.Regular | FontStyle.Strikeout | FontStyle.Underline, GraphicsUnit.Point, 10, 165, CFE_EFFECTS.CFE_BOLD | CFE_EFFECTS.CFE_ITALIC | CFE_EFFECTS.CFE_UNDERLINE | CFE_EFFECTS.CFE_STRIKEOUT };
    }

    [WinFormsTheory]
    [MemberData(nameof(Font_GetCharFormat_TestData))]
    public unsafe void RichTextBox_Font_GetCharFormat_Success(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet, int expectedYHeight, int expectedEffects)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        var format = new CHARFORMAT2W
        {
            cbSize = (uint)sizeof(CHARFORMAT2W),
            dwMask = (CFM_MASK)int.MaxValue
        };

        nint result;

        using (Font font = new(familyName, emSize, style, unit, gdiCharSet))
        {
            control.Font = font;
            result = PInvokeCore.SendMessage(control, PInvokeCore.EM_GETCHARFORMAT, (WPARAM)PInvoke.SCF_ALL, ref format);
            Assert.NotEqual(0, result);
            Assert.Equal(familyName, format.FaceName.ToString());
            Assert.Equal(expectedYHeight, format.yHeight);
            Assert.Equal(CFE_EFFECTS.CFE_AUTOBACKCOLOR | CFE_EFFECTS.CFE_AUTOCOLOR | (CFE_EFFECTS)expectedEffects, format.dwEffects);
            Assert.Equal(0, format.bPitchAndFamily);

            // Set null.
            control.Font = null;
        }

        var format1 = new CHARFORMAT2W
        {
            cbSize = (uint)sizeof(CHARFORMAT2W),
            dwMask = (CFM_MASK)int.MaxValue
        };

        result = PInvokeCore.SendMessage(control, PInvokeCore.EM_GETCHARFORMAT, (WPARAM)PInvoke.SCF_ALL, ref format1);
        Assert.NotEqual(0, result);
        Assert.Equal(Control.DefaultFont.Name, format1.FaceName.ToString());
        Assert.Equal((int)(Control.DefaultFont.SizeInPoints * 20), format1.yHeight);
        Assert.True(format1.dwEffects.HasFlag(CFE_EFFECTS.CFE_AUTOBACKCOLOR));
        Assert.True(format1.dwEffects.HasFlag(CFE_EFFECTS.CFE_AUTOCOLOR));
        Assert.Equal(0, format1.bPitchAndFamily);
    }

    [WinFormsFact]
    public void RichTextBox_Font_SetWithHandler_CallsFontChanged()
    {
        using RichTextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.FontChanged += handler;

        // Set different.
        using Font font1 = new("Arial", 8.25f);
        control.Font = font1;
        Assert.Same(font1, control.Font);
        Assert.Equal(1, callCount);

        // Set same.
        control.Font = font1;
        Assert.Same(font1, control.Font);
        Assert.Equal(1, callCount);

        // Set different.
        using var font2 = SystemFonts.DialogFont;
        control.Font = font2;
        Assert.Same(font2, control.Font);
        Assert.Equal(2, callCount);

        // Set null.
        control.Font = null;
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.Equal(3, callCount);

        // Remove handler.
        control.FontChanged -= handler;
        control.Font = font1;
        Assert.Same(font1, control.Font);
        Assert.Equal(3, callCount);
    }

    public static IEnumerable<object[]> ForeColor_Set_TestData()
    {
        yield return new object[] { Color.Empty, SystemColors.WindowText };
        yield return new object[] { Color.FromArgb(254, 1, 2, 3), Color.FromArgb(254, 1, 2, 3) };
        yield return new object[] { Color.White, Color.White };
        yield return new object[] { Color.Black, Color.Black };
        yield return new object[] { Color.Red, Color.Red };
    }

    [WinFormsTheory]
    [MemberData(nameof(ForeColor_Set_TestData))]
    public void RichTextBox_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using RichTextBox control = new()
        {
            ForeColor = value
        };
        Assert.Equal(expected, control.ForeColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ForeColor = value;
        Assert.Equal(expected, control.ForeColor);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> ForeColor_SetWithHandle_TestData()
    {
        yield return new object[] { Color.Empty, SystemColors.WindowText, 0 };
        yield return new object[] { Color.FromArgb(254, 1, 2, 3), Color.FromArgb(254, 1, 2, 3), 1 };
        yield return new object[] { Color.White, Color.White, 1 };
        yield return new object[] { Color.Black, Color.Black, 1 };
        yield return new object[] { Color.Red, Color.Red, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(ForeColor_SetWithHandle_TestData))]
    public void RichTextBox_ForeColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ForeColor = value;
        Assert.Equal(expected, control.ForeColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ForeColor = value;
        Assert.Equal(expected, control.ForeColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public unsafe void RichTextBox_ForeColor_GetCharFormat_Success()
    {
        using RichTextBox control = new();

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.ForeColor = Color.FromArgb(0x12, 0x34, 0x56, 0x78);
        var format = new CHARFORMAT2W
        {
            cbSize = (uint)sizeof(CHARFORMAT2W)
        };
        Assert.NotEqual(0, (int)PInvokeCore.SendMessage(control, PInvokeCore.EM_GETCHARFORMAT, (WPARAM)PInvoke.SCF_ALL, ref format));
        Assert.Equal(0x785634, format.crTextColor);
    }

    [WinFormsFact]
    public unsafe void RichTextBox_ForeColor_GetCharFormatWithTextColor_Success()
    {
        using RichTextBox control = new();

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.ForeColor = Color.FromArgb(0x12, 0x34, 0x56, 0x78);
        var format = new CHARFORMAT2W
        {
            cbSize = (uint)sizeof(CHARFORMAT2W)
        };
        Assert.NotEqual(0, (int)PInvokeCore.SendMessage(control, PInvokeCore.EM_GETCHARFORMAT, (WPARAM)PInvoke.SCF_ALL, ref format));
        Assert.Equal(0x785634, format.crTextColor);

        // Set different.
        control.ForeColor = Color.FromArgb(0x34, 0x56, 0x78, 0x90);
        format = new CHARFORMAT2W
        {
            cbSize = (uint)sizeof(CHARFORMAT2W)
        };
        Assert.NotEqual(0, (int)PInvokeCore.SendMessage(control, PInvokeCore.EM_GETCHARFORMAT, (WPARAM)PInvoke.SCF_ALL, ref format));
        Assert.Equal(0x907856, format.crTextColor);
    }

    [WinFormsFact]
    public void RichTextBox_ForeColor_SetWithHandler_CallsForeColorChanged()
    {
        using RichTextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.ForeColorChanged += handler;

        // Set different.
        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.Equal(1, callCount);

        // Set same.
        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.Equal(1, callCount);

        // Set different.
        control.ForeColor = Color.Empty;
        Assert.Equal(SystemColors.WindowText, control.ForeColor);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.ForeColorChanged -= handler;
        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.Equal(2, callCount);
    }

    public static IEnumerable<object[]> LanguageOption_CustomGetLangOptions_TestData()
    {
        foreach (RichTextBoxLanguageOptions options in Enum.GetValues(typeof(RichTextBoxLanguageOptions)))
        {
            yield return new object[] { (IntPtr)options, options };
        }

        yield return new object[] { IntPtr.Zero, 0 };
        yield return new object[] { (IntPtr)(-1), -1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(LanguageOption_CustomGetLangOptions_TestData))]
    public void RichTextBox_LanguageOption_CustomGetLangOptions_ReturnsExpected(IntPtr result, RichTextBoxLanguageOptions expected)
    {
        using CustomGetLangOptionsRichTextBox control = new()
        {
            GetLangOptionsResult = result
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.MakeCustom = true;
        Assert.Equal(expected, control.LanguageOption);
    }

    private class CustomGetLangOptionsRichTextBox : RichTextBox
    {
        public bool MakeCustom { get; set; }
        public IntPtr GetLangOptionsResult { get; set; }

        protected override void WndProc(ref Message m)
        {
            if (MakeCustom && m.Msg == (int)PInvokeCore.EM_GETLANGOPTIONS)
            {
                m.Result = GetLangOptionsResult;
                return;
            }

            base.WndProc(ref m);
        }
    }

    [WinFormsTheory]
    [EnumData<RichTextBoxLanguageOptions>]
    public void RichTextBox_LanguageOption_Set_GetReturnsExpected(RichTextBoxLanguageOptions value)
    {
        using RichTextBox control = new()
        {
            LanguageOption = value
        };
        Assert.Equal(value, control.LanguageOption);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.LanguageOption = value;
        Assert.Equal(value, control.LanguageOption);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [EnumData<RichTextBoxLanguageOptions>]
    public void RichTextBox_LanguageOption_SetWithHandle_GetReturnsExpected(RichTextBoxLanguageOptions value)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.LanguageOption = value;
        Assert.Equal(value, control.LanguageOption);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.LanguageOption = value;
        Assert.Equal(value, control.LanguageOption);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [EnumData<RichTextBoxLanguageOptions>]
    public unsafe void RichTextBox_LanguageOption_GetCharFormat_Success(RichTextBoxLanguageOptions value)
    {
        using RichTextBox control = new();

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.LanguageOption = value;
        Assert.Equal(value, (RichTextBoxLanguageOptions)(int)PInvokeCore.SendMessage(control, PInvokeCore.EM_GETLANGOPTIONS));
    }

    [WinFormsFact]
    public void RichTextBox_MaxLength_GetWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(0x7FFFFFFF, control.MaxLength);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call EM_LIMITTEXT.
        PInvokeCore.SendMessage(control, PInvokeCore.EM_LIMITTEXT, 0, 1);
        Assert.Equal(0x7FFFFFFF, control.MaxLength);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call EM_EXLIMITTEXT.
        PInvokeCore.SendMessage(control, PInvokeCore.EM_EXLIMITTEXT, 0, 2);
        Assert.Equal(0x7FFFFFFF, control.MaxLength);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(64000)]
    [InlineData(0x7FFFFFFE)]
    [InlineData(int.MaxValue)]
    public void RichTextBox_MaxLength_Set_GetReturnsExpected(int value)
    {
        using RichTextBox control = new()
        {
            MaxLength = value
        };
        Assert.Equal(value, control.MaxLength);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.MaxLength = value;
        Assert.Equal(value, control.MaxLength);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_MaxLength_SetWithLongText_Success()
    {
        using RichTextBox control = new()
        {
            Text = "Text",
            MaxLength = 2
        };
        Assert.Equal(2, control.MaxLength);
        Assert.Equal("Text", control.Text);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(64000)]
    [InlineData(0x7FFFFFFE)]
    [InlineData(int.MaxValue)]
    public void RichTextBox_MaxLength_SetWithHandle_GetReturnsExpected(int value)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.MaxLength = value;
        Assert.Equal(value, control.MaxLength);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.MaxLength = value;
        Assert.Equal(value, control.MaxLength);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_MaxLength_SetWithLongTextWithHandle_Success()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Text = "Text";
        control.MaxLength = 2;
        Assert.Equal(2, control.MaxLength);
        Assert.Equal("Text", control.Text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(0, 0x10000)]
    [InlineData(1, 1)]
    [InlineData(64000, 64000)]
    [InlineData(0x7FFFFFFE, 0x7FFFFFFE)]
    [InlineData(int.MaxValue, 0x7FFFFFFF)]
    public void RichTextBox_MaxLength_GetLimitText_Success(int value, int expected)
    {
        using RichTextBox control = new();

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.MaxLength = value;
        Assert.Equal(expected, (int)PInvokeCore.SendMessage(control, PInvokeCore.EM_GETLIMITTEXT));
    }

    [WinFormsFact]
    public void RichTextBox_MaxLength_SetNegative_ThrowsArgumentOutOfRangeException()
    {
        using RichTextBox control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.MaxLength = -1);
    }

    [WinFormsTheory]
    [BoolData]
    public void RichTextBox_Multiline_Set_GetReturnsExpected(bool value)
    {
        using SubRichTextBox control = new()
        {
            AutoSize = false,
            Multiline = value
        };
        Assert.Equal(value, control.Multiline);
        Assert.Equal(96, control.Height);
        Assert.False(control.GetStyle(ControlStyles.FixedHeight));
        Assert.False(control.AutoSize);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Multiline = value;
        Assert.Equal(value, control.Multiline);
        Assert.Equal(96, control.Height);
        Assert.False(control.GetStyle(ControlStyles.FixedHeight));
        Assert.False(control.AutoSize);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.Multiline = !value;
        Assert.Equal(!value, control.Multiline);
        Assert.Equal(96, control.Height);
        Assert.False(control.GetStyle(ControlStyles.FixedHeight));
        Assert.False(control.AutoSize);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Multiline_SetAutoSize_TestData()
    {
        yield return new object[] { true, 96, s_preferredHeight };
        yield return new object[] { false, s_preferredHeight, 96 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Multiline_SetAutoSize_TestData))]
    public void RichTextBox_Multiline_SetAutoSize_GetReturnsExpected(bool value, int expectedHeight1, int expectedHeight2)
    {
        using SubRichTextBox control = new()
        {
            AutoSize = true,
            Multiline = value
        };
        Assert.Equal(value, control.Multiline);
        Assert.Equal(expectedHeight1, control.Height);
        Assert.Equal(!value, control.GetStyle(ControlStyles.FixedHeight));
        Assert.True(control.AutoSize);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Multiline = value;
        Assert.Equal(value, control.Multiline);
        Assert.Equal(expectedHeight1, control.Height);
        Assert.Equal(!value, control.GetStyle(ControlStyles.FixedHeight));
        Assert.True(control.AutoSize);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.Multiline = !value;
        Assert.Equal(!value, control.Multiline);
        Assert.Equal(expectedHeight2, control.Height);
        Assert.Equal(value, control.GetStyle(ControlStyles.FixedHeight));
        Assert.True(control.AutoSize);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void RichTextBox_Multiline_SetWithParent_GetReturnsExpected(bool value)
    {
        using Control parent = new();
        using SubRichTextBox control = new()
        {
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            control.Multiline = value;
            Assert.Equal(value, control.Multiline);
            Assert.Equal(96, control.Height);
            Assert.False(control.GetStyle(ControlStyles.FixedHeight));
            Assert.False(control.AutoSize);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            control.Multiline = value;
            Assert.Equal(value, control.Multiline);
            Assert.Equal(96, control.Height);
            Assert.False(control.GetStyle(ControlStyles.FixedHeight));
            Assert.False(control.AutoSize);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set different.
            control.Multiline = !value;
            Assert.Equal(!value, control.Multiline);
            Assert.Equal(96, control.Height);
            Assert.False(control.GetStyle(ControlStyles.FixedHeight));
            Assert.False(control.AutoSize);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> Multiline_SetAutoSizeWithParent_TestData()
    {
        yield return new object[] { true, 96, 0, s_preferredHeight };
        yield return new object[] { false, s_preferredHeight, 1, 96 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Multiline_SetAutoSizeWithParent_TestData))]
    public void RichTextBox_Multiline_SetAutoSizeWithParent_GetReturnsExpected(bool value, int expectedHeight1, int expectedParentLayoutCallCount, int expectedHeight2)
    {
        using Control parent = new();
        using SubRichTextBox control = new()
        {
            AutoSize = true,
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.Multiline = value;
            Assert.Equal(value, control.Multiline);
            Assert.Equal(expectedHeight1, control.Height);
            Assert.Equal(!value, control.GetStyle(ControlStyles.FixedHeight));
            Assert.True(control.AutoSize);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            control.Multiline = value;
            Assert.Equal(value, control.Multiline);
            Assert.Equal(expectedHeight1, control.Height);
            Assert.Equal(!value, control.GetStyle(ControlStyles.FixedHeight));
            Assert.True(control.AutoSize);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set different.
            control.Multiline = !value;
            Assert.Equal(!value, control.Multiline);
            Assert.Equal(expectedHeight2, control.Height);
            Assert.Equal(value, control.GetStyle(ControlStyles.FixedHeight));
            Assert.True(control.AutoSize);
            Assert.Equal(expectedParentLayoutCallCount + 1, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(true, 0)]
    [InlineData(false, 1)]
    public void RichTextBox_Multiline_SetWithHandle_GetReturnsExpected(bool value, int expectedCreatedCallCount)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Multiline = value;
        Assert.Equal(value, control.Multiline);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        control.Multiline = value;
        Assert.Equal(value, control.Multiline);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set different.
        control.Multiline = !value;
        Assert.Equal(!value, control.Multiline);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount + 1, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_Multiline_SetWithHandler_CallsMultilineChanged()
    {
        using RichTextBox control = new()
        {
            Multiline = true
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.MultilineChanged += handler;

        // Set different.
        control.Multiline = false;
        Assert.False(control.Multiline);
        Assert.Equal(1, callCount);

        // Set same.
        control.Multiline = false;
        Assert.False(control.Multiline);
        Assert.Equal(1, callCount);

        // Set different.
        control.Multiline = true;
        Assert.True(control.Multiline);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.MultilineChanged -= handler;
        control.Multiline = false;
        Assert.False(control.Multiline);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void RichTextBox_RedoActionName_GetWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Empty(control.RedoActionName);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> RedoActionName_CustomGetRedoName_TestData()
    {
        yield return new object[] { IntPtr.Zero, IntPtr.Zero, string.Empty };
        yield return new object[] { (IntPtr)1, IntPtr.Zero, "Unknown" };
        yield return new object[] { (IntPtr)1, (IntPtr)1, "Typing" };
        yield return new object[] { (IntPtr)1, (IntPtr)2, "Delete" };
        yield return new object[] { (IntPtr)1, (IntPtr)3, "Drag and Drop" };
        yield return new object[] { (IntPtr)1, (IntPtr)4, "Cut" };
        yield return new object[] { (IntPtr)1, (IntPtr)5, "Paste" };
        yield return new object[] { (IntPtr)1, (IntPtr)6, "Unknown" };
        yield return new object[] { (IntPtr)1, (IntPtr)7, "Unknown" };
    }

    [WinFormsTheory]
    [MemberData(nameof(RedoActionName_CustomGetRedoName_TestData))]
    public void RichTextBox_RedoActionName_CustomGetRedoName_ReturnsExpected(IntPtr canRedoResult, IntPtr getRedoNameResult, string expected)
    {
        using CustomGetRedoNameRichTextBox control = new()
        {
            CanRedoResult = canRedoResult,
            GetRedoNameResult = getRedoNameResult
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(expected, control.RedoActionName);
    }

    private class CustomGetRedoNameRichTextBox : RichTextBox
    {
        public IntPtr CanRedoResult { get; set; }
        public IntPtr GetRedoNameResult { get; set; }

        protected override unsafe void WndProc(ref Message m)
        {
            if (m.Msg == (int)PInvokeCore.EM_CANREDO)
            {
                m.Result = CanRedoResult;
                return;
            }
            else if (m.Msg == (int)PInvokeCore.EM_GETREDONAME)
            {
                m.Result = GetRedoNameResult;
                return;
            }

            base.WndProc(ref m);
        }
    }

    [WinFormsFact]
    public void RichTextBox_RedoActionName_GetCantCreateHandle_GetReturnsExpected()
    {
        using CantCreateHandleRichTextBox control = new();
        Assert.Empty(control.RedoActionName);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_RedoActionName_GetDisposed_ThrowsObjectDisposedException()
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Empty(control.RedoActionName);
    }

    [WinFormsTheory]
    [BoolData]
    public void RichTextBox_RichTextShortcutsEnabled_Set_GetReturnsExpected(bool value)
    {
        using RichTextBox control = new()
        {
            RichTextShortcutsEnabled = value
        };
        Assert.Equal(value, control.RichTextShortcutsEnabled);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.RichTextShortcutsEnabled = value;
        Assert.Equal(value, control.RichTextShortcutsEnabled);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.RichTextShortcutsEnabled = !value;
        Assert.Equal(!value, control.RichTextShortcutsEnabled);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void RichTextBox_RichTextShortcutsEnabled_SetWithHandle_GetReturnsExpected(bool value)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.RichTextShortcutsEnabled = value;
        Assert.Equal(value, control.RichTextShortcutsEnabled);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.RichTextShortcutsEnabled = value;
        Assert.Equal(value, control.RichTextShortcutsEnabled);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.RichTextShortcutsEnabled = !value;
        Assert.Equal(!value, control.RichTextShortcutsEnabled);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_RightMargin_GetWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(0, control.RightMargin);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(64000)]
    [InlineData(0x7FFFFFFE)]
    [InlineData(int.MaxValue)]
    public void RichTextBox_RightMargin_Set_GetReturnsExpected(int value)
    {
        using RichTextBox control = new()
        {
            RightMargin = value
        };
        Assert.Equal(value, control.RightMargin);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.RightMargin = value;
        Assert.Equal(value, control.RightMargin);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(64000)]
    [InlineData(0x7FFFFFFE)]
    [InlineData(int.MaxValue)]
    public void RichTextBox_RightMargin_SetWithCustomOldValue_GetReturnsExpected(int value)
    {
        using RichTextBox control = new()
        {
            RightMargin = 1
        };

        control.RightMargin = value;
        Assert.Equal(value, control.RightMargin);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.RightMargin = value;
        Assert.Equal(value, control.RightMargin);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(64000)]
    [InlineData(0x7FFFFFFE)]
    [InlineData(int.MaxValue)]
    public void RichTextBox_RightMargin_SetWithHandle_GetReturnsExpected(int value)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.RightMargin = value;
        Assert.Equal(value, control.RightMargin);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.RightMargin = value;
        Assert.Equal(value, control.RightMargin);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    [InlineData(64000, 0)]
    [InlineData(0x7FFFFFFE, 0)]
    [InlineData(int.MaxValue, 0)]
    public void RichTextBox_RightMargin_SetWithCustomOldValueWithHandle_GetReturnsExpected(int value, int expectedCreatedCallCount)
    {
        using RichTextBox control = new()
        {
            RightMargin = 1
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.RightMargin = value;
        Assert.Equal(value, control.RightMargin);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        control.RightMargin = value;
        Assert.Equal(value, control.RightMargin);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_RightMargin_SetNegative_ThrowsArgumentOutOfRangeException()
    {
        using RichTextBox control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.RightMargin = -1);
    }

    [WinFormsFact]
    public void RichTextBox_Rtf_GetWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        string rtf1 = control.Rtf;
        Assert.StartsWith("{\\rtf", rtf1);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Get again.
        string rtf2 = control.Rtf;
        Assert.Equal(rtf1, rtf2);
        Assert.NotSame(rtf1, rtf2);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_Rtf_GetWithPlainText_ReturnsExpected()
    {
        using RichTextBox control = new()
        {
            Text = "Text"
        };
        string rtf1 = control.Rtf;
        Assert.Contains("Text", rtf1);
        Assert.StartsWith("{\\rtf", rtf1);
        Assert.True(control.IsHandleCreated);

        // Get again.
        string rtf2 = control.Rtf;
        Assert.Equal(rtf1, rtf2);
        Assert.NotSame(rtf1, rtf2);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_Rtf_GetWithPlainTextWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new()
        {
            Text = "Text"
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        string rtf1 = control.Rtf;
        Assert.Contains("Text", rtf1);
        Assert.StartsWith("{\\rtf", rtf1);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Get again.
        string rtf2 = control.Rtf;
        Assert.Equal(rtf1, rtf2);
        Assert.NotSame(rtf1, rtf2);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_Rtf_GetCantCreateHandle_ReturnsNull()
    {
        using CantCreateHandleRichTextBox control = new();
        Assert.Null(control.Rtf);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_Rtf_GetDisposed_ReturnsNull()
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Null(control.Rtf);
    }

    [WinFormsFact]
    public void RichTextBox_Rtf_GetDisposedWithText_ThrowsObjectDisposedException()
    {
        using RichTextBox control = new();
        control.Dispose();
        control.Text = "text";
        Assert.Throws<ObjectDisposedException>(() => control.Rtf);
    }

    [WinFormsTheory]
    [NullAndEmptyStringData]
    public void RichTextBox_Rtf_Set_GetReturnsExpected(string nullOrEmpty)
    {
        using RichTextBox control = new()
        {
            Rtf = "{\\rtf1Hello World}"
        };

        string rtf = control.Rtf;
        Assert.StartsWith("{\\rtf", rtf);
        Assert.NotSame(rtf, control.Rtf);
        Assert.Equal("Hello World", control.Text);
        Assert.True(control.IsHandleCreated);

        control.Rtf = nullOrEmpty;
        rtf = control.Rtf;
        Assert.StartsWith("{\\rtf", rtf);
        Assert.Empty(control.Text);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData("{\\rtf1Hello World}", "Hello World")]
    [InlineData(@"{\rtf1\ansi{Sample for {\v HIDDEN }text}}", "Sample for HIDDEN text")]
    public void RichTextBox_Rtf_Set_GetTextExpected(string rtf, string plainText)
    {
        using RichTextBox control = new()
        {
            Rtf = rtf
        };

        string readRtf = control.Rtf;
        Assert.StartsWith("{\\rtf", readRtf);
        Assert.NotSame(readRtf, control.Rtf);
        Assert.Equal(plainText, control.Text);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NullAndEmptyStringData]
    public void RichTextBox_Rtf_SetWithHandle_GetReturnsExpected(string nullOrEmpty)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Rtf = "{\\rtf1Hello World}";
        string rtf = control.Rtf;
        Assert.StartsWith("{\\rtf", rtf);
        Assert.NotSame(rtf, control.Rtf);
        Assert.Equal("Hello World", control.Text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        control.Rtf = nullOrEmpty;
        rtf = control.Rtf;
        Assert.StartsWith("{\\rtf", rtf);
        Assert.Empty(control.Text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_Rtf_SetWithHandler_CallsTextChanged()
    {
        using RichTextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(EventArgs.Empty, e);
            callCount++;
        };
        control.TextChanged += handler;

        // Set different.
        control.Rtf = "{\\rtf1text}";
        Assert.Equal("text", control.Text);
        Assert.Equal(2, callCount);

        // Set same.
        control.Rtf = "{\\rtf1text}";
        Assert.Equal("text", control.Text);
        Assert.Equal(4, callCount);

        // Set different.
        control.Text = null;
        Assert.Empty(control.Text);
        Assert.Equal(5, callCount);

        // Remove handler.
        control.TextChanged -= handler;
        control.Rtf = "{\\rtf1text}";
        Assert.Equal("text", control.Text);
        Assert.Equal(5, callCount);
    }

    [WinFormsFact]
    public void RichTextBox_Rtf_SetWithHandlerWithHandle_CallsTextChanged()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(EventArgs.Empty, e);
            callCount++;
        };
        control.TextChanged += handler;

        // Set different.
        control.Rtf = "{\\rtf1text}";
        Assert.Equal("text", control.Text);
        Assert.Equal(2, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Rtf = "{\\rtf1text}";
        Assert.Equal("text", control.Text);
        Assert.Equal(4, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.Rtf = null;
        Assert.Empty(control.Text);
        Assert.Equal(6, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.TextChanged -= handler;
        control.Rtf = "{\\rtf1text}";
        Assert.Equal("text", control.Text);
        Assert.Equal(6, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_Rtf_SetInvalidValue_ThrowsArgumentException()
    {
        using RichTextBox control = new();
        Assert.Throws<ArgumentException>(() => control.Rtf = "text");
        Assert.True(control.IsHandleCreated);
        Assert.DoesNotContain("text", control.Rtf);
        Assert.Empty(control.Text);
    }

    [WinFormsTheory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("{\\rtf Hello World}", "{\\rtf Hello World}")]
    public void RichTextBox_Rtf_SetCantCreateHandle_GetReturnsExpected(string value, string expected)
    {
        using CantCreateHandleRichTextBox control = new();
        control.Rtf = value;
        Assert.Equal(expected, control.Rtf);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Rtf = value;
        Assert.Equal(expected, control.Rtf);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void RichTextBox_Rtf_SetDisposed_ThrowsObjectDisposedException(string value)
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.Rtf = value);
    }

    public static IEnumerable<object[]> ScrollBars_Set_TestData()
    {
        foreach (bool autoSize in new bool[] { true, false })
        {
            foreach (RichTextBoxScrollBars value in Enum.GetValues(typeof(RichTextBoxScrollBars)))
            {
                yield return new object[] { autoSize, value };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ScrollBars_Set_TestData))]
    public void RichTextBox_ScrollBars_Set_GetReturnsExpected(bool autoSize, RichTextBoxScrollBars value)
    {
        using RichTextBox control = new()
        {
            AutoSize = autoSize
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.ScrollBars = value;
        Assert.Equal(value, control.ScrollBars);
        Assert.Equal(autoSize, control.AutoSize);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ScrollBars = value;
        Assert.Equal(value, control.ScrollBars);
        Assert.Equal(autoSize, control.AutoSize);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> ScrollBars_SetWithParent_TestData()
    {
        yield return new object[] { false, RichTextBoxScrollBars.Both, 0 };
        yield return new object[] { false, RichTextBoxScrollBars.ForcedBoth, 0 };
        yield return new object[] { false, RichTextBoxScrollBars.ForcedHorizontal, 0 };
        yield return new object[] { false, RichTextBoxScrollBars.ForcedVertical, 0 };
        yield return new object[] { false, RichTextBoxScrollBars.Horizontal, 0 };
        yield return new object[] { false, RichTextBoxScrollBars.None, 0 };
        yield return new object[] { false, RichTextBoxScrollBars.Vertical, 0 };

        yield return new object[] { true, RichTextBoxScrollBars.Both, 0 };
        yield return new object[] { true, RichTextBoxScrollBars.ForcedBoth, 1 };
        yield return new object[] { true, RichTextBoxScrollBars.ForcedHorizontal, 1 };
        yield return new object[] { true, RichTextBoxScrollBars.ForcedVertical, 1 };
        yield return new object[] { true, RichTextBoxScrollBars.Horizontal, 1 };
        yield return new object[] { true, RichTextBoxScrollBars.None, 1 };
        yield return new object[] { true, RichTextBoxScrollBars.Vertical, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(ScrollBars_SetWithParent_TestData))]
    public void RichTextBox_ScrollBars_SetWithParent_GetReturnsExpected(bool autoSize, RichTextBoxScrollBars value, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using RichTextBox control = new()
        {
            AutoSize = autoSize,
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("ScrollBars", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.ScrollBars = value;
            Assert.Equal(value, control.ScrollBars);
            Assert.Equal(autoSize, control.AutoSize);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ScrollBars = value;
            Assert.Equal(value, control.ScrollBars);
            Assert.Equal(autoSize, control.AutoSize);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> ScrollBars_SetWithHandle_TestData()
    {
        foreach (bool autoSize in new bool[] { true, false })
        {
            yield return new object[] { autoSize, RichTextBoxScrollBars.Both, 0, 0 };
            yield return new object[] { autoSize, RichTextBoxScrollBars.ForcedBoth, 1, 1 };
            yield return new object[] { autoSize, RichTextBoxScrollBars.ForcedHorizontal, 0, 1 };
            yield return new object[] { autoSize, RichTextBoxScrollBars.ForcedVertical, 1, 1 };
            yield return new object[] { autoSize, RichTextBoxScrollBars.Horizontal, 0, 1 };
            yield return new object[] { autoSize, RichTextBoxScrollBars.None, 0, 1 };
            yield return new object[] { autoSize, RichTextBoxScrollBars.Vertical, 0, 1 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ScrollBars_SetWithHandle_TestData))]
    public void RichTextBox_ScrollBars_SetWithHandle_GetReturnsExpected(bool autoSize, RichTextBoxScrollBars value, int expectedLayoutCallCount, int expectedCreatedCallCount)
    {
        using RichTextBox control = new()
        {
            AutoSize = autoSize
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            layoutCallCount++;
        };

        control.ScrollBars = value;
        Assert.Equal(value, control.ScrollBars);
        Assert.Equal(autoSize, control.AutoSize);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        control.ScrollBars = value;
        Assert.Equal(value, control.ScrollBars);
        Assert.Equal(autoSize, control.AutoSize);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    public static IEnumerable<object[]> ScrollBars_SetWithParentWithHandle_TestData()
    {
        yield return new object[] { false, RichTextBoxScrollBars.Both, 0, 0, 0 };
        yield return new object[] { false, RichTextBoxScrollBars.ForcedBoth, 1, 1, 1 };
        yield return new object[] { false, RichTextBoxScrollBars.ForcedHorizontal, 0, 0, 1 };
        yield return new object[] { false, RichTextBoxScrollBars.ForcedVertical, 1, 1, 1 };
        yield return new object[] { false, RichTextBoxScrollBars.Horizontal, 0, 0, 1 };
        yield return new object[] { false, RichTextBoxScrollBars.None, 0, 0, 1 };
        yield return new object[] { false, RichTextBoxScrollBars.Vertical, 0, 0, 1 };

        yield return new object[] { true, RichTextBoxScrollBars.Both, 0, 0, 0 };
        yield return new object[] { true, RichTextBoxScrollBars.ForcedBoth, 1, 1, 1 };
        yield return new object[] { true, RichTextBoxScrollBars.ForcedHorizontal, 0, 1, 1 };
        yield return new object[] { true, RichTextBoxScrollBars.ForcedVertical, 1, 1, 1 };
        yield return new object[] { true, RichTextBoxScrollBars.Horizontal, 0, 1, 1 };
        yield return new object[] { true, RichTextBoxScrollBars.None, 0, 1, 1 };
        yield return new object[] { true, RichTextBoxScrollBars.Vertical, 0, 1, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(ScrollBars_SetWithParentWithHandle_TestData))]
    public void RichTextBox_ScrollBars_SetWithParentWithHandle_GetReturnsExpected(bool autoSize, RichTextBoxScrollBars value, int expectedLayoutCallCount, int expectedParentLayoutCallCount, int expectedCreatedCallCount)
    {
        using Control parent = new();
        using RichTextBox control = new()
        {
            AutoSize = autoSize,
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            layoutCallCount++;
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.ScrollBars = value;
            Assert.Equal(value, control.ScrollBars);
            Assert.Equal(autoSize, control.AutoSize);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set same.
            control.ScrollBars = value;
            Assert.Equal(value, control.ScrollBars);
            Assert.Equal(autoSize, control.AutoSize);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InvalidEnumData<RichTextBoxScrollBars>]
    [InlineData(RichTextBoxScrollBars.Both + 1)]
    [InlineData(RichTextBoxScrollBars.ForcedHorizontal - 1)]
    public void RichTextBox_ScrollBars_SetInvalidValue_ThrowsInvalidEnumArgumentException(RichTextBoxScrollBars value)
    {
        using RichTextBox control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.ScrollBars = value);
    }

    [WinFormsFact]
    public void RichTextBox_SelectedRtf_Get_ReturnsExpected()
    {
        using RichTextBox control = new();
        string rtf1 = control.SelectedRtf;
        Assert.Empty(rtf1);
        Assert.True(control.IsHandleCreated);

        // Get again.
        string rtf2 = control.SelectedRtf;
        Assert.Equal(rtf1, rtf2);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectedRtf_GetWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        string rtf1 = control.SelectedRtf;
        Assert.Empty(rtf1);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Get again.
        string rtf2 = control.SelectedRtf;
        Assert.Equal(rtf1, rtf2);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_SelectedRtf_GetWithPlainText_ReturnsExpected()
    {
        using RichTextBox control = new()
        {
            Text = "Text",
            SelectedText = "ex"
        };
        string rtf1 = control.SelectedRtf;
        Assert.Empty(rtf1);
        Assert.True(control.IsHandleCreated);

        // Get again.
        string rtf2 = control.SelectedRtf;
        Assert.Equal(rtf1, rtf2);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectedRtf_GetWithPlainTextWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new()
        {
            Text = "Text",
            SelectedText = "ex"
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        string rtf1 = control.SelectedRtf;
        Assert.Empty(rtf1);
        Assert.DoesNotContain("ex", rtf1);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Get again.
        string rtf2 = control.SelectedRtf;
        Assert.Equal(rtf1, rtf2);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_SelectedRtf_GetCantCreateHandle_GetReturnsExpected()
    {
        using CantCreateHandleRichTextBox control = new();
        Assert.Empty(control.SelectedRtf);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectedRtf_GetDisposed_ThrowsObjectDisposedException()
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectedRtf);
    }

    [WinFormsTheory]
    [NullAndEmptyStringData]
    public void RichTextBox_SelectedRtf_Set_GetReturnsExpected(string nullOrEmpty)
    {
        using RichTextBox control = new()
        {
            SelectedRtf = "{\\rtf1Hell}"
        };

        string rtf = control.SelectedRtf;
        Assert.Empty(rtf);
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);

        control.SelectedRtf = nullOrEmpty;
        rtf = control.SelectedRtf;
        Assert.Empty(rtf);
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NullAndEmptyStringData]
    public void RichTextBox_SelectedRtf_SetWithHandle_GetReturnsExpected(string nullOrEmpty)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SelectedRtf = "{\\rtf1Hell}";
        string rtf = control.SelectedRtf;
        Assert.Empty(rtf);
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        control.SelectedRtf = nullOrEmpty;
        rtf = control.SelectedRtf;
        Assert.Empty(rtf);
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [NullAndEmptyStringData]
    public void RichTextBox_SelectedRtf_SetWithRtf_GetReturnsExpected(string nullOrEmpty)
    {
        using RichTextBox control = new()
        {
            Rtf = "{\\rtf1Hello World}",
            SelectedRtf = "{\\rtf1Hell}"
        };

        string rtf = control.SelectedRtf;
        Assert.Empty(rtf);
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);

        control.SelectedRtf = nullOrEmpty;
        rtf = control.SelectedRtf;
        Assert.Empty(rtf);
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NullAndEmptyStringData]
    public void RichTextBox_SelectedRtf_SetWithRtfWithHandle_GetReturnsExpected(string nullOrEmpty)
    {
        using RichTextBox control = new()
        {
            Rtf = "{\\rtf1Hello World}"
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SelectedRtf = "{\\rtf1Hell}";
        string rtf = control.SelectedRtf;
        Assert.Empty(rtf);
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        control.SelectedRtf = nullOrEmpty;
        rtf = control.SelectedRtf;
        Assert.Empty(rtf);
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_SelectedRtf_SetInvalidValue_ThrowsArgumentException()
    {
        using RichTextBox control = new();
        Assert.Throws<ArgumentException>(() => control.SelectedRtf = "text");
        Assert.True(control.IsHandleCreated);
        Assert.DoesNotContain("text", control.Rtf);
        Assert.Empty(control.Text);
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("{\\rtf Hello World}")]
    public void RichTextBox_SelectedRtf_SetCantCreateHandle_GetReturnsExpected(string value)
    {
        using CantCreateHandleRichTextBox control = new();
        control.SelectedRtf = value;
        Assert.Empty(control.SelectedRtf);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SelectedRtf = value;
        Assert.Empty(control.SelectedRtf);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void RichTextBox_SelectedRtf_SetDisposed_ThrowsObjectDisposedException(string value)
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectedRtf = value);
    }

    [WinFormsFact]
    public void RichTextBox_SelectedText_Get_ReturnsExpected()
    {
        using RichTextBox control = new();
        string text1 = control.SelectedText;
        Assert.Empty(text1);
        Assert.True(control.IsHandleCreated);

        // Get again.
        string text2 = control.SelectedText;
        Assert.Equal(text1, text2);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectedText_GetWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        string text1 = control.SelectedText;
        Assert.Empty(text1);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Get again.
        string text2 = control.SelectedText;
        Assert.Equal(text1, text2);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_SelectedText_GetWithPlainText_ReturnsExpected()
    {
        using RichTextBox control = new()
        {
            Text = "Text",
            SelectedText = "ex"
        };
        string text1 = control.SelectedText;
        Assert.Empty(text1);
        Assert.True(control.IsHandleCreated);

        // Get again.
        string text2 = control.SelectedText;
        Assert.Equal(text1, text2);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectedText_GetWithPlainTextWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new()
        {
            Text = "Text",
            SelectedText = "ex"
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        string text1 = control.SelectedText;
        Assert.Empty(text1);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Get again.
        string text2 = control.SelectedText;
        Assert.Equal(text1, text2);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_SelectedText_GetDisposed_ThrowsObjectDisposedException()
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectedText);
    }

    [WinFormsTheory]
    [StringData]
    public void RichTextBox_SelectedText_Set_GetReturnsExpected(string value)
    {
        using RichTextBox control = new()
        {
            SelectedText = value
        };
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.SelectedText = value;
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [StringData]
    public void RichTextBox_SelectedText_SetWithHandle_GetReturnsExpected(string value)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SelectedText = value;
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SelectedText = value;
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [StringData]
    public void RichTextBox_SelectedText_SetWithRtf_GetReturnsExpected(string value)
    {
        using RichTextBox control = new()
        {
            Rtf = "{\\rtf1Hello World}",
            SelectedText = value
        };
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.SelectedText = value;
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [StringData]
    public void RichTextBox_SelectedText_SetWithText_GetReturnsExpected(string value)
    {
        using RichTextBox control = new()
        {
            Text = "Hello World",
            SelectedText = value
        };
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.SelectedText = value;
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [StringData]
    public void RichTextBox_SelectedText_SetWithTextWithHandle_GetReturnsExpected(string value)
    {
        using RichTextBox control = new()
        {
            Text = "Hello World"
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SelectedText = value;
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SelectedText = value;
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void RichTextBox_SelectedText_SetDisposed_ThrowsObjectDisposedException(string value)
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectedText = value);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionAlignment_GetWithoutHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.Equal(HorizontalAlignment.Left, control.SelectionAlignment);
        Assert.True(control.IsHandleCreated);

        // Call again.
        Assert.Equal(HorizontalAlignment.Left, control.SelectionAlignment);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionAlignment_GetWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(HorizontalAlignment.Left, control.SelectionAlignment);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        Assert.Equal(HorizontalAlignment.Left, control.SelectionAlignment);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> SelectionAlignment_CustomGetParamFormat_TestData()
    {
        yield return new object[] { 0, 0, HorizontalAlignment.Left };
        yield return new object[] { 0, PFA.CENTER, HorizontalAlignment.Left };
        yield return new object[] { PFM.ALIGNMENT, PFA.LEFT, HorizontalAlignment.Left };
        yield return new object[] { PFM.ALIGNMENT, PFA.CENTER, HorizontalAlignment.Center };
        yield return new object[] { PFM.ALIGNMENT, PFA.RIGHT, HorizontalAlignment.Right };
        yield return new object[] { PFM.ALIGNMENT, PFA.JUSTIFY, HorizontalAlignment.Left };
        yield return new object[] { PFM.ALIGNMENT, PFA.FULL_INTERWORD, HorizontalAlignment.Left };
        yield return new object[] { PFM.ALIGNMENT | PFM.ALIGNMENT, PFA.CENTER, HorizontalAlignment.Center };
        yield return new object[] { PFM.NUMBERING, PFA.CENTER, HorizontalAlignment.Left };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionAlignment_CustomGetParamFormat_TestData))]
    public void RichTextBox_SelectionAlignment_CustomGetParaFormat_ReturnsExpected(uint mask, uint alignment, HorizontalAlignment expected)
    {
        using CustomGetParaFormatRichTextBox control = new()
        {
            GetParaFormatResult = new PARAFORMAT
            {
                dwMask = (PFM)mask,
                wAlignment = (PFA)alignment
            }
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.MakeCustom = true;
        Assert.Equal(expected, control.SelectionAlignment);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionAlignment_GetCantCreateHandle_ReturnsExpected()
    {
        using CantCreateHandleRichTextBox control = new();
        Assert.Equal(HorizontalAlignment.Left, control.SelectionAlignment);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionAlignment_GetDisposed_ThrowsObjectDisposedException()
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectionAlignment);
    }

    [WinFormsTheory]
    [EnumData<HorizontalAlignment>]
    public void RichTextBox_SelectionAlignment_Set_GetReturnsExpected(HorizontalAlignment value)
    {
        using RichTextBox control = new()
        {
            SelectionAlignment = value
        };
        Assert.Equal(value, control.SelectionAlignment);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.SelectionAlignment = value;
        Assert.Equal(value, control.SelectionAlignment);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [EnumData<HorizontalAlignment>]
    public void RichTextBox_SelectionAlignment_SetWithHandle_GetReturnsExpected(HorizontalAlignment value)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SelectionAlignment = value;
        Assert.Equal(value, control.SelectionAlignment);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SelectionAlignment = value;
        Assert.Equal(value, control.SelectionAlignment);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(HorizontalAlignment.Left, 1)]
    [InlineData(HorizontalAlignment.Center, 3)]
    [InlineData(HorizontalAlignment.Right, 2)]
    public unsafe void RichTextBox_SelectionAlignment_GetCharFormat_Success(HorizontalAlignment value, int expected)
    {
        using RichTextBox control = new()
        {
            BulletIndent = 11,
            SelectionHangingIndent = 10
        };

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.SelectionAlignment = value;
        PARAFORMAT format = new()
        {
            cbSize = (uint)sizeof(PARAFORMAT)
        };
        Assert.NotEqual(0, (int)PInvokeCore.SendMessage(control, PInvokeCore.EM_GETPARAFORMAT, (WPARAM)PInvoke.SCF_SELECTION, ref format));
        Assert.Equal(expected, (int)format.wAlignment);
    }

    [WinFormsTheory]
    [InvalidEnumData<HorizontalAlignment>]
    public void RichTextBox_SelectionAlignment_SetInvalidValue_ThrowsInvalidEnumArgumentException(HorizontalAlignment value)
    {
        using RichTextBox control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.SelectionAlignment = value);
    }

    [WinFormsTheory]
    [EnumData<HorizontalAlignment>]
    public void RichTextBox_SelectionAlignment_SetCantCreateHandle_GetReturnsExpected(HorizontalAlignment value)
    {
        using CantCreateHandleRichTextBox control = new();
        control.SelectionAlignment = value;
        Assert.Equal(HorizontalAlignment.Left, control.SelectionAlignment);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SelectionAlignment = value;
        Assert.Equal(HorizontalAlignment.Left, control.SelectionAlignment);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [EnumData<HorizontalAlignment>]
    public void RichTextBox_SelectionAlignment_SetDisposed_ThrowsObjectDisposedException(HorizontalAlignment value)
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectionAlignment = value);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionBackColor_GetWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(SystemColors.Window, control.SelectionBackColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> SelectionBackColor_CustomGetCharFormat_TestData()
    {
        yield return new object[] { 0, 0, 0x78563412, Color.Empty };
        yield return new object[] { 0, CFE_EFFECTS.CFE_AUTOBACKCOLOR, 0x78563412, Color.Red };
        yield return new object[] { 0, CFE_EFFECTS.CFE_AUTOBACKCOLOR | CFE_EFFECTS.CFE_ALLCAPS, 0x78563412, Color.Red };
        yield return new object[] { CFM_MASK.CFM_BACKCOLOR, 0, 0x78563412, Color.FromArgb(0xFF, 0x12, 0x34, 0x56) };
        yield return new object[] { CFM_MASK.CFM_BACKCOLOR, 0, 0x785634, Color.FromArgb(0xFF, 0x34, 0x56, 0x78) };
        yield return new object[] { CFM_MASK.CFM_BACKCOLOR | CFM_MASK.CFM_ANIMATION, 0, 0x78563412, Color.FromArgb(0xFF, 0x12, 0x34, 0x56) };
        yield return new object[] { CFM_MASK.CFM_BACKCOLOR, CFE_EFFECTS.CFE_AUTOBACKCOLOR, 0x78563412, Color.Red };
        yield return new object[] { CFM_MASK.CFM_ALLCAPS, 0, 0x78563412, Color.Empty };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionBackColor_CustomGetCharFormat_TestData))]
    public void RichTextBox_SelectionBackColor_CustomGetCharFormat_ReturnsExpected(uint mask, uint effects, int backColor, Color expected)
    {
        using CustomGetCharFormatRichTextBox control = new()
        {
            ExpectedWParam = (IntPtr)PInvoke.SCF_SELECTION,
            GetCharFormatResult = new CHARFORMAT2W
            {
                dwMask = (CFM_MASK)mask,
                dwEffects = (CFE_EFFECTS)effects,
                crBackColor = backColor
            },
            BackColor = Color.Red
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.MakeCustom = true;
        Assert.Equal(expected, control.SelectionBackColor);
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionBackColor_CustomGetCharFormat_TestData))]
    public void RichTextBox_SelectionBackColor_CustomGetCharFormatWithBackColor_ReturnsExpected(uint mask, uint effects, int backColor, Color expected)
    {
        using CustomGetCharFormatRichTextBox control = new()
        {
            ExpectedWParam = (IntPtr)PInvoke.SCF_SELECTION,
            GetCharFormatResult = new CHARFORMAT2W
            {
                dwMask = (CFM_MASK)mask,
                dwEffects = (CFE_EFFECTS)effects,
                crBackColor = backColor
            },
            BackColor = Color.Red
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.SelectionBackColor = Color.Yellow;
        control.MakeCustom = true;
        Assert.Equal(expected, control.SelectionBackColor);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionBackColor_GetCantCreateHandle_ReturnsExpected()
    {
        using CantCreateHandleRichTextBox control = new();
        Assert.Equal(Color.Empty, control.SelectionBackColor);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionBackColor_GetDisposed_ReturnsExpected()
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Equal(Color.Empty, control.SelectionBackColor);
    }

    public static IEnumerable<object[]> SelectionBackColor_Set_TestData()
    {
        yield return new object[] { Color.Empty };
        yield return new object[] { Color.Red };
        yield return new object[] { Color.FromArgb(0x12, 0x34, 0x56, 0x78) };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionBackColor_Set_TestData))]
    public void RichTextBox_SelectionBackColor_Set_GetReturnsExpected(Color value)
    {
        using RichTextBox control = new()
        {
            SelectionBackColor = value
        };
        Assert.Equal(value, control.SelectionBackColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SelectionBackColor = value;
        Assert.Equal(value, control.SelectionBackColor);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> SelectionBackColor_SetWithHandle_TestData()
    {
        yield return new object[] { Color.Empty, SystemColors.Window };
        yield return new object[] { Color.Red, Color.Red };
        yield return new object[] { Color.FromArgb(0x12, 0x34, 0x56, 0x78), Color.FromArgb(0xFF, 0x34, 0x56, 0x78) };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionBackColor_SetWithHandle_TestData))]
    public void RichTextBox_SelectionBackColor_SetWithHandle_GetReturnsExpected(Color value, Color expected)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SelectionBackColor = value;
        Assert.Equal(expected, control.SelectionBackColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SelectionBackColor = value;
        Assert.Equal(expected, control.SelectionBackColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public unsafe void RichTextBox_SelectionBackColor_GetCharFormat_Success()
    {
        using RichTextBox control = new();

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.SelectionBackColor = Color.FromArgb(0x12, 0x34, 0x56, 0x78);
        var format = new CHARFORMAT2W
        {
            cbSize = (uint)sizeof(CHARFORMAT2W)
        };
        Assert.NotEqual(0, (int)PInvokeCore.SendMessage(control, PInvokeCore.EM_GETCHARFORMAT, (WPARAM)PInvoke.SCF_SELECTION, ref format));
        Assert.Equal(0x785634, format.crBackColor);
    }

    public static IEnumerable<object[]> SelectionBackColor_SetCantCreate_TestData()
    {
        yield return new object[] { Color.Empty };
        yield return new object[] { Color.Red };
        yield return new object[] { Color.FromArgb(0x12, 0x34, 0x56, 0x78) };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionBackColor_SetCantCreate_TestData))]
    public void RichTextBox_SelectionBackColor_SetCantCreateHandle_GetReturnsExpected(Color value)
    {
        using CantCreateHandleRichTextBox control = new();
        control.SelectionBackColor = value;
        Assert.Equal(value, control.SelectionBackColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SelectionBackColor = value;
        Assert.Equal(value, control.SelectionBackColor);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionBackColor_SetCantCreate_TestData))]
    public void RichTextBox_SelectionBackColor_SetDisposed_ThrowsObjectDisposedException(Color value)
    {
        using RichTextBox control = new();
        control.Dispose();

        control.SelectionBackColor = value;
        Assert.Equal(value, control.SelectionBackColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SelectionBackColor = value;
        Assert.Equal(value, control.SelectionBackColor);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionBullet_GetWithoutHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.False(control.SelectionBullet);
        Assert.True(control.IsHandleCreated);

        // Call again.
        Assert.False(control.SelectionBullet);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionBullet_GetWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.False(control.SelectionBullet);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        Assert.False(control.SelectionBullet);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> SelectionBullet_CustomGetParamFormat_TestData()
    {
        yield return new object[] { 0, 0, false };
        yield return new object[] { 0, PARAFORMAT_NUMBERING.PFN_BULLET, false };
        yield return new object[] { PFM.NUMBERING, PARAFORMAT_NUMBERING.PFN_BULLET, true };
        yield return new object[] { PFM.NUMBERING, 0, false };
        yield return new object[] { PFM.NUMBERING, PARAFORMAT_NUMBERING.PFN_ARABIC, false };
        yield return new object[] { PFM.NUMBERING, PARAFORMAT_NUMBERING.PFN_LCLETTER, false };
        yield return new object[] { PFM.NUMBERING, PARAFORMAT_NUMBERING.PFN_LCROMAN, false };
        yield return new object[] { PFM.NUMBERING, PARAFORMAT_NUMBERING.PFN_UCLETTER, false };
        yield return new object[] { PFM.NUMBERING, PARAFORMAT_NUMBERING.PFN_UCROMAN, false };
        yield return new object[] { PFM.NUMBERING, PARAFORMAT_NUMBERING.PFN_UCROMAN + 1, false };
        yield return new object[] { PFM.NUMBERING | PFM.ALIGNMENT, PARAFORMAT_NUMBERING.PFN_BULLET, true };
        yield return new object[] { PFM.ALIGNMENT, PARAFORMAT_NUMBERING.PFN_BULLET, false };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionBullet_CustomGetParamFormat_TestData))]
    public void RichTextBox_SelectionBullet_CustomGetParaFormat_ReturnsExpected(uint mask, uint numbering, bool expected)
    {
        using CustomGetParaFormatRichTextBox control = new()
        {
            GetParaFormatResult = new PARAFORMAT
            {
                dwMask = (PFM)mask,
                wNumbering = (PARAFORMAT_NUMBERING)numbering
            }
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.MakeCustom = true;
        Assert.Equal(expected, control.SelectionBullet);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionBullet_GetCantCreateHandle_ReturnsExpected()
    {
        using CantCreateHandleRichTextBox control = new();
        Assert.False(control.SelectionBullet);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionBullet_GetDisposed_ThrowsObjectDisposedException()
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectionBullet);
    }

    [WinFormsTheory]
    [BoolData]
    public void RichTextBox_SelectionBullet_Set_GetReturnsExpected(bool value)
    {
        using RichTextBox control = new()
        {
            SelectionBullet = value
        };
        Assert.Equal(value, control.SelectionBullet);
        Assert.Equal(0, control.BulletIndent);
        Assert.Equal(0, control.SelectionHangingIndent);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.SelectionBullet = value;
        Assert.Equal(value, control.SelectionBullet);
        Assert.Equal(0, control.BulletIndent);
        Assert.Equal(0, control.SelectionHangingIndent);
        Assert.True(control.IsHandleCreated);

        // Set different.
        control.SelectionBullet = !value;
        Assert.Equal(!value, control.SelectionBullet);
        Assert.Equal(0, control.BulletIndent);
        Assert.Equal(0, control.SelectionHangingIndent);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void RichTextBox_SelectionBullet_SetWithHandle_GetReturnsExpected(bool value)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SelectionBullet = value;
        Assert.Equal(value, control.SelectionBullet);
        Assert.Equal(0, control.BulletIndent);
        Assert.Equal(0, control.SelectionHangingIndent);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SelectionBullet = value;
        Assert.Equal(value, control.SelectionBullet);
        Assert.Equal(0, control.BulletIndent);
        Assert.Equal(0, control.SelectionHangingIndent);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.SelectionBullet = !value;
        Assert.Equal(!value, control.SelectionBullet);
        Assert.Equal(0, control.BulletIndent);
        Assert.Equal(0, control.SelectionHangingIndent);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void RichTextBox_SelectionBullet_SetWithSelectionHangingIndentWithHandle_GetReturnsExpected(bool value)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        control.SelectionHangingIndent = 10;

        control.SelectionBullet = value;
        Assert.Equal(value, control.SelectionBullet);
        Assert.Equal(0, control.BulletIndent);
        Assert.Equal(0, control.SelectionHangingIndent);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SelectionBullet = value;
        Assert.Equal(value, control.SelectionBullet);
        Assert.Equal(0, control.BulletIndent);
        Assert.Equal(0, control.SelectionHangingIndent);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.SelectionBullet = !value;
        Assert.Equal(!value, control.SelectionBullet);
        Assert.Equal(0, control.BulletIndent);
        Assert.Equal(0, control.SelectionHangingIndent);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(true, 1, 165)]
    [InlineData(false, 0, 0)]
    public unsafe void RichTextBox_SelectionBullet_GetParaFormat_Success(bool value, int expected, int expectedOffset)
    {
        using RichTextBox control = new()
        {
            BulletIndent = 11,
            SelectionHangingIndent = 10
        };

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.SelectionBullet = value;
        PARAFORMAT format = new()
        {
            cbSize = (uint)sizeof(PARAFORMAT)
        };
        Assert.NotEqual(0, (int)PInvokeCore.SendMessage(control, PInvokeCore.EM_GETPARAFORMAT, (WPARAM)PInvoke.SCF_SELECTION, ref format));
        Assert.Equal(expectedOffset, format.dxOffset);
        Assert.Equal(expected, (int)format.wNumbering);
    }

    [WinFormsTheory]
    [BoolData]
    public void RichTextBox_SelectionBullet_SetCantCreateHandle_GetReturnExpected(bool value)
    {
        using CantCreateHandleRichTextBox control = new();
        control.SelectionBullet = value;
        Assert.False(control.SelectionBullet);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SelectionBullet = value;
        Assert.False(control.SelectionBullet);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.SelectionBullet = !value;
        Assert.False(control.SelectionBullet);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void RichTextBox_SelectionBullet_SetDisposed_ThrowsObjectDisposedException(bool value)
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectionBullet = value);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionCharOffset_GetWithoutHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.Equal(0, control.SelectionCharOffset);
        Assert.True(control.IsHandleCreated);

        // Call again.
        Assert.Equal(0, control.SelectionCharOffset);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionCharOffset_GetWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(0, control.SelectionCharOffset);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        Assert.Equal(0, control.SelectionCharOffset);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> SelectionCharOffset_CustomGetCharFormat_TestData()
    {
        yield return new object[] { 0, 0, 0 };
        yield return new object[] { 0, 900, 60 };
        yield return new object[] { 0, 30000, 2000 };
        yield return new object[] { 0, 60000, 4000 };
        yield return new object[] { 0, -900, -60 };

        yield return new object[] { CFM_MASK.CFM_OFFSET, 0, 0 };
        yield return new object[] { CFM_MASK.CFM_OFFSET, 900, 60 };
        yield return new object[] { CFM_MASK.CFM_OFFSET, 30000, 2000 };
        yield return new object[] { CFM_MASK.CFM_OFFSET, 60000, 4000 };
        yield return new object[] { CFM_MASK.CFM_OFFSET, -900, -60 };
        yield return new object[] { CFM_MASK.CFM_OFFSET | CFM_MASK.CFM_ALLCAPS, -900, -60 };

        yield return new object[] { CFM_MASK.CFM_ALLCAPS, 0, 0 };
        yield return new object[] { CFM_MASK.CFM_ALLCAPS, 900, 60 };
        yield return new object[] { CFM_MASK.CFM_ALLCAPS, 30000, 2000 };
        yield return new object[] { CFM_MASK.CFM_ALLCAPS, 60000, 4000 };
        yield return new object[] { CFM_MASK.CFM_ALLCAPS, -900, -60 };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionCharOffset_CustomGetCharFormat_TestData))]
    public void RichTextBox_SelectionCharOffset_CustomGetCharFormat_Success(uint mask, int yOffset, int expected)
    {
        using CustomGetCharFormatRichTextBox control = new()
        {
            ExpectedWParam = (IntPtr)PInvoke.SCF_SELECTION,
            GetCharFormatResult = new CHARFORMAT2W
            {
                dwMask = (CFM_MASK)mask,
                yOffset = yOffset
            }
        };

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.MakeCustom = true;
        Assert.Equal(expected, control.SelectionCharOffset);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionCharOffset_GetCantCreateHandle_ReturnsExpected()
    {
        using CantCreateHandleRichTextBox control = new();
        Assert.Equal(0, control.SelectionCharOffset);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionCharOffset_GetDisposed_ThrowsObjectDisposedException()
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectionCharOffset);
    }

    [WinFormsTheory]
    [InlineData(2000)]
    [InlineData(10)]
    [InlineData(0)]
    [InlineData(-10)]
    [InlineData(-2000)]
    public void RichTextBox_SelectionCharOffset_Set_GetReturnsExpected(int value)
    {
        using RichTextBox control = new()
        {
            SelectionCharOffset = value
        };
        Assert.Equal(value, control.SelectionCharOffset);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.SelectionCharOffset = value;
        Assert.Equal(value, control.SelectionCharOffset);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(2000)]
    [InlineData(10)]
    [InlineData(0)]
    [InlineData(-10)]
    [InlineData(-2000)]
    public void RichTextBox_SelectionCharOffset_SetWithHandle_GetReturnsExpected(int value)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SelectionCharOffset = value;
        Assert.Equal(value, control.SelectionCharOffset);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SelectionCharOffset = value;
        Assert.Equal(value, control.SelectionCharOffset);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public unsafe void RichTextBox_SelectionCharOffset_GetCharFormat_Success()
    {
        using RichTextBox control = new();

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.SelectionCharOffset = 60;
        var format = new CHARFORMAT2W
        {
            cbSize = (uint)sizeof(CHARFORMAT2W)
        };
        Assert.NotEqual(0, (int)PInvokeCore.SendMessage(control, PInvokeCore.EM_GETCHARFORMAT, (WPARAM)PInvoke.SCF_SELECTION, ref format));
        Assert.Equal(900, format.yOffset);
    }

    [WinFormsTheory]
    [InlineData(2001)]
    [InlineData(-20001)]
    public void RichTextBox_SelectionCharOffset_SetInvalidValue_ThrowsArgumentOutOfRangeException(int value)
    {
        using RichTextBox control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.SelectionCharOffset = value);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void RichTextBox_SelectionCharOffset_SetCantCreateHandle_GetReturnsExpected(int value)
    {
        using CantCreateHandleRichTextBox control = new();
        control.SelectionCharOffset = value;
        Assert.Equal(0, control.SelectionCharOffset);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SelectionCharOffset = value;
        Assert.Equal(0, control.SelectionCharOffset);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void RichTextBox_SelectionCharOffset_SetDisposed_ThrowsObjectDisposedException(int value)
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectionCharOffset = value);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionColor_GetWithoutHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.Equal(Color.Black, control.SelectionColor);
        Assert.True(control.IsHandleCreated);

        // Call again.
        Assert.Equal(Color.Black, control.SelectionColor);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionColor_GetWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(Color.Black, control.SelectionColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        Assert.Equal(Color.Black, control.SelectionColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> SelectionColor_CustomGetCharFormat_TestData()
    {
        yield return new object[] { CFM_MASK.CFM_COLOR, 0x785634, Color.FromArgb(0xFF, 0x34, 0x56, 0x78) };
        yield return new object[] { CFM_MASK.CFM_COLOR, 0x78563412, Color.FromArgb(0xFF, 0x12, 0x34, 0x56) };
        yield return new object[] { CFM_MASK.CFM_COLOR, 0, Color.Black };

        yield return new object[] { 0, 0x785634, Color.Empty };
        yield return new object[] { 0, 0x78563412, Color.Empty };
        yield return new object[] { 0, 0, Color.Empty };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionColor_CustomGetCharFormat_TestData))]
    public void RichTextBox_SelectionColor_CustomGetCharFormat_Success(uint mask, int textColor, Color expected)
    {
        using CustomGetCharFormatRichTextBox control = new()
        {
            ExpectedWParam = (IntPtr)PInvoke.SCF_SELECTION,
            GetCharFormatResult = new CHARFORMAT2W
            {
                dwMask = (CFM_MASK)mask,
                crTextColor = textColor
            }
        };

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.MakeCustom = true;
        Assert.Equal(expected, control.SelectionColor);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionColor_GetCantCreateHandle_ReturnsExpected()
    {
        using CantCreateHandleRichTextBox control = new();
        Assert.Equal(Color.Empty, control.SelectionColor);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionColor_GetDisposed_ThrowsObjectDisposedException()
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectionColor);
    }

    public static IEnumerable<object[]> SelectionColor_Set_TestData()
    {
        yield return new object[] { Color.Empty, Color.Black };
        yield return new object[] { Color.Red, Color.Red };
        yield return new object[] { Color.FromArgb(0x12, 0x34, 0x56, 0x78), Color.FromArgb(0xFF, 0x34, 0x56, 0x78) };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionColor_Set_TestData))]
    public void RichTextBox_SelectionColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using RichTextBox control = new()
        {
            SelectionColor = value
        };
        Assert.Equal(expected, control.SelectionColor);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.SelectionColor = value;
        Assert.Equal(expected, control.SelectionColor);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionColor_Set_TestData))]
    public void RichTextBox_SelectionColor_SetWithHandle_GetReturnsExpected(Color value, Color expected)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SelectionColor = value;
        Assert.Equal(expected, control.SelectionColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SelectionColor = value;
        Assert.Equal(expected, control.SelectionColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public unsafe void RichTextBox_SelectionColor_GetCharFormat_Success()
    {
        using RichTextBox control = new();

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.SelectionColor = Color.FromArgb(0x12, 0x34, 0x56, 0x78);
        var format = new CHARFORMAT2W
        {
            cbSize = (uint)sizeof(CHARFORMAT2W)
        };
        Assert.NotEqual(0, (int)PInvokeCore.SendMessage(control, PInvokeCore.EM_GETCHARFORMAT, (WPARAM)PInvoke.SCF_SELECTION, ref format));
        Assert.Equal(0x785634, format.crTextColor);
    }

    public static IEnumerable<object[]> SelectionColor_SetCantCreate_TestData()
    {
        yield return new object[] { Color.Empty };
        yield return new object[] { Color.Red };
        yield return new object[] { Color.FromArgb(0x12, 0x34, 0x56, 0x78) };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionColor_SetCantCreate_TestData))]
    public void RichTextBox_SelectionColor_SetCantCreateHandle_GetReturnsExpected(Color value)
    {
        using CantCreateHandleRichTextBox control = new();
        control.SelectionColor = value;
        Assert.Equal(Color.Empty, control.SelectionColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SelectionColor = value;
        Assert.Equal(Color.Empty, control.SelectionColor);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionColor_SetCantCreate_TestData))]
    public void RichTextBox_SelectionColor_SetDisposed_ThrowsObjectDisposedException(Color value)
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectionColor = value);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionFont_GetWithoutHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Font result1 = control.SelectionFont;
        Assert.NotNull(result1);
        Assert.True(control.IsHandleCreated);

        // Call again.
        Font result2 = control.SelectionFont;
        Assert.NotNull(result2);
        Assert.NotSame(result1, result2);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionFont_GetWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Font result1 = control.SelectionFont;
        Assert.NotNull(result1);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        Font result2 = control.SelectionFont;
        Assert.NotNull(result2);
        Assert.NotSame(result1, result2);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> SelectionFont_CustomGetCharFormat_TestData()
    {
        char[] arial = ['A', 'r', 'i', 'a', 'l'];

        yield return new object[] { 0, 0, 0, Array.Empty<char>(), string.Empty, 13, FontStyle.Regular };
        yield return new object[] { 0, 0, 0, arial, "Arial", 13, FontStyle.Regular };
        yield return new object[] { 0, 0, 0, new char[] { 'A', 'r', 'i', 'a', 'l', '\0', 'm' }, "Arial", 13, FontStyle.Regular };
        yield return new object[] { 0, 0, 0, new char[] { 'N', 'r', 'i', 'a', 'l', '\0', 'm' }, "Nrial", 13, FontStyle.Regular };

        yield return new object[] { 0, 0, 200, arial, "Arial", 13, FontStyle.Regular };
        yield return new object[] { CFM_MASK.CFM_SIZE, 0, 200, arial, "Arial", 10, FontStyle.Regular };
        yield return new object[] { CFM_MASK.CFM_SIZE, 0, 250, arial, "Arial", 12.5f, FontStyle.Regular };
        yield return new object[] { CFM_MASK.CFM_SIZE | CFM_MASK.CFM_ALLCAPS, 0, 250, arial, "Arial", 12.5f, FontStyle.Regular };

        yield return new object[] { CFM_MASK.CFM_BOLD, CFE_EFFECTS.CFE_BOLD, 0, arial, "Arial", 13, FontStyle.Bold };
        yield return new object[] { CFM_MASK.CFM_BOLD, CFE_EFFECTS.CFE_BOLD | CFE_EFFECTS.CFE_ALLCAPS, 0, arial, "Arial", 13, FontStyle.Bold };

        yield return new object[] { CFM_MASK.CFM_ITALIC, CFE_EFFECTS.CFE_ITALIC, 0, arial, "Arial", 13, FontStyle.Italic };
        yield return new object[] { CFM_MASK.CFM_ITALIC, CFE_EFFECTS.CFE_ITALIC | CFE_EFFECTS.CFE_ALLCAPS, 0, arial, "Arial", 13, FontStyle.Italic };

        yield return new object[] { CFM_MASK.CFM_STRIKEOUT, CFE_EFFECTS.CFE_STRIKEOUT, 0, arial, "Arial", 13, FontStyle.Strikeout };
        yield return new object[] { CFM_MASK.CFM_STRIKEOUT, CFE_EFFECTS.CFE_STRIKEOUT | CFE_EFFECTS.CFE_STRIKEOUT, 0, arial, "Arial", 13, FontStyle.Strikeout };

        yield return new object[] { CFM_MASK.CFM_UNDERLINE, CFE_EFFECTS.CFE_UNDERLINE, 0, arial, "Arial", 13, FontStyle.Underline };
        yield return new object[] { CFM_MASK.CFM_UNDERLINE, CFE_EFFECTS.CFE_UNDERLINE | CFE_EFFECTS.CFE_UNDERLINE, 0, arial, "Arial", 13, FontStyle.Underline };

        yield return new object[] { CFM_MASK.CFM_ALLCAPS, CFE_EFFECTS.CFE_BOLD, 0, arial, "Arial", 13, FontStyle.Regular };
        yield return new object[] { CFM_MASK.CFM_ALLCAPS, CFE_EFFECTS.CFE_BOLD | CFE_EFFECTS.CFE_ALLCAPS, 0, arial, "Arial", 13, FontStyle.Regular };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionFont_CustomGetCharFormat_TestData))]
    public unsafe void RichTextBox_SelectionFont_CustomGetCharFormat_ReturnsExpected(uint mask, uint effects, int yHeight, char[] faceName, string expectedFontName, float expectedSize, FontStyle expectedStyle)
    {
        var result = new CHARFORMAT2W
        {
            dwMask = CFM_MASK.CFM_FACE | (CFM_MASK)mask,
            dwEffects = (CFE_EFFECTS)effects,
            yHeight = yHeight,
            bCharSet = 2
        };
        for (int i = 0; i < faceName.Length; i++)
        {
            result._szFaceName[i] = faceName[i];
        }

        using CustomGetCharFormatRichTextBox control = new()
        {
            ExpectedWParam = (IntPtr)PInvoke.SCF_SELECTION,
            GetCharFormatResult = result
        };

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.MakeCustom = true;
        Font font = control.SelectionFont;
        Assert.Equal(expectedFontName, font.OriginalFontName);
        Assert.Equal(expectedSize, font.Size);
        Assert.Equal(expectedStyle, font.Style);
        Assert.Equal(GraphicsUnit.Point, font.Unit);
        Assert.Equal(2, font.GdiCharSet);
    }

    public static IEnumerable<object[]> SelectionFont_InvalidCustomGetCharFormat_TestData()
    {
        char[] arial = ['A', 'r', 'i', 'a', 'l'];

        yield return new object[] { 0, 0, arial };
        yield return new object[] { CFM_MASK.CFM_ANIMATION, 0, arial };
        yield return new object[] { CFM_MASK.CFM_SIZE, -200, arial };
        yield return new object[] { CFM_MASK.CFM_SIZE, 0, arial };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionFont_InvalidCustomGetCharFormat_TestData))]
    public unsafe void RichTextBox_SelectionFont_InvalidCustomGetCharFormat_ReturnsExpected(uint mask, int yHeight, char[] faceName)
    {
        var result = new CHARFORMAT2W
        {
            dwMask = (CFM_MASK)mask,
            yHeight = yHeight
        };
        for (int i = 0; i < faceName.Length; i++)
        {
            result._szFaceName[i] = faceName[i];
        }

        using CustomGetCharFormatRichTextBox control = new()
        {
            ExpectedWParam = (IntPtr)PInvoke.SCF_SELECTION,
            GetCharFormatResult = result
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.MakeCustom = true;
        Assert.Null(control.SelectionFont);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionFont_GetCantCreateHandle_GetReturnsExpected()
    {
        using CantCreateHandleRichTextBox control = new();
        Assert.Null(control.SelectionFont);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionFont_GetDisposed_ThrowsObjectDisposedException()
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectionFont);
    }

    public static IEnumerable<object[]> SelectionFont_Set_TestData()
    {
        yield return new object[] { "Arial", 8.25f, false, FontStyle.Regular, 0, 0, 1 };
        yield return new object[] { "Arial", 8.25f, true, FontStyle.Bold | FontStyle.Italic | FontStyle.Regular | FontStyle.Strikeout | FontStyle.Underline, GraphicsUnit.Point, 10, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionFont_Set_TestData))]
    public void RichTextBox_SelectionFont_Set_GetReturnsExpected(string fontName, float fontSize, bool hasStyle, FontStyle fontStyle, GraphicsUnit units, byte gdiCharSet, byte expectedGdiCharset)
    {
        using Font value = hasStyle ? new Font(fontName, fontSize, fontStyle, units, gdiCharSet) : new Font(fontName, fontSize);
        using RichTextBox control = new()
        {
            SelectionFont = value
        };
        Font result1 = control.SelectionFont;
        Assert.NotSame(result1, value);
        Assert.Equal(value?.Name, result1.Name);
        Assert.Equal(value?.Size, result1.Size);
        Assert.Equal(value?.Style, result1.Style);
        Assert.Equal(expectedGdiCharset, result1.GdiCharSet);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.SelectionFont = value;
        Font result2 = control.SelectionFont;
        Assert.Equal(value?.Name, result2.Name);
        Assert.Equal(value?.Size, result2.Size);
        Assert.Equal(value?.Style, result2.Style);
        Assert.Equal(expectedGdiCharset, result2.GdiCharSet);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionFont_Set_TestData))]
    public void RichTextBox_SelectionFont_SetWithHandle_GetReturnsExpected(string fontName, float fontSize, bool hasStyle, FontStyle fontStyle, GraphicsUnit units, byte gdiCharSet, byte expectedGdiCharset)
    {
        using Font value = hasStyle ? new Font(fontName, fontSize, fontStyle, units, gdiCharSet) : new Font(fontName, fontSize);
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SelectionFont = value;
        Font result1 = control.SelectionFont;
        Assert.NotSame(result1, value);
        Assert.Equal(value.Name, result1.Name);
        Assert.Equal(value.Size, result1.Size);
        Assert.Equal(value.Style, result1.Style);
        Assert.Equal(expectedGdiCharset, result1.GdiCharSet);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SelectionFont = value;
        Font result2 = control.SelectionFont;
        Assert.Equal(value.Name, result2.Name);
        Assert.Equal(value.Size, result2.Size);
        Assert.Equal(value.Style, result2.Style);
        Assert.Equal(expectedGdiCharset, result2.GdiCharSet);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> SelectionFont_SetCantCreate_TestData()
    {
        yield return new object[] { new Font("Arial", 8.25f) };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionFont_SetCantCreate_TestData))]
    public void RichTextBox_SelectionFont_SetCantCreateHandle_GetReturnsExpected(Font value)
    {
        using CantCreateHandleRichTextBox control = new();
        control.SelectionFont = value;
        Assert.Null(control.SelectionFont);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SelectionFont = value;
        Assert.Null(control.SelectionFont);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionFont_SetCantCreate_TestData))]
    public void RichTextBox_SelectionFont_SetDisposed_ThrowsObjectDisposedException(Font value)
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectionFont = value);
    }

    public static IEnumerable<object[]> SelectionFont_GetCharFormat_TestData()
    {
        yield return new object[] { new Font("Arial", 8.25f), 165, 0 };
        yield return new object[] { new Font("Arial", 8.25f, FontStyle.Bold), 165, CFE_EFFECTS.CFE_BOLD };
        yield return new object[] { new Font("Arial", 8.25f, FontStyle.Italic), 165, CFE_EFFECTS.CFE_ITALIC };
        yield return new object[] { new Font("Arial", 8.25f, FontStyle.Strikeout), 165, CFE_EFFECTS.CFE_STRIKEOUT };
        yield return new object[] { new Font("Arial", 8.25f, FontStyle.Underline), 165, CFE_EFFECTS.CFE_UNDERLINE };
        yield return new object[] { new Font("Arial", 8.25f, FontStyle.Bold | FontStyle.Italic | FontStyle.Regular | FontStyle.Strikeout | FontStyle.Underline, GraphicsUnit.Point, 10), 165, CFE_EFFECTS.CFE_BOLD | CFE_EFFECTS.CFE_ITALIC | CFE_EFFECTS.CFE_UNDERLINE | CFE_EFFECTS.CFE_STRIKEOUT };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionFont_GetCharFormat_TestData))]
    public unsafe void RichTextBox_SelectionFont_GetCharFormat_Success(Font value, int expectedYHeight, int expectedEffects)
    {
        using RichTextBox control = new();

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.SelectionFont = value;
        var format = new CHARFORMAT2W
        {
            cbSize = (uint)sizeof(CHARFORMAT2W),
            dwMask = (CFM_MASK)int.MaxValue
        };
        Assert.NotEqual(0, (int)PInvokeCore.SendMessage(control, PInvokeCore.EM_GETCHARFORMAT, (WPARAM)PInvoke.SCF_SELECTION, ref format));
        Assert.Equal("Arial", format.FaceName.ToString());
        Assert.Equal(expectedYHeight, format.yHeight);
        Assert.Equal(CFE_EFFECTS.CFE_AUTOBACKCOLOR | CFE_EFFECTS.CFE_AUTOCOLOR | (CFE_EFFECTS)expectedEffects, format.dwEffects);
        Assert.Equal(0, format.bPitchAndFamily);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionFont_SetNull_ThrowsNullReferenceException()
    {
        using RichTextBox control = new();
        Assert.Throws<NullReferenceException>(() => control.SelectionFont = null);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionHangingIndent_GetWithoutHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.Equal(0, control.SelectionHangingIndent);
        Assert.True(control.IsHandleCreated);

        // Call again.
        Assert.Equal(0, control.SelectionHangingIndent);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionHangingIndent_GetWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(0, control.SelectionHangingIndent);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        Assert.Equal(0, control.SelectionHangingIndent);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> SelectionHangingIndent_CustomGetParamFormat_TestData()
    {
        yield return new object[] { 0, 0, 0 };
        yield return new object[] { 0, 20, 0 };
        yield return new object[] { PFM.OFFSET, 900, 60 };
        yield return new object[] { PFM.OFFSET, 30000, 2000 };
        yield return new object[] { PFM.OFFSET, 60000, 4000 };
        yield return new object[] { PFM.OFFSET, -900, -60 };
        yield return new object[] { PFM.OFFSET | PFM.ALIGNMENT, -900, -60 };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionHangingIndent_CustomGetParamFormat_TestData))]
    public void RichTextBox_SelectionHangingIndent_CustomGetParaFormat_ReturnsExpected(uint mask, int dxOffset, int expected)
    {
        using CustomGetParaFormatRichTextBox control = new()
        {
            GetParaFormatResult = new PARAFORMAT
            {
                dwMask = (PFM)mask,
                dxOffset = dxOffset
            }
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.MakeCustom = true;
        Assert.Equal(expected, control.SelectionHangingIndent);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionHangingIndent_GetCantCreateHandle_GetReturnsExpected()
    {
        using CantCreateHandleRichTextBox control = new();
        Assert.Equal(0, control.SelectionHangingIndent);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionHangingIndent_GetDisposed_ThrowsObjectDisposedException()
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectionHangingIndent);
    }

    [WinFormsTheory]
    [InlineData(int.MinValue, 0)]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2000, 2000)]
    [InlineData(2001, 2001)]
    [InlineData(int.MaxValue, 0)]
    public void RichTextBox_SelectionHangingIndent_Set_GetReturnsExpected(int value, int expected)
    {
        using RichTextBox control = new()
        {
            SelectionHangingIndent = value
        };
        Assert.Equal(expected, control.SelectionHangingIndent);
        Assert.Equal(0, control.BulletIndent);
        Assert.False(control.SelectionBullet);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.SelectionHangingIndent = value;
        Assert.Equal(expected, control.SelectionHangingIndent);
        Assert.Equal(0, control.BulletIndent);
        Assert.False(control.SelectionBullet);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(int.MinValue, 0)]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2000, 2000)]
    [InlineData(2001, 2001)]
    [InlineData(int.MaxValue, 0)]
    public void RichTextBox_SelectionHangingIndent_SetWithHandle_GetReturnsExpected(int value, int expected)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SelectionHangingIndent = value;
        Assert.Equal(expected, control.SelectionHangingIndent);
        Assert.Equal(0, control.BulletIndent);
        Assert.False(control.SelectionBullet);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SelectionHangingIndent = value;
        Assert.Equal(expected, control.SelectionHangingIndent);
        Assert.Equal(0, control.BulletIndent);
        Assert.False(control.SelectionBullet);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(int.MinValue, 0)]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2000, 2000)]
    [InlineData(2001, 2001)]
    [InlineData(int.MaxValue, 0)]
    public void RichTextBox_SelectionHangingIndent_SetSelectionBulletWithHandle_GetReturnsExpected(int value, int expected)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        control.SelectionBullet = true;

        control.SelectionHangingIndent = value;
        Assert.Equal(expected, control.SelectionHangingIndent);
        Assert.Equal(0, control.BulletIndent);
        Assert.True(control.SelectionBullet);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SelectionHangingIndent = value;
        Assert.Equal(expected, control.SelectionHangingIndent);
        Assert.Equal(0, control.BulletIndent);
        Assert.True(control.SelectionBullet);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(int.MinValue, 0)]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 15)]
    [InlineData(2000, 30000)]
    [InlineData(2001, 30015)]
    [InlineData(int.MaxValue, 0)]
    public unsafe void RichTextBox_SelectionHangingIndent_GetCharFormat_Success(int value, int expected)
    {
        using RichTextBox control = new();

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.SelectionHangingIndent = value;
        PARAFORMAT format = new()
        {
            cbSize = (uint)sizeof(PARAFORMAT)
        };
        Assert.NotEqual(0, (int)PInvokeCore.SendMessage(control, PInvokeCore.EM_GETPARAFORMAT, (WPARAM)PInvoke.SCF_SELECTION, ref format));
        Assert.Equal(expected, format.dxOffset);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void RichTextBox_SelectionHangingIndent_SetCantCreateHandle_GetReturnsExpected(int value)
    {
        using CantCreateHandleRichTextBox control = new();
        control.SelectionHangingIndent = value;
        Assert.Equal(0, control.SelectionHangingIndent);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SelectionHangingIndent = value;
        Assert.Equal(0, control.SelectionHangingIndent);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void RichTextBox_SelectionHangingIndent_SetDisposed_ThrowsObjectDisposedException(int value)
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectionHangingIndent = value);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionIndent_GetWithoutHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.Equal(0, control.SelectionIndent);
        Assert.True(control.IsHandleCreated);

        // Call again.
        Assert.Equal(0, control.SelectionIndent);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionIndent_GetWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(0, control.SelectionIndent);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        Assert.Equal(0, control.SelectionIndent);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> SelectionIndent_CustomGetParamFormat_TestData()
    {
        yield return new object[] { 0, 0, 0 };
        yield return new object[] { 0, 20, 0 };
        yield return new object[] { PFM.STARTINDENT, 900, 60 };
        yield return new object[] { PFM.STARTINDENT, 30000, 2000 };
        yield return new object[] { PFM.STARTINDENT, 60000, 4000 };
        yield return new object[] { PFM.STARTINDENT, -900, -60 };
        yield return new object[] { PFM.STARTINDENT | PFM.ALIGNMENT, -900, -60 };
        yield return new object[] { PFM.ALIGNMENT, 20, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionIndent_CustomGetParamFormat_TestData))]
    public void RichTextBox_SelectionIndent_CustomGetParaFormat_ReturnsExpected(uint mask, int dxStartIndent, int expected)
    {
        using CustomGetParaFormatRichTextBox control = new()
        {
            GetParaFormatResult = new PARAFORMAT
            {
                dwMask = (PFM)mask,
                dxStartIndent = dxStartIndent
            }
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.MakeCustom = true;
        Assert.Equal(expected, control.SelectionIndent);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionIndent_GetCantCreateHandle_GetReturnsExpected()
    {
        using CantCreateHandleRichTextBox control = new();
        Assert.Equal(0, control.SelectionIndent);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionIndent_GetDisposed_ThrowsObjectDisposedException()
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectionIndent);
    }

    [WinFormsTheory]
    [InlineData(int.MinValue, 0)]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2000, 2000)]
    [InlineData(2001, 2001)]
    [InlineData(int.MaxValue, 0)]
    public void RichTextBox_SelectionIndent_Set_GetReturnsExpected(int value, int expected)
    {
        using RichTextBox control = new()
        {
            SelectionIndent = value
        };
        Assert.Equal(expected, control.SelectionIndent);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.SelectionIndent = value;
        Assert.Equal(expected, control.SelectionIndent);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(int.MinValue, 0)]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2000, 2000)]
    [InlineData(2001, 2001)]
    [InlineData(int.MaxValue, 0)]
    public void RichTextBox_SelectionIndent_SetWithHandle_GetReturnsExpected(int value, int expected)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SelectionIndent = value;
        Assert.Equal(expected, control.SelectionIndent);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SelectionIndent = value;
        Assert.Equal(expected, control.SelectionIndent);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(int.MinValue, 0)]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 15)]
    [InlineData(2000, 30000)]
    [InlineData(2001, 30015)]
    [InlineData(int.MaxValue, 0)]
    public unsafe void RichTextBox_SelectionIndent_GetCharFormat_Success(int value, int expected)
    {
        using RichTextBox control = new();

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.SelectionIndent = value;
        PARAFORMAT format = new()
        {
            cbSize = (uint)sizeof(PARAFORMAT)
        };
        Assert.NotEqual(0, (int)PInvokeCore.SendMessage(control, PInvokeCore.EM_GETPARAFORMAT, (WPARAM)PInvoke.SCF_SELECTION, ref format));
        Assert.Equal(expected, format.dxStartIndent);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void RichTextBox_SelectionIndent_SetCantCreateHandle_GetReturnsExpected(int value)
    {
        using CantCreateHandleRichTextBox control = new();
        control.SelectionIndent = value;
        Assert.Equal(0, control.SelectionIndent);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SelectionIndent = value;
        Assert.Equal(0, control.SelectionIndent);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void RichTextBox_SelectionIndent_SetDisposed_ThrowsObjectDisposedException(int value)
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectionIndent = value);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionLength_GetWithoutHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.Equal(0, control.SelectionLength);
        Assert.True(control.IsHandleCreated);

        // Get again.
        Assert.Equal(0, control.SelectionLength);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionLength_GetWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(0, control.SelectionLength);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Get again.
        Assert.Equal(0, control.SelectionLength);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionLength_GetCantCreateHandle_GetReturnsExpected()
    {
        using CantCreateHandleRichTextBox control = new();
        Assert.Equal(0, control.SelectionLength);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionLength_GetDisposed_ThrowsObjectDisposedException()
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectionLength);
    }

    [WinFormsTheory]
    [InlineData("", 0, 0, "")]
    [InlineData("", 1, 0, "")]
    [InlineData("text", 0, 0, "")]
    [InlineData("text", 1, 1, "t")]
    [InlineData("text", 2, 2, "te")]
    [InlineData("text", 3, 3, "tex")]
    [InlineData("text", 4, 4, "text")]
    [InlineData("text", 5, 4, "text")]
    public void RichTextBox_SelectionLength_Set_GetReturnsExpected(string text, int value, int expected, string expectedSelectedText)
    {
        using RichTextBox control = new()
        {
            Text = text,
            SelectionLength = value
        };
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(expected, control.SelectionLength);
        Assert.Equal(expectedSelectedText, control.SelectedText);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.SelectionLength = value;
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(expected, control.SelectionLength);
        Assert.Equal(expectedSelectedText, control.SelectedText);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData("", 0, 0, "")]
    [InlineData("", 1, 0, "")]
    [InlineData("text", 0, 0, "")]
    [InlineData("text", 1, 1, "e")]
    [InlineData("text", 2, 2, "ex")]
    [InlineData("text", 3, 3, "ext")]
    [InlineData("text", 5, 3, "ext")]
    public void RichTextBox_SelectionLength_SetWithSelectionStart_Success(string text, int value, int expected, string expectedSelectedText)
    {
        using RichTextBox control = new()
        {
            Text = text,
            SelectionStart = 1,
            SelectionLength = value
        };
        Assert.Equal(expected, control.SelectionLength);
        Assert.Equal(expectedSelectedText, control.SelectedText);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.SelectionLength = value;
        Assert.Equal(expected, control.SelectionLength);
        Assert.Equal(expectedSelectedText, control.SelectedText);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData("", 0, 0, "")]
    [InlineData("", 1, 0, "")]
    [InlineData("text", 0, 0, "")]
    [InlineData("text", 1, 1, "t")]
    [InlineData("text", 2, 2, "te")]
    [InlineData("text", 4, 4, "text")]
    [InlineData("text", 5, 4, "text")]
    public void RichTextBox_SelectionLength_SetWithHandle_Success(string text, int value, int expected, string expectedSelectedText)
    {
        using RichTextBox control = new()
        {
            Text = text
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SelectionLength = value;
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(expected, control.SelectionLength);
        Assert.Equal(expectedSelectedText, control.SelectedText);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SelectionLength = value;
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(expected, control.SelectionLength);
        Assert.Equal(expectedSelectedText, control.SelectedText);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(0, 1)]
    [InlineData(1, 2)]
    [InlineData(2, 3)]
    [InlineData(4, 4)]
    [InlineData(5, 4)]
    public unsafe void RichTextBox_SelectionLength_GetSel_Success(int value, int expected)
    {
        using RichTextBox control = new()
        {
            Text = "Text",
            SelectionStart = 1
        };

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.SelectionLength = value;
        int selectionStart = 0;
        int selectionEnd = 0;
        IntPtr result = PInvokeCore.SendMessage(control, PInvokeCore.EM_GETSEL, (WPARAM)(&selectionStart), (LPARAM)(&selectionEnd));
        Assert.Equal(1, PARAM.LOWORD(result));
        Assert.Equal(expected, PARAM.HIWORD(result));
        Assert.Equal(1, selectionStart);
        Assert.Equal(expected, selectionEnd);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionLength_SetNegative_ThrowArgumentOutOfRangeException()
    {
        using RichTextBox control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.SelectionLength = -1);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    public void RichTextBox_SelectionLength_SetCantCreateHandle_GetReturnsExpected(int value)
    {
        using CantCreateHandleRichTextBox control = new();
        control.SelectionLength = value;
        Assert.Equal(0, control.SelectionLength);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SelectionLength = value;
        Assert.Equal(0, control.SelectionLength);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    public void RichTextBox_SelectionLength_SetDisposed_ThrowsObjectDisposedException(int value)
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectionLength = value);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionProtected_GetWithoutHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.False(control.SelectionProtected);
        Assert.True(control.IsHandleCreated);

        // Call again.
        Assert.False(control.SelectionProtected);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionProtected_GetWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.False(control.SelectionProtected);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        Assert.False(control.SelectionProtected);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> SelectionProtected_CustomGetCharFormat_TestData()
    {
        yield return new object[] { 0, 0, false };
        yield return new object[] { 0, CFE_EFFECTS.CFE_PROTECTED, false };
        yield return new object[] { 0, CFE_EFFECTS.CFE_PROTECTED | CFE_EFFECTS.CFE_ALLCAPS, false };
        yield return new object[] { CFM_MASK.CFM_PROTECTED, 0, false };
        yield return new object[] { CFM_MASK.CFM_PROTECTED, CFE_EFFECTS.CFE_PROTECTED, true };
        yield return new object[] { CFM_MASK.CFM_PROTECTED, CFE_EFFECTS.CFE_PROTECTED | CFE_EFFECTS.CFE_ALLCAPS, true };
        yield return new object[] { CFM_MASK.CFM_PROTECTED, CFE_EFFECTS.CFE_ALLCAPS, false };
        yield return new object[] { CFM_MASK.CFM_ALLCAPS, CFE_EFFECTS.CFE_PROTECTED, false };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionProtected_CustomGetCharFormat_TestData))]
    public void RichTextBox_SelectionProtected_CustomGetCharFormat_ReturnsExpected(uint mask, uint effects, bool expected)
    {
        using CustomGetCharFormatRichTextBox control = new()
        {
            ExpectedWParam = (IntPtr)PInvoke.SCF_SELECTION,
            GetCharFormatResult = new CHARFORMAT2W
            {
                dwMask = (CFM_MASK)mask,
                dwEffects = (CFE_EFFECTS)effects
            }
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.MakeCustom = true;
        Assert.Equal(expected, control.SelectionProtected);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionProtected_GetCantCreateHandle_ReturnsExpected()
    {
        using CantCreateHandleRichTextBox control = new();
        Assert.False(control.SelectionProtected);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionProtected_GetDisposed_ThrowsObjectDisposedException()
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectionProtected);
    }

    [WinFormsTheory]
    [BoolData]
    public void RichTextBox_SelectionProtected_Set_GetReturnsExpected(bool value)
    {
        using RichTextBox control = new()
        {
            SelectionProtected = value
        };
        Assert.Equal(value, control.SelectionProtected);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.SelectionProtected = value;
        Assert.Equal(value, control.SelectionProtected);
        Assert.True(control.IsHandleCreated);

        // Set different.
        control.SelectionProtected = !value;
        Assert.Equal(!value, control.SelectionProtected);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void RichTextBox_SelectionProtected_SetWithHandle_GetReturnsExpected(bool value)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SelectionProtected = value;
        Assert.Equal(value, control.SelectionProtected);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SelectionProtected = value;
        Assert.Equal(value, control.SelectionProtected);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.SelectionProtected = !value;
        Assert.Equal(!value, control.SelectionProtected);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public unsafe void RichTextBox_SelectionProtected_GetCharFormat_Success(bool value)
    {
        using RichTextBox control = new();

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.SelectionProtected = value;
        var format = new CHARFORMAT2W
        {
            cbSize = (uint)sizeof(CHARFORMAT2W),
            dwMask = CFM_MASK.CFM_PROTECTED
        };
        Assert.NotEqual(0, (int)PInvokeCore.SendMessage(control, PInvokeCore.EM_GETCHARFORMAT, (WPARAM)PInvoke.SCF_SELECTION, ref format));
        Assert.Equal(value, (format.dwEffects & CFE_EFFECTS.CFE_PROTECTED) != 0);
    }

    [WinFormsTheory]
    [BoolData]
    public void RichTextBox_SelectionProtected_SetCantCreateHandle_GetReturnExpected(bool value)
    {
        using CantCreateHandleRichTextBox control = new();
        control.SelectionProtected = value;
        Assert.False(control.SelectionProtected);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SelectionProtected = value;
        Assert.False(control.SelectionProtected);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.SelectionProtected = !value;
        Assert.False(control.SelectionProtected);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void RichTextBox_SelectionProtected_SetDisposed_ThrowsObjectDisposedException(bool value)
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectionProtected = value);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionRightIndent_GetWithoutHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.Equal(0, control.SelectionRightIndent);
        Assert.True(control.IsHandleCreated);

        // Call again.
        Assert.Equal(0, control.SelectionRightIndent);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionRightIndent_GetWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(0, control.SelectionRightIndent);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        Assert.Equal(0, control.SelectionRightIndent);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> SelectionRightIndent_CustomGetParamFormat_TestData()
    {
        yield return new object[] { 0, 0, 0 };
        yield return new object[] { 0, 20, 0 };
        yield return new object[] { PFM.RIGHTINDENT, 900, 60 };
        yield return new object[] { PFM.RIGHTINDENT, 30000, 2000 };
        yield return new object[] { PFM.RIGHTINDENT, 60000, 4000 };
        yield return new object[] { PFM.RIGHTINDENT, -900, -60 };
        yield return new object[] { PFM.RIGHTINDENT | PFM.ALIGNMENT, -900, -60 };
        yield return new object[] { PFM.ALIGNMENT, 20, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionRightIndent_CustomGetParamFormat_TestData))]
    public void RichTextBox_SelectionRightIndent_CustomGetParaFormat_ReturnsExpected(uint mask, int dxRightIndent, int expected)
    {
        using CustomGetParaFormatRichTextBox control = new()
        {
            GetParaFormatResult = new PARAFORMAT
            {
                dwMask = (PFM)mask,
                dxRightIndent = dxRightIndent
            }
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.MakeCustom = true;
        Assert.Equal(expected, control.SelectionRightIndent);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionRightIndent_GetCantCreateHandle_GetReturnsExpected()
    {
        using CantCreateHandleRichTextBox control = new();
        Assert.Equal(0, control.SelectionRightIndent);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionRightIndent_GetDisposed_ThrowsObjectDisposedException()
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectionRightIndent);
    }

    [WinFormsTheory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2000, 2000)]
    [InlineData(2001, 2001)]
    [InlineData(int.MaxValue, 0)]
    public void RichTextBox_SelectionRightIndent_Set_GetReturnsExpected(int value, int expected)
    {
        using RichTextBox control = new()
        {
            SelectionRightIndent = value
        };
        Assert.Equal(expected, control.SelectionRightIndent);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.SelectionRightIndent = value;
        Assert.Equal(expected, control.SelectionRightIndent);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2000, 2000)]
    [InlineData(2001, 2001)]
    [InlineData(int.MaxValue, 0)]
    public void RichTextBox_SelectionRightIndent_SetWithHandle_GetReturnsExpected(int value, int expected)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SelectionRightIndent = value;
        Assert.Equal(expected, control.SelectionRightIndent);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SelectionRightIndent = value;
        Assert.Equal(expected, control.SelectionRightIndent);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(0, 0)]
    [InlineData(1, 15)]
    [InlineData(2000, 30000)]
    [InlineData(2001, 30015)]
    [InlineData(int.MaxValue, 0)]
    public unsafe void RichTextBox_SelectionRightIndent_GetCharFormat_Success(int value, int expected)
    {
        using RichTextBox control = new();

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.SelectionRightIndent = value;
        PARAFORMAT format = new()
        {
            cbSize = (uint)sizeof(PARAFORMAT)
        };
        Assert.NotEqual(0, (int)PInvokeCore.SendMessage(control, PInvokeCore.EM_GETPARAFORMAT, (WPARAM)PInvoke.SCF_SELECTION, ref format));
        Assert.Equal(expected, format.dxRightIndent);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionRightIndent_SetNegative_ThrowArgumentOutOfRangeException()
    {
        using RichTextBox control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.SelectionRightIndent = -1);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    public void RichTextBox_SelectionRightIndent_SetCantCreateHandle_GetReturnsExpected(int value)
    {
        using CantCreateHandleRichTextBox control = new();
        control.SelectionRightIndent = value;
        Assert.Equal(0, control.SelectionRightIndent);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SelectionRightIndent = value;
        Assert.Equal(0, control.SelectionRightIndent);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    public void RichTextBox_SelectionRightIndent_SetDisposed_ThrowsObjectDisposedException(int value)
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectionRightIndent = value);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionStart_GetWithoutHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.Equal(0, control.SelectionStart);
        Assert.True(control.IsHandleCreated);

        // Get again.
        Assert.Equal(0, control.SelectionStart);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionStart_GetWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(0, control.SelectionStart);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Get again.
        Assert.Equal(0, control.SelectionStart);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionStart_GetCantCreateHandle_GetReturnsExpected()
    {
        using CantCreateHandleRichTextBox control = new();
        Assert.Equal(0, control.SelectionStart);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionStart_GetDisposed_ThrowsObjectDisposedException()
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectionStart);
    }

    [WinFormsTheory]
    [InlineData("", 0, 0)]
    [InlineData("", 1, 0)]
    [InlineData("text", 0, 0)]
    [InlineData("text", 1, 1)]
    [InlineData("text", 2, 2)]
    [InlineData("text", 3, 3)]
    [InlineData("text", 4, 4)]
    [InlineData("text", 5, 4)]
    public void RichTextBox_SelectionStart_Set_GetReturnsExpected(string text, int value, int expected)
    {
        using RichTextBox control = new()
        {
            Text = text,
            SelectionStart = value
        };
        Assert.Equal(0, control.SelectionLength);
        Assert.Equal(expected, control.SelectionStart);
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.SelectionStart = value;
        Assert.Equal(0, control.SelectionLength);
        Assert.Equal(expected, control.SelectionStart);
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData("", 0, 0, "")]
    [InlineData("", 1, 0, "")]
    [InlineData("text", 0, 0, "te")]
    [InlineData("text", 1, 1, "ex")]
    [InlineData("text", 2, 2, "xt")]
    [InlineData("text", 3, 3, "t")]
    [InlineData("text", 5, 4, "")]
    public void RichTextBox_SelectionStart_SetWithSelectionLength_Success(string text, int value, int expected, string expectedSelectedText)
    {
        using RichTextBox control = new()
        {
            Text = text,
            SelectionLength = 2,
            SelectionStart = value
        };
        Assert.Equal(expected, control.SelectionStart);
        Assert.Equal(expectedSelectedText, control.SelectedText);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.SelectionStart = value;
        Assert.Equal(expected, control.SelectionStart);
        Assert.Equal(expectedSelectedText, control.SelectedText);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData("", 0, 0)]
    [InlineData("", 1, 0)]
    [InlineData("text", 0, 0)]
    [InlineData("text", 1, 1)]
    [InlineData("text", 2, 2)]
    [InlineData("text", 3, 3)]
    [InlineData("text", 4, 4)]
    [InlineData("text", 5, 4)]
    public void RichTextBox_SelectionStart_SetWithHandle_Success(string text, int value, int expected)
    {
        using RichTextBox control = new()
        {
            Text = text
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SelectionStart = value;
        Assert.Equal(0, control.SelectionLength);
        Assert.Equal(expected, control.SelectionStart);
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SelectionStart = value;
        Assert.Equal(0, control.SelectionLength);
        Assert.Equal(expected, control.SelectionStart);
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(0, 0, 1)]
    [InlineData(1, 1, 2)]
    [InlineData(2, 2, 3)]
    [InlineData(3, 3, 4)]
    [InlineData(5, 4, 4)]
    public unsafe void RichTextBox_SelectionStart_GetSel_Success(int value, int expectedSelectionStart, int expectedEnd)
    {
        using RichTextBox control = new()
        {
            Text = "Text",
            SelectionLength = 1
        };

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.SelectionStart = value;
        int selectionStart = 0;
        int selectionEnd = 0;
        IntPtr result = PInvokeCore.SendMessage(control, PInvokeCore.EM_GETSEL, (WPARAM)(&selectionStart), (LPARAM)(&selectionEnd));
        Assert.Equal(expectedSelectionStart, PARAM.LOWORD(result));
        Assert.Equal(expectedEnd, PARAM.HIWORD(result));
        Assert.Equal(expectedSelectionStart, selectionStart);
        Assert.Equal(expectedEnd, selectionEnd);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    public void RichTextBox_SelectionStart_SetCantCreateHandle_GetReturnsExpected(int value)
    {
        using CantCreateHandleRichTextBox control = new();
        control.SelectionStart = value;
        Assert.Equal(0, control.SelectionStart);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SelectionStart = value;
        Assert.Equal(0, control.SelectionStart);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    public void RichTextBox_SelectionStart_SetDisposed_ThrowsObjectDisposedException(int value)
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectionStart = value);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionTabs_GetWithoutHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.Empty(control.SelectionTabs);
        Assert.True(control.IsHandleCreated);

        // Call again.
        Assert.Empty(control.SelectionTabs);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionTabs_GetWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Empty(control.SelectionTabs);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        Assert.Empty(control.SelectionTabs);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> SelectionTabs_CustomGetParaFormat_TestData()
    {
        yield return new object[] { 0, 3, new int[] { 900, 600, -900 }, Array.Empty<int>() };
        yield return new object[] { PFM.TABSTOPS, 3, new int[] { 900, 600, -900 }, new int[] { 60, 40, -60 } };
        yield return new object[] { PFM.TABSTOPS, 1, new int[] { 900, 600, -900 }, new int[] { 60 } };
        yield return new object[] { PFM.TABSTOPS, 32, Enumerable.Repeat(900, 32).ToArray(), Enumerable.Repeat(60, 32).ToArray() };
        yield return new object[] { PFM.TABSTOPS | PFM.ALIGNMENT, 3, new int[] { 900, 600, -900 }, new int[] { 60, 40, -60 } };
        yield return new object[] { PFM.ALIGNMENT, 3, new int[] { 900, 600, -900 }, Array.Empty<int>() };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionTabs_CustomGetParaFormat_TestData))]
    public unsafe void RichTextBox_SelectionTabs_CustomGetParaFormat_ReturnsExpected(int mask, short tabCount, int[] tabs, int[] expected)
    {
        PARAFORMAT result = new()
        {
            dwMask = (PFM)mask,
            cTabCount = tabCount
        };
        for (int i = 0; i < tabs.Length; i++)
        {
            result.rgxTabs[i] = tabs[i];
        }

        using CustomGetParaFormatRichTextBox control = new()
        {
            GetParaFormatResult = result
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.MakeCustom = true;
        Assert.Equal(expected, control.SelectionTabs);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionTabs_GetCantCreateHandle_GetReturnsExpected()
    {
        using CantCreateHandleRichTextBox control = new();
        Assert.Empty(control.SelectionTabs);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionTabs_GetDisposed_ThrowsObjectDisposedException()
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectionTabs);
    }

    public static IEnumerable<object[]> SelectionTabs_Set_TestData()
    {
        yield return new object[] { null, Array.Empty<int>() };
        yield return new object[] { Array.Empty<int>(), Array.Empty<int>() };
        yield return new object[] { new int[] { 1, 2 }, new int[] { 1, 2 } };
        yield return new object[] { new int[] { 1, 2, 3 }, new int[] { 1, 2, 3 } };
        yield return new object[] { new int[] { 1, 2, 3, 4 }, new int[] { 1, 2, 3, 4 } };
        yield return new object[] { new int[] { 1, 0, 3 }, new int[] { 1, 0, 3 } };
        yield return new object[] { new int[] { 1, -1, 3 }, Array.Empty<int>() };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionTabs_Set_TestData))]
    public void RichTextBox_SelectionTabs_Set_GetReturnsExpected(int[] value, int[] expected)
    {
        using RichTextBox control = new()
        {
            SelectionTabs = value
        };
        Assert.Equal(expected, control.SelectionTabs);
        Assert.NotSame(control.SelectionTabs, control.SelectionTabs);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.SelectionTabs = value;
        Assert.Equal(expected, control.SelectionTabs);
        Assert.NotSame(control.SelectionTabs, control.SelectionTabs);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionTabs_Set_TestData))]
    public void RichTextBox_SelectionTabs_SetWithHandle_GetReturnsExpected(int[] value, int[] expected)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SelectionTabs = value;
        Assert.Equal(expected, control.SelectionTabs);
        Assert.NotSame(control.SelectionTabs, control.SelectionTabs);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SelectionTabs = value;
        Assert.Equal(expected, control.SelectionTabs);
        Assert.NotSame(control.SelectionTabs, control.SelectionTabs);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> SelectionTabs_SetWithCustomOldValue_TestData()
    {
        yield return new object[] { null, Array.Empty<int>() };
        yield return new object[] { Array.Empty<int>(), Array.Empty<int>() };
        yield return new object[] { new int[] { 1, 2 }, new int[] { 1, 2 } };
        yield return new object[] { new int[] { 1, 2, 3 }, new int[] { 1, 2, 3 } };
        yield return new object[] { new int[] { 1, 2, 3, 4 }, new int[] { 1, 2, 3, 4 } };
        yield return new object[] { new int[] { 1, 0, 3 }, new int[] { 1, 0, 3 } };
        yield return new object[] { new int[] { 1, -1, 3 }, new int[] { 2, 3, 4 } };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionTabs_SetWithCustomOldValue_TestData))]
    public void RichTextBox_SelectionTabs_SetWithCustomOldValueWithHandle_GetReturnsExpected(int[] value, int[] expected)
    {
        using RichTextBox control = new()
        {
            SelectionTabs = [2, 3, 4]
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SelectionTabs = value;
        Assert.Equal(expected, control.SelectionTabs);
        Assert.NotSame(control.SelectionTabs, control.SelectionTabs);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SelectionTabs = value;
        Assert.Equal(expected, control.SelectionTabs);
        Assert.NotSame(control.SelectionTabs, control.SelectionTabs);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(new int[0])]
    [InlineData(null)]
    public unsafe void RichTextBox_SelectionTabs_GetParaFormat_Success(int[] nullOrEmptyValue)
    {
        using RichTextBox control = new();

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.SelectionTabs = [1, 2, 3];
        PARAFORMAT format = new()
        {
            cbSize = (uint)sizeof(PARAFORMAT)
        };
        Assert.NotEqual(0, (int)PInvokeCore.SendMessage(control, PInvokeCore.EM_GETPARAFORMAT, (WPARAM)PInvoke.SCF_SELECTION, ref format));
        Assert.Equal(3, format.cTabCount);
        Assert.Equal(15, format.rgxTabs[0]);
        Assert.Equal(30, format.rgxTabs[1]);
        Assert.Equal(45, format.rgxTabs[2]);

        // Set null or empty.
        control.SelectionTabs = nullOrEmptyValue;
        Assert.NotEqual(0, (int)PInvokeCore.SendMessage(control, PInvokeCore.EM_GETPARAFORMAT, (WPARAM)PInvoke.SCF_SELECTION, ref format));
        Assert.Equal(0, format.cTabCount);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionTabs_SetLongLength_ThrowsArgumentOutOfRangeException()
    {
        using RichTextBox control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.SelectionTabs = new int[33]);
    }

    public static IEnumerable<object[]> SelectionTabs_SetCantCreate_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { Array.Empty<int>() };
        yield return new object[] { new int[] { 1 } };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionTabs_SetCantCreate_TestData))]
    public void RichTextBox_SelectionTabs_SetCantCreateHandle_GetReturnsExpected(int[] value)
    {
        using CantCreateHandleRichTextBox control = new();
        control.SelectionTabs = value;
        Assert.Empty(control.SelectionTabs);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SelectionTabs = value;
        Assert.Empty(control.SelectionTabs);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionTabs_SetCantCreate_TestData))]
    public void RichTextBox_SelectionTabs_SetDisposed_ThrowsObjectDisposedException(int[] value)
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectionTabs = value);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionType_GetWithoutHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.Equal(RichTextBoxSelectionTypes.Empty, control.SelectionType);
        Assert.True(control.IsHandleCreated);

        // Call again.
        Assert.Equal(RichTextBoxSelectionTypes.Empty, control.SelectionType);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionType_GetWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(RichTextBoxSelectionTypes.Empty, control.SelectionType);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        Assert.Equal(RichTextBoxSelectionTypes.Empty, control.SelectionType);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(1, RichTextBoxSelectionTypes.Text)]
    [InlineData(4, RichTextBoxSelectionTypes.Text | RichTextBoxSelectionTypes.MultiChar)]
    public void RichTextBox_SelectionType_GetWithSelectionLengthWithHandle_ReturnsExpected(int selectionLength, RichTextBoxSelectionTypes expected)
    {
        using RichTextBox control = new()
        {
            Text = "text",
            SelectionLength = selectionLength
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(expected, control.SelectionType);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        Assert.Equal(expected, control.SelectionType);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> SelectionType_CustomSelectionType_TestData()
    {
        yield return new object[] { 0, (IntPtr)RichTextBoxSelectionTypes.Text, RichTextBoxSelectionTypes.Empty };
        yield return new object[] { 0, (IntPtr)RichTextBoxSelectionTypes.MultiObject, RichTextBoxSelectionTypes.Empty };

        yield return new object[] { 1, (IntPtr)RichTextBoxSelectionTypes.Text, RichTextBoxSelectionTypes.Text };
        yield return new object[] { 1, (IntPtr)RichTextBoxSelectionTypes.MultiObject, RichTextBoxSelectionTypes.MultiObject };
        yield return new object[] { 1, (IntPtr)(-1), -1 };

        yield return new object[] { 4, (IntPtr)RichTextBoxSelectionTypes.Text, RichTextBoxSelectionTypes.Text };
        yield return new object[] { 4, (IntPtr)RichTextBoxSelectionTypes.MultiObject, RichTextBoxSelectionTypes.MultiObject };
        yield return new object[] { 4, (IntPtr)(-1), -1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectionType_CustomSelectionType_TestData))]
    public void RichTextBox_SelectionType_CustomSelectionType_ReturnsExpected(int selectionLength, IntPtr result, RichTextBoxSelectionTypes expected)
    {
        using CustomSelectionTypeRichTextBox control = new()
        {
            Text = "text",
            SelectionLength = selectionLength,
            SelectionTypeResult = result
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(expected, control.SelectionType);
    }

    private class CustomSelectionTypeRichTextBox : RichTextBox
    {
        public IntPtr SelectionTypeResult { get; set; }

        protected override unsafe void WndProc(ref Message m)
        {
            if (m.Msg == (int)PInvokeCore.EM_SELECTIONTYPE)
            {
                Assert.Equal(IntPtr.Zero, m.WParam);
                Assert.Equal(IntPtr.Zero, m.LParam);
                m.Result = SelectionTypeResult;
                return;
            }

            base.WndProc(ref m);
        }
    }

    [WinFormsFact]
    public void RichTextBox_SelectionType_GetCantCreateHandle_GetReturnsExpected()
    {
        using CantCreateHandleRichTextBox control = new();
        Assert.Equal(RichTextBoxSelectionTypes.Empty, control.SelectionType);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SelectionType_GetDisposed_ThrowsObjectDisposedException()
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectionType);
    }

    [WinFormsFact]
    public void RichTextBox_ShowSelectionMargin_GetWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.False(control.ShowSelectionMargin);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call EM_SETOPTIONS.
        PInvokeCore.SendMessage(control, PInvokeCore.EM_SETOPTIONS, (WPARAM)(int)PInvoke.ECOOP_OR, (LPARAM)(nint)PInvoke.ECO_SELECTIONBAR);
        Assert.False(control.ShowSelectionMargin);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void RichTextBox_ShowSelectionMargin_Set_GetReturnsExpected(bool value)
    {
        using RichTextBox control = new()
        {
            ShowSelectionMargin = value
        };
        Assert.Equal(value, control.ShowSelectionMargin);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ShowSelectionMargin = value;
        Assert.Equal(value, control.ShowSelectionMargin);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.ShowSelectionMargin = !value;
        Assert.Equal(!value, control.ShowSelectionMargin);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void RichTextBox_ShowSelectionMargin_SetWithHandle_GetReturnsExpected(bool value)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ShowSelectionMargin = value;
        Assert.Equal(value, control.ShowSelectionMargin);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ShowSelectionMargin = value;
        Assert.Equal(value, control.ShowSelectionMargin);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.ShowSelectionMargin = !value;
        Assert.Equal(!value, control.ShowSelectionMargin);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(true, 0x1000041)]
    [InlineData(false, 0x41)]
    public void RichTextBox_ShowSelectionMargin_GetOptions_Success(bool value, int expected)
    {
        using RichTextBox control = new();

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.ShowSelectionMargin = value;
        Assert.Equal(expected, (int)PInvokeCore.SendMessage(control, PInvokeCore.EM_GETOPTIONS));
    }

    [WinFormsFact]
    public void RichTextBox_TextLength_GetDefaultWithoutHandle_Success()
    {
        using RichTextBox control = new();
        Assert.Equal(0, control.TextLength);
        Assert.True(control.IsHandleCreated);

        // Call again.
        Assert.Equal(0, control.TextLength);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_TextLength_GetDefaultWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(0, control.TextLength);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        Assert.Equal(0, control.TextLength);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData("", 0)]
    [InlineData("a\0b", 1)]
    [InlineData("a", 1)]
    [InlineData("\ud83c\udf09", 2)] // emoji, surrogate pair https://charbase.com/1f309-unicode-bridge-at-night
    public void RichTextBox_TextLength_GetSetWithHandle_Success(string text, int expected)
    {
        using RichTextBox control = new()
        {
            Text = text
        };
        Assert.Equal(expected, control.TextLength);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData("", 0)]
    [InlineData("a\0b", 1)]
    [InlineData("a", 1)]
    [InlineData("\ud83c\udf09", 2)] // emoji, surrogate pair https://charbase.com/1f309-unicode-bridge-at-night
    public void RichTextBox_TextLength_GetWithHandle_ReturnsExpected(string text, int expected)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Text = text;
        Assert.Equal(expected, control.TextLength);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> TextLength_GetCustomGetTextLengthEx_TestData()
    {
        yield return new object[] { IntPtr.Zero, 0 };
        yield return new object[] { (IntPtr)(-1), -1 };
        yield return new object[] { (IntPtr)1, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(TextLength_GetCustomGetTextLengthEx_TestData))]
    public void RichTextBox_TextLength_GetCustomGetTextLengthEx_Success(IntPtr result, int expected)
    {
        using CustomGetTextLengthExRichTextBox control = new()
        {
            GetTextLengthExResult = result
        };
        Assert.Equal(expected, control.TextLength);
    }

    private class CustomGetTextLengthExRichTextBox : RichTextBox
    {
        public IntPtr GetTextLengthExResult { get; set; }

        protected override unsafe void WndProc(ref Message m)
        {
            if (m.Msg == (int)PInvokeCore.EM_GETTEXTLENGTHEX)
            {
                GETTEXTLENGTHEX* gtl = (GETTEXTLENGTHEX*)m.WParam;
                Assert.Equal(GETTEXTLENGTHEX_FLAGS.GTL_NUMCHARS, gtl->flags);
                Assert.Equal(1200u, gtl->codepage);
                Assert.Equal(IntPtr.Zero, m.LParam);
                m.Result = GetTextLengthExResult;
                return;
            }

            base.WndProc(ref m);
        }
    }

    public static IEnumerable<object[]> RichTextBox_Text_GetWithHandle_TestData()
    {
        yield return new object[] { null, string.Empty };
        yield return new object[] { string.Empty, string.Empty };
        yield return new object[] { " ", " " };
        yield return new object[] { "abc", "abc" };
        yield return new object[] { "a\0b", "a" };
        yield return new object[] { "\ud83c\udf09", "\ud83c\udf09" }; // emoji, surrogate pair https://charbase.com/1f309-unicode-bridge-at-night
        yield return new object[] { "\n\n", "\n\n" };
        yield return new object[] { "\r\r", "\n\n" };
        yield return new object[] { "\n\r", "\n\n" };
        yield return new object[] { "\r\n\r\n", "\n\n" };
        yield return new object[] { "a6\r\nb\r\nc\r\nd\r\n", "a6\nb\nc\nd\n" };
        yield return new object[] { "a7\nb\rc\r\n\n\rd\r\r\n", "a7\nb\nc\n\n\nd", SAME, "a7\nb\nc\n\n\nd " }; // RichEdit20W has a trailing space

        // 0x0008 to 0x007F: https://www.unicode.org/charts/PDF/U0000.pdf
        // 0x2000 to 0x2069: https://www.unicode.org/charts/PDF/U2000.pdf

        var chars = Enumerable.Range(0x0008, /* 0x0008 to 0x007F */ 0x007F - 0x0008 + 1).Union(
                    Enumerable.Range(0x2000, /* 0x2000 to 0x2069 */ 0x2069 - 0x2000 + 1));

        foreach (int i in chars)
        {
            if (i == 0x000B) // Vertical Tabulation
            {
                // NOTE: The old control works the same, but StreamOut() substituted 0x000B with \n
                yield return new object[] { $"{(char)i}ab", "ab", "\nab" };
                yield return new object[] { $"a{(char)i}b", "ab", "a\nb" };
                yield return new object[] { $"ab\r\n{(char)i}\r\n", "ab\n\n", "ab\n\n\n" };
                yield return new object[] { $"ab{(char)i}", $"ab", "ab\n" };
                yield return new object[] { $"ab\r\n{(char)i}", "ab\n", "ab\n\n" };

                continue;
            }

            if (i == 0x000D) // Carriage Return (\r) gets replaced with Line Feed (\n)
            {
                yield return new object[] { $"{(char)i}ab", "\nab" };
                yield return new object[] { $"a{(char)i}b", "a\nb" };
                yield return new object[] { $"ab\r\n{(char)i}\r\n", "ab\n", SAME, "ab\n " }; // RichEdit20W has a trailing space
                yield return new object[] { $"ab{(char)i}", "ab\n" };
                yield return new object[] { $"ab\r\n{(char)i}", "ab\n\n" };

                continue;
            }

            if (i == 0x2028) // Line Separator (\v)
            {
                // NOTE: The old control works the same, but StreamOut() substituted 0x2028 with \n
                yield return new object[] { $"{(char)i}ab", "ab", "\nab" };
                yield return new object[] { $"a{(char)i}b", "ab", "a\nb" };
                yield return new object[] { $"ab\r\n{(char)i}\r\n", "ab\n\n", "ab\n\n\n" };
                yield return new object[] { $"ab{(char)i}", $"ab", "ab\n" };
                yield return new object[] { $"ab\r\n{(char)i}", "ab\n", "ab\n\n" };

                continue;
            }

            if (i == 0x2029) // Paragraph Separator
            {
                yield return new object[] { $"{(char)i}ab", "\nab" };
                yield return new object[] { $"a{(char)i}b", "a\nb" };
                yield return new object[] { $"ab\r\n{(char)i}\r\n", "ab\n", SAME, "ab\n\n\n" }; // RichEdit20W has extra line feeds
                yield return new object[] { $"ab{(char)i}", $"ab\n" };
                yield return new object[] { $"ab\r\n{(char)i}", "ab\n\n" };

                continue;
            }

            yield return new object[] { $"{(char)i}ab", $"{(char)i}ab" };
            yield return new object[] { $"a{(char)i}b", $"a{(char)i}b" };
            yield return new object[] { $"ab\r\n{(char)i}\r\n", $"ab\n{(char)i}\n" };
            yield return new object[] { $"ab{(char)i}", $"ab{(char)i}" };
            yield return new object[] { $"ab\r\n{(char)i}", $"ab\n{(char)i}" };
        }
    }

    private const string SAME = "SAME";

    // NOTE: do not convert this into a theory as it will run hundreds of tests
    // and with that will cycle through hundreds of UI controls.
    [ActiveIssue("https://github.com/dotnet/winforms/issues/6609")]
    [WinFormsFact]
    [SkipOnArchitecture(TestArchitectures.X86,
        "Flaky tests, see: https://github.com/dotnet/winforms/issues/6609")]
    public void RichTextBox_Text_GetWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        control.CreateControl();

        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        // verify against RichEdit20W
        using (var riched20 = new RichEdit20W())
        {
            riched20.CreateControl();

            foreach (object[] testCaseData in RichTextBox_Text_GetWithHandle_TestData())
            {
                string text = (string)testCaseData[0];
                string expectedText = (string)testCaseData[1];
                string oldWayExpectedText = testCaseData.Length > 2 ? (string)testCaseData[2] : SAME;
                string oldControlExpectedText = testCaseData.Length > 3 ? (string)testCaseData[3] : SAME;

                // NOTE: in certain scenarios the old way (using StreamOut() method) returned a different
                // text value to the new way (via GetTextEx() method).
                // If oldWayExpectedText is SAME, assume StreamOut() returned the same expectedText.
                if (oldWayExpectedText is SAME)
                {
                    oldWayExpectedText = expectedText;
                }

                // NOTE: in certain scenarios the old control returns a different text value to the new control.
                // If oldControlExpectedText is SAME, assume the old control returns the same expectedText.
                if (oldControlExpectedText is SAME)
                {
                    oldControlExpectedText = expectedText;
                }

                control.Text = text;
                Assert.Equal(expectedText, control.Text);

                // verify the old behavior via StreamOut(SF.TEXT | SF.UNICODE)
                string textOldWay = control.TestAccessor().Dynamic.StreamOut(PInvoke.SF_TEXT | PInvoke.SF_UNICODE);
                Assert.Equal(oldWayExpectedText, textOldWay);

                // verify against RichEdit20W
                riched20.Text = text;
                Assert.Equal(oldControlExpectedText, riched20.Text);
            }
        }

        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("abc", "abc")]
    [InlineData("a\0b", "a\0b")]
    [InlineData("\ud83c\udf09", "\ud83c\udf09")] // emoji, surrogate pair https://charbase.com/1f309-unicode-bridge-at-night
    [InlineData("\n\n", "\n\n")]
    [InlineData("\r\r", "\r\r")]
    [InlineData("\r\n\r\n", "\r\n\r\n")]
    [InlineData("a\r\nb\r\nc\r\nd\r\n", "a\r\nb\r\nc\r\nd\r\n")]
    [InlineData("a\nb\rc\r\n\n\rd\r\r\n", "a\nb\rc\r\n\n\rd\r\r\n")]
    public void RichTextBox_Text_GetWithoutHandle_ReturnsExpected(string text, string expected)
    {
        using (RichTextBox control = new())
        {
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Empty(control.Text);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            control.Text = text;

            Assert.Equal(expected, control.Text);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.False(control.IsHandleCreated);
        }

        // verify against RichEdit20W
        using var riched20 = new RichEdit20W();
        Assert.Empty(riched20.Text);
        Assert.False(riched20.IsHandleCreated);

        riched20.Text = text;

        Assert.Equal(expected, riched20.Text);
        Assert.False(riched20.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("abc", "abc")]
    [InlineData("a\0b", "a")]
    [InlineData("\ud83c\udf09", "\ud83c\udf09")] // emoji, surrogate pair https://charbase.com/1f309-unicode-bridge-at-night
    [InlineData("\n\n", "\r\n\r\n")]
    [InlineData("\r\r", "\r\n\r\n")]
    [InlineData("\r\n\r\n", "\r\n\r\n")]
    [InlineData("a\r\nb\r\nc\r\nd\r\n", "a\r\nb\r\nc\r\nd\r\n")]
    [InlineData("a1\nb\rc\n\r\n\rd\r\r\n", "a1\r\nb\r\nc\r\n\r\n\r\nd")]
    public void RichTextBox_Text_GetTextEx_USECRLF_ReturnsExpected(string text, string expected)
    {
        using RichTextBox control = new();
        control.CreateControl();

        Assert.Empty(control.Text);

        control.Text = text;

        string textOldWay = control.TestAccessor().Dynamic.GetTextEx(GETTEXTEX_FLAGS.GT_USECRLF);
        Assert.Equal(expected, textOldWay);
    }

    [WinFormsFact]
    public void RichTextBox_Text_GetCantCreateHandle_ReturnsExpected()
    {
        using CantCreateHandleRichTextBox control = new();
        Assert.Empty(control.Text);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_Text_GetWithRtf_ReturnsExpected()
    {
        using RichTextBox control = new()
        {
            Rtf = "{\\rtf Hello World}"
        };
        Assert.Equal("Hello World", control.Text);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_Text_GetDisposed_ReturnsExpected()
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Empty(control.Text);
    }

    public static IEnumerable<object[]> Text_Set_TestData()
    {
        foreach (bool autoSize in new bool[] { true, false })
        {
            yield return new object[] { autoSize, null, string.Empty };
            yield return new object[] { autoSize, string.Empty, string.Empty };
            yield return new object[] { autoSize, "text", "text" };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Text_Set_TestData))]
    public void RichTextBox_Text_Set_GetReturnsExpected(bool autoSize, string value, string expected)
    {
        using RichTextBox control = new()
        {
            AutoSize = autoSize
        };
        int layoutCallCount = 0;
        control.Layout += (control, e) => layoutCallCount++;

        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.False(control.Modified);
        Assert.False(control.CanUndo);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.False(control.Modified);
        Assert.False(control.CanUndo);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void RichTextBox_Text_SetWithRtfText_GetReturnsExpected(string value, string expected)
    {
        using RichTextBox control = new()
        {
            Rtf = "{\\rtf Hello World}",
            Text = value
        };
        Assert.Equal(expected, control.Text);
        Assert.Contains(expected, control.Rtf);
        Assert.DoesNotContain("Hello World", control.Rtf);
        Assert.Equal(expected.Length, control.TextLength);
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.False(control.Modified);
        Assert.False(control.CanUndo);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.Contains(expected, control.Rtf);
        Assert.DoesNotContain("Hello World", control.Rtf);
        Assert.Equal(expected.Length, control.TextLength);
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.False(control.Modified);
        Assert.False(control.CanUndo);
        Assert.True(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Text_SetWithParent_TestData()
    {
        yield return new object[] { true, null, string.Empty, 1 };
        yield return new object[] { true, string.Empty, string.Empty, 1 };
        yield return new object[] { true, "text", "text", 1 };

        yield return new object[] { false, null, string.Empty, 0 };
        yield return new object[] { false, string.Empty, string.Empty, 0 };
        yield return new object[] { false, "text", "text", 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Text_SetWithParent_TestData))]
    public void RichTextBox_Text_SetWithParent_GetReturnsExpected(bool autoSize, string value, string expected, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using RichTextBox control = new()
        {
            Parent = parent,
            AutoSize = autoSize
        };
        int layoutCallCount = 0;
        control.Layout += (control, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Text", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.False(control.Modified);
            Assert.False(control.CanUndo);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.False(control.Modified);
            Assert.False(control.CanUndo);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount * 2, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> Text_SetWithSelection_TestData()
    {
        yield return new object[] { string.Empty, 0, 2, null, string.Empty };
        yield return new object[] { string.Empty, 0, 2, "t", "t" };
        yield return new object[] { string.Empty, 0, 2, "text", "text" };

        yield return new object[] { string.Empty, 1, 2, null, string.Empty };
        yield return new object[] { string.Empty, 1, 2, "t", "t" };
        yield return new object[] { string.Empty, 1, 2, "text", "text" };

        yield return new object[] { "text", 0, 2, null, string.Empty };
        yield return new object[] { "text", 0, 2, "t", "t" };
        yield return new object[] { "text", 0, 2, "te", "te" };
        yield return new object[] { "text", 0, 2, "tex", "tex" };

        yield return new object[] { "text", 1, 2, null, string.Empty };
        yield return new object[] { "text", 1, 2, "t", "t" };
        yield return new object[] { "text", 1, 2, "te", "te" };
        yield return new object[] { "text", 1, 2, "tex", "tex" };
    }

    [WinFormsTheory]
    [MemberData(nameof(Text_SetWithSelection_TestData))]
    public void RichTextBox_Text_SetWithSelection_GetReturnsExpected(string oldValue, int selectionStart, int selectionLength, string value, string expected)
    {
        using RichTextBox control = new()
        {
            Text = oldValue,
            SelectionStart = selectionStart,
            SelectionLength = selectionLength,
        };

        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.Equal(expected.Length, control.TextLength);
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.False(control.Modified);
        Assert.False(control.CanUndo);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.Equal(expected.Length, control.TextLength);
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.False(control.Modified);
        Assert.False(control.CanUndo);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(null, "", true)]
    [InlineData("", "", false)]
    [InlineData("text", "text", false)]
    public void RichTextBox_Text_SetModified_GetReturnsExpected(string value, string expected, bool expectedModified)
    {
        using RichTextBox control = new()
        {
            Modified = true,
            Text = value
        };
        Assert.Equal(expected, control.Text);
        Assert.Equal(expected.Length, control.TextLength);
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.Equal(expectedModified, control.Modified);
        Assert.False(control.CanUndo);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.Equal(expected.Length, control.TextLength);
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.False(control.Modified);
        Assert.False(control.CanUndo);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Text_Set_TestData))]
    public void RichTextBox_Text_SetWithHandle_GetReturnsExpected(bool autoSize, string value, string expected)
    {
        using RichTextBox control = new()
        {
            AutoSize = autoSize
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCallCount = 0;
        control.Layout += (control, e) => layoutCallCount++;

        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.Equal(expected.Length, control.TextLength);
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.False(control.Modified);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.Equal(expected.Length, control.TextLength);
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.False(control.Modified);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void RichTextBox_Text_SetWithRtfTextWithHandle_GetReturnsExpected(string value, string expected)
    {
        using RichTextBox control = new()
        {
            Rtf = "{\\rtf Hello World}"
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.Contains(expected, control.Rtf);
        Assert.DoesNotContain("Hello World", control.Rtf);
        Assert.Equal(expected.Length, control.TextLength);
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.False(control.Modified);
        Assert.False(control.CanUndo);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.Contains(expected, control.Rtf);
        Assert.DoesNotContain("Hello World", control.Rtf);
        Assert.Equal(expected.Length, control.TextLength);
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.False(control.Modified);
        Assert.False(control.CanUndo);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Text_SetWithParent_TestData))]
    public void RichTextBox_Text_SetWithParentWithHandle_GetReturnsExpected(bool autoSize, string value, string expected, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using RichTextBox control = new()
        {
            Parent = parent,
            AutoSize = autoSize
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCallCount = 0;
        control.Layout += (control, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Text", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.False(control.Modified);
            Assert.False(control.CanUndo);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.False(control.Modified);
            Assert.False(control.CanUndo);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount * 2, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("text", "text")]
    public void RichTextBox_Text_SetModifiedWithHandle_GetReturnsExpected(string value, string expected)
    {
        using RichTextBox control = new()
        {
            Modified = true
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.Equal(expected.Length, control.TextLength);
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.False(control.Modified);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.Equal(expected.Length, control.TextLength);
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.False(control.Modified);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> Text_SetWithSelectionWithHandle_TestData()
    {
        yield return new object[] { string.Empty, 0, 2, null, string.Empty };
        yield return new object[] { string.Empty, 0, 2, "t", "t" };
        yield return new object[] { string.Empty, 0, 2, "text", "text" };

        yield return new object[] { string.Empty, 1, 2, null, string.Empty };
        yield return new object[] { string.Empty, 1, 2, "t", "t" };
        yield return new object[] { string.Empty, 1, 2, "text", "text" };

        yield return new object[] { "text", 0, 2, null, string.Empty };
        yield return new object[] { "text", 0, 2, "t", "t" };
        yield return new object[] { "text", 0, 2, "te", "te" };
        yield return new object[] { "text", 0, 2, "tex", "tex" };

        yield return new object[] { "text", 1, 2, null, string.Empty };
        yield return new object[] { "text", 1, 2, "t", "t" };
        yield return new object[] { "text", 1, 2, "te", "te" };
        yield return new object[] { "text", 1, 2, "tex", "tex" };
    }

    [WinFormsTheory]
    [MemberData(nameof(Text_SetWithSelectionWithHandle_TestData))]
    public void RichTextBox_Text_SetWithSelectionWith_GetReturnsExpected(string oldValue, int selectionStart, int selectionLength, string value, string expected)
    {
        using RichTextBox control = new()
        {
            Text = oldValue,
            SelectionStart = selectionStart,
            SelectionLength = selectionLength,
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.Equal(expected.Length, control.TextLength);
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.False(control.Modified);
        Assert.False(control.CanUndo);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.Equal(expected.Length, control.TextLength);
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.False(control.Modified);
        Assert.False(control.CanUndo);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_Text_SetWithHandler_CallsTextChanged()
    {
        using RichTextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(EventArgs.Empty, e);
            callCount++;
        };
        control.TextChanged += handler;

        // Set different.
        control.Text = "text";
        Assert.Equal("text", control.Text);
        Assert.Equal(0, callCount);

        // Set same.
        control.Text = "text";
        Assert.Equal("text", control.Text);
        Assert.Equal(0, callCount);

        // Set different.
        control.Text = null;
        Assert.Empty(control.Text);
        Assert.Equal(0, callCount);

        // Remove handler.
        control.TextChanged -= handler;
        control.Text = "text";
        Assert.Equal("text", control.Text);
        Assert.Equal(0, callCount);
    }

    [WinFormsFact]
    public void RichTextBox_Text_SetWithHandlerWithHandle_CallsTextChanged()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(EventArgs.Empty, e);
            callCount++;
        };
        control.TextChanged += handler;

        // Set different.
        control.Text = "text";
        Assert.Equal("text", control.Text);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Text = "text";
        Assert.Equal("text", control.Text);
        Assert.Equal(2, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.Text = null;
        Assert.Empty(control.Text);
        Assert.Equal(3, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.TextChanged -= handler;
        control.Text = "text";
        Assert.Equal("text", control.Text);
        Assert.Equal(3, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void RichTextBox_Text_SetCantCreateHandle_GetReturnsExpected(string value, string expected)
    {
        using CantCreateHandleRichTextBox control = new();
        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void RichTextBox_Text_SetDisposed_ThrowsObjectDisposedException(string value)
    {
        using RichTextBox control = new();
        control.Dispose();

        control.Text = value;
        Assert.Empty(control.Text);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Text = value;
        Assert.Empty(control.Text);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_UndoActionName_GetWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Empty(control.UndoActionName);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> UndoActionName_CustomGetUndoName_TestData()
    {
        yield return new object[] { IntPtr.Zero, IntPtr.Zero, string.Empty };
        yield return new object[] { (IntPtr)1, IntPtr.Zero, "Unknown" };
        yield return new object[] { (IntPtr)1, (IntPtr)1, "Typing" };
        yield return new object[] { (IntPtr)1, (IntPtr)2, "Delete" };
        yield return new object[] { (IntPtr)1, (IntPtr)3, "Drag and Drop" };
        yield return new object[] { (IntPtr)1, (IntPtr)4, "Cut" };
        yield return new object[] { (IntPtr)1, (IntPtr)5, "Paste" };
        yield return new object[] { (IntPtr)1, (IntPtr)6, "Unknown" };
        yield return new object[] { (IntPtr)1, (IntPtr)7, "Unknown" };
    }

    [WinFormsTheory]
    [MemberData(nameof(UndoActionName_CustomGetUndoName_TestData))]
    public void RichTextBox_UndoActionName_CustomGetUndoName_ReturnsExpected(IntPtr canUndoResult, IntPtr getUndoNameResult, string expected)
    {
        using CustomGetUndoNameRichTextBox control = new()
        {
            CanUndoResult = canUndoResult,
            GetUndoNameResult = getUndoNameResult
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(expected, control.UndoActionName);
    }

    private class CustomGetUndoNameRichTextBox : RichTextBox
    {
        public IntPtr CanUndoResult { get; set; }
        public IntPtr GetUndoNameResult { get; set; }

        protected override unsafe void WndProc(ref Message m)
        {
            if (m.Msg == (int)PInvokeCore.EM_CANUNDO)
            {
                m.Result = CanUndoResult;
                return;
            }
            else if (m.Msg == (int)PInvokeCore.EM_GETUNDONAME)
            {
                m.Result = GetUndoNameResult;
                return;
            }

            base.WndProc(ref m);
        }
    }

    [WinFormsFact]
    public void RichTextBox_UndoActionName_GetCantCreateHandle_GetReturnsExpected()
    {
        using CantCreateHandleRichTextBox control = new();
        Assert.Empty(control.UndoActionName);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_UndoActionName_GetDisposed_ThrowsObjectDisposedException()
    {
        using RichTextBox control = new();
        control.Dispose();
        Assert.Empty(control.UndoActionName);
    }

    [WinFormsFact]
    public void RichTextBox_ZoomFactor_GetWithHandle_ReturnsExpected()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(1, control.ZoomFactor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call EM_SETZOOM.
        PInvokeCore.SendMessage(control, PInvokeCore.EM_SETZOOM, 2, 10);
        Assert.Equal(0.2f, control.ZoomFactor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(1, 1, 1.0f)]
    [InlineData(10, 1, 10.0f)]
    [InlineData(10, 2, 5.0f)]
    [InlineData(1, 10, 0.1f)]
    [InlineData(1, 0, 1f)]
    [InlineData(0, 1, 1f)]
    [InlineData(0, 0, 1f)]
    public void RichTextBox_ZoomFactor_CustomGetZoom_ReturnsExpected(int numerator, int denominator, float expected)
    {
        using CustomGetZoomRichTextBox control = new()
        {
            NumeratorResult = numerator,
            DenominatorResult = denominator
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(expected, control.ZoomFactor);
    }

    private class CustomGetZoomRichTextBox : RichTextBox
    {
        public int NumeratorResult { get; set; }
        public int DenominatorResult { get; set; }

        protected override unsafe void WndProc(ref Message m)
        {
            if (m.Msg == (int)PInvokeCore.EM_GETZOOM)
            {
                int* pNumerator = (int*)m.WParam;
                int* pDenominator = (int*)m.LParam;

                *pNumerator = NumeratorResult;
                *pDenominator = DenominatorResult;
                return;
            }

            base.WndProc(ref m);
        }
    }

    [WinFormsTheory]
    [InlineData(0.015626f, 0.016f)]
    [InlineData(63.9f, 63.9f)]
    [InlineData(1.0f, 1.0f)]
    [InlineData(2.0f, 2.0f)]
    [InlineData(float.NaN, 1.0f)]
    public void RichTextBox_ZoomFactor_Set_GetReturnsExpected(float value, float expected)
    {
        using RichTextBox control = new()
        {
            ZoomFactor = value
        };
        Assert.Equal(expected, control.ZoomFactor, 2f);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ZoomFactor = value;
        Assert.Equal(expected, control.ZoomFactor, 2f);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0.015626f, 0.016f)]
    [InlineData(63.9f, 63.9f)]
    [InlineData(1.0f, 1.0f)]
    [InlineData(2.0f, 2.0f)]
    [InlineData(float.NaN, 1.0f)]
    public void RichTextBox_ZoomFactor_SetWithHandle_GetReturnsExpected(float value, float expected)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ZoomFactor = value;
        Assert.Equal(expected, control.ZoomFactor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ZoomFactor = value;
        Assert.Equal(expected, control.ZoomFactor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(0.015624f)]
    [InlineData(0.015625f)]
    [InlineData(64.0f)]
    [InlineData(64.1f)]
    [InlineData(float.PositiveInfinity)]
    [InlineData(float.NegativeInfinity)]
    public void RichTextBox_ZoomFactor_SetInvalidValue_ThrowsArgumentOutOfRangeException(float value)
    {
        using RichTextBox control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.ZoomFactor = value);
    }

    public static IEnumerable<object[]> CanPaste_TestData()
    {
        yield return new object[] { DataFormats.GetFormat(DataFormats.Palette), false };
        yield return new object[] { new DataFormats.Format("UnknownName", int.MaxValue), false };
    }

    [WinFormsTheory]
    [MemberData(nameof(CanPaste_TestData))]
    public void RichTextBox_CanPaste_Invoke_ReturnsExpected(DataFormats.Format format, bool expected)
    {
        using RichTextBox control = new();
        Assert.Equal(expected, control.CanPaste(format));
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(CanPaste_TestData))]
    public void RichTextBox_CanPaste_InvokeWithHandle_ReturnsExpected(DataFormats.Format format, bool expected)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(expected, control.CanPaste(format));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> CanPaste_CustomCanPaste_TestData()
    {
        yield return new object[] { IntPtr.Zero, false };
        yield return new object[] { (IntPtr)1, true };
    }

    [WinFormsTheory]
    [MemberData(nameof(CanPaste_CustomCanPaste_TestData))]
    public void RichTextBox_CanPaste_CustomCanPaste_ReturnsExpected(IntPtr result, bool expected)
    {
        using CustomCanPasteRichTextBox control = new()
        {
            Result = result
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(expected, control.CanPaste(DataFormats.GetFormat(DataFormats.Text)));
    }

    private class CustomCanPasteRichTextBox : RichTextBox
    {
        public IntPtr Result { get; set; }

        protected override unsafe void WndProc(ref Message m)
        {
            if (m.Msg == (int)PInvokeCore.EM_CANPASTE)
            {
                m.Result = Result;
                return;
            }

            base.WndProc(ref m);
        }
    }

    [WinFormsFact]
    public void RichTextBox_CanPaste_NullFormat_ThrowsNullReferenceException()
    {
        using RichTextBox control = new();
        Assert.Throws<NullReferenceException>(() => control.CanPaste(null));
    }

    [WinFormsFact]
    public void RichTextBox_Find_String_Table_ReturnsExpected()
    {
        using RichTextBox control = new()
        {
            Rtf = "{\\rtf1\\ansi\\deff0\\trowd\\pard\\intbl Test\\cell Example\\cell Meow\\cell Woof\\cell \\row}"
        };

        Assert.Equal(2, control.Find("Test"));
        Assert.Equal(7, control.Find("Example"));
        Assert.Equal(15, control.Find("Meow"));
        Assert.Equal(20, control.Find("Woof"));
    }

    public static IEnumerable<object[]> Find_String_TestData()
    {
        yield return new object[] { string.Empty, string.Empty, -1 };
        yield return new object[] { string.Empty, "abc", -1 };

        yield return new object[] { "abc", string.Empty, -1 };
        yield return new object[] { "abc", "a", 0 };
        yield return new object[] { "abc", "ab", 0 };
        yield return new object[] { "abc", "abc", 0 };
        yield return new object[] { "abc", "abcd", -1 };
        yield return new object[] { "abc", "b", 1 };
        yield return new object[] { "abc", "d", -1 };
        yield return new object[] { "abc", "ABC", 0 };

        yield return new object[] { "aa", "a", 0 };
        yield return new object[] { "abc def", "ef", 5 };
        yield return new object[] { "abc def", "def", 4 };
        yield return new object[] { "abc def", " ", 3 };

        yield return new object[] { "ab\u0640cd", "abcd", 0 };
        yield return new object[] { "ab\u0640cd", "\u0640", 2 };
        yield return new object[] { "ab\u0640cd", "bc", 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Find_String_TestData))]
    public void RichTextBox_Find_String_ReturnsExpected(string text, string str, int expected)
    {
        using RichTextBox control = new()
        {
            Text = text
        };
        Assert.Equal(expected, control.Find(str));
        Assert.True(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Find_String_RichTextBoxFinds_TestData()
    {
        yield return new object[] { string.Empty, string.Empty, RichTextBoxFinds.None, -1 };
        yield return new object[] { string.Empty, "abc", RichTextBoxFinds.None, -1 };
        yield return new object[] { string.Empty, string.Empty, RichTextBoxFinds.Reverse, -1 };
        yield return new object[] { string.Empty, "abc", RichTextBoxFinds.Reverse, -1 };

        yield return new object[] { "abc", string.Empty, RichTextBoxFinds.None, -1 };
        yield return new object[] { "abc", "a", RichTextBoxFinds.None, 0 };
        yield return new object[] { "abc", "a", RichTextBoxFinds.Reverse, 0 };
        yield return new object[] { "abc", "ab", RichTextBoxFinds.None, 0 };
        yield return new object[] { "abc", "abc", RichTextBoxFinds.None, 0 };
        yield return new object[] { "abc", "abcd", RichTextBoxFinds.None, -1 };
        yield return new object[] { "abc", "b", RichTextBoxFinds.None, 1 };
        yield return new object[] { "abc", "d", RichTextBoxFinds.None, -1 };

        yield return new object[] { "abc", "ABC", RichTextBoxFinds.None, 0 };
        yield return new object[] { "abc", "abc", RichTextBoxFinds.MatchCase, 0 };
        yield return new object[] { "abc", "ABC", RichTextBoxFinds.MatchCase, -1 };

        yield return new object[] { "aa", "a", RichTextBoxFinds.None, 0 };
        yield return new object[] { "aa", "a", RichTextBoxFinds.Reverse, 1 };
        yield return new object[] { "aa", string.Empty, RichTextBoxFinds.Reverse, -1 };
        yield return new object[] { "abc def", "ef", RichTextBoxFinds.None, 5 };
        yield return new object[] { "abc def", "def", RichTextBoxFinds.None, 4 };
        yield return new object[] { "abc def", " ", RichTextBoxFinds.None, 3 };
        yield return new object[] { "abc def", "ef", RichTextBoxFinds.WholeWord, -1 };
        yield return new object[] { "abc def", "def", RichTextBoxFinds.WholeWord, 4 };
        yield return new object[] { "abc def", " ", RichTextBoxFinds.WholeWord, -1 };

        yield return new object[] { "ab\u0640cd", "abcd", RichTextBoxFinds.None, 0 };
        yield return new object[] { "ab\u0640cd", "\u0640", RichTextBoxFinds.None, 2 };
        yield return new object[] { "ab\u0640cd", "bc", RichTextBoxFinds.None, 1 };
        yield return new object[] { "ab\u0640cd", "abcd", RichTextBoxFinds.NoHighlight, 0 };
        yield return new object[] { "ab\u0640cd", "\u0640", RichTextBoxFinds.NoHighlight, 2 };
        yield return new object[] { "ab\u0640cd", "bc", RichTextBoxFinds.NoHighlight, 1 };
        yield return new object[] { "abcd", "abcd", RichTextBoxFinds.NoHighlight, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Find_String_RichTextBoxFinds_TestData))]
    public void RichTextBox_Find_StringRichTextBoxFinds_ReturnsExpected(string text, string str, RichTextBoxFinds options, int expected)
    {
        using RichTextBox control = new()
        {
            Text = text
        };
        Assert.Equal(expected, control.Find(str, options));
        Assert.True(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Find_String_Int_RichTextBoxFinds_TestData()
    {
        yield return new object[] { string.Empty, string.Empty, 0, RichTextBoxFinds.None, -1 };
        yield return new object[] { string.Empty, "abc", 0, RichTextBoxFinds.None, -1 };
        yield return new object[] { string.Empty, string.Empty, 0, RichTextBoxFinds.Reverse, -1 };
        yield return new object[] { string.Empty, "abc", 0, RichTextBoxFinds.Reverse, -1 };

        yield return new object[] { "abc", string.Empty, 0, RichTextBoxFinds.None, -1 };
        yield return new object[] { "abc", "a", 0, RichTextBoxFinds.None, 0 };
        yield return new object[] { "abc", "a", 0, RichTextBoxFinds.Reverse, 0 };
        yield return new object[] { "abc", "ab", 0, RichTextBoxFinds.None, 0 };
        yield return new object[] { "abc", "abc", 0, RichTextBoxFinds.None, 0 };
        yield return new object[] { "abc", "abcd", 0, RichTextBoxFinds.None, -1 };
        yield return new object[] { "abc", "b", 0, RichTextBoxFinds.None, 1 };
        yield return new object[] { "abc", "d", 0, RichTextBoxFinds.None, -1 };

        yield return new object[] { "abc", "ABC", 0, RichTextBoxFinds.None, 0 };
        yield return new object[] { "abc", "abc", 0, RichTextBoxFinds.MatchCase, 0 };
        yield return new object[] { "abc", "ABC", 0, RichTextBoxFinds.MatchCase, -1 };

        yield return new object[] { "aa", "a", 0, RichTextBoxFinds.None, 0 };
        yield return new object[] { "aa", "a", 0, RichTextBoxFinds.Reverse, 1 };
        yield return new object[] { "aa", string.Empty, 0, RichTextBoxFinds.Reverse, -1 };
        yield return new object[] { "abc def", "ef", 0, RichTextBoxFinds.None, 5 };
        yield return new object[] { "abc def", "def", 0, RichTextBoxFinds.None, 4 };
        yield return new object[] { "abc def", " ", 0, RichTextBoxFinds.None, 3 };
        yield return new object[] { "abc def", "ef", 0, RichTextBoxFinds.WholeWord, -1 };
        yield return new object[] { "abc def", "def", 0, RichTextBoxFinds.WholeWord, 4 };
        yield return new object[] { "abc def", " ", 0, RichTextBoxFinds.WholeWord, -1 };

        yield return new object[] { "abc", "a", 1, RichTextBoxFinds.None, -1 };
        yield return new object[] { "abc", "a", 2, RichTextBoxFinds.None, -1 };
        yield return new object[] { "abc", "c", 2, RichTextBoxFinds.None, 2 };
        yield return new object[] { "abc", "a", 1, RichTextBoxFinds.Reverse, -1 };
        yield return new object[] { "abc", "a", 2, RichTextBoxFinds.Reverse, -1 };
        yield return new object[] { "abc", "c", 2, RichTextBoxFinds.Reverse, 2 };

        yield return new object[] { "ab\u0640cd", "abcd", 0, RichTextBoxFinds.None, 0 };
        yield return new object[] { "ab\u0640cd", "\u0640", 0, RichTextBoxFinds.None, 2 };
        yield return new object[] { "ab\u0640cd", "bc", 0, RichTextBoxFinds.None, 1 };
        yield return new object[] { "ab\u0640cd", "abcd", 0, RichTextBoxFinds.NoHighlight, 0 };
        yield return new object[] { "ab\u0640cd", "\u0640", 0, RichTextBoxFinds.NoHighlight, 2 };
        yield return new object[] { "ab\u0640cd", "bc", 0, RichTextBoxFinds.NoHighlight, 1 };
        yield return new object[] { "abcd", "abcd", 0, RichTextBoxFinds.NoHighlight, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Find_String_Int_RichTextBoxFinds_TestData))]
    public void RichTextBox_Find_StringIntRichTextBoxFinds_ReturnsExpected(string text, string str, int start, RichTextBoxFinds options, int expected)
    {
        using RichTextBox control = new()
        {
            Text = text
        };
        Assert.Equal(expected, control.Find(str, start, options));
        Assert.True(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Find_String_Int_Int_RichTextBoxFinds_TestData()
    {
        foreach (int end in new int[] { -1, 0 })
        {
            yield return new object[] { string.Empty, string.Empty, 0, end, RichTextBoxFinds.None, -1 };
            yield return new object[] { string.Empty, "abc", 0, end, RichTextBoxFinds.None, -1 };
            yield return new object[] { string.Empty, string.Empty, 0, end, RichTextBoxFinds.Reverse, -1 };
            yield return new object[] { string.Empty, "abc", 0, end, RichTextBoxFinds.Reverse, -1 };

            yield return new object[] { "abc", string.Empty, 0, end, RichTextBoxFinds.None, -1 };
            yield return new object[] { "abc", "a", 0, end, RichTextBoxFinds.None, 0 };
            yield return new object[] { "abc", "a", 0, end, RichTextBoxFinds.Reverse, 0 };
            yield return new object[] { "abc", "ab", 0, end, RichTextBoxFinds.None, 0 };
            yield return new object[] { "abc", "abc", 0, end, RichTextBoxFinds.None, 0 };
            yield return new object[] { "abc", "abcd", 0, end, RichTextBoxFinds.None, -1 };
            yield return new object[] { "abc", "b", 0, end, RichTextBoxFinds.None, 1 };
            yield return new object[] { "abc", "d", 0, end, RichTextBoxFinds.None, -1 };

            yield return new object[] { "abc", "ABC", 0, end, RichTextBoxFinds.None, 0 };
            yield return new object[] { "abc", "abc", 0, end, RichTextBoxFinds.MatchCase, 0 };
            yield return new object[] { "abc", "ABC", 0, end, RichTextBoxFinds.MatchCase, -1 };

            yield return new object[] { "aa", "a", 0, end, RichTextBoxFinds.None, 0 };
            yield return new object[] { "aa", "a", 0, end, RichTextBoxFinds.Reverse, 1 };
            yield return new object[] { "abc", string.Empty, 0, end, RichTextBoxFinds.Reverse, -1 };
            yield return new object[] { "abc def", "ef", 0, end, RichTextBoxFinds.None, 5 };
            yield return new object[] { "abc def", "def", 0, end, RichTextBoxFinds.None, 4 };
            yield return new object[] { "abc def", " ", 0, end, RichTextBoxFinds.None, 3 };
            yield return new object[] { "abc def", "ef", 0, end, RichTextBoxFinds.WholeWord, -1 };
            yield return new object[] { "abc def", "def", 0, end, RichTextBoxFinds.WholeWord, 4 };
            yield return new object[] { "abc def", " ", 0, end, RichTextBoxFinds.WholeWord, -1 };

            yield return new object[] { "ab\u0640cd", "abcd", 0, end, RichTextBoxFinds.None, 0 };
            yield return new object[] { "ab\u0640cd", "\u0640", 0, end, RichTextBoxFinds.None, 2 };
            yield return new object[] { "ab\u0640cd", "bc", 0, end, RichTextBoxFinds.None, 1 };
            yield return new object[] { "ab\u0640cd", "abcd", 0, end, RichTextBoxFinds.NoHighlight, 0 };
            yield return new object[] { "ab\u0640cd", "\u0640", 0, end, RichTextBoxFinds.NoHighlight, 2 };
            yield return new object[] { "ab\u0640cd", "bc", 0, end, RichTextBoxFinds.NoHighlight, 1 };
            yield return new object[] { "abcd", "abcd", 0, end, RichTextBoxFinds.NoHighlight, 0 };
        }

        yield return new object[] { "abc", "a", 1, 3, RichTextBoxFinds.None, -1 };
        yield return new object[] { "abc", "a", 2, 3, RichTextBoxFinds.None, -1 };
        yield return new object[] { "abc", "c", 2, 3, RichTextBoxFinds.None, 2 };
        yield return new object[] { "abc", "a", 1, 3, RichTextBoxFinds.Reverse, -1 };
        yield return new object[] { "abc", "a", 2, 3, RichTextBoxFinds.Reverse, -1 };
        yield return new object[] { "abc", "c", 2, 3, RichTextBoxFinds.Reverse, 2 };

        yield return new object[] { "abc", "c", 0, 5, RichTextBoxFinds.None, 2 };
        yield return new object[] { "abc", "c", 0, 2, RichTextBoxFinds.None, -1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Find_String_Int_Int_RichTextBoxFinds_TestData))]
    public void RichTextBox_Find_StringIntIntRichTextBoxFinds_ReturnsExpected(string text, string str, int start, int end, RichTextBoxFinds options, int expected)
    {
        using RichTextBox control = new()
        {
            Text = text
        };
        Assert.Equal(expected, control.Find(str, start, end, options));
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Find_String_TestData))]
    public void RichTextBox_Find_StringWithHandle_ReturnsExpected(string text, string str, int expected)
    {
        using RichTextBox control = new()
        {
            Text = text
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(expected, control.Find(str));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Find_String_RichTextBoxFinds_TestData))]
    public void RichTextBox_Find_StringRichTextBoxFindsWithHandle_ReturnsExpected(string text, string str, RichTextBoxFinds options, int expected)
    {
        using RichTextBox control = new()
        {
            Text = text
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(expected, control.Find(str, options));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Find_String_Int_RichTextBoxFinds_TestData))]
    public void RichTextBox_Find_StringIntRichTextBoxFindsWithHandle_ReturnsExpected(string text, string str, int start, RichTextBoxFinds options, int expected)
    {
        using RichTextBox control = new()
        {
            Text = text
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(expected, control.Find(str, start, options));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Find_String_Int_Int_RichTextBoxFinds_TestData))]
    public void RichTextBox_Find_StringIntIntRichTextBoxFindsWithHandle_ReturnsExpected(string text, string str, int start, int end, RichTextBoxFinds options, int expected)
    {
        using RichTextBox control = new()
        {
            Text = text
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(expected, control.Find(str, start, end, options));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> Find_CharArray_TestData()
    {
        yield return new object[] { string.Empty, Array.Empty<char>(), -1 };
        yield return new object[] { string.Empty, new char[] { 'a', 'b', 'c' }, -1 };

        yield return new object[] { "abc", Array.Empty<char>(), -1 };
        yield return new object[] { "abc", new char[] { 'a' }, 0 };
        yield return new object[] { "abc", new char[] { 'a', 'b' }, 0 };
        yield return new object[] { "abc", new char[] { 'a', 'b', 'c' }, 0 };
        yield return new object[] { "abc", new char[] { 'a', 'b', 'c', 'd' }, 0 };
        yield return new object[] { "abc", new char[] { 'c', 'b', 'a' }, 0 };
        yield return new object[] { "abc", new char[] { 'c', 'b' }, 1 };
        yield return new object[] { "abc", new char[] { 'b' }, 1 };
        yield return new object[] { "abc", new char[] { 'd' }, -1 };
        yield return new object[] { "abc", new char[] { 'A', 'B', 'C' }, -1 };

        yield return new object[] { "aa", new char[] { 'a' }, 0 };
        yield return new object[] { "abc def", new char[] { 'e', 'f' }, 5 };
        yield return new object[] { "abc def", new char[] { 'd', 'e', 'f' }, 4 };
        yield return new object[] { "abc def", new char[] { ' ' }, 3 };

        yield return new object[] { "ab\u0640cd", new char[] { 'a', 'b', 'c', 'd' }, 0 };
        yield return new object[] { "ab\u0640cd", new char[] { '\u0640' }, 2 };
        yield return new object[] { "ab\u0640cd", new char[] { 'b', 'c' }, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Find_CharArray_TestData))]
    public void RichTextBox_Find_CharArray_ReturnsExpected(string text, char[] characterSet, int expected)
    {
        using RichTextBox control = new()
        {
            Text = text
        };
        Assert.Equal(expected, control.Find(characterSet));
        Assert.True(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Find_CharArray_Int_TestData()
    {
        yield return new object[] { string.Empty, Array.Empty<char>(), 0, -1 };
        yield return new object[] { string.Empty, new char[] { 'a', 'b', 'c' }, 0, -1 };

        yield return new object[] { "abc", Array.Empty<char>(), 0, -1 };
        yield return new object[] { "abc", new char[] { 'a' }, 0, 0 };
        yield return new object[] { "abc", new char[] { 'a', 'b' }, 0, 0 };
        yield return new object[] { "abc", new char[] { 'a', 'b', 'c' }, 0, 0 };
        yield return new object[] { "abc", new char[] { 'a', 'b', 'c', 'd' }, 0, 0 };
        yield return new object[] { "abc", new char[] { 'c', 'b', 'a' }, 0, 0 };
        yield return new object[] { "abc", new char[] { 'c', 'b' }, 0, 1 };
        yield return new object[] { "abc", new char[] { 'b' }, 0, 1 };
        yield return new object[] { "abc", new char[] { 'd' }, 0, -1 };
        yield return new object[] { "abc", new char[] { 'A', 'B', 'C' }, 0, -1 };

        yield return new object[] { "aa", new char[] { 'a' }, 0, 0 };
        yield return new object[] { "abc def", new char[] { 'e', 'f' }, 0, 5 };
        yield return new object[] { "abc def", new char[] { 'd', 'e', 'f' }, 0, 4 };
        yield return new object[] { "abc def", new char[] { ' ' }, 0, 3 };

        yield return new object[] { "ab\u0640cd", new char[] { 'a', 'b', 'c', 'd' }, 0, 0 };
        yield return new object[] { "ab\u0640cd", new char[] { '\u0640' }, 0, 2 };
        yield return new object[] { "ab\u0640cd", new char[] { 'b', 'c' }, 0, 1 };

        yield return new object[] { "abc", new char[] { 'a' }, 1, -1 };
        yield return new object[] { "abc", new char[] { 'a' }, 2, -1 };
        yield return new object[] { "abc", new char[] { 'c' }, 2, 2 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Find_CharArray_Int_TestData))]
    public void RichTextBox_Find_CharArrayInt_ReturnsExpected(string text, char[] characterSet, int start, int expected)
    {
        using RichTextBox control = new()
        {
            Text = text
        };
        Assert.Equal(expected, control.Find(characterSet, start));
        Assert.True(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Find_CharArray_Int_Int_TestData()
    {
        foreach (int end in new int[] { -1, 0 })
        {
            yield return new object[] { string.Empty, Array.Empty<char>(), 0, end, -1 };
            yield return new object[] { string.Empty, new char[] { 'a', 'b', 'c' }, 0, end, -1 };

            yield return new object[] { "abc", Array.Empty<char>(), 0, end, -1 };
            yield return new object[] { "abc", new char[] { 'a' }, 0, end, 0 };
            yield return new object[] { "abc", new char[] { 'a', 'b' }, 0, end, 0 };
            yield return new object[] { "abc", new char[] { 'a', 'b', 'c' }, 0, end, 0 };
            yield return new object[] { "abc", new char[] { 'a', 'b', 'c', 'd' }, 0, end, 0 };
            yield return new object[] { "abc", new char[] { 'c', 'b', 'a' }, 0, end, 0 };
            yield return new object[] { "abc", new char[] { 'c', 'b' }, 0, end, 1 };
            yield return new object[] { "abc", new char[] { 'b' }, 0, end, 1 };
            yield return new object[] { "abc", new char[] { 'd' }, 0, end, -1 };
            yield return new object[] { "abc", new char[] { 'A', 'B', 'C' }, 0, end, -1 };

            yield return new object[] { "aa", new char[] { 'a' }, 0, end, 0 };
            yield return new object[] { "abc def", new char[] { 'e', 'f' }, 0, end, 5 };
            yield return new object[] { "abc def", new char[] { 'd', 'e', 'f' }, 0, end, 4 };
            yield return new object[] { "abc def", new char[] { ' ' }, 0, end, 3 };

            yield return new object[] { "ab\u0640cd", new char[] { 'a', 'b', 'c', 'd' }, 0, end, 0 };
            yield return new object[] { "ab\u0640cd", new char[] { '\u0640' }, 0, end, 2 };
            yield return new object[] { "ab\u0640cd", new char[] { 'b', 'c' }, 0, end, 1 };
        }

        yield return new object[] { "abc", new char[] { 'a' }, 1, 3, -1 };
        yield return new object[] { "abc", new char[] { 'a' }, 2, 3, -1 };
        yield return new object[] { "abc", new char[] { 'c' }, 2, 3, 2 };

        yield return new object[] { "abc", new char[] { 'c' }, 0, 5, 2 };
        yield return new object[] { "abc", new char[] { 'c' }, 0, 2, -1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Find_CharArray_Int_Int_TestData))]
    public void RichTextBox_Find_CharArrayIntInt_ReturnsExpected(string text, char[] characterSet, int start, int end, int expected)
    {
        using RichTextBox control = new()
        {
            Text = text
        };
        Assert.Equal(expected, control.Find(characterSet, start, end));
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Find_CharArray_TestData))]
    public void RichTextBox_Find_CharArrayWithHandle_ReturnsExpected(string text, char[] characterSet, int expected)
    {
        using RichTextBox control = new()
        {
            Text = text
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(expected, control.Find(characterSet));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Find_CharArray_Int_TestData))]
    public void RichTextBox_Find_CharArrayIntWithHandle_ReturnsExpected(string text, char[] characterSet, int start, int expected)
    {
        using RichTextBox control = new()
        {
            Text = text
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(expected, control.Find(characterSet, start));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Find_CharArray_Int_Int_TestData))]
    public void RichTextBox_Find_CharArrayIntIntWithHandle_ReturnsExpected(string text, char[] characterSet, int start, int end, int expected)
    {
        using RichTextBox control = new()
        {
            Text = text
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(expected, control.Find(characterSet, start, end));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_Find_NullStrEmpty_ThrowsArgumentNullException()
    {
        using RichTextBox control = new();
        Assert.Throws<ArgumentNullException>("str", () => control.Find((string)null));
        Assert.Throws<ArgumentNullException>("str", () => control.Find(null, RichTextBoxFinds.None));
        Assert.Throws<ArgumentNullException>("str", () => control.Find(null, 0, RichTextBoxFinds.None));
        Assert.Throws<ArgumentNullException>("str", () => control.Find(null, -1, RichTextBoxFinds.None));
        Assert.Throws<ArgumentNullException>("str", () => control.Find(null, 1, RichTextBoxFinds.None));
        Assert.Throws<ArgumentNullException>("str", () => control.Find(null, 0, 0, RichTextBoxFinds.None));
        Assert.Throws<ArgumentNullException>("str", () => control.Find(null, -1, 0, RichTextBoxFinds.None));
        Assert.Throws<ArgumentNullException>("str", () => control.Find(null, 1, 0, RichTextBoxFinds.None));
        Assert.Throws<ArgumentNullException>("str", () => control.Find(null, 0, -2, RichTextBoxFinds.None));
    }

    [WinFormsFact]
    public void RichTextBox_Find_NullStrNotEmpty_ThrowsArgumentNullException()
    {
        using RichTextBox control = new()
        {
            Text = "t"
        };
        Assert.Throws<ArgumentNullException>("str", () => control.Find((string)null));
        Assert.Throws<ArgumentNullException>("str", () => control.Find(null, RichTextBoxFinds.None));
        Assert.Throws<ArgumentNullException>("str", () => control.Find(null, 0, RichTextBoxFinds.None));
        Assert.Throws<ArgumentNullException>("str", () => control.Find(null, -1, RichTextBoxFinds.None));
        Assert.Throws<ArgumentNullException>("str", () => control.Find(null, 2, RichTextBoxFinds.None));
        Assert.Throws<ArgumentNullException>("str", () => control.Find(null, 0, 0, RichTextBoxFinds.None));
        Assert.Throws<ArgumentNullException>("str", () => control.Find(null, -1, 0, RichTextBoxFinds.None));
        Assert.Throws<ArgumentNullException>("str", () => control.Find(null, 2, 0, RichTextBoxFinds.None));
        Assert.Throws<ArgumentNullException>("str", () => control.Find(null, 0, -2, RichTextBoxFinds.None));
    }

    [WinFormsFact]
    public void RichTextBox_Find_NullCharacterSetEmpty_ThrowsArgumentNullException()
    {
        using RichTextBox control = new();
        Assert.Throws<ArgumentNullException>("characterSet", () => control.Find((char[])null));
        Assert.Throws<ArgumentNullException>("characterSet", () => control.Find(null, 0));
        Assert.Throws<ArgumentNullException>("characterSet", () => control.Find(null, -1));
        Assert.Throws<ArgumentNullException>("characterSet", () => control.Find(null, 1));
        Assert.Throws<ArgumentNullException>("characterSet", () => control.Find(null, 0, 0));
        Assert.Throws<ArgumentNullException>("characterSet", () => control.Find(null, -1, 0));
        Assert.Throws<ArgumentNullException>("characterSet", () => control.Find(null, 1, 0));
        Assert.Throws<ArgumentNullException>("characterSet", () => control.Find(null, 0, -2));
    }

    [WinFormsFact]
    public void RichTextBox_Find_NullCharacterSetNotEmpty_ThrowsArgumentNullException()
    {
        using RichTextBox control = new()
        {
            Text = "t"
        };
        Assert.Throws<ArgumentNullException>("characterSet", () => control.Find((char[])null));
        Assert.Throws<ArgumentNullException>("characterSet", () => control.Find(null, 0));
        Assert.Throws<ArgumentNullException>("characterSet", () => control.Find(null, -1));
        Assert.Throws<ArgumentNullException>("characterSet", () => control.Find(null, 2));
        Assert.Throws<ArgumentNullException>("characterSet", () => control.Find(null, 0, 0));
        Assert.Throws<ArgumentNullException>("characterSet", () => control.Find(null, -1, 0));
        Assert.Throws<ArgumentNullException>("characterSet", () => control.Find(null, 2, 0));
        Assert.Throws<ArgumentNullException>("characterSet", () => control.Find(null, 0, -2));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    public void RichTextBox_Find_InvalidStartEmpty_ThrowsArgumentOutOfRangeException(int start)
    {
        using RichTextBox control = new();
        Assert.Throws<ArgumentOutOfRangeException>("start", () => control.Find("s", start, RichTextBoxFinds.NoHighlight));
        Assert.Throws<ArgumentOutOfRangeException>("start", () => control.Find("s", start, 0, RichTextBoxFinds.NoHighlight));
        Assert.Throws<ArgumentOutOfRangeException>("start", () => control.Find(['s'], start));
        Assert.Throws<ArgumentOutOfRangeException>("start", () => control.Find(['s'], start, 0));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(2)]
    public void RichTextBox_Find_InvalidStartNotEmpty_ThrowsArgumentOutOfRangeException(int start)
    {
        using RichTextBox control = new()
        {
            Text = "t"
        };
        Assert.Throws<ArgumentOutOfRangeException>("start", () => control.Find("s", start, RichTextBoxFinds.NoHighlight));
        Assert.Throws<ArgumentOutOfRangeException>("start", () => control.Find("s", start, 0, RichTextBoxFinds.NoHighlight));
        Assert.Throws<ArgumentOutOfRangeException>("start", () => control.Find(['s'], start));
        Assert.Throws<ArgumentOutOfRangeException>("start", () => control.Find(['s'], start, 0));
    }

    [WinFormsFact]
    public void RichTextBox_Find_InvalidEndEmpty_ThrowsArgumentOutOfRangeException()
    {
        using RichTextBox control = new();
        Assert.Throws<ArgumentOutOfRangeException>("end", () => control.Find("s", 0, -2, RichTextBoxFinds.NoHighlight));
        Assert.Throws<ArgumentOutOfRangeException>("end", () => control.Find(['s'], 0, -2));
    }

    [WinFormsFact]
    public void RichTextBox_Find_InvalidEndNotEmpty_ThrowsArgumentOutOfRangeException()
    {
        using RichTextBox control = new()
        {
            Text = "t"
        };
        Assert.Throws<ArgumentOutOfRangeException>("end", () => control.Find("s", 0, -2, RichTextBoxFinds.NoHighlight));
        Assert.Throws<ArgumentOutOfRangeException>("end", () => control.Find(['s'], 0, -2));
    }

    [WinFormsFact]
    public void RichTextBox_Find_StartGreaterThanEnd_ThrowsArgumentOutOfRangeException()
    {
        using RichTextBox control = new()
        {
            Text = "t"
        };
        Assert.Throws<ArgumentException>(() => control.Find("s", 1, 0, RichTextBoxFinds.None));
        Assert.Throws<ArgumentException>(() => control.Find("s", 1, 0, RichTextBoxFinds.Reverse));
        Assert.Throws<ArgumentOutOfRangeException>("end", () => control.Find(['s'], 1, 0));
        Assert.Throws<ArgumentOutOfRangeException>("end", () => control.Find(['s'], 1, 0));
    }

    [WinFormsFact]
    public void RichTextBox_LoadFile_InvokePlainTextEmpty_Success()
    {
        using RichTextBox control = new();
        using MemoryStream stream = new();
        control.LoadFile(stream, RichTextBoxStreamType.PlainText);
        Assert.Empty(control.Text);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_LoadFile_InvokePlainTextNotEmpty_Success()
    {
        using RichTextBox control = new();
        using MemoryStream stream = new();
        byte[] buffer = Encoding.ASCII.GetBytes("Hello World");
        stream.Write(buffer, 0, buffer.Length);
        stream.Position = 0;

        control.LoadFile(stream, RichTextBoxStreamType.PlainText);
        Assert.Equal("Hello World", control.Text);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_LoadFile_InvokePlainTextEmptyWithHandle_Success()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        using MemoryStream stream = new();
        control.LoadFile(stream, RichTextBoxStreamType.PlainText);
        Assert.Empty(control.Text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_LoadFile_InvokePlainTextNotEmptyWithHandle_Success()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        using MemoryStream stream = new();
        byte[] buffer = Encoding.ASCII.GetBytes("Hello World");
        stream.Write(buffer, 0, buffer.Length);
        stream.Position = 0;
        control.LoadFile(stream, RichTextBoxStreamType.PlainText);
        Assert.Equal("Hello World", control.Text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_LoadFile_InvokePlainTextWithRtf_Success()
    {
        using RichTextBox control = new()
        {
            Rtf = "{\\rtf1OldText}"
        };
        using MemoryStream stream = new();
        byte[] buffer = Encoding.ASCII.GetBytes("Hello World");
        stream.Write(buffer, 0, buffer.Length);
        stream.Position = 0;

        control.LoadFile(stream, RichTextBoxStreamType.PlainText);
        Assert.Equal("Hello World", control.Text);
        Assert.DoesNotContain("OldText", control.Rtf);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_LoadFile_InvokeUnicodeTextEmpty_Success()
    {
        using RichTextBox control = new();
        using MemoryStream stream = new();
        control.LoadFile(stream, RichTextBoxStreamType.UnicodePlainText);
        Assert.Empty(control.Text);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_LoadFile_InvokeUnicodeTextNotEmpty_Success()
    {
        using RichTextBox control = new();
        using MemoryStream stream = new();
        byte[] buffer = Encoding.Unicode.GetBytes("Hello World");
        stream.Write(buffer, 0, buffer.Length);
        stream.Position = 0;

        control.LoadFile(stream, RichTextBoxStreamType.UnicodePlainText);
        Assert.Equal("Hello World", control.Text);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_LoadFile_InvokeUnicodeTextEmptyWithHandle_Success()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        using MemoryStream stream = new();
        control.LoadFile(stream, RichTextBoxStreamType.UnicodePlainText);
        Assert.Empty(control.Text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_LoadFile_InvokeUnicodeTextNotEmptyWithHandle_Success()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        using MemoryStream stream = new();
        byte[] buffer = Encoding.Unicode.GetBytes("Hello World");
        stream.Write(buffer, 0, buffer.Length);
        stream.Position = 0;
        control.LoadFile(stream, RichTextBoxStreamType.UnicodePlainText);
        Assert.Equal("Hello World", control.Text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_LoadFile_InvokeUnicodeTextWithRtf_Success()
    {
        using RichTextBox control = new()
        {
            Rtf = "{\\rtf1OldText}"
        };
        using MemoryStream stream = new();
        byte[] buffer = Encoding.Unicode.GetBytes("Hello World");
        stream.Write(buffer, 0, buffer.Length);
        stream.Position = 0;

        control.LoadFile(stream, RichTextBoxStreamType.UnicodePlainText);
        Assert.Equal("Hello World", control.Text);
        Assert.DoesNotContain("OldText", control.Rtf);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_LoadFile_InvokeRichTextNotEmpty_Success()
    {
        using RichTextBox control = new();
        using MemoryStream stream = new();
        byte[] buffer = Encoding.ASCII.GetBytes("{\\rtf1Hello World}");
        stream.Write(buffer, 0, buffer.Length);
        stream.Position = 0;

        control.LoadFile(stream, RichTextBoxStreamType.RichText);
        Assert.Equal("Hello World", control.Text);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_LoadFile_InvokeRichTextWithRtf_Success()
    {
        using RichTextBox control = new()
        {
            Rtf = "{\\rtf1OldText}"
        };
        using MemoryStream stream = new();
        byte[] buffer = Encoding.ASCII.GetBytes("{\\rtf1Hello World}");
        stream.Write(buffer, 0, buffer.Length);
        stream.Position = 0;

        control.LoadFile(stream, RichTextBoxStreamType.RichText);
        Assert.Equal("Hello World", control.Text);
        Assert.DoesNotContain("OldText", control.Rtf);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_LoadFile_NullData_ThrowsArgumentNullException()
    {
        using RichTextBox control = new();
        Assert.Throws<ArgumentNullException>("data", () => control.LoadFile((Stream)null, RichTextBoxStreamType.RichText));
    }

    [WinFormsTheory]
    [InvalidEnumData<RichTextBoxStreamType>]
    public void RichTextBox_LoadFile_InvalidFileType_ThrowsInvalidEnumArgumentException(RichTextBoxStreamType fileType)
    {
        using RichTextBox control = new();
        using MemoryStream stream = new();
        Assert.Throws<InvalidEnumArgumentException>("fileType", () => control.LoadFile(stream, fileType));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(RichTextBoxStreamType.RichNoOleObjs)]
    [InlineData(RichTextBoxStreamType.TextTextOleObjs)]
    public void RichTextBox_LoadFile_UnprocessableFileType_ThrowsArgumentException(RichTextBoxStreamType fileType)
    {
        using RichTextBox control = new();
        using MemoryStream stream = new();
        Assert.Throws<ArgumentException>(() => control.LoadFile(stream, fileType));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_LoadFile_RichTextEmpty_ThrowsArgumentException()
    {
        using RichTextBox control = new();
        using MemoryStream stream = new();
        Assert.Throws<ArgumentException>(() => control.LoadFile(stream, RichTextBoxStreamType.RichText));
        Assert.Empty(control.Text);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_LoadFile_InvalidRtf_ThrowsArgumentException()
    {
        using RichTextBox control = new();
        using MemoryStream stream = new();
        byte[] buffer = Encoding.ASCII.GetBytes("Hello World");
        stream.Write(buffer, 0, buffer.Length);
        stream.Position = 0;

        Assert.Throws<ArgumentException>(() => control.LoadFile(stream, RichTextBoxStreamType.RichText));
    }

    [WinFormsFact]
    public void RichTextBox_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using SubRichTextBox control = new();
        Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void RichTextBox_GetLineFromCharIndex_InvokeEmpty_Success(int index)
    {
        using RichTextBox control = new();
        Assert.Equal(0, control.GetLineFromCharIndex(index));
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(4)]
    [InlineData(5)]
    public void RichTextBox_GetLineFromCharIndex_InvokeNotEmpty_Success(int index)
    {
        using RichTextBox control = new()
        {
            Text = "text"
        };
        Assert.Equal(0, control.GetLineFromCharIndex(index));
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void RichTextBox_GetLineFromCharIndex_InvokeEmptyWithHandle_Success(int index)
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(0, control.GetLineFromCharIndex(index));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(4)]
    [InlineData(5)]
    public void RichTextBox_GetLineFromCharIndex_InvokeNotEmptyWithHandle_Success(int index)
    {
        using RichTextBox control = new()
        {
            Text = "text"
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(0, control.GetLineFromCharIndex(index));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> GetLineFromCharIndex_CustomLineFromChar_TestData()
    {
        yield return new object[] { (IntPtr)(-1) };
        yield return new object[] { IntPtr.Zero };
        yield return new object[] { (IntPtr)1 };
        yield return new object[] { (IntPtr)int.MaxValue };
        yield return new object[] { PARAM.FromLowHigh(1, 2) };
        yield return new object[] { PARAM.FromLowHigh(int.MaxValue, int.MaxValue) };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetLineFromCharIndex_CustomLineFromChar_TestData))]
    public void RichTextBox_GetLineFromCharIndex_CustomLineFromChar_Success(IntPtr result)
    {
        using CustomLineFromCharRichTextBox control = new()
        {
            LineFromCharResult = result
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(0, control.GetLineFromCharIndex(1));
    }

    private class CustomLineFromCharRichTextBox : RichTextBox
    {
        public IntPtr LineFromCharResult { get; set; }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)PInvokeCore.EM_LINEFROMCHAR)
            {
                Assert.Equal(1, m.WParam);
                Assert.Equal(IntPtr.Zero, m.LParam);
                m.Result = LineFromCharResult;
                return;
            }

            base.WndProc(ref m);
        }
    }

    public static IEnumerable<object[]> GetLineFromCharIndex_CustomExLineFromChar_TestData()
    {
        yield return new object[] { (IntPtr)(-1), -1 };
        yield return new object[] { IntPtr.Zero, 0 };
        yield return new object[] { (IntPtr)1, 1 };
        yield return new object[] { (IntPtr)int.MaxValue, 0x7FFFFFFF };
        yield return new object[] { PARAM.FromLowHigh(1, 2), 0x20001 };
        yield return new object[] { PARAM.FromLowHigh(int.MaxValue, int.MaxValue), -1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetLineFromCharIndex_CustomExLineFromChar_TestData))]
    public void RichTextBox_GetLineFromCharIndex_CustomExLineFromChar_Success(IntPtr result, int expected)
    {
        using CustomExLineFromCharRichTextBox control = new()
        {
            ExLineFromCharResult = result
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(expected, control.GetLineFromCharIndex(1));
    }

    private class CustomExLineFromCharRichTextBox : RichTextBox
    {
        public IntPtr ExLineFromCharResult { get; set; }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)PInvokeCore.EM_EXLINEFROMCHAR)
            {
                Assert.Equal(IntPtr.Zero, m.WParam);
                Assert.Equal(1, m.LParam);
                m.Result = ExLineFromCharResult;
                return;
            }

            base.WndProc(ref m);
        }
    }

    [WinFormsTheory]
    [InlineData(ControlStyles.ContainerControl, false)]
    [InlineData(ControlStyles.UserPaint, false)]
    [InlineData(ControlStyles.Opaque, false)]
    [InlineData(ControlStyles.ResizeRedraw, false)]
    [InlineData(ControlStyles.FixedWidth, false)]
    [InlineData(ControlStyles.FixedHeight, false)]
    [InlineData(ControlStyles.StandardClick, false)]
    [InlineData(ControlStyles.Selectable, true)]
    [InlineData(ControlStyles.UserMouse, false)]
    [InlineData(ControlStyles.SupportsTransparentBackColor, false)]
    [InlineData(ControlStyles.StandardDoubleClick, false)]
    [InlineData(ControlStyles.AllPaintingInWmPaint, true)]
    [InlineData(ControlStyles.CacheText, false)]
    [InlineData(ControlStyles.EnableNotifyMessage, false)]
    [InlineData(ControlStyles.DoubleBuffer, false)]
    [InlineData(ControlStyles.OptimizedDoubleBuffer, false)]
    [InlineData(ControlStyles.UseTextForAccessibility, false)]
    [InlineData((ControlStyles)0, true)]
    [InlineData((ControlStyles)int.MaxValue, false)]
    [InlineData((ControlStyles)(-1), false)]
    public void RichTextBox_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
    {
        using SubRichTextBox control = new();
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    [WinFormsFact]
    public void RichTextBox_GetTopLevel_Invoke_ReturnsExpected()
    {
        using SubRichTextBox control = new();
        Assert.False(control.GetTopLevel());
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void RichTextBox_OnBackColorChanged_Invoke_CallsBackColorChanged(EventArgs eventArgs)
    {
        using SubRichTextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.BackColorChanged += handler;
        control.OnBackColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.BackColorChanged -= handler;
        control.OnBackColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void RichTextBox_OnBackColorChanged_InvokeWithHandle_CallsBackColorChanged(EventArgs eventArgs)
    {
        using SubRichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.BackColorChanged += handler;
        control.OnBackColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.BackColorChanged -= handler;
        control.OnBackColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> OnContentsResized_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new ContentsResizedEventArgs(default) };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnContentsResized_TestData))]
    public void RichTextBox_OnContentsResized_Invoke_CallsContentsResized(ContentsResizedEventArgs eventArgs)
    {
        using SubRichTextBox control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int resizeCallCount = 0;
        control.Resize += (sender, e) => resizeCallCount++;
        int callCount = 0;
        ContentsResizedEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.ContentsResized += handler;
        control.OnContentsResized(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, resizeCallCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.ContentsResized -= handler;
        control.OnContentsResized(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, resizeCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void RichTextBox_OnHScroll_Invoke_CallsHScroll(EventArgs eventArgs)
    {
        using SubRichTextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.HScroll += handler;
        control.OnHScroll(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.HScroll -= handler;
        control.OnHScroll(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void RichTextBox_OnImeChange_Invoke_CallsImeChange(EventArgs eventArgs)
    {
        using SubRichTextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.ImeChange += handler;
        control.OnImeChange(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.ImeChange -= handler;
        control.OnImeChange(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnLinkClicked_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new LinkClickedEventArgs(null) };
        yield return new object[] { new LinkClickedEventArgs("") };
        yield return new object[] { new LinkClickedEventArgs("text") };
        yield return new object[] { new LinkClickedEventArgs("", 10, 0) };
        yield return new object[] { new LinkClickedEventArgs("text", 10, 4) };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnLinkClicked_TestData))]
    public void RichTextBox_OnLinkClicked_Invoke_CallsLinkClicked(LinkClickedEventArgs eventArgs)
    {
        using SubRichTextBox control = new();
        int callCount = 0;
        LinkClickedEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.LinkClicked += handler;
        control.OnLinkClicked(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.LinkClicked -= handler;
        control.OnLinkClicked(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> RichTextBox_InvalidLinkClickedEventArgs_TestData()
    {
        yield return new object[] { -1, 0 };
        yield return new object[] { 0, -1 };
        yield return new object[] { int.MaxValue, 1 };
        yield return new object[] { 1, int.MaxValue };
    }

    [WinFormsTheory]
    [MemberData(nameof(RichTextBox_InvalidLinkClickedEventArgs_TestData))]
    public void RichTextBox_InvalidLinkClickedEventArgs_ThrowsArgumentOutOfRangeException(int linkStart, int linkLength)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new LinkClickedEventArgs("text", linkStart, linkLength));
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void RichTextBox_OnProtected_Invoke_CallsProtected(EventArgs eventArgs)
    {
        using SubRichTextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.Protected += handler;
        control.OnProtected(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.Protected -= handler;
        control.OnProtected(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void RichTextBox_OnSelectionChanged_Invoke_CallsSelectionChanged(EventArgs eventArgs)
    {
        using SubRichTextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.SelectionChanged += handler;
        control.OnSelectionChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.SelectionChanged -= handler;
        control.OnSelectionChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void RichTextBox_OnVScroll_Invoke_CallsVScroll(EventArgs eventArgs)
    {
        using SubRichTextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.VScroll += handler;
        control.OnVScroll(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.VScroll -= handler;
        control.OnVScroll(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_Redo_Invoke_Success()
    {
        using RichTextBox control = new();
        control.Redo();
        Assert.True(control.IsHandleCreated);

        // Call again.
        control.Redo();
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_Redo_InvokeWithHandle_Success()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Redo();
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.Redo();
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_SaveFile_InvokePlainTextEmpty_Success()
    {
        using RichTextBox control = new();
        using MemoryStream stream = new();
        control.SaveFile(stream, RichTextBoxStreamType.PlainText);
        Assert.Empty(Encoding.ASCII.GetString(stream.ToArray()));
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SaveFile_InvokePlainTextNotEmpty_Success()
    {
        using RichTextBox control = new()
        {
            Text = "Hello World"
        };

        using MemoryStream stream = new();
        control.SaveFile(stream, RichTextBoxStreamType.PlainText);
        Assert.Empty(Encoding.ASCII.GetString(stream.ToArray()));
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SaveFile_InvokePlainTextEmptyWithHandle_Success()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        using MemoryStream stream = new();
        control.SaveFile(stream, RichTextBoxStreamType.PlainText);
        Assert.Empty(Encoding.ASCII.GetString(stream.ToArray()));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_SaveFile_InvokePlainTextNotEmptyWithHandle_Success()
    {
        using RichTextBox control = new()
        {
            Text = "Hello World"
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        using MemoryStream stream = new();
        control.SaveFile(stream, RichTextBoxStreamType.PlainText);
        Assert.Equal("Hello World", Encoding.ASCII.GetString(stream.ToArray()));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_SaveFile_InvokeUnicodeTextEmpty_Success()
    {
        using RichTextBox control = new();
        using MemoryStream stream = new();
        control.SaveFile(stream, RichTextBoxStreamType.PlainText);
        Assert.Empty(Encoding.Unicode.GetString(stream.ToArray()));
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SaveFile_InvokeUnicodeTextNotEmpty_Success()
    {
        using RichTextBox control = new()
        {
            Text = "Hello World"
        };

        using MemoryStream stream = new();
        control.SaveFile(stream, RichTextBoxStreamType.PlainText);
        Assert.Empty(Encoding.Unicode.GetString(stream.ToArray()));
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SaveFile_InvokeUnicodePlainTextEmptyWithHandle_Success()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        using MemoryStream stream = new();
        control.SaveFile(stream, RichTextBoxStreamType.UnicodePlainText);
        Assert.Empty(Encoding.Unicode.GetString(stream.ToArray()));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_SaveFile_InvokeUnicodePlainTextNotEmptyWithHandle_Success()
    {
        using RichTextBox control = new()
        {
            Text = "Hello World"
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        using MemoryStream stream = new();
        control.SaveFile(stream, RichTextBoxStreamType.UnicodePlainText);
        Assert.Equal("Hello World", Encoding.Unicode.GetString(stream.ToArray()));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_SaveFile_InvokeRichTextEmpty_Success()
    {
        using RichTextBox control = new();
        using MemoryStream stream = new();
        control.SaveFile(stream, RichTextBoxStreamType.RichText);
        string text = Encoding.ASCII.GetString(stream.ToArray());
        Assert.StartsWith("{\\rtf", text);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SaveFile_InvokeRichTextNotEmpty_Success()
    {
        using RichTextBox control = new()
        {
            Text = "Hello World"
        };

        using MemoryStream stream = new();
        control.SaveFile(stream, RichTextBoxStreamType.RichText);
        Assert.Empty(Encoding.ASCII.GetString(stream.ToArray()));
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SaveFile_InvokeRichTextEmptyWithHandle_Success()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        using MemoryStream stream = new();
        control.SaveFile(stream, RichTextBoxStreamType.RichText);
        string text = Encoding.ASCII.GetString(stream.ToArray());
        Assert.StartsWith("{\\rtf", text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_SaveFile_InvokeRichTextNotEmptyWithHandle_Success()
    {
        using RichTextBox control = new()
        {
            Text = "Hello World"
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        using MemoryStream stream = new();
        control.SaveFile(stream, RichTextBoxStreamType.RichText);
        string text = Encoding.ASCII.GetString(stream.ToArray());
        Assert.StartsWith("{\\rtf", text);
        Assert.Contains("Hello World", text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_SaveFile_InvokeTextTextOleObjsEmpty_Success()
    {
        using RichTextBox control = new();
        using MemoryStream stream = new();
        control.SaveFile(stream, RichTextBoxStreamType.TextTextOleObjs);
        Assert.Empty(Encoding.ASCII.GetString(stream.ToArray()));
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SaveFile_InvokeTextTextOleObjsNotEmpty_Success()
    {
        using RichTextBox control = new()
        {
            Text = "Hello World"
        };

        using MemoryStream stream = new();
        control.SaveFile(stream, RichTextBoxStreamType.TextTextOleObjs);
        Assert.Empty(Encoding.ASCII.GetString(stream.ToArray()));
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SaveFile_InvokeTextTextOleObjsEmptyWithHandle_Success()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        using MemoryStream stream = new();
        control.SaveFile(stream, RichTextBoxStreamType.TextTextOleObjs);
        Assert.Empty(Encoding.ASCII.GetString(stream.ToArray()));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_SaveFile_InvokeTextTextOleObjsNotEmptyWithHandle_Success()
    {
        using RichTextBox control = new()
        {
            Text = "Hello World"
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        using MemoryStream stream = new();
        control.SaveFile(stream, RichTextBoxStreamType.TextTextOleObjs);
        Assert.Equal("Hello World", Encoding.ASCII.GetString(stream.ToArray()));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_SaveFile_InvokeRichNoOleObjsNotEmpty_Success()
    {
        using RichTextBox control = new()
        {
            Text = "Hello World"
        };

        using MemoryStream stream = new();
        control.SaveFile(stream, RichTextBoxStreamType.RichNoOleObjs);
        Assert.Empty(Encoding.ASCII.GetString(stream.ToArray()));
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SaveFile_InvokeRichNoOleObjsEmptyWithHandle_Success()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        using MemoryStream stream = new();
        control.SaveFile(stream, RichTextBoxStreamType.RichNoOleObjs);
        string text = Encoding.ASCII.GetString(stream.ToArray());
        Assert.StartsWith("{\\rtf", text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_SaveFile_InvokeRichNoOleObjsNotEmptyWithHandle_Success()
    {
        using RichTextBox control = new()
        {
            Text = "Hello World"
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        using MemoryStream stream = new();
        control.SaveFile(stream, RichTextBoxStreamType.RichNoOleObjs);
        string text = Encoding.ASCII.GetString(stream.ToArray());
        Assert.StartsWith("{\\rtf", text);
        Assert.Contains("Hello World", text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_SaveFile_NullData_Nop()
    {
        using RichTextBox control = new();
        control.SaveFile((Stream)null, RichTextBoxStreamType.RichText);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SaveFile_NullDataWithText_Nop()
    {
        using RichTextBox control = new()
        {
            Text = "Text"
        };
        control.SaveFile((Stream)null, RichTextBoxStreamType.RichText);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SaveFile_NullDataWithHandle_Nop()
    {
        using RichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SaveFile((Stream)null, RichTextBoxStreamType.RichText);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SaveFile_NullDataWithTextWithHandle_Nop()
    {
        using RichTextBox control = new()
        {
            Text = "Text"
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SaveFile((Stream)null, RichTextBoxStreamType.RichText);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InvalidEnumData<RichTextBoxStreamType>]
    public void RichTextBox_SaveFile_InvalidFileType_ThrowsInvalidEnumArgumentException(RichTextBoxStreamType fileType)
    {
        using RichTextBox control = new();
        using MemoryStream stream = new();
        Assert.Throws<InvalidEnumArgumentException>("fileType", () => control.SaveFile(stream, fileType));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void RichTextBox_SetAnsiRtf_DoesNotCorrupt()
    {
        // RichTextBox.Rtf treats input as code page specific (i.e. not Unicode). To see
        // that we're actually using the code page we'll set the text to 0xA0 (160 / nbsp) to see
        // that we can get it back out. If the encoding is not done in the codepage we'll likely
        // get multiple ASCII bytes back out. UTF-8, for example, encodes the .NET UTF-16 0x00A0 (160)
        // to 0xC2 0xA0.

        // Ultimately we should really update RichTextBox to always stream data in Unicode as
        // we're hard-coded to load 4.1 (see RichTextBox.CreateParams).

        using RichTextBox control = new();

        char input = (char)0xA0;
        control.Rtf = $"{{\\rtf1\\ansi {input}}}";

        Span<byte> output = stackalloc byte[16];

        int currentCodePage = (CodePagesEncodingProvider.Instance.GetEncoding(0) ?? Encoding.UTF8).CodePage;

        // The non-lossy conversion of nbsp only works single byte Windows code pages (e.g. not Japanese).
        if (currentCodePage is >= 1250 and <= 1258)
        {
            Assert.Equal(input, control.Text[0]);
        }
    }

    public static IEnumerable<object[]> WndProc_GetDlgCode_TestData()
    {
        yield return new object[] { true, (IntPtr)2 };
        yield return new object[] { false, IntPtr.Zero };
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_GetDlgCode_TestData))]
    public void RichTextBox_WndProc_InvokeGetDlgCodeWithoutHandle_ReturnsExpected(bool acceptsTabs, IntPtr expectedResult)
    {
        using (new NoAssertContext())
        {
            using SubRichTextBox control = new()
            {
                AcceptsTab = acceptsTabs
            };
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_GETDLGCODE,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(expectedResult, m.Result);
            Assert.False(control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_GetDlgCode_TestData))]
    public void RichTextBox_WndProc_InvokeGetDlgCodeWithHandle_ReturnsExpected(bool acceptsTabs, IntPtr expectedResult)
    {
        using SubRichTextBox control = new()
        {
            AcceptsTab = acceptsTabs
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_GETDLGCODE,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(expectedResult, m.Result);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_WndProc_InvokeImeNotifyCallCountWithoutHandle_Success()
    {
        using (new NoAssertContext())
        {
            using SubRichTextBox control = new();
            int imeModeChangedCallCount = 0;
            control.ImeModeChanged += (sender, e) => imeModeChangedCallCount++;
            int imeChangeCallCount = 0;
            control.ImeChange += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                imeChangeCallCount++;
            };
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_IME_NOTIFY,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(0, imeModeChangedCallCount);
            Assert.Equal(1, imeChangeCallCount);
            Assert.False(control.IsHandleCreated);
        }
    }

    [WinFormsFact]
    public void RichTextBox_WndProc_InvokeImeNotifyCallCountWithHandle_Success()
    {
        using SubRichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int imeModeChangedCallCount = 0;
        control.ImeModeChanged += (sender, e) => imeModeChangedCallCount++;
        int imeChangeCallCount = 0;
        control.ImeChange += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            imeChangeCallCount++;
        };
        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_IME_NOTIFY,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(0, imeModeChangedCallCount);
        Assert.Equal(1, imeChangeCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_WndProc_InvokeMouseHoverWithHandle_Success()
    {
        using SubRichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.MouseHover += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_MOUSEHOVER,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> WndProc_ReflectCommand_TestData()
    {
        foreach (IntPtr lParam in new IntPtr[] { IntPtr.Zero, 1 })
        {
            yield return new object[] { IntPtr.Zero, lParam, 0 };
            yield return new object[] { PARAM.FromLowHigh(0, (int)PInvoke.EN_CHANGE), lParam, 1 };
            yield return new object[] { PARAM.FromLowHigh(0, (int)PInvoke.EN_UPDATE), lParam, 0 };
            yield return new object[] { PARAM.FromLowHigh(123, (int)PInvoke.EN_CHANGE), lParam, 1 };
            yield return new object[] { PARAM.FromLowHigh(123, (int)PInvoke.EN_HSCROLL), lParam, 0 };
            yield return new object[] { PARAM.FromLowHigh(123, (int)PInvoke.EN_VSCROLL), lParam, 0 };
            yield return new object[] { PARAM.FromLowHigh(123, 456), lParam, 0 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_ReflectCommand_TestData))]
    public void RichTextBox_WndProc_InvokeReflectCommandWithoutHandle_Success(IntPtr wParam, IntPtr lParam, int expectedTextChangedCallCount)
    {
        using SubRichTextBox control = new();

        int textChangedCallCount = 0;
        control.TextChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            textChangedCallCount++;
        };
        int modifiedCallCount = 0;
        control.ModifiedChanged += (sender, e) => modifiedCallCount++;
        int hScrollCallCount = 0;
        control.HScroll += (sender, e) => hScrollCallCount++;
        int vScrollCallCount = 0;
        control.VScroll += (sender, e) => vScrollCallCount++;
        Message m = new()
        {
            Msg = (int)(MessageId.WM_REFLECT_COMMAND),
            WParam = wParam,
            LParam = lParam,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(250, m.Result);
        Assert.Equal(expectedTextChangedCallCount, textChangedCallCount);
        Assert.Equal(0, hScrollCallCount);
        Assert.Equal(0, vScrollCallCount);
        Assert.Equal(0, modifiedCallCount);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_ReflectCommand_TestData))]
    public void RichTextBox_WndProc_InvokeReflectCommandWithHandle_Success(IntPtr wParam, IntPtr lParam, int expectedTextChangedCallCount)
    {
        using SubRichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int textChangedCallCount = 0;
        control.TextChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            textChangedCallCount++;
        };
        int hScrollCallCount = 0;
        control.HScroll += (sender, e) => hScrollCallCount++;
        int vScrollCallCount = 0;
        control.VScroll += (sender, e) => vScrollCallCount++;
        int modifiedCallCount = 0;
        control.ModifiedChanged += (sender, e) => modifiedCallCount++;
        Message m = new()
        {
            Msg = (int)(MessageId.WM_REFLECT_COMMAND),
            WParam = wParam,
            LParam = lParam,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(250, m.Result);
        Assert.Equal(expectedTextChangedCallCount, textChangedCallCount);
        Assert.Equal(0, hScrollCallCount);
        Assert.Equal(0, vScrollCallCount);
        Assert.Equal(0, modifiedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> WndProc_ReflectCommandWithLParam_TestData()
    {
        yield return new object[] { IntPtr.Zero, 0, 0, 0 };
        yield return new object[] { PARAM.FromLowHigh(0, (int)PInvoke.EN_CHANGE), 1, 0, 0 };
        yield return new object[] { PARAM.FromLowHigh(0, (int)PInvoke.EN_UPDATE), 0, 0, 0 };
        yield return new object[] { PARAM.FromLowHigh(123, (int)PInvoke.EN_CHANGE), 1, 0, 0 };
        yield return new object[] { PARAM.FromLowHigh(123, (int)PInvoke.EN_HSCROLL), 0, 1, 0 };
        yield return new object[] { PARAM.FromLowHigh(123, (int)PInvoke.EN_VSCROLL), 0, 0, 1 };
        yield return new object[] { PARAM.FromLowHigh(123, 456), 0, 0, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_ReflectCommandWithLParam_TestData))]
    public void RichTextBox_WndProc_InvokeReflectCommandWithHandleWithLParam_Success(IntPtr wParam, int expectedTextChangedCallCount, int expectedHScrollCallCount, int expectedVScrollCallCount)
    {
        using SubRichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int textChangedCallCount = 0;
        control.TextChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            textChangedCallCount++;
        };
        int hScrollCallCount = 0;
        control.HScroll += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            hScrollCallCount++;
        };
        int vScrollCallCount = 0;
        control.VScroll += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            vScrollCallCount++;
        };
        int modifiedCallCount = 0;
        control.ModifiedChanged += (sender, e) => modifiedCallCount++;
        Message m = new()
        {
            Msg = (int)(MessageId.WM_REFLECT_COMMAND),
            WParam = wParam,
            LParam = control.Handle,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(250, m.Result);
        Assert.Equal(expectedTextChangedCallCount, textChangedCallCount);
        Assert.Equal(expectedHScrollCallCount, hScrollCallCount);
        Assert.Equal(expectedVScrollCallCount, vScrollCallCount);
        Assert.Equal(0, modifiedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> WndProc_ReflectNotify_TestData()
    {
        foreach (IntPtr hWnd in new IntPtr[] { IntPtr.Zero, 1 })
        {
            yield return new object[] { hWnd, (int)PInvoke.EN_LINK };
            yield return new object[] { hWnd, (int)PInvoke.EN_DROPFILES };
            yield return new object[] { hWnd, (int)PInvoke.EN_REQUESTRESIZE };
            yield return new object[] { hWnd, (int)PInvoke.EN_SELCHANGE };
            yield return new object[] { hWnd, (int)PInvoke.EN_PROTECTED };
            yield return new object[] { hWnd, 0 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_ReflectNotify_TestData))]
    public unsafe void RichTextBox_WndProc_InvokeReflectNotifyWithoutHandle_Success(IntPtr hWnd, int code)
    {
        using SubRichTextBox control = new();

        NMHDR nmhdr = new()
        {
            code = (uint)code
        };

        Message m = new()
        {
            Msg = (int)(MessageId.WM_REFLECT_NOTIFY),
            HWnd = hWnd,
            LParam = (IntPtr)(&nmhdr),
            Result = 250
        };

        control.WndProc(ref m);
        Assert.Equal(0, m.Result);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public unsafe void RichTextBox_WndProc_InvokeReflectNotifyWithHWndDropFiles_Success()
    {
        using SubRichTextBox control = new()
        {
            Text = "text"
        };

        ENDROPFILES dropFiles = new()
        {
            nmhdr = new NMHDR
            {
                code = (int)PInvoke.EN_DROPFILES
            }
        };
        Message m = new()
        {
            Msg = (int)(MessageId.WM_REFLECT_NOTIFY),
            HWnd = control.Handle,
            LParam = (IntPtr)(&dropFiles),
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(1, m.Result);
        Assert.Equal("text", control.Text);
        Assert.True(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> WndProc_ReflectNotifyWithHWndSelChange_TestData()
    {
        yield return new object[] { -1, -1, -1, 0 };
        yield return new object[] { -2, -1, -1, 1 };
        yield return new object[] { 0, -1, -1, 1 };
        yield return new object[] { 1, -1, -1, 1 };
        yield return new object[] { -1, -2, -1, 1 };
        yield return new object[] { -1, 0, -1, 1 };
        yield return new object[] { -1, 1, -1, 1 };
        yield return new object[] { -1, -1, (int)RICH_EDIT_GET_CONTEXT_MENU_SEL_TYPE.SEL_EMPTY, 1 };
        yield return new object[] { -1, -1, (int)RICH_EDIT_GET_CONTEXT_MENU_SEL_TYPE.SEL_TEXT, 1 };
        yield return new object[] { 0, 1, (int)RICH_EDIT_GET_CONTEXT_MENU_SEL_TYPE.SEL_TEXT, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_ReflectNotifyWithHWndSelChange_TestData))]
    public unsafe void RichTextBox_WndProc_InvokeReflectNotifyWithHWndSelChange_Success(int min, int max, int selectionType, int expectedSelectionChangedCallCount)
    {
        using SubRichTextBox control = new();

        int selectionChangedCallCount = 0;
        control.SelectionChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            selectionChangedCallCount++;
        };
        SELCHANGE selChange = new()
        {
            nmhdr = new NMHDR
            {
                code = (int)PInvoke.EN_SELCHANGE
            },
            chrg = new CHARRANGE
            {
                cpMin = min,
                cpMax = max
            },
            seltyp = (RICH_EDIT_GET_CONTEXT_MENU_SEL_TYPE)selectionType
        };
        Message m = new()
        {
            Msg = (int)(MessageId.WM_REFLECT_NOTIFY),
            HWnd = control.Handle,
            LParam = (IntPtr)(&selChange),
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(250, m.Result);
        Assert.Equal(expectedSelectionChangedCallCount, selectionChangedCallCount);
        Assert.Equal(0, control.SelectionLength);
        Assert.Equal(0, control.SelectionStart);
        Assert.True(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> WndProc_ReflectNotifyWithHWndProtected_TestData()
    {
        yield return new object[] { 0, (IntPtr)1, 1 };
        yield return new object[] { CFM_MASK.CFM_ALLCAPS, (IntPtr)1, 1 };
        yield return new object[] { CFM_MASK.CFM_PROTECTED, IntPtr.Zero, 0 };
        yield return new object[] { CFM_MASK.CFM_PROTECTED | CFM_MASK.CFM_ALLCAPS, IntPtr.Zero, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_ReflectNotifyWithHWndProtected_TestData))]
    public unsafe void RichTextBox_WndProc_InvokeReflectNotifyWithHWndProtected_Success(int mask, IntPtr expectedResult, int expectedProtectedCallCount)
    {
        using SubRichTextBox control = new();
        int protectedCallCount = 0;
        control.Protected += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            protectedCallCount++;
        };

        var format = new CHARFORMAT2W
        {
            dwMask = (CFM_MASK)mask
        };
        IntPtr ptr = Marshal.AllocCoTaskMem(100);
        try
        {
            Marshal.WriteInt32(ptr, IntPtr.Size * 2, (int)PInvoke.EN_PROTECTED);
            Marshal.WriteInt32(ptr, IntPtr.Size * 2 + IntPtr.Size, (int)PInvokeCore.EM_SETCHARFORMAT);
            Marshal.WriteIntPtr(ptr, IntPtr.Size * 2 + IntPtr.Size + 4 + IntPtr.Size, (IntPtr)(&format));
            Message m = new()
            {
                Msg = (int)(MessageId.WM_REFLECT_NOTIFY),
                HWnd = control.Handle,
                LParam = ptr,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(expectedResult, m.Result);
            Assert.Equal(expectedProtectedCallCount, protectedCallCount);
            Assert.True(control.IsHandleCreated);
        }
        finally
        {
            Marshal.FreeCoTaskMem(ptr);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_ReflectNotify_TestData))]
    public unsafe void RichTextBox_WndProc_InvokeReflectNotifyWithHandle_Success(IntPtr hWnd, int code)
    {
        using SubRichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        NMHDR nmhdr = new()
        {
            code = (uint)code
        };

        Message m = new()
        {
            Msg = (int)(MessageId.WM_REFLECT_NOTIFY),
            HWnd = hWnd,
            LParam = (IntPtr)(&nmhdr),
            Result = 250
        };

        control.WndProc(ref m);
        Assert.Equal(0, m.Result);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public unsafe void RichTextBox_WndProc_InvokeReflectNotifyWithHandleWithHWndDropFiles_Success()
    {
        using SubRichTextBox control = new()
        {
            Text = "text"
        };

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        ENDROPFILES dropFiles = new()
        {
            nmhdr = new NMHDR
            {
                code = (int)PInvoke.EN_DROPFILES
            }
        };
        Message m = new()
        {
            Msg = (int)(MessageId.WM_REFLECT_NOTIFY),
            HWnd = control.Handle,
            LParam = (IntPtr)(&dropFiles),
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(1, m.Result);
        Assert.Equal("text", control.Text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> WndProc_ReflectNotifyWithHwndRequestResize_TestData()
    {
        yield return new object[] { BorderStyle.Fixed3D, Rectangle.Empty, new Rectangle(0, 0, 0, 1) };
        yield return new object[] { BorderStyle.FixedSingle, Rectangle.Empty, Rectangle.Empty };
        yield return new object[] { BorderStyle.None, Rectangle.Empty, Rectangle.Empty };

        yield return new object[] { BorderStyle.Fixed3D, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 5) };
        yield return new object[] { BorderStyle.FixedSingle, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4) };
        yield return new object[] { BorderStyle.None, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4) };
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_ReflectNotifyWithHwndRequestResize_TestData))]
    public unsafe void RichTextBox_WndProc_InvokeReflectNotifyWithHandleWithHWndRequestResize_Success(BorderStyle borderStyle, Rectangle result, Rectangle expected)
    {
        using SubRichTextBox control = new()
        {
            BorderStyle = borderStyle
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int contentsResizedCallCount = 0;
        control.ContentsResized += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(expected, e.NewRectangle);
            contentsResizedCallCount++;
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int resizeCallCount = 0;
        control.Resize += (sender, e) => resizeCallCount++;
        REQRESIZE reqResize = new()
        {
            nmhdr = new NMHDR
            {
                code = (int)PInvoke.EN_REQUESTRESIZE
            },
            rc = result
        };
        Message m = new()
        {
            Msg = (int)(MessageId.WM_REFLECT_NOTIFY),
            HWnd = control.Handle,
            LParam = (IntPtr)(&reqResize),
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(250, m.Result);
        Assert.Equal(1, contentsResizedCallCount);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, resizeCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_ReflectNotifyWithHWndSelChange_TestData))]
    public unsafe void RichTextBox_WndProc_InvokeReflectNotifyWithHandleWithHWndSelChange_Success(int min, int max, int selectionType, int expectedSelectionChangedCallCount)
    {
        using SubRichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int selectionChangedCallCount = 0;
        control.SelectionChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            selectionChangedCallCount++;
        };
        SELCHANGE selChange = new()
        {
            nmhdr = new NMHDR
            {
                code = (int)PInvoke.EN_SELCHANGE
            },
            chrg = new CHARRANGE
            {
                cpMin = min,
                cpMax = max
            },
            seltyp = (RICH_EDIT_GET_CONTEXT_MENU_SEL_TYPE)selectionType
        };
        Message m = new()
        {
            Msg = (int)(MessageId.WM_REFLECT_NOTIFY),
            HWnd = control.Handle,
            LParam = (IntPtr)(&selChange),
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(250, m.Result);
        Assert.Equal(expectedSelectionChangedCallCount, selectionChangedCallCount);
        Assert.Equal(0, control.SelectionLength);
        Assert.Equal(0, control.SelectionStart);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_ReflectNotifyWithHWndProtected_TestData))]
    public unsafe void RichTextBox_WndProc_InvokeReflectNotifyWithHandleWithHWndProtected_Success(int mask, IntPtr expectedResult, int expectedProtectedCallCount)
    {
        using SubRichTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int protectedCallCount = 0;
        control.Protected += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            protectedCallCount++;
        };

        var format = new CHARFORMAT2W
        {
            dwMask = (CFM_MASK)mask
        };
        IntPtr ptr = Marshal.AllocCoTaskMem(100);
        try
        {
            Marshal.WriteInt32(ptr, IntPtr.Size * 2, (int)PInvoke.EN_PROTECTED);
            Marshal.WriteInt32(ptr, IntPtr.Size * 2 + IntPtr.Size, (int)PInvokeCore.EM_SETCHARFORMAT);
            Marshal.WriteIntPtr(ptr, IntPtr.Size * 2 + IntPtr.Size + 4 + IntPtr.Size, (IntPtr)(&format));
            Message m = new()
            {
                Msg = (int)(MessageId.WM_REFLECT_NOTIFY),
                HWnd = control.Handle,
                LParam = ptr,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(expectedResult, m.Result);
            Assert.Equal(expectedProtectedCallCount, protectedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            Marshal.FreeCoTaskMem(ptr);
        }
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void RichTextBox_WndProc_InvokeSetFontWithoutHandle_ReturnsExpected(bool multiline, int expectedMargin)
    {
        using (new NoAssertContext())
        {
            using SubRichTextBox control = new()
            {
                Multiline = multiline
            };
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_SETFONT,
                Result = 250
            };
            int textChangedCallCount = 0;
            control.TextChanged += (sender, e) => textChangedCallCount++;
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, textChangedCallCount);
            IntPtr result = PInvokeCore.SendMessage(control, PInvokeCore.EM_GETMARGINS);
            Assert.Equal(expectedMargin, PARAM.HIWORD(result));
            Assert.Equal(expectedMargin, PARAM.LOWORD(result));
        }
    }

    [WinFormsTheory]
    [InlineData(true, 1, 2)]
    [InlineData(false, 0, 0)]
    public void RichTextBox_WndProc_InvokeSetFontWithHandle_ReturnsExpected(bool multiline, int expectedLeft, int expectedRight)
    {
        using SubRichTextBox control = new()
        {
            Multiline = multiline
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        PInvokeCore.SendMessage(control, PInvokeCore.EM_SETMARGINS, (WPARAM)(PInvoke.EC_LEFTMARGIN | PInvoke.EC_RIGHTMARGIN), LPARAM.MAKELPARAM(1, 2));
        int textChangedCallCount = 0;
        control.TextChanged += (sender, e) => textChangedCallCount++;

        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_SETFONT,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(0, textChangedCallCount);
        IntPtr result = PInvokeCore.SendMessage(control, PInvokeCore.EM_GETMARGINS);
        Assert.Equal(expectedLeft, PARAM.LOWORD(result));
        Assert.Equal(expectedRight, PARAM.HIWORD(result));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void RichTextBox_CheckDefaultNativeControlVersions()
    {
        using RichTextBox control = new();
        control.CreateControl();

        Assert.Contains("RICHEDIT50W", GetClassName(control.HWND), StringComparison.Ordinal);
    }

    [WinFormsFact]
    public void RichTextBox_CheckRichEditWithVersionCanCreateOldVersions()
    {
        using (RichEdit riched32 = new())
        {
            riched32.CreateControl();
            Assert.Contains(".RichEdit.", GetClassName(riched32.HWND), StringComparison.Ordinal);
        }

        using var riched20 = new RichEdit20W();
        riched20.CreateControl();
        Assert.Contains(".RichEdit20W.", GetClassName(riched20.HWND), StringComparison.Ordinal);

        string rtfString = @"{\rtf1\ansi{" +
            @"The next line\par " +
            @"is {\v ###NOT### }hidden\par in plain text!}}";

        riched20.Rtf = rtfString;

        using RichTextBox richTextBox = new();
        richTextBox.CreateControl();
        richTextBox.Rtf = rtfString;

        Assert.Equal(riched20.TextLength, richTextBox.TextLength);
        Assert.Equal(riched20.Text, richTextBox.Text);
        Assert.Equal(richTextBox.Text.Length, richTextBox.TextLength);

        int startOfIs = riched20.Text.IndexOf("is", StringComparison.Ordinal);
        int endOfHidden = riched20.Text.IndexOf("hidden", StringComparison.Ordinal) + "hidden".Length;
        richTextBox.Select(startOfIs, endOfHidden - startOfIs);
        Assert.Equal("is ###NOT### hidden", richTextBox.SelectedText);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void RichTextBox_OnGotFocus_RaisesAutomationNotification_WithText(EventArgs eventArgs)
    {
        Mock<Control> mockParent = new() { CallBase = true };
        Mock<Control.ControlAccessibleObject> mockAccessibleObject = new(MockBehavior.Strict, mockParent.Object);
        mockAccessibleObject
            .Setup(a => a.InternalRaiseAutomationNotification(
                It.IsAny<AutomationNotificationKind>(),
                It.IsAny<AutomationNotificationProcessing>(),
                It.IsAny<string>()))
            .Returns(true)
            .Verifiable();
        mockParent.Protected().Setup<AccessibleObject>("CreateAccessibilityInstance")
            .Returns(mockAccessibleObject.Object);

        using Control parent = mockParent.Object;
        string richTextBoxContent = "RichTextBox";
        using SubRichTextBox control = new()
        {
            Parent = parent,
            Text = richTextBoxContent,
        };

        // Enforce accessible object creation
        Assert.Equal(mockAccessibleObject.Object, parent.AccessibilityObject);

        control.OnGotFocus(eventArgs);

        mockAccessibleObject.Verify(a => a.InternalRaiseAutomationNotification(
            AutomationNotificationKind.Other,
            AutomationNotificationProcessing.MostRecent,
            richTextBoxContent),
            Times.Once);
    }

    [WinFormsTheory]
    [InlineData(-1, -1, 0)]
    [InlineData(0, 0, 0)]
    [InlineData(10, 10, 1)]
    public void RichTextBox_GetCharIndexFromPosition_Invoke_ReturnsExpected(int x, int y, int expectedIndex)
    {
        using RichTextBox richTextBox1 = new();
        richTextBox1.Text = "Hello, World!";

        Assert.NotEqual(0, richTextBox1.Handle);
        int invalidatedCallCount = 0;
        richTextBox1.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        richTextBox1.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        richTextBox1.HandleCreated += (sender, e) => createdCallCount++;

        Point pt = new(x, y);
        int index = richTextBox1.GetCharIndexFromPosition(pt);

        Assert.Equal(expectedIndex, index);
        Assert.True(richTextBox1.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    // DrawToBitmap doesn't work for this control, so we should hide it.
    // We'll still call base so that this has a chance to work if it can.
    [WinFormsFact]
    public void RichTextBox_DrawToBitmap_Invoke_Success()
    {
        using Bitmap bitmap1 = new(10, 10);
        using RichTextBox richTextBox1 = new();
        richTextBox1.DrawToBitmap(bitmap1, new Rectangle(0, 0, 10, 10));

        bitmap1.Width.Should().Be(10);
        bitmap1.Height.Should().Be(10);
    }

    [WinFormsFact]
    public void RichTextBox_SaveFilePath_Invoke_Success()
    {
        using RichTextBox richTextBox1 = new()
        {
            Rtf = @"{\rtf1\ansi{Sample for {\v HIDDEN }text}}"
        };
        using RichTextBox richTextBox2 = new();

        string fileName = "SaveRichTextBox.rtf";
        string projectDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..");
        string filePath = $"{projectDirectory}/src/System.Windows.Forms/tests/UnitTests/TestResources/Files/{fileName}";

        try
        {
            richTextBox1.SaveFile(filePath);
            richTextBox2.LoadFile(filePath);
            int startOfSample = richTextBox2.Text.IndexOf("Sample", StringComparison.Ordinal);
            int endOfText = richTextBox2.Text.IndexOf("text", StringComparison.Ordinal) + "text".Length;
            richTextBox2.Select(startOfSample, endOfText - startOfSample);

            richTextBox2.Rtf.Should().NotBeNullOrEmpty();
            richTextBox2.SelectedText.Should().Be("Sample for HIDDEN text");
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [WinFormsTheory]
    [InlineData(RichTextBoxStreamType.RichText)]
    [InlineData(RichTextBoxStreamType.PlainText)]
    [InlineData(RichTextBoxStreamType.UnicodePlainText)]
    [InlineData(RichTextBoxStreamType.RichNoOleObjs)]
    [InlineData(RichTextBoxStreamType.TextTextOleObjs)]
    public void RichTextBox_SaveFile_Invoke_Success(RichTextBoxStreamType fileType)
    {
        using RichTextBox richTextBox1 = new();

        string projectDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..");
        string filePath = Path.Combine(projectDirectory, "src", "System.Windows.Forms", "tests", "UnitTests", "TestResources", "Files", "Test");

        try
        {
            richTextBox1.SaveFile(filePath, fileType);
            File.Exists(filePath).Should().BeTrue();
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [WinFormsFact]
    public void RichTextBox_DragDropEvent_AddRemove_Success()
    {
        using SubRichTextBox richTextBox1 = new();
        int callCount = 0;
        DragEventHandler handler = (sender, e) =>
        {
            sender.Should().Be(richTextBox1);
            callCount++;
        };

        DragEventArgs dragEventArgs = new DragEventArgs(
            data: null,
            keyState: 0,
            x: 0,
            y: 0,
            allowedEffect: DragDropEffects.None,
            effect: DragDropEffects.None);

        richTextBox1.DragDrop += handler;
        richTextBox1.OnDragDrop(dragEventArgs);
        callCount.Should().Be(1);

        richTextBox1.DragDrop -= handler;
        richTextBox1.OnDragDrop(dragEventArgs);
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void RichTextBox_DragEnterEvent_AddRemove_Success()
    {
        using SubRichTextBox richTextBox1 = new();
        int callCount = 0;
        DragEventHandler handler = (sender, e) =>
        {
            sender.Should().Be(richTextBox1);
            callCount++;
        };

        DragEventArgs dragEventArgs = new DragEventArgs(
            data: null,
            keyState: 0,
            x: 0,
            y: 0,
            allowedEffect: DragDropEffects.None,
            effect: DragDropEffects.None);

        richTextBox1.DragEnter += handler;
        richTextBox1.OnDragDrop(dragEventArgs);
        callCount.Should().Be(0);

        richTextBox1.DragEnter -= handler;
        richTextBox1.OnDragDrop(dragEventArgs);
        callCount.Should().Be(0);
    }

    [WinFormsFact]
    public void RichTextBox_DragLeaveEvent_AddRemove_Success()
    {
        using SubRichTextBox richTextBox1 = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(richTextBox1);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };

        richTextBox1.DragLeave += handler;
        richTextBox1.OnDragLeave(EventArgs.Empty);
        callCount.Should().Be(1);

        richTextBox1.DragLeave -= handler;
        richTextBox1.OnDragLeave(EventArgs.Empty);
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void RichTextBox_DragOverEvent_AddRemove_Success()
    {
        using SubRichTextBox richTextBox1 = new();
        int callCount = 0;
        DragEventHandler handler = (sender, e) =>
        {
            sender.Should().Be(richTextBox1);
            callCount++;
        };

        DragEventArgs dragEventArgs = new DragEventArgs(
            data: null,
            keyState: 0,
            x: 0,
            y: 0,
            allowedEffect: DragDropEffects.None,
            effect: DragDropEffects.None);

        richTextBox1.DragOver += handler;
        richTextBox1.OnDragOver(dragEventArgs);
        callCount.Should().Be(1);

        richTextBox1.DragOver -= handler;
        richTextBox1.OnDragOver(dragEventArgs);
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void RichTextBox_GiveFeedbackEvent_AddRemove_Success()
    {
        using SubRichTextBox richTextBox1 = new();
        int callCount = 0;
        GiveFeedbackEventHandler handler = (sender, e) =>
        {
            sender.Should().Be(richTextBox1);
            callCount++;
        };

        GiveFeedbackEventArgs giveFeedbackEventArgs = new(DragDropEffects.None, useDefaultCursors: true);

        richTextBox1.GiveFeedback += handler;
        richTextBox1.OnGiveFeedback(giveFeedbackEventArgs);
        callCount.Should().Be(1);

        richTextBox1.GiveFeedback -= handler;
        richTextBox1.OnGiveFeedback(giveFeedbackEventArgs);
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void RichTextBox_QueryContinueDragEvent_AddRemove_Success()
    {
        using SubRichTextBox richTextBox1 = new();
        int callCount = 0;
        QueryContinueDragEventHandler handler = (sender, e) =>
        {
            sender.Should().Be(richTextBox1);
            callCount++;
        };

        QueryContinueDragEventArgs queryContinueDragEventArgs = new(keyState: 0, escapePressed: true, action: DragAction.Continue);

        richTextBox1.QueryContinueDrag += handler;
        richTextBox1.OnQueryContinueDrag(queryContinueDragEventArgs);
        callCount.Should().Be(1);

        richTextBox1.QueryContinueDrag -= handler;
        richTextBox1.OnQueryContinueDrag(queryContinueDragEventArgs);
        callCount.Should().Be(1);
    }

    private class CustomGetParaFormatRichTextBox : RichTextBox
    {
        public bool MakeCustom { get; set; }
        public IntPtr ExpectedWParam { get; set; }
        public PARAFORMAT GetParaFormatResult { get; set; }

        protected override unsafe void WndProc(ref Message m)
        {
            if (MakeCustom && m.Msg == (int)PInvokeCore.EM_GETPARAFORMAT)
            {
                PARAFORMAT* format = (PARAFORMAT*)m.LParam;
                Assert.Equal(IntPtr.Zero, m.WParam);
                Assert.NotEqual(IntPtr.Zero, m.LParam);
                *format = GetParaFormatResult;
                return;
            }

            base.WndProc(ref m);
        }
    }

    private class CustomGetCharFormatRichTextBox : RichTextBox
    {
        public bool MakeCustom { get; set; }
        public IntPtr ExpectedWParam { get; set; }
        public CHARFORMAT2W GetCharFormatResult { get; set; }

        protected override unsafe void WndProc(ref Message m)
        {
            if (MakeCustom && m.Msg == (int)PInvokeCore.EM_GETCHARFORMAT)
            {
                CHARFORMAT2W* format = (CHARFORMAT2W*)m.LParam;
                Assert.Equal(ExpectedWParam, m.WParam);
                Assert.NotEqual(IntPtr.Zero, m.LParam);
                *format = GetCharFormatResult;
                return;
            }

            base.WndProc(ref m);
        }
    }

    private class CantCreateHandleRichTextBox : RichTextBox
    {
        protected override void CreateHandle()
        {
            // Nop.
        }
    }

    private class SubRichTextBox : RichTextBox
    {
        public new bool CanEnableIme => base.CanEnableIme;

        public new bool CanRaiseEvents => base.CanRaiseEvents;

        public new CreateParams CreateParams => base.CreateParams;

        public new Cursor DefaultCursor => base.DefaultCursor;

        public new ImeMode DefaultImeMode => base.DefaultImeMode;

        public new Padding DefaultMargin => base.DefaultMargin;

        public new Size DefaultMaximumSize => base.DefaultMaximumSize;

        public new Size DefaultMinimumSize => base.DefaultMinimumSize;

        public new Padding DefaultPadding => base.DefaultPadding;

        public new Size DefaultSize => base.DefaultSize;

        public new bool DesignMode => base.DesignMode;

        public new bool DoubleBuffered
        {
            get => base.DoubleBuffered;
            set => base.DoubleBuffered = value;
        }

        public new EventHandlerList Events => base.Events;

        public new int FontHeight
        {
            get => base.FontHeight;
            set => base.FontHeight = value;
        }

        public new ImeMode ImeModeBase
        {
            get => base.ImeModeBase;
            set => base.ImeModeBase = value;
        }

        public new bool ResizeRedraw
        {
            get => base.ResizeRedraw;
            set => base.ResizeRedraw = value;
        }

        public new bool ShowFocusCues => base.ShowFocusCues;

        public new bool ShowKeyboardCues => base.ShowKeyboardCues;

        public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

        public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

        public new bool GetTopLevel() => base.GetTopLevel();

        public new void OnBackColorChanged(EventArgs e) => base.OnBackColorChanged(e);

        public new void OnContentsResized(ContentsResizedEventArgs e) => base.OnContentsResized(e);

        public new void OnGotFocus(EventArgs e) => base.OnGotFocus(e);

        public new void OnHScroll(EventArgs e) => base.OnHScroll(e);

        public new void OnImeChange(EventArgs e) => base.OnImeChange(e);

        public new void OnLinkClicked(LinkClickedEventArgs e) => base.OnLinkClicked(e);

        public new void OnProtected(EventArgs e) => base.OnProtected(e);

        public new void OnSelectionChanged(EventArgs e) => base.OnSelectionChanged(e);

        public new void OnVScroll(EventArgs e) => base.OnVScroll(e);

        public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);

        public new void WndProc(ref Message m) => base.WndProc(ref m);

        public new void OnDragDrop(DragEventArgs e) => base.OnDragDrop(e);

        public new void OnDragLeave(EventArgs e) => base.OnDragLeave(e);

        public new void OnDragOver(DragEventArgs e) => base.OnDragOver(e);

        public new void OnGiveFeedback(GiveFeedbackEventArgs e) => base.OnGiveFeedback(e);

        public new void OnQueryContinueDrag(QueryContinueDragEventArgs e) => base.OnQueryContinueDrag(e);
    }

    private static unsafe string GetClassName(HWND hWnd)
    {
        int length = 0;
        Span<char> buffer = stackalloc char[PInvokeCore.MaxClassName];
        fixed (char* lpClassName = buffer)
        {
            length = PInvoke.GetClassName(hWnd, lpClassName, buffer.Length);
        }

        return new string(buffer[..length]);
    }

    /// <summary>
    /// Represents RichEdit 1.0 control.
    /// </summary>
    private class RichEdit : RichEditWithVersion
    {
        protected override string NativeDll => "riched32.dll";
        protected override string WindowClassName => "RichEdit";
    }

    /// <summary>
    /// Represents RichEdit 2.0 control.
    /// </summary>
    private class RichEdit20W : RichEditWithVersion
    {
        protected override string NativeDll => "riched20.dll";
        protected override string WindowClassName => "RichEdit20W";
    }

    private abstract class RichEditWithVersion : RichTextBox
    {
        // NOTE: Do not unload the library once it is loaded!
        // To prevent race conditions where one thread unloads the library
        // while another thread instantiates the control.
        private IntPtr _nativeDllHandle = IntPtr.Zero;

        protected abstract string NativeDll { get; }
        protected abstract string WindowClassName { get; }

        protected override CreateParams CreateParams
        {
            get
            {
                // This code is copied and adapted from the original RichTextBox implemenation
                // https://referencesource.microsoft.com/#System.Windows.Forms/winforms/Managed/System/WinForms/RichTextBox.cs,357

                if (_nativeDllHandle == IntPtr.Zero)
                {
                    _nativeDllHandle = PInvoke.LoadLibraryFromSystemPathIfAvailable(NativeDll);

                    int lastWin32Error = Marshal.GetLastWin32Error();
                    if ((ulong)_nativeDllHandle < 32)
                    {
                        throw new Win32Exception(lastWin32Error, $"Failed to load '{NativeDll}'");
                    }
                }

                CreateParams cp = base.CreateParams;

                cp.ClassName = WindowClassName;

                return cp;
            }
        }
    }
}
