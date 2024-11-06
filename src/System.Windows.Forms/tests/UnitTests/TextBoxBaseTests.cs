// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.TestUtilities;
using Moq;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests;

public partial class TextBoxBaseTests
{
    private static readonly int s_preferredHeight = Control.DefaultFont.Height + SystemInformation.BorderSize.Height * 4 + 3;

    [WinFormsFact]
    public void TextBoxBase_CreateParams_GetDefault_ReturnsExpected()
    {
        using SubTextBox control = new();
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("Edit", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0x200, createParams.ExStyle);
        Assert.Equal(control.PreferredHeight, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(0x560100C0, createParams.Style);
        Assert.Equal(100, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 0x560100C0)]
    [InlineData(false, 0x560101C0)]
    public void TextBoxBase_CreateParams_GetHideSelection_ReturnsExpected(bool hideSelection, int expectedStyle)
    {
        using SubTextBox control = new()
        {
            HideSelection = hideSelection
        };

        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("Edit", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0x200, createParams.ExStyle);
        Assert.Equal(control.PreferredHeight, createParams.Height);
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
    [InlineData(true, 0x560108C0)]
    [InlineData(false, 0x560100C0)]
    public void TextBoxBase_CreateParams_GetReadOnly_ReturnsExpected(bool readOnly, int expectedStyle)
    {
        using SubTextBox control = new()
        {
            ReadOnly = readOnly
        };

        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("Edit", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0x200, createParams.ExStyle);
        Assert.Equal(control.PreferredHeight, createParams.Height);
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
    [InlineData(BorderStyle.None, 0x560100C0, 0)]
    [InlineData(BorderStyle.Fixed3D, 0x560100C0, 0x00200)]
    [InlineData(BorderStyle.FixedSingle, 0x568100C0, 0)]
    public void TextBoxBase_CreateParams_GetBorderStyle_ReturnsExpected(BorderStyle borderStyle, int expectedStyle, int expectedExStyle)
    {
        using SubTextBox control = new()
        {
            BorderStyle = borderStyle
        };

        CreateParams createParams = control.CreateParams;
        Assert.Equal("Edit", createParams.ClassName);
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
    [InlineData(true, true, 0x56010044)]
    [InlineData(true, false, 0x560100C4)]
    [InlineData(false, true, 0x560100C0)]
    [InlineData(false, false, 0x560100C0)]
    public void TextBoxBase_CreateParams_GetMultiline_ReturnsExpected(bool multiline, bool wordWrap, int expectedStyle)
    {
        using SubTextBox control = new()
        {
            Multiline = multiline,
            WordWrap = wordWrap
        };

        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("Edit", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0x200, createParams.ExStyle);
        Assert.Equal(control.PreferredHeight, createParams.Height);
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
    public void TextBoxBase_AcceptsTab_Set_GetReturnsExpected(bool value)
    {
        using TextBox control = new()
        {
            AcceptsTab = value
        };
        Assert.Equal(value, control.AcceptsTab);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AcceptsTab = value;
        Assert.Equal(value, control.AcceptsTab);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.AcceptsTab = !value;
        Assert.Equal(!value, control.AcceptsTab);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBase_AcceptsTab_SetWithHandler_CallsAcceptsTabChanged()
    {
        using TextBox control = new()
        {
            AcceptsTab = true
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.AcceptsTabChanged += handler;

        // Set different.
        control.AcceptsTab = false;
        Assert.False(control.AcceptsTab);
        Assert.Equal(1, callCount);

        // Set same.
        control.AcceptsTab = false;
        Assert.False(control.AcceptsTab);
        Assert.Equal(1, callCount);

        // Set different.
        control.AcceptsTab = true;
        Assert.True(control.AcceptsTab);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.AcceptsTabChanged -= handler;
        control.AcceptsTab = false;
        Assert.False(control.AcceptsTab);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void TextBoxBase_AutoSize_Set_GetReturnsExpected(bool value)
    {
        using SubTextBox control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.Equal(s_preferredHeight, control.Height);
        Assert.Equal(value, control.GetStyle(ControlStyles.FixedHeight));
        Assert.False(control.Multiline);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.Equal(s_preferredHeight, control.Height);
        Assert.Equal(value, control.GetStyle(ControlStyles.FixedHeight));
        Assert.False(control.Multiline);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.AutoSize = !value;
        Assert.Equal(!value, control.AutoSize);
        Assert.Equal(s_preferredHeight, control.Height);
        Assert.Equal(!value, control.GetStyle(ControlStyles.FixedHeight));
        Assert.False(control.Multiline);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void TextBoxBase_AutoSize_SetMultiline_GetReturnsExpected(bool value)
    {
        using SubTextBox control = new()
        {
            Multiline = true
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.Equal(s_preferredHeight, control.Height);
        Assert.False(control.GetStyle(ControlStyles.FixedHeight));
        Assert.True(control.Multiline);

        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.Equal(s_preferredHeight, control.Height);
        Assert.False(control.GetStyle(ControlStyles.FixedHeight));
        Assert.True(control.Multiline);

        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.AutoSize = !value;
        Assert.Equal(!value, control.AutoSize);
        Assert.Equal(s_preferredHeight, control.Height);
        Assert.False(control.GetStyle(ControlStyles.FixedHeight));
        Assert.True(control.Multiline);

        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void TextBoxBase_AutoSize_SetWithParent_GetReturnsExpected(bool value)
    {
        using Control parent = new();
        using SubTextBox control = new()
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
            Assert.Equal(s_preferredHeight, control.Height);
            Assert.Equal(value, control.GetStyle(ControlStyles.FixedHeight));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);
            Assert.Equal(s_preferredHeight, control.Height);
            Assert.Equal(value, control.GetStyle(ControlStyles.FixedHeight));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set different.
            control.AutoSize = !value;
            Assert.Equal(!value, control.AutoSize);
            Assert.Equal(s_preferredHeight, control.Height);
            Assert.Equal(!value, control.GetStyle(ControlStyles.FixedHeight));
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
    public void TextBoxBase_AutoSize_SetWithHandler_CallsAutoSizeChanged()
    {
        using TextBox control = new()
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
    public void TextBoxBase_BackColor_GetReadOnly_ReturnsExpected()
    {
        using TextBox control = new()
        {
            ReadOnly = true
        };
        Assert.Equal(SystemColors.Control, control.BackColor);
    }

    public static IEnumerable<object[]> BackColor_Set_TestData()
    {
        yield return new object[] { Color.Empty, SystemColors.Window };
        yield return new object[] { Color.Red, Color.Red };
    }

    [WinFormsTheory]
    [MemberData(nameof(BackColor_Set_TestData))]
    public void TextBoxBase_BackColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using TextBox control = new()
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

    [WinFormsFact]
    public void TextBoxBase_BackColor_SetWithHandler_CallsBackColorChanged()
    {
        using TextBox control = new();
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
    public void TextBoxBase_BackgroundImage_Set_GetReturnsExpected(Image value)
    {
        using TextBox control = new()
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
    public void TextBoxBase_BackgroundImage_SetWithHandler_CallsBackgroundImageChanged()
    {
        using TextBox control = new();
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
    public void TextBoxBase_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
    {
        using SubTextBox control = new()
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
    public void TextBoxBase_BackgroundImageLayout_SetWithHandler_CallsBackgroundImageLayoutChanged()
    {
        using TextBox control = new();
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
    [EnumData<BorderStyle>]
    public void TextBoxBase_BorderStyle_Set_GetReturnsExpected(BorderStyle value)
    {
        using TextBox control = new()
        {
            BorderStyle = value
        };
        Assert.Equal(value, control.BorderStyle);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BorderStyle = value;
        Assert.Equal(value, control.BorderStyle);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, BorderStyle.Fixed3D, 0)]
    [InlineData(true, BorderStyle.FixedSingle, 1)]
    [InlineData(true, BorderStyle.None, 1)]
    [InlineData(false, BorderStyle.Fixed3D, 0)]
    [InlineData(false, BorderStyle.FixedSingle, 0)]
    [InlineData(false, BorderStyle.None, 0)]
    public void TextBoxBase_BorderStyle_SetWithParent_GetReturnsExpected(bool autoSize, BorderStyle value, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using TextBox control = new()
        {
            AutoSize = autoSize,
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("BorderStyle", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.BorderStyle = value;
            Assert.Equal(value, control.BorderStyle);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            control.BorderStyle = value;
            Assert.Equal(value, control.BorderStyle);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(BorderStyle.Fixed3D, 0)]
    [InlineData(BorderStyle.FixedSingle, 1)]
    [InlineData(BorderStyle.None, 1)]
    public void TextBoxBase_BorderStyle_SetWithHandle_GetReturnsExpected(BorderStyle value, int expectedStyleChangedCallCount)
    {
        using TextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.BorderStyle = value;
        Assert.Equal(value, control.BorderStyle);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, createdCallCount);

        // Set same.
        control.BorderStyle = value;
        Assert.Equal(value, control.BorderStyle);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, createdCallCount);
    }

    [WinFormsFact]
    public void TextBoxBase_BorderStyle_SetWithHandler_CallsBorderStyleChanged()
    {
        using TextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.BorderStyleChanged += handler;

        // Set different.
        control.BorderStyle = BorderStyle.FixedSingle;
        Assert.Equal(BorderStyle.FixedSingle, control.BorderStyle);
        Assert.Equal(1, callCount);

        // Set same.
        control.BorderStyle = BorderStyle.FixedSingle;
        Assert.Equal(BorderStyle.FixedSingle, control.BorderStyle);
        Assert.Equal(1, callCount);

        // Set different.
        control.BorderStyle = BorderStyle.Fixed3D;
        Assert.Equal(BorderStyle.Fixed3D, control.BorderStyle);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.BorderStyleChanged -= handler;
        control.BorderStyle = BorderStyle.FixedSingle;
        Assert.Equal(BorderStyle.FixedSingle, control.BorderStyle);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<BorderStyle>]
    public void TextBoxBase_BorderStyle_SetInvalid_ThrowsInvalidEnumArgumentException(BorderStyle value)
    {
        using TextBox control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.BorderStyle = value);
    }

    [WinFormsFact]
    public void TextBoxBase_CanUndo_GetWithHandle_ReturnsExpected()
    {
        using TextBox control = new();
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
    public void TextBoxBase_CanUndo_CustomCanUndo_ReturnsExpected(IntPtr result, bool expected)
    {
        using CustomCanUndoTextBoxBase control = new()
        {
            Result = result
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(expected, control.CanUndo);
    }

    private class CustomCanUndoTextBoxBase : TextBoxBase
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
    public void TextBoxBase_CanUndo_GetCantCreateHandle_GetReturnsExpected()
    {
        using CantCreateHandleTextBox control = new();
        Assert.False(control.CanUndo);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBase_Dispose_ClearsTextProvider()
    {
        using TextBox control = new();
        control.CreateControl();
        var textBoxBaseAccessibleObject = (TextBoxBase.TextBoxBaseAccessibleObject)control.AccessibilityObject;
        TextBoxBase.TextBoxBaseUiaTextProvider provider = textBoxBaseAccessibleObject.TestAccessor().Dynamic._textProvider;

        Assert.IsType<TextBoxBase.TextBoxBaseUiaTextProvider>(provider);

        control.Dispose();
        provider = textBoxBaseAccessibleObject.TestAccessor().Dynamic._textProvider;

        Assert.Null(provider);
    }

    [WinFormsFact]
    public void TextBoxBase_RecreateControl_DoesntClearTextProvider()
    {
        using TextBox control = new();
        control.CreateControl();
        TextBoxBase.TextBoxBaseUiaTextProvider provider = control.AccessibilityObject.TestAccessor().Dynamic._textProvider;

        Assert.IsType<TextBoxBase.TextBoxBaseUiaTextProvider>(provider);

        control.RecreateHandleCore();
        provider = control.AccessibilityObject.TestAccessor().Dynamic._textProvider;

        // The control's accessible object and its providers shouldn't be cleaned when recreating of the control
        // because this object and all its providers will continue to be used.
        Assert.IsType<TextBoxBase.TextBoxBaseUiaTextProvider>(provider);
    }

    [WinFormsFact]
    public void TextBoxBase_CanUndo_GetDisposed_ThrowsObjectDisposedException()
    {
        using TextBox control = new();
        control.Dispose();
        Assert.False(control.CanUndo);
    }

    [WinFormsTheory]
    [BoolData]
    public void TextBoxBase_DoubleBuffered_Get_ReturnsExpected(bool value)
    {
        using SubTextBox control = new();
        control.SetStyle(ControlStyles.OptimizedDoubleBuffer, value);
        Assert.Equal(value, control.DoubleBuffered);
    }

    [WinFormsTheory]
    [BoolData]
    public void TextBoxBase_DoubleBuffered_Set_GetReturnsExpected(bool value)
    {
        using SubTextBox control = new()
        {
            DoubleBuffered = value
        };
        Assert.Equal(value, control.DoubleBuffered);
        Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.DoubleBuffered = value;
        Assert.Equal(value, control.DoubleBuffered);
        Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.DoubleBuffered = !value;
        Assert.Equal(!value, control.DoubleBuffered);
        Assert.Equal(!value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void TextBoxBase_DoubleBuffered_SetWithHandle_GetReturnsExpected(bool value)
    {
        using SubTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.DoubleBuffered = value;
        Assert.Equal(value, control.DoubleBuffered);
        Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.DoubleBuffered = value;
        Assert.Equal(value, control.DoubleBuffered);
        Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.DoubleBuffered = !value;
        Assert.Equal(!value, control.DoubleBuffered);
        Assert.Equal(!value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetFontTheoryData))]
    public void TextBoxBase_Font_Set_GetReturnsExpected(Font value)
    {
        using SubTextBox control = new()
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
    public void TextBoxBase_Font_SetWithText_GetReturnsExpected(Font value)
    {
        using SubTextBox control = new()
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
    public void TextBoxBase_Font_SetWithNonNullOldValue_GetReturnsExpected(Font value)
    {
        using Font oldValue = new("Arial", 1);
        using SubTextBox control = new()
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
    public void TextBoxBase_Font_SetWithNonNullOldValueWithText_GetReturnsExpected(Font value)
    {
        using Font oldValue = new("Arial", 1);
        using SubTextBox control = new()
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
    public void TextBoxBase_Font_SetWithHandle_GetReturnsExpected(bool userPaint, Font value, int expectedInvalidatedCallCount)
    {
        using SubTextBox control = new();
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
    public void TextBoxBase_Font_SetWithTextWithHandle_GetReturnsExpected(bool userPaint, Font value, int expectedInvalidatedCallCount)
    {
        using SubTextBox control = new()
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
    public void TextBoxBase_Font_SetWithNonNullOldValueWithHandle_GetReturnsExpected(bool userPaint, Font value)
    {
        using Font oldValue = new("Arial", 1);
        using SubTextBox control = new()
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
    public void TextBoxBase_Font_SetWithNonNullOldValueWithTextWithHandle_GetReturnsExpected(bool userPaint, Font value)
    {
        using Font oldValue = new("Arial", 1);
        using SubTextBox control = new()
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

    [WinFormsFact]
    public void TextBoxBase_Font_SetWithHandler_CallsFontChanged()
    {
        using TextBox control = new();
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
    public void TextBoxBase_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using TextBox control = new()
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
    public void TextBoxBase_ForeColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
    {
        using TextBox control = new();
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
    public void TextBoxBase_ForeColor_SetWithHandler_CallsForeColorChanged()
    {
        using TextBox control = new();
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

    [WinFormsTheory]
    [InlineData(true, 3)]
    [InlineData(false, 0)]
    public void TextBoxBase_Handle_GetMargins_Success(bool multiline, int expected)
    {
        using TextBox control = new()
        {
            Multiline = multiline
        };
        control.CreateControl();
        IntPtr result = PInvokeCore.SendMessage(control, PInvokeCore.EM_GETMARGINS);
        Assert.Equal(expected, PARAM.LOWORD(result));
        Assert.Equal(expected, PARAM.HIWORD(result));
    }

    [WinFormsTheory]
    [InlineData(true, 3)]
    [InlineData(false, 0)]
    public void TextBoxBase_Handle_GetMarginsWithFont_Success(bool multiline, int expected)
    {
        using Font font = new("Arial", 8.25f);
        using TextBox control = new()
        {
            Multiline = multiline,
            Font = font
        };

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        IntPtr result = PInvokeCore.SendMessage(control, PInvokeCore.EM_GETMARGINS);
        Assert.Equal(expected, PARAM.LOWORD(result));
        Assert.Equal(expected, PARAM.HIWORD(result));
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void TextBoxBase_Handle_GetModify_Success(bool value, int expected)
    {
        using TextBox control = new()
        {
            Modified = value
        };

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(expected, (int)PInvokeCore.SendMessage(control, PInvokeCore.EM_GETMODIFY));
    }

    [WinFormsTheory]
    [BoolData]
    public void TextBoxBase_HideSelection_Set_GetReturnsExpected(bool value)
    {
        using SubTextBox control = new()
        {
            HideSelection = value
        };
        Assert.Equal(value, control.HideSelection);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.HideSelection = value;
        Assert.Equal(value, control.HideSelection);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.HideSelection = !value;
        Assert.Equal(!value, control.HideSelection);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 0)]
    [InlineData(false, 1)]
    public void TextBoxBase_HideSelection_SetWithHandle_GetReturnsExpected(bool value, int expectedCreatedCallCount)
    {
        using SubTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.HideSelection = value;
        Assert.Equal(value, control.HideSelection);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        control.HideSelection = value;
        Assert.Equal(value, control.HideSelection);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set different.
        control.HideSelection = !value;
        Assert.Equal(!value, control.HideSelection);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount + 1, createdCallCount);
    }

    [WinFormsFact]
    public void TextBoxBase_HideSelection_SetWithHandler_CallsHideSelectionChanged()
    {
        using TextBox control = new()
        {
            HideSelection = true
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.HideSelectionChanged += handler;

        // Set different.
        control.HideSelection = false;
        Assert.False(control.HideSelection);
        Assert.Equal(1, callCount);

        // Set same.
        control.HideSelection = false;
        Assert.False(control.HideSelection);
        Assert.Equal(1, callCount);

        // Set different.
        control.HideSelection = true;
        Assert.True(control.HideSelection);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.HideSelectionChanged -= handler;
        control.HideSelection = false;
        Assert.False(control.HideSelection);
        Assert.Equal(2, callCount);
    }

    public static IEnumerable<object[]> ImeModeBase_Set_TestData()
    {
        yield return new object[] { ImeMode.Inherit, ImeMode.NoControl };
        yield return new object[] { ImeMode.NoControl, ImeMode.NoControl };
        yield return new object[] { ImeMode.On, ImeMode.On };
        yield return new object[] { ImeMode.Off, ImeMode.Off };
        yield return new object[] { ImeMode.Disable, ImeMode.Disable };
        yield return new object[] { ImeMode.Hiragana, ImeMode.Hiragana };
        yield return new object[] { ImeMode.Katakana, ImeMode.Katakana };
        yield return new object[] { ImeMode.KatakanaHalf, ImeMode.KatakanaHalf };
        yield return new object[] { ImeMode.AlphaFull, ImeMode.AlphaFull };
        yield return new object[] { ImeMode.Alpha, ImeMode.Alpha };
        yield return new object[] { ImeMode.HangulFull, ImeMode.HangulFull };
        yield return new object[] { ImeMode.Hangul, ImeMode.Hangul };
        yield return new object[] { ImeMode.Close, ImeMode.Close };
        yield return new object[] { ImeMode.OnHalf, ImeMode.OnHalf };
    }

    [WinFormsTheory]
    [MemberData(nameof(ImeModeBase_Set_TestData))]
    public void TextBoxBase_ImeModeBase_Set_GetReturnsExpected(ImeMode value, ImeMode expected)
    {
        using SubTextBox control = new()
        {
            ImeModeBase = value
        };
        Assert.Equal(expected, control.ImeModeBase);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.ImeModeBase = value;
        Assert.Equal(expected, control.ImeModeBase);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [EnumData<ImeMode>]
    public void TextBoxBase_ImeModeBase_SetReadOnly_GetReturnsExpected(ImeMode value)
    {
        using SubTextBox control = new()
        {
            ReadOnly = true,
            ImeModeBase = value
        };
        Assert.Equal(ImeMode.Disable, control.ImeModeBase);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ImeModeBase = value;
        Assert.Equal(ImeMode.Disable, control.ImeModeBase);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ImeModeBase_Set_TestData))]
    public void TextBoxBase_ImeModeBase_SetDesignMode_GetReturnsExpected(ImeMode value, ImeMode expected)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.Name)
            .Returns((string)null);
        using SubTextBox control = new()
        {
            Site = mockSite.Object,
            ImeModeBase = value
        };
        Assert.Equal(expected, control.ImeModeBase);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ImeModeBase = value;
        Assert.Equal(expected, control.ImeModeBase);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ImeModeBase_Set_TestData))]
    public void TextBoxBase_ImeModeBase_SetReadOnlyDesignMode_GetReturnsExpected(ImeMode value, ImeMode expected)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.Name)
            .Returns((string)null);
        using SubTextBox control = new()
        {
            Site = mockSite.Object,
            ReadOnly = true,
            ImeModeBase = value
        };
        Assert.Equal(expected, control.ImeModeBase);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ImeModeBase = value;
        Assert.Equal(expected, control.ImeModeBase);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ImeModeBase_Set_TestData))]
    public void TextBoxBase_ImeModeBase_SetWithHandle_GetReturnsExpected(ImeMode value, ImeMode expected)
    {
        using SubTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ImeModeBase = value;
        Assert.Equal(expected, control.ImeModeBase);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ImeModeBase = value;
        Assert.Equal(expected, control.ImeModeBase);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TextBoxBase_ImeModeBase_SetWithHandler_CallsImeModeChanged()
    {
        using SubTextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.ImeModeChanged += handler;

        // Set different.
        control.ImeModeBase = ImeMode.On;
        Assert.Equal(ImeMode.On, control.ImeModeBase);
        Assert.Equal(1, callCount);

        // Set same.
        control.ImeModeBase = ImeMode.On;
        Assert.Equal(ImeMode.On, control.ImeModeBase);
        Assert.Equal(1, callCount);

        // Set different.
        control.ImeModeBase = ImeMode.Off;
        Assert.Equal(ImeMode.Off, control.ImeModeBase);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.ImeModeChanged -= handler;
        control.ImeModeBase = ImeMode.Off;
        Assert.Equal(ImeMode.Off, control.ImeModeBase);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<ImeMode>]
    public void TextBoxBase_ImeModeBase_SetInvalid_ThrowsInvalidEnumArgumentException(ImeMode value)
    {
        using SubTextBox control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.ImeModeBase = value);
    }

    public static IEnumerable<object[]> Lines_Get_TestData()
    {
        yield return new object[] { string.Empty, Array.Empty<string>() };
        yield return new object[] { "\r", new string[] { string.Empty, string.Empty } };
        yield return new object[] { "\n", new string[] { string.Empty, string.Empty } };
        yield return new object[] { "\r\n", new string[] { string.Empty, string.Empty } };

        yield return new object[] { "abc", new string[] { "abc" } };

        yield return new object[] { "\rabc", new string[] { string.Empty, "abc" } };
        yield return new object[] { "\nabc", new string[] { string.Empty, "abc" } };
        yield return new object[] { "\r\nabc", new string[] { string.Empty, "abc" } };

        yield return new object[] { "abc\r", new string[] { "abc", string.Empty } };
        yield return new object[] { "abc\n", new string[] { "abc", string.Empty } };
        yield return new object[] { "abc\r\n", new string[] { "abc", string.Empty } };

        yield return new object[] { "abc\rdef", new string[] { "abc", "def" } };
        yield return new object[] { "abc\ndef", new string[] { "abc", "def" } };
        yield return new object[] { "abc\r\ndef", new string[] { "abc", "def" } };
    }

    [WinFormsTheory]
    [MemberData(nameof(Lines_Get_TestData))]
    public void TextBoxBase_Lines_Get_ReturnsExpected(string text, string[] expected)
    {
        using SubTextBox control = new();
        Assert.Empty(control.Lines);

        control.Text = text;
        Assert.Equal(expected, control.Lines);
    }

    public static IEnumerable<object[]> Lines_Set_TestData()
    {
        yield return new object[] { null, Array.Empty<string>(), string.Empty };
        yield return new object[] { Array.Empty<string>(), Array.Empty<string>(), string.Empty };
        yield return new object[] { Array.Empty<string>(), Array.Empty<string>(), string.Empty };
        yield return new object[] { new string[] { "abc" }, new string[] { "abc" }, "abc" };
        yield return new object[] { new string[] { "abc", "def" }, new string[] { "abc", "def" }, "abc\r\ndef" };
    }

    [WinFormsTheory]
    [MemberData(nameof(Lines_Set_TestData))]
    public void TextBoxBase_Lines_Set_GetReturnsExpected(string[] lines, string[] expected, string expectedText)
    {
        using SubTextBox control = new()
        {
            Lines = lines
        };
        Assert.NotSame(lines, control.Lines);
        Assert.Equal(expected, control.Lines);
        Assert.Equal(expectedText, control.Text);
        Assert.False(control.IsHandleCreated);

        // Set same.
        Assert.NotSame(lines, control.Lines);
        Assert.Equal(expected, control.Lines);
        Assert.Equal(expectedText, control.Text);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBase_MaxLength_GetWithHandle_ReturnsExpected()
    {
        using TextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(0x7FFF, control.MaxLength);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call EM_LIMITTEXT.
        PInvokeCore.SendMessage(control, PInvokeCore.EM_LIMITTEXT, 0, 1);
        Assert.Equal(0x7FFF, control.MaxLength);
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
    public void TextBoxBase_MaxLength_Set_GetReturnsExpected(int value)
    {
        using TextBox control = new()
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
    public void TextBoxBase_MaxLength_SetWithLongText_Success()
    {
        using TextBox control = new()
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
    public void TextBoxBase_MaxLength_SetWithHandle_GetReturnsExpected(int value)
    {
        using TextBox control = new();
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
    public void TextBoxBase_MaxLength_SetWithLongTextWithHandle_Success()
    {
        using TextBox control = new();
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
    [InlineData(0, 0x7FFFFFFE)]
    [InlineData(1, 1)]
    [InlineData(64000, 64000)]
    [InlineData(0x7FFFFFFE, 0x7FFFFFFE)]
    [InlineData(int.MaxValue, 0x7FFFFFFE)]
    public void TextBoxBase_MaxLength_GetLimitText_Success(int value, int expected)
    {
        using TextBox control = new();

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.MaxLength = value;
        Assert.Equal(expected, (int)PInvokeCore.SendMessage(control, PInvokeCore.EM_GETLIMITTEXT));
    }

    [WinFormsFact]
    public void TextBoxBase_MaxLength_SetNegative_ThrowsArgumentOutOfRangeException()
    {
        using TextBox control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.MaxLength = -1);
    }

    [WinFormsFact]
    public void TextBoxBase_Modified_GetWithHandle_ReturnsExpected()
    {
        using TextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int modifiedChangedCallCount = 0;
        control.ModifiedChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.True(control.Modified);
            modifiedChangedCallCount++;
        };

        Assert.False(control.Modified);
        Assert.Equal(0, modifiedChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call EM_SETMODIFY.
        PInvokeCore.SendMessage(control, PInvokeCore.EM_SETMODIFY, (WPARAM)(BOOL)true);
        Assert.Equal(0, modifiedChangedCallCount);

        Assert.True(control.Modified);
        Assert.Equal(1, modifiedChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void TextBoxBase_Modified_Set_GetReturnsExpected(bool value)
    {
        using SubTextBox control = new();

        control.Modified = value;
        Assert.Equal(value, control.Modified);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Modified = value;
        Assert.Equal(value, control.Modified);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.Modified = !value;
        Assert.Equal(!value, control.Modified);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void TextBoxBase_Modified_SetWithHandle_GetReturnsExpected(bool value)
    {
        using SubTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Modified = value;
        Assert.Equal(value, control.Modified);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Modified = value;
        Assert.Equal(value, control.Modified);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.Modified = !value;
        Assert.Equal(!value, control.Modified);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void TextBoxBase_Modified_GetModify_Success(bool value, int expected)
    {
        using TextBox control = new();

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.Modified = value;
        Assert.Equal(expected, (int)PInvokeCore.SendMessage(control, PInvokeCore.EM_GETMODIFY));
    }

    [WinFormsFact]
    public void TextBoxBase_Modified_SetWithHandler_CallsModifiedChanged()
    {
        using TextBox control = new()
        {
            Modified = true
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.ModifiedChanged += handler;

        // Set different.
        control.Modified = false;
        Assert.False(control.Modified);
        Assert.Equal(1, callCount);

        // Set same.
        control.Modified = false;
        Assert.False(control.Modified);
        Assert.Equal(1, callCount);

        // Set different.
        control.Modified = true;
        Assert.True(control.Modified);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.ModifiedChanged -= handler;
        control.Modified = false;
        Assert.False(control.Modified);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void TextBoxBase_Multiline_Set_GetReturnsExpected(bool value)
    {
        using SubTextBox control = new()
        {
            Multiline = value
        };
        Assert.Equal(value, control.Multiline);
        Assert.Equal(s_preferredHeight, control.Height);
        Assert.Equal(!value, control.GetStyle(ControlStyles.FixedHeight));
        Assert.True(control.AutoSize);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Multiline = value;
        Assert.Equal(value, control.Multiline);
        Assert.Equal(s_preferredHeight, control.Height);
        Assert.Equal(!value, control.GetStyle(ControlStyles.FixedHeight));
        Assert.True(control.AutoSize);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.Multiline = !value;
        Assert.Equal(!value, control.Multiline);
        Assert.Equal(s_preferredHeight, control.Height);
        Assert.Equal(value, control.GetStyle(ControlStyles.FixedHeight));
        Assert.True(control.AutoSize);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void TextBoxBase_Multiline_SetNotAutoSize_GetReturnsExpected(bool value)
    {
        using SubTextBox control = new()
        {
            AutoSize = false,
            Multiline = value
        };
        Assert.Equal(value, control.Multiline);
        Assert.Equal(s_preferredHeight, control.Height);
        Assert.False(control.GetStyle(ControlStyles.FixedHeight));
        Assert.False(control.AutoSize);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Multiline = value;
        Assert.Equal(value, control.Multiline);
        Assert.Equal(s_preferredHeight, control.Height);
        Assert.False(control.GetStyle(ControlStyles.FixedHeight));
        Assert.False(control.AutoSize);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.Multiline = !value;
        Assert.Equal(!value, control.Multiline);
        Assert.Equal(s_preferredHeight, control.Height);
        Assert.False(control.GetStyle(ControlStyles.FixedHeight));
        Assert.False(control.AutoSize);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void TextBoxBase_Multiline_SetWithParent_GetReturnsExpected(bool value, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using SubTextBox control = new()
        {
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            // TODO
            // Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.Multiline = value;
            Assert.Equal(value, control.Multiline);
            Assert.Equal(s_preferredHeight, control.Height);
            Assert.Equal(!value, control.GetStyle(ControlStyles.FixedHeight));
            Assert.True(control.AutoSize);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            control.Multiline = value;
            Assert.Equal(value, control.Multiline);
            Assert.Equal(s_preferredHeight, control.Height);
            Assert.Equal(!value, control.GetStyle(ControlStyles.FixedHeight));
            Assert.True(control.AutoSize);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set different.
            control.Multiline = !value;
            Assert.Equal(!value, control.Multiline);
            Assert.Equal(s_preferredHeight, control.Height);
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
    [InlineData(true, 4)]
    [InlineData(false, 0)]
    public void TextBoxBase_Multiline_SetNotAutoSizeWithParent_GetReturnsExpected(bool value, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using SubTextBox control = new()
        {
            Parent = parent,
            AutoSize = false
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            control.Multiline = value;
            Assert.Equal(value, control.Multiline);
            Assert.Equal(s_preferredHeight, control.Height);
            Assert.False(control.GetStyle(ControlStyles.FixedHeight));
            Assert.False(control.AutoSize);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            control.Multiline = value;
            Assert.Equal(value, control.Multiline);
            Assert.Equal(s_preferredHeight, control.Height);
            Assert.False(control.GetStyle(ControlStyles.FixedHeight));
            Assert.False(control.AutoSize);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set different.
            control.Multiline = !value;
            Assert.Equal(!value, control.Multiline);
            Assert.Equal(s_preferredHeight, control.Height);
            Assert.False(control.GetStyle(ControlStyles.FixedHeight));
            Assert.False(control.AutoSize);
            Assert.Equal(4, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void TextBoxBase_Multiline_SetWithHandle_GetReturnsExpected(bool value, int expectedCreatedCallCount)
    {
        using SubTextBox control = new();
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
    public void TextBoxBase_Multiline_SetWithHandler_CallsMultilineChanged()
    {
        using TextBox control = new()
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

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaddingNormalizedTheoryData))]
    public void TextBoxBase_Padding_Set_GetReturnsExpected(Padding value, Padding expected)
    {
        using TextBox control = new()
        {
            Padding = value
        };
        Assert.Equal(expected, control.Padding);
        Assert.Equal(s_preferredHeight, control.PreferredHeight);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Padding = value;
        Assert.Equal(expected, control.Padding);
        Assert.Equal(s_preferredHeight, control.PreferredHeight);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaddingNormalizedTheoryData))]
    public void TextBoxBase_Padding_SetWithHandle_GetReturnsExpected(Padding value, Padding expected)
    {
        using TextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Padding = value;
        Assert.Equal(expected, control.Padding);
        Assert.Equal(s_preferredHeight, control.PreferredHeight);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Padding = value;
        Assert.Equal(expected, control.Padding);
        Assert.Equal(s_preferredHeight, control.PreferredHeight);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TextBoxBase_Padding_SetWithHandler_CallsPaddingChanged()
    {
        using TextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Equal(control, sender);
            Assert.Equal(EventArgs.Empty, e);
            callCount++;
        };
        control.PaddingChanged += handler;

        // Set different.
        Padding padding1 = new(1);
        control.Padding = padding1;
        Assert.Equal(padding1, control.Padding);
        Assert.Equal(1, callCount);

        // Set same.
        control.Padding = padding1;
        Assert.Equal(padding1, control.Padding);
        Assert.Equal(1, callCount);

        // Set different.
        Padding padding2 = new(2);
        control.Padding = padding2;
        Assert.Equal(padding2, control.Padding);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.PaddingChanged -= handler;
        control.Padding = padding1;
        Assert.Equal(padding1, control.Padding);
        Assert.Equal(2, callCount);
    }

    public static IEnumerable<object[]> PreferredHeight_Get_TestData()
    {
        yield return new object[] { BorderStyle.None, Control.DefaultFont.Height };
        yield return new object[] { BorderStyle.Fixed3D, s_preferredHeight };
        yield return new object[] { BorderStyle.FixedSingle, s_preferredHeight };
    }

    [WinFormsTheory]
    [MemberData(nameof(PreferredHeight_Get_TestData))]
    public void TextBoxBase_PreferredHeight_Get_ReturnsExpected(BorderStyle borderStyle, int expected)
    {
        using SubTextBox control = new()
        {
            BorderStyle = borderStyle
        };
        Assert.Equal(expected, control.PreferredHeight);
    }

    [WinFormsFact]
    public void TextBoxBase_ReadOnly_GetWithHandle_ReturnsExpected()
    {
        using TextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int readOnlyChangedCallCount = 0;
        control.ReadOnlyChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.True(control.ReadOnly);
            readOnlyChangedCallCount++;
        };

        Assert.False(control.ReadOnly);
        Assert.Equal(0, readOnlyChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call EM_SETREADONLY.
        PInvokeCore.SendMessage(control, PInvokeCore.EM_SETREADONLY, (WPARAM)(BOOL)true);
        Assert.Equal(0, readOnlyChangedCallCount);

        Assert.False(control.ReadOnly);
        Assert.Equal(0, readOnlyChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void TextBoxBase_ReadOnly_Set_GetReturnsExpected(bool value)
    {
        using SubTextBox control = new();

        control.ReadOnly = value;
        Assert.Equal(value, control.ReadOnly);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ReadOnly = value;
        Assert.Equal(value, control.ReadOnly);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.ReadOnly = !value;
        Assert.Equal(!value, control.ReadOnly);
        Assert.Equal(value, control.IsHandleCreated);

        // Set different again.
        control.ReadOnly = value;
        Assert.Equal(value, control.ReadOnly);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void TextBoxBase_ReadOnly_SetWithHandle_GetReturnsExpected(bool value)
    {
        using SubTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ReadOnly = value;
        Assert.Equal(value, control.ReadOnly);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ReadOnly = value;
        Assert.Equal(value, control.ReadOnly);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.ReadOnly = !value;
        Assert.Equal(!value, control.ReadOnly);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void TextBoxBase_ReadOnly_GetModify_Success(bool value)
    {
        using TextBox control = new();

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.ReadOnly = value;

        WINDOW_STYLE style = (WINDOW_STYLE)PInvokeCore.GetWindowLong(control, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
        Assert.Equal(value, ((int)style & PInvoke.ES_READONLY) != 0);
    }

    [WinFormsFact]
    public void TextBoxBase_ReadOnly_SetWithHandler_CallsReadOnlyChanged()
    {
        using TextBox control = new()
        {
            ReadOnly = true
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.ReadOnlyChanged += handler;

        // Set different.
        control.ReadOnly = false;
        Assert.False(control.ReadOnly);
        Assert.Equal(1, callCount);

        // Set same.
        control.ReadOnly = false;
        Assert.False(control.ReadOnly);
        Assert.Equal(1, callCount);

        // Set different.
        control.ReadOnly = true;
        Assert.True(control.ReadOnly);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.ReadOnlyChanged -= handler;
        control.ReadOnly = false;
        Assert.False(control.ReadOnly);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InlineData(ImeMode.NoControl, 0)]
    [InlineData(ImeMode.Katakana, 1)]
    public void TextBoxBase_ReadOnly_SetWithHandler_CallsImeModeChanged(ImeMode imeMode, int expectedCallCount)
    {
        using TextBox control = new()
        {
            ReadOnly = true,
            ImeMode = imeMode
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.ImeModeChanged += handler;

        // Set different.
        control.ReadOnly = false;
        Assert.False(control.ReadOnly);
        Assert.Equal(imeMode, control.ImeMode);
        Assert.Equal(expectedCallCount, callCount);

        // Set same.
        control.ReadOnly = false;
        Assert.False(control.ReadOnly);
        Assert.Equal(imeMode, control.ImeMode);
        Assert.Equal(expectedCallCount * 1, callCount);

        // Set different.
        control.ReadOnly = true;
        Assert.True(control.ReadOnly);
        Assert.Equal(ImeMode.Disable, control.ImeMode);
        Assert.Equal(expectedCallCount + 1, callCount);

        // Remove handler.
        control.ImeModeChanged -= handler;
        control.ReadOnly = false;
        Assert.False(control.ReadOnly);
        Assert.Equal(imeMode, control.ImeMode);
        Assert.Equal(expectedCallCount + 1, callCount);
    }

    [WinFormsTheory]
    [InlineData(ImeMode.NoControl)]
    [InlineData(ImeMode.Katakana)]
    public void TextBoxBase_ReadOnly_SetWithHandlerImeNotSupported_DoesNotCallImeModeChanged(ImeMode imeMode)
    {
        using ImeNotSupportedTextBox control = new()
        {
            ReadOnly = true,
            ImeMode = imeMode
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.ImeModeChanged += handler;

        // Set different.
        control.ReadOnly = false;
        Assert.False(control.ReadOnly);
        Assert.Equal(ImeMode.Disable, control.ImeMode);
        Assert.Equal(0, callCount);

        // Set same.
        control.ReadOnly = false;
        Assert.False(control.ReadOnly);
        Assert.Equal(ImeMode.Disable, control.ImeMode);
        Assert.Equal(0, callCount);

        // Set different.
        control.ReadOnly = true;
        Assert.True(control.ReadOnly);
        Assert.Equal(ImeMode.Disable, control.ImeMode);
        Assert.Equal(0, callCount);

        // Remove handler.
        control.ImeModeChanged -= handler;
        control.ReadOnly = false;
        Assert.False(control.ReadOnly);
        Assert.Equal(ImeMode.Disable, control.ImeMode);
        Assert.Equal(0, callCount);
    }

    [WinFormsFact]
    public void TextBoxBase_SelectedText_GetWithHandle_ReturnsExpected()
    {
        using TextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Get again.
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TextBoxBase_SelectedText_GetCantCreateHandle_ReturnsExpected()
    {
        using CantCreateHandleTextBox control = new();
        Assert.Empty(control.SelectedText);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBase_SelectedText_GetDisposed_ReturnsExpected()
    {
        using TextBox control = new();
        control.Dispose();
        Assert.Empty(control.SelectedText);
    }

    [WinFormsTheory]
    [InlineData("", null, "", 0)]
    [InlineData("", "", "", 0)]
    [InlineData("", "abc", "abc", 3)]
    [InlineData("text", null, "text", 0)]
    [InlineData("text", "", "text", 0)]
    [InlineData("text", "abc", "abctext", 3)]
    public void TextBoxBase_SelectedText_Set_GetReturnsExpected(string text, string value, string expected, int expectedSelectionStart)
    {
        using SubTextBox control = new()
        {
            Text = text,
            SelectedText = value
        };
        Assert.Equal(expectedSelectionStart, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.Equal(expected, control.Text);
        Assert.False(control.CanUndo);
        Assert.Equal(0x7FFF, control.MaxLength);
        Assert.False(control.Modified);
        Assert.True(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> SelectedText_Set_TestData()
    {
        yield return new object[] { string.Empty, 0, 0, null, string.Empty, 0 };
        yield return new object[] { string.Empty, 0, 0, string.Empty, string.Empty, 0 };
        yield return new object[] { string.Empty, 0, 0, "abc", "abc", 3 };

        yield return new object[] { "text", 0, 0, null, "text", 0 };
        yield return new object[] { "text", 0, 0, string.Empty, "text", 0 };
        yield return new object[] { "text", 0, 0, "abc", "abctext", 3 };

        yield return new object[] { "text", 1, 0, null, "text", 1 };
        yield return new object[] { "text", 1, 0, string.Empty, "text", 1 };
        yield return new object[] { "text", 1, 0, "abc", "tabcext", 4 };

        yield return new object[] { "text", 3, 0, null, "text", 3 };
        yield return new object[] { "text", 3, 0, string.Empty, "text", 3 };
        yield return new object[] { "text", 3, 0, "abc", "texabct", 6 };

        yield return new object[] { "text", 0, 2, null, "xt", 0 };
        yield return new object[] { "text", 0, 2, string.Empty, "xt", 0 };
        yield return new object[] { "text", 0, 2, "abc", "abcxt", 3 };

        yield return new object[] { "text", 1, 2, null, "tt", 1 };
        yield return new object[] { "text", 1, 2, string.Empty, "tt", 1 };
        yield return new object[] { "text", 1, 2, "abc", "tabct", 4 };

        yield return new object[] { "text", 0, 4, null, string.Empty, 0 };
        yield return new object[] { "text", 0, 4, string.Empty, string.Empty, 0 };
        yield return new object[] { "text", 0, 4, "abc", "abc", 3 };
        yield return new object[] { "text", 0, 4, "abcd", "abcd", 4 };
        yield return new object[] { "text", 0, 4, "abcde", "abcde", 5 };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectedText_Set_TestData))]
    public void TextBoxBase_SelectedText_SetWithSelectionStartAndLength_GetReturnsExpected(string text, int selectionStart, int selectionLength, string value, string expected, int expectedSelectionStart)
    {
        using SubTextBox control = new()
        {
            Text = text,
            SelectionStart = selectionStart,
            SelectionLength = selectionLength,
            SelectedText = value
        };
        Assert.Equal(expectedSelectionStart, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.Equal(expected, control.Text);
        Assert.False(control.CanUndo);
        Assert.Equal(0x7FFF, control.MaxLength);
        Assert.False(control.Modified);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectedText_Set_TestData))]
    public void TextBoxBase_SelectedText_SetModified_GetReturnsExpected(string text, int selectionStart, int selectionLength, string value, string expected, int expectedSelectionStart)
    {
        using SubTextBox control = new()
        {
            Text = text,
            SelectionStart = selectionStart,
            SelectionLength = selectionLength,
            Modified = true,
            SelectedText = value
        };
        Assert.Equal(expectedSelectionStart, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.Equal(expected, control.Text);
        Assert.False(control.CanUndo);
        Assert.Equal(0x7FFF, control.MaxLength);
        Assert.False(control.Modified);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectedText_Set_TestData))]
    public void TextBoxBase_SelectedText_SetWithSelectionStartAndLengthWithMaxLength_GetReturnsExpected(string text, int selectionStart, int selectionLength, string value, string expected, int expectedSelectionStart)
    {
        using SubTextBox control = new()
        {
            Text = text,
            SelectionStart = selectionStart,
            SelectionLength = selectionLength,
            SelectedText = value,
            MaxLength = 1
        };
        Assert.Equal(expectedSelectionStart, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.Equal(expected, control.Text);
        Assert.False(control.CanUndo);
        Assert.Equal(1, control.MaxLength);
        Assert.False(control.Modified);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectedText_Set_TestData))]
    public void TextBoxBase_SelectedText_SetWithHandle_GetReturnsExpected(string text, int selectionStart, int selectionLength, string value, string expected, int expectedSelectionStart)
    {
        using SubTextBox control = new()
        {
            Text = text,
            SelectionStart = selectionStart,
            SelectionLength = selectionLength
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SelectedText = value;
        Assert.Equal(expectedSelectionStart, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.Equal(expected, control.Text);
        Assert.False(control.CanUndo);
        Assert.Equal(0x7FFF, control.MaxLength);
        Assert.False(control.Modified);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void TextBoxBase_SelectedText_SetCantCreateHandle_GetReturnsExpected(string value)
    {
        using CantCreateHandleTextBox control = new();
        control.SelectedText = value;
        Assert.Empty(control.SelectedText);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SelectedText = value;
        Assert.Empty(control.SelectedText);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void TextBoxBase_SelectedText_SetDisposed_ThrowsObjectDisposedException(string value)
    {
        using TextBox control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.SelectedText = value);
    }

    [WinFormsFact]
    public void TextBoxBase_SelectionLength_GetWithHandle_ReturnsExpected()
    {
        using TextBox control = new();
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
    public void TextBoxBase_SelectionLength_GetCantCreateHandle_ReturnsExpected()
    {
        using CantCreateHandleTextBox control = new();
        Assert.Equal(0, control.SelectionLength);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBase_SelectionLength_GetDisposed_ReturnsExpected()
    {
        using TextBox control = new();
        control.Dispose();
        Assert.Equal(0, control.SelectionLength);
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
    public void TextBoxBase_SelectionLength_Set_GetReturnsExpected(string text, int value, int expected, string expectedSelectedText)
    {
        using SubTextBox control = new()
        {
            Text = text,
            SelectionLength = value
        };
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(expected, control.SelectionLength);
        Assert.Equal(expectedSelectedText, control.SelectedText);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SelectionLength = value;
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(expected, control.SelectionLength);
        Assert.Equal(expectedSelectedText, control.SelectedText);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData("", 0, 0, "")]
    [InlineData("", 1, 0, "")]
    [InlineData("text", 0, 0, "")]
    [InlineData("text", 1, 1, "e")]
    [InlineData("text", 2, 2, "ex")]
    [InlineData("text", 3, 3, "ext")]
    [InlineData("text", 5, 3, "ext")]
    public void TextBoxBase_SelectionLength_SetWithSelectionStart_Success(string text, int value, int expected, string expectedSelectedText)
    {
        using SubTextBox control = new()
        {
            Text = text,
            SelectionStart = 1,
            SelectionLength = value
        };
        Assert.Equal(expected, control.SelectionLength);
        Assert.Equal(expectedSelectedText, control.SelectedText);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SelectionLength = value;
        Assert.Equal(expected, control.SelectionLength);
        Assert.Equal(expectedSelectedText, control.SelectedText);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData("", 0, 0, "")]
    [InlineData("", 1, 0, "")]
    [InlineData("text", 0, 0, "")]
    [InlineData("text", 1, 1, "t")]
    [InlineData("text", 2, 2, "te")]
    [InlineData("text", 4, 4, "text")]
    [InlineData("text", 5, 4, "text")]
    public void TextBoxBase_SelectionLength_SetWithHandle_Success(string text, int value, int expected, string expectedSelectedText)
    {
        using SubTextBox control = new()
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
    public unsafe void TextBoxBase_SelectionLength_GetSel_Success(int value, int expected)
    {
        using SubTextBox control = new()
        {
            Text = "Text",
            SelectionStart = 1
        };

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.SelectionLength = value;
        int selectionStart = 0;
        int selectionEnd = 0;
        LRESULT result = PInvokeCore.SendMessage(
            control,
            PInvokeCore.EM_GETSEL,
            (WPARAM)(&selectionStart),
            (LPARAM)(&selectionEnd));
        Assert.Equal(1, result.LOWORD);
        Assert.Equal(expected, result.HIWORD);
        Assert.Equal(1, selectionStart);
        Assert.Equal(expected, selectionEnd);
    }

    [WinFormsFact]
    public void TextBoxBase_SelectionLength_SetNegative_ThrowArgumentOutOfRangeException()
    {
        using SubTextBox control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.SelectionLength = -1);
    }

    [WinFormsFact]
    public void TextBoxBase_SelectionStart_GetWithHandle_ReturnsExpected()
    {
        using TextBox control = new();
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
    public void TextBoxBase_SelectionStart_GetCantCreateHandle_ReturnsExpected()
    {
        using CantCreateHandleTextBox control = new();
        Assert.Equal(0, control.SelectionStart);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBase_SelectionStart_GetDisposed_ReturnsExpected()
    {
        using TextBox control = new();
        control.Dispose();
        Assert.Equal(0, control.SelectionStart);
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
    public void TextBoxBase_SelectionStart_Set_GetReturnsExpected(string text, int value, int expected)
    {
        using SubTextBox control = new()
        {
            Text = text,
            SelectionStart = value
        };
        Assert.Equal(0, control.SelectionLength);
        Assert.Equal(expected, control.SelectionStart);
        Assert.Empty(control.SelectedText);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SelectionStart = value;
        Assert.Equal(0, control.SelectionLength);
        Assert.Equal(expected, control.SelectionStart);
        Assert.Empty(control.SelectedText);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData("", 0, 0, "")]
    [InlineData("", 1, 0, "")]
    [InlineData("text", 0, 0, "te")]
    [InlineData("text", 1, 1, "ex")]
    [InlineData("text", 2, 2, "xt")]
    [InlineData("text", 3, 3, "t")]
    [InlineData("text", 5, 4, "")]
    public void TextBoxBase_SelectionStart_SetWithSelectionLength_Success(string text, int value, int expected, string expectedSelectedText)
    {
        using SubTextBox control = new()
        {
            Text = text,
            SelectionLength = 2,
            SelectionStart = value
        };
        Assert.Equal(expected, control.SelectionStart);
        Assert.Equal(expectedSelectedText, control.SelectedText);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SelectionStart = value;
        Assert.Equal(expected, control.SelectionStart);
        Assert.Equal(expectedSelectedText, control.SelectedText);
        Assert.False(control.IsHandleCreated);
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
    public void TextBoxBase_SelectionStart_SetWithHandle_Success(string text, int value, int expected)
    {
        using SubTextBox control = new()
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
    public unsafe void TextBoxBase_SelectionStart_GetSel_Success(int value, int expectedSelectionStart, int expectedEnd)
    {
        using SubTextBox control = new()
        {
            Text = "Text",
            SelectionLength = 1
        };

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.SelectionStart = value;
        int selectionStart = 0;
        int selectionEnd = 0;
        LRESULT result = PInvokeCore.SendMessage(
            control,
            PInvokeCore.EM_GETSEL,
            (WPARAM)(&selectionStart),
            (LPARAM)(&selectionEnd));
        Assert.Equal(expectedSelectionStart, result.LOWORD);
        Assert.Equal(expectedEnd, result.HIWORD);
        Assert.Equal(expectedSelectionStart, selectionStart);
        Assert.Equal(expectedEnd, selectionEnd);
    }

    [WinFormsFact]
    public void TextBoxBase_SelectionStart_SetNegative_ThrowArgumentOutOfRangeException()
    {
        using SubTextBox control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.SelectionStart = -1);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    public void TextBoxBase_SelectionStart_SetCantCreateHandle_GetReturnsExpected(int value)
    {
        using CantCreateHandleTextBox control = new();
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
    public void TextBoxBase_SelectionStart_SetDisposed_ReturnsExpected(int value)
    {
        using TextBox control = new();
        control.Dispose();

        control.SelectionStart = value;
        Assert.Equal(0, control.SelectionStart);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SelectionStart = value;
        Assert.Equal(0, control.SelectionStart);
        Assert.False(control.IsHandleCreated);
    }

    private class ImeNotSupportedTextBox : TextBox
    {
        protected override ImeMode DefaultImeMode => ImeMode.Disable;
    }

    [WinFormsTheory]
    [BoolData]
    public void TextBoxBase_ShortcutsEnabled_Set_GetReturnsExpected(bool value)
    {
        using TextBox control = new()
        {
            ShortcutsEnabled = value
        };
        Assert.Equal(value, control.ShortcutsEnabled);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ShortcutsEnabled = value;
        Assert.Equal(value, control.ShortcutsEnabled);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.ShortcutsEnabled = !value;
        Assert.Equal(!value, control.ShortcutsEnabled);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBase_Text_GetWithHandle_ReturnsExpected()
    {
        using TextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Empty(control.Text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Get again.
        Assert.Empty(control.Text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TextBoxBase_Text_GetCantCreateHandle_ReturnsExpected()
    {
        using CantCreateHandleTextBox control = new();
        Assert.Empty(control.Text);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBase_Text_GetDisposed_ReturnsExpected()
    {
        using TextBox control = new();
        control.Dispose();
        Assert.Empty(control.Text);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void TextBoxBase_Text_Set_GetReturnsExpected(string value, string expected)
    {
        using SubTextBox control = new()
        {
            Text = value
        };
        Assert.Equal(expected, control.Text);
        Assert.Equal(expected.Length, control.TextLength);
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.False(control.Modified);
        Assert.False(control.CanUndo);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.Equal(expected.Length, control.TextLength);
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.False(control.Modified);
        Assert.False(control.CanUndo);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Text_SetWithSelection_TestData()
    {
        yield return new object[] { string.Empty, 0, 2, null, string.Empty, 0, 0, string.Empty };
        yield return new object[] { string.Empty, 0, 2, "t", "t", 0, 1, "t" };
        yield return new object[] { string.Empty, 0, 2, "text", "text", 0, 2, "te" };

        yield return new object[] { string.Empty, 1, 2, null, string.Empty, 0, 0, string.Empty };
        yield return new object[] { string.Empty, 1, 2, "t", "t", 0, 1, "t" };
        yield return new object[] { string.Empty, 1, 2, "text", "text", 0, 2, "te" };

        yield return new object[] { "text", 0, 2, null, string.Empty, 0, 0, string.Empty };
        yield return new object[] { "text", 0, 2, "t", "t", 0, 1, "t" };
        yield return new object[] { "text", 0, 2, "te", "te", 0, 2, "te" };
        yield return new object[] { "text", 0, 2, "tex", "tex", 0, 2, "te" };

        yield return new object[] { "text", 1, 2, null, string.Empty, 0, 0, string.Empty };
        yield return new object[] { "text", 1, 2, "t", "t", 1, 0, string.Empty };
        yield return new object[] { "text", 1, 2, "te", "te", 1, 1, "e" };
        yield return new object[] { "text", 1, 2, "tex", "tex", 1, 2, "ex" };
    }

    [WinFormsTheory]
    [MemberData(nameof(Text_SetWithSelection_TestData))]
    public void TextBoxBase_Text_SetWithSelection_GetReturnsExpected(string oldValue, int selectionStart, int selectionLength, string value, string expected, int expectedSelectionStart, int expectedSelectionLength, string expectedSelectedText)
    {
        using SubTextBox control = new()
        {
            Text = oldValue,
            SelectionStart = selectionStart,
            SelectionLength = selectionLength,
        };

        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.Equal(expected.Length, control.TextLength);
        Assert.Equal(expectedSelectionStart, control.SelectionStart);
        Assert.Equal(expectedSelectionLength, control.SelectionLength);
        Assert.Equal(expectedSelectedText, control.SelectedText);
        Assert.False(control.Modified);
        Assert.False(control.CanUndo);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.Equal(expected.Length, control.TextLength);
        Assert.Equal(expectedSelectionStart, control.SelectionStart);
        Assert.Equal(expectedSelectionLength, control.SelectionLength);
        Assert.Equal(expectedSelectedText, control.SelectedText);
        Assert.False(control.Modified);
        Assert.False(control.CanUndo);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void TextBoxBase_Text_SetModified_GetReturnsExpected(string value, string expected)
    {
        using SubTextBox control = new()
        {
            Modified = true,
            Text = value
        };
        Assert.Equal(expected, control.Text);
        Assert.Equal(expected.Length, control.TextLength);
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.True(control.Modified);
        Assert.False(control.CanUndo);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.Equal(expected.Length, control.TextLength);
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.True(control.Modified);
        Assert.False(control.CanUndo);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void TextBoxBase_Text_SetWithHandle_GetReturnsExpected(string value, string expected)
    {
        using SubTextBox control = new();
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

    [WinFormsTheory]
    [InlineData(null, "", false)]
    [InlineData("", "", true)]
    [InlineData("text", "text", false)]
    public void TextBoxBase_Text_SetModifiedWithHandle_GetReturnsExpected(string value, string expected, bool expectedModified)
    {
        using SubTextBox control = new()
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
        Assert.Equal(expectedModified, control.Modified);
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
        Assert.Equal(expectedModified, control.Modified);
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
    public void TextBoxBase_Text_SetWithSelectionWith_GetReturnsExpected(string oldValue, int selectionStart, int selectionLength, string value, string expected)
    {
        using SubTextBox control = new()
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
    public void TextBoxBase_Text_SetWithHandler_CallsTextChanged()
    {
        using SubTextBox control = new();
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

        // Set same.
        control.Text = "text";
        Assert.Equal("text", control.Text);
        Assert.Equal(1, callCount);

        // Set different.
        control.Text = null;
        Assert.Empty(control.Text);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.TextChanged -= handler;
        control.Text = "text";
        Assert.Equal("text", control.Text);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void TextBoxBase_Text_SetWithHandlerWithHandle_CallsTextChanged()
    {
        using SubTextBox control = new();
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
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.Text = null;
        Assert.Empty(control.Text);
        Assert.Equal(2, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.TextChanged -= handler;
        control.Text = "text";
        Assert.Equal("text", control.Text);
        Assert.Equal(2, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void TextBoxBase_Text_SetCantCreateHandle_GetReturnsExpected(string value, string expected)
    {
        using CantCreateHandleTextBox control = new();
        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void TextBoxBase_Text_SetDisposed_ThrowsObjectDisposedException(string value, string expected)
    {
        using TextBox control = new();
        control.Dispose();

        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBase_TextLength_GetDefaultWithoutHandle_Success()
    {
        using SubTextBox control = new();
        Assert.Equal(0, control.TextLength);
        Assert.False(control.IsHandleCreated);

        // Call again.
        Assert.Equal(0, control.TextLength);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBase_TextLength_GetDefaultWithHandle_ReturnsExpected()
    {
        using SubTextBox control = new();
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
    [InlineData("a\0b", 3)]
    [InlineData("a", 1)]
    [InlineData("\ud83c\udf09", 2)]
    public void TextBoxBase_TextLength_GetSetWithHandle_Success(string text, int expected)
    {
        using SubTextBox control = new()
        {
            Text = text
        };
        Assert.Equal(expected, control.TextLength);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData("", 0)]
    [InlineData("a\0b", 1)]
    [InlineData("a", 1)]
    [InlineData("\ud83c\udf09", 2)]
    public void TextBoxBase_TextLength_GetWithHandle_ReturnsExpected(string text, int expected)
    {
        using SubTextBox control = new();
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

    [WinFormsFact]
    public void TextBoxBase_TextLength_GetCantCreateHandle_GetReturnsExpected()
    {
        using CantCreateHandleTextBox control = new();
        Assert.Equal(0, control.TextLength);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBase_TextLength_GetDisposed_ReturnsExpected()
    {
        using TextBox control = new();
        control.Dispose();
        Assert.Equal(0, control.TextLength);
    }

    [WinFormsTheory]
    [BoolData]
    public void TextBoxBase_WordWrap_Set_GetReturnsExpected(bool value)
    {
        using SubTextBox control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.WordWrap = value;
        Assert.Equal(value, control.WordWrap);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.WordWrap = value;
        Assert.Equal(value, control.WordWrap);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.WordWrap = !value;
        Assert.Equal(!value, control.WordWrap);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 1)]
    public void TextBoxBase_WordWrap_SetWithParent_GetReturnsExpected(bool value, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using SubTextBox control = new()
        {
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("WordWrap", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.WordWrap = value;
            Assert.Equal(value, control.WordWrap);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            control.WordWrap = value;
            Assert.Equal(value, control.WordWrap);
            Assert.Equal(expectedParentLayoutCallCount * 2, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set different.
            control.WordWrap = !value;
            Assert.Equal(!value, control.WordWrap);
            Assert.Equal(expectedParentLayoutCallCount * 2 + 1, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [BoolData]
    public void TextBoxBase_WordWrap_SetNotAutoSizeWithParent_GetReturnsExpected(bool value)
    {
        using Control parent = new();
        using SubTextBox control = new()
        {
            Parent = parent,
            AutoSize = false
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            control.WordWrap = value;
            Assert.Equal(value, control.WordWrap);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            control.WordWrap = value;
            Assert.Equal(value, control.WordWrap);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set different.
            control.WordWrap = !value;
            Assert.Equal(!value, control.WordWrap);
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

    [WinFormsTheory]
    [InlineData(true, 0)]
    [InlineData(false, 1)]
    public void TextBoxBase_WordWrap_SetWithHandle_GetReturnsExpected(bool value, int expectedCreatedCallCount)
    {
        using SubTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.WordWrap = value;
        Assert.Equal(value, control.WordWrap);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        control.WordWrap = value;
        Assert.Equal(value, control.WordWrap);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set different.
        control.WordWrap = !value;
        Assert.Equal(!value, control.WordWrap);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount + 1, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(null, "", false)]
    [InlineData("", "", false)]
    [InlineData("text", "text", true)]
    public void TextBoxBase_AppendText_InvokeEmpty_Success(string text, string expected, bool expectedHandleCreated)
    {
        using SubTextBox control = new();
        control.AppendText(text);
        Assert.Equal(expected, control.Text);
        Assert.Equal(expected.Length, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.Equal(expectedHandleCreated, control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(null, 0, false)]
    [InlineData("", 0, false)]
    [InlineData("text", 7, true)]
    public void TextBoxBase_AppendText_InvokeNotEmpty_Success(string text, int expectedSelectionStart, bool expectedHandleCreated)
    {
        using SubTextBox control = new()
        {
            Text = "abc"
        };
        control.AppendText(text);
        Assert.Equal("abc" + text, control.Text);
        Assert.Equal(expectedSelectionStart, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.Equal(expectedHandleCreated, control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(null, 1, 2, "bc", false)]
    [InlineData("", 1, 2, "bc", false)]
    [InlineData("text", 7, 0, "", true)]
    public void TextBoxBase_AppendText_InvokeNotEmptyWithSelectionStart_Success(string text, int expectedSelectionStart, int expectedSelectionLength, string expectedSelectedText, bool expectedHandleCreated)
    {
        using SubTextBox control = new()
        {
            Text = "abc",
            SelectionStart = 1,
            SelectionLength = 2
        };
        control.AppendText(text);
        Assert.Equal("abc" + text, control.Text);
        Assert.Equal(expectedSelectionStart, control.SelectionStart);
        Assert.Equal(expectedSelectionLength, control.SelectionLength);
        Assert.Equal(expectedSelectedText, control.SelectedText);
        Assert.Equal(expectedHandleCreated, control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(null, 1, 2, "bc", false)]
    [InlineData("", 1, 2, "bc", false)]
    [InlineData("text", 1, 2, "bc", true)]
    public void TextBoxBase_AppendText_InvokeNotEmptyWithSelectionStartZeroWidth_Success(string text, int expectedSelectionStart, int expectedSelectionLength, string expectedSelectedText, bool expectedHandleCreated)
    {
        using SubTextBox control = new()
        {
            Text = "abc",
            SelectionStart = 1,
            SelectionLength = 2,
            Width = 0
        };
        control.AppendText(text);
        Assert.Equal("abc" + text, control.Text);
        Assert.Equal(expectedSelectionStart, control.SelectionStart);
        Assert.Equal(expectedSelectionLength, control.SelectionLength);
        Assert.Equal(expectedSelectedText, control.SelectedText);
        Assert.Equal(expectedHandleCreated, control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(null, 1, 2, "bc", false)]
    [InlineData("", 1, 2, "bc", false)]
    [InlineData("text", 1, 2, "bc", true)]
    public void TextBoxBase_AppendText_InvokeNotEmptyWithSelectionStartZeroHeight_Success(string text, int expectedSelectionStart, int expectedSelectionLength, string expectedSelectedText, bool expectedHandleCreated)
    {
        using SubTextBox control = new()
        {
            Text = "abc",
            SelectionStart = 1,
            SelectionLength = 2,
            AutoSize = false,
            Height = 0
        };
        control.AppendText(text);
        Assert.Equal("abc" + text, control.Text);
        Assert.Equal(expectedSelectionStart, control.SelectionStart);
        Assert.Equal(expectedSelectionLength, control.SelectionLength);
        Assert.Equal(expectedSelectedText, control.SelectedText);
        Assert.Equal(expectedHandleCreated, control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("text", "text")]
    public void TextBoxBase_AppendText_InvokeEmptyWithHandle_Success(string text, string expected)
    {
        using SubTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.AppendText(text);
        Assert.Equal(expected, control.Text);
        Assert.Equal(expected.Length, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(null, 0)]
    [InlineData("", 0)]
    [InlineData("text", 7)]
    public void TextBoxBase_AppendText_InvokeNotEmptyWithHandle_Success(string text, int expectedSelectionStart)
    {
        using SubTextBox control = new()
        {
            Text = "abc"
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.AppendText(text);
        Assert.Equal("abc" + text, control.Text);
        Assert.Equal(expectedSelectionStart, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TextBoxBase_Clear_InvokeEmpty_Success()
    {
        using SubTextBox control = new();
        control.Clear();
        Assert.Empty(control.Text);
        Assert.False(control.IsHandleCreated);

        control.Clear();
        Assert.Empty(control.Text);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBase_Clear_InvokeNotEmpty_Success()
    {
        using SubTextBox control = new()
        {
            Text = "Text"
        };

        control.Clear();
        Assert.Empty(control.Text);
        Assert.False(control.IsHandleCreated);

        control.Clear();
        Assert.Empty(control.Text);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBase_Clear_InvokeEmptyWithHandle_Success()
    {
        using SubTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Clear();
        Assert.Empty(control.Text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        control.Clear();
        Assert.Empty(control.Text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TextBoxBase_Clear_InvokeNotEmptyWithHandle_Success()
    {
        using SubTextBox control = new()
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

        control.Clear();
        Assert.Empty(control.Text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        control.Clear();
        Assert.Empty(control.Text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TextBoxBase_ClearUndo_Invoke_Nop()
    {
        using SubTextBox control = new();
        control.ClearUndo();
        Assert.False(control.CanUndo);
        Assert.False(control.IsHandleCreated);

        control.ClearUndo();
        Assert.False(control.CanUndo);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBase_ClearUndo_InvokeWithHandle_Success()
    {
        using SubTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ClearUndo();
        Assert.False(control.CanUndo);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        control.ClearUndo();
        Assert.False(control.CanUndo);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TextBoxBase_CreateHandle_Invoke_Success()
    {
        using SubTextBox control = new();
        control.CreateHandle();
        Assert.True(control.Created);
        Assert.True(control.IsHandleCreated);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
    }

    [WinFormsFact]
    public unsafe void TextBoxBase_CreateHandle_GetSel_Success()
    {
        using SubTextBox control = new()
        {
            Text = "Text",
            SelectionStart = 1,
            SelectionLength = 2
        };

        control.CreateHandle();
        int selectionStart = 0;
        int selectionEnd = 0;
        LRESULT result = PInvokeCore.SendMessage(
            control,
            PInvokeCore.EM_GETSEL,
            (WPARAM)(&selectionStart),
            (LPARAM)(&selectionEnd));
        Assert.Equal(1, PARAM.LOWORD(result));
        Assert.Equal(3, PARAM.HIWORD(result));
        Assert.Equal(1, selectionStart);
        Assert.Equal(3, selectionEnd);
    }

    [WinFormsFact]
    public void TextBoxBase_Cut_PasteEmpty_Success()
    {
        using SubTextBox control = new();
        control.Cut();
        Assert.Empty(control.Text);
        Assert.True(control.IsHandleCreated);

        control.Text = "text";
        control.SelectionLength = 2;
        Assert.Equal("text", control.Text);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBase_Cut_PasteEmptyWithHandle_Success()
    {
        using SubTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Cut();
        Assert.Empty(control.Text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        control.Text = "text";
        control.SelectionLength = 2;
        Assert.Equal("text", control.Text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TextBoxBase_DeselectAll_InvokeEmpty_Success()
    {
        using SubTextBox control = new();
        control.DeselectAll();
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBase_DeselectAll_InvokeNotEmpty_Success()
    {
        using SubTextBox control = new()
        {
            Text = "text",
            SelectionStart = 1,
            SelectionLength = 2
        };
        control.DeselectAll();
        Assert.Equal(1, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBase_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using SubTextBox control = new();
        Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
    }

    public static IEnumerable<object[]> GetCharFromPosition_TestData()
    {
        yield return new object[] { Point.Empty };
        yield return new object[] { new Point(-1, -2) };
        yield return new object[] { new Point(1, 2) };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetCharFromPosition_TestData))]
    public void TextBoxBase_GetCharFromPosition_InvokeEmpty_Success(Point pt)
    {
        using SubTextBox control = new();
        Assert.Equal('\0', control.GetCharFromPosition(pt));
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBase_GetCharFromPosition_InvokeNotEmptyValid_Success()
    {
        using SubTextBox control = new()
        {
            Text = "Text"
        };

        Assert.NotEqual('\0', control.GetCharFromPosition(new Point(10, 2)));
        Assert.True(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> GetCharFromPosition_NotEmptyInvalid_TestData()
    {
        yield return new object[] { Point.Empty, "T" };
        yield return new object[] { new Point(-1, -2), "t" };
        yield return new object[] { new Point(1, 2), "T" };
        yield return new object[] { new Point(int.MaxValue, int.MaxValue), "t" };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetCharFromPosition_NotEmptyInvalid_TestData))]
    public void TextBoxBase_GetCharFromPosition_InvokeNotEmptyInvalid_Success(Point pt, char expected)
    {
        using SubTextBox control = new()
        {
            Text = "Text"
        };
        Assert.Equal(expected, control.GetCharFromPosition(pt));
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(GetCharFromPosition_TestData))]
    public void TextBoxBase_GetCharFromPosition_InvokeEmptyWithHandle_Success(Point pt)
    {
        using SubTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal('\0', control.GetCharFromPosition(pt));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TextBoxBase_GetCharFromPosition_InvokeNotEmptyValidWithHandle_Success()
    {
        using SubTextBox control = new()
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

        Assert.NotEqual('\0', control.GetCharFromPosition(new Point(10, 2)));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(GetCharFromPosition_NotEmptyInvalid_TestData))]
    public void TextBoxBase_GetCharFromPosition_InvokeNotEmptyInvalidWithHandle_Success(Point pt, char expected)
    {
        using SubTextBox control = new()
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

        Assert.Equal(expected, control.GetCharFromPosition(pt));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> GetCharIndexFromPosition_TestData()
    {
        yield return new object[] { Point.Empty };
        yield return new object[] { new Point(-1, -2) };
        yield return new object[] { new Point(1, 2) };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetCharIndexFromPosition_TestData))]
    public void TextBoxBase_GetCharIndexFromPosition_InvokeEmpty_Success(Point pt)
    {
        using SubTextBox control = new();
        Assert.Equal(0, control.GetCharIndexFromPosition(pt));
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBase_GetCharIndexFromPosition_InvokeNotEmptyValid_Success()
    {
        using SubTextBox control = new()
        {
            Text = "Text"
        };

        int index = control.GetCharIndexFromPosition(new Point(10, 2));
        Assert.True(index is > 0 and < 4);
        Assert.True(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> GetCharIndexFromPosition_NotEmptyInvalid_TestData()
    {
        yield return new object[] { Point.Empty, 0 };
        yield return new object[] { new Point(-1, -2), 3 };
        yield return new object[] { new Point(1, 2), 0 };
        yield return new object[] { new Point(int.MaxValue, int.MaxValue), 3 };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetCharIndexFromPosition_NotEmptyInvalid_TestData))]
    public void TextBoxBase_GetCharIndexFromPosition_InvokeNotEmptyInvalid_Success(Point pt, int expected)
    {
        using SubTextBox control = new()
        {
            Text = "Text"
        };
        Assert.Equal(expected, control.GetCharIndexFromPosition(pt));
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(GetCharIndexFromPosition_TestData))]
    public void TextBoxBase_GetCharIndexFromPosition_InvokeEmptyWithHandle_Success(Point pt)
    {
        using SubTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(0, control.GetCharIndexFromPosition(pt));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TextBoxBase_GetCharIndexFromPosition_InvokeNotEmptyValidWithHandle_Success()
    {
        using SubTextBox control = new()
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

        int index = control.GetCharIndexFromPosition(new Point(10, 2));
        Assert.True(index is > 0 and < 4);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(GetCharIndexFromPosition_NotEmptyInvalid_TestData))]
    public void TextBoxBase_GetCharIndexFromPosition_InvokeNotEmptyInvalidWithHandle_Success(Point pt, int expected)
    {
        using SubTextBox control = new()
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

        Assert.Equal(expected, control.GetCharIndexFromPosition(pt));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> GetCharIndexFromPosition_CustomCharFromPos_TestData()
    {
        yield return new object[] { string.Empty, (IntPtr)(-1), 0 };
        yield return new object[] { string.Empty, (IntPtr)0, 0 };
        yield return new object[] { string.Empty, (IntPtr)1, 0 };
        yield return new object[] { string.Empty, (IntPtr)short.MaxValue, 0 };
        yield return new object[] { string.Empty, (IntPtr)int.MaxValue, 0 };

        yield return new object[] { "text", (IntPtr)(-1), 3 };
        yield return new object[] { "text", (IntPtr)0, 0 };
        yield return new object[] { "text", (IntPtr)1, 1 };
        yield return new object[] { "text", (IntPtr)3, 3 };
        yield return new object[] { "text", (IntPtr)4, 3 };
        yield return new object[] { "text", PARAM.FromLowHigh(1, 2), 1 };
        yield return new object[] { "text", (IntPtr)short.MaxValue, 3 };
        yield return new object[] { "text", (IntPtr)int.MaxValue, 3 };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetCharIndexFromPosition_CustomCharFromPos_TestData))]
    public void TextBoxBase_GetCharIndexFromPosition_CustomCharFromPos_Success(string text, IntPtr result, int expected)
    {
        using CustomCharFromPosTextBox control = new()
        {
            Text = text,
            CharFromPosResult = result
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(expected, control.GetCharIndexFromPosition(new Point(1, 2)));
    }

    private class CustomCharFromPosTextBox : TextBox
    {
        public IntPtr CharFromPosResult { get; set; }

        protected override unsafe void WndProc(ref Message m)
        {
            if (m.Msg == (int)PInvokeCore.EM_CHARFROMPOS)
            {
                Assert.Equal(IntPtr.Zero, m.WParam);
                Assert.Equal(2, PARAM.SignedHIWORD(m.LParam));
                Assert.Equal(1, PARAM.SignedLOWORD(m.LParam));
                m.Result = CharFromPosResult;
                return;
            }

            base.WndProc(ref m);
        }
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    public void TextBoxBase_GetFirstCharIndexFromLine_InvokeEmpty_ReturnsExpected(int lineNumber)
    {
        using SubTextBox control = new();
        Assert.Equal(0, control.GetFirstCharIndexFromLine(lineNumber));
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    public void TextBoxBase_GetFirstCharIndexFromLine_InvokeNotEmpty_ReturnsExpected(int lineNumber)
    {
        using SubTextBox control = new();
        Assert.Equal(0, control.GetFirstCharIndexFromLine(lineNumber));
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    public void TextBoxBase_GetFirstCharIndexFromLine_InvokeEmptyWithHandle_ReturnsExpected(int lineNumber)
    {
        using SubTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(0, control.GetFirstCharIndexFromLine(lineNumber));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    public void TextBoxBase_GetFirstCharIndexFromLine_InvokeNotEmptyWithHandle_ReturnsExpected(int lineNumber)
    {
        using SubTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(0, control.GetFirstCharIndexFromLine(lineNumber));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> GetFirstCharIndexFromLine_CustomLineFromChar_TestData()
    {
        yield return new object[] { (IntPtr)(-1), -1 };
        yield return new object[] { IntPtr.Zero, 0 };
        yield return new object[] { (IntPtr)1, 1 };
        yield return new object[] { (IntPtr)int.MaxValue, 0x7FFFFFFF };
        yield return new object[] { PARAM.FromLowHigh(1, 2), 0x20001 };
        yield return new object[] { PARAM.FromLowHigh(int.MaxValue, int.MaxValue), -1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetFirstCharIndexFromLine_CustomLineFromChar_TestData))]
    public void TextBoxBase_GetFirstCharIndexFromLine_CustomLineIndex_Success(IntPtr result, int expected)
    {
        using CustomLineIndexTextBox control = new()
        {
            ExpectedWParam = 1,
            LineIndexResult = result
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(expected, control.GetFirstCharIndexFromLine(1));
    }

    [WinFormsFact]
    public void TextBoxBase_GetFirstCharIndexFromLine_NegativeLineNumber_ThrowsArgumentOutOfRangeException()
    {
        using SubTextBox control = new();
        Assert.Throws<ArgumentOutOfRangeException>("lineNumber", () => control.GetFirstCharIndexFromLine(-1));
    }

    [WinFormsFact]
    public void TextBoxBase_GetFirstCharIndexOfCurrentLine_InvokeEmpty_ReturnsExpected()
    {
        using SubTextBox control = new();
        Assert.Equal(0, control.GetFirstCharIndexOfCurrentLine());
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBase_GetFirstCharIndexOfCurrentLine_InvokeNotEmpty_ReturnsExpected()
    {
        using SubTextBox control = new()
        {
            Text = "text"
        };
        Assert.Equal(0, control.GetFirstCharIndexOfCurrentLine());
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBase_GetFirstCharIndexOfCurrentLine_InvokeEmptyWithHandle_ReturnsExpected()
    {
        using SubTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(0, control.GetFirstCharIndexOfCurrentLine());
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TextBoxBase_GetFirstCharIndexOfCurrentLine_InvokeNotEmptyWithHandle_ReturnsExpected()
    {
        using SubTextBox control = new()
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

        Assert.Equal(0, control.GetFirstCharIndexOfCurrentLine());
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> GetFirstCharIndexOfCurrentLine_CustomLineFromChar_TestData()
    {
        yield return new object[] { (IntPtr)(-1), -1 };
        yield return new object[] { IntPtr.Zero, 0 };
        yield return new object[] { (IntPtr)1, 1 };
        yield return new object[] { (IntPtr)int.MaxValue, 0x7FFFFFFF };
        yield return new object[] { PARAM.FromLowHigh(1, 2), 0x20001 };
        yield return new object[] { PARAM.FromLowHigh(int.MaxValue, int.MaxValue), -1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetFirstCharIndexOfCurrentLine_CustomLineFromChar_TestData))]
    public void TextBoxBase_GetFirstCharIndexOfCurrentLine_CustomLineIndex_Success(IntPtr result, int expected)
    {
        using CustomLineIndexTextBox control = new()
        {
            ExpectedWParam = -1,
            LineIndexResult = result
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(expected, control.GetFirstCharIndexOfCurrentLine());
    }

    private class CustomLineIndexTextBox : TextBox
    {
        public IntPtr ExpectedWParam { get; set; }
        public IntPtr LineIndexResult { get; set; }

        protected override unsafe void WndProc(ref Message m)
        {
            if (m.Msg == (int)PInvokeCore.EM_LINEINDEX)
            {
                Assert.Equal(ExpectedWParam, m.WParam);
                Assert.Equal(IntPtr.Zero, m.LParam);
                m.Result = LineIndexResult;
                return;
            }

            base.WndProc(ref m);
        }
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void TextBoxBase_GetLineFromCharIndex_InvokeEmpty_Success(int index)
    {
        using SubTextBox control = new();
        Assert.Equal(0, control.GetLineFromCharIndex(index));
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(4)]
    [InlineData(5)]
    public void TextBoxBase_GetLineFromCharIndex_InvokeNotEmpty_Success(int index)
    {
        using SubTextBox control = new()
        {
            Text = "text"
        };
        control.CreateControl();
        Assert.Equal(0, control.GetLineFromCharIndex(index));
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void TextBoxBase_GetLineFromCharIndex_InvokeEmptyWithHandle_Success(int index)
    {
        using SubTextBox control = new();
        control.CreateControl();
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
    public void TextBoxBase_GetLineFromCharIndex_InvokeNotEmptyWithHandle_Success(int index)
    {
        using SubTextBox control = new()
        {
            Text = "text"
        };
        control.CreateControl();
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
        yield return new object[] { (IntPtr)(-1), -1 };
        yield return new object[] { IntPtr.Zero, 0 };
        yield return new object[] { (IntPtr)1, 1 };
        yield return new object[] { (IntPtr)int.MaxValue, 0x7FFFFFFF };
        yield return new object[] { PARAM.FromLowHigh(1, 2), 0x20001 };
        yield return new object[] { PARAM.FromLowHigh(int.MaxValue, int.MaxValue), -1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetLineFromCharIndex_CustomLineFromChar_TestData))]
    public void TextBoxBase_GetLineFromCharIndex_CustomLineFromChar_Success(IntPtr result, int expected)
    {
        using CustomLineFromCharTextBox control = new()
        {
            LineFromCharResult = result
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(expected, control.GetLineFromCharIndex(1));
    }

    private class CustomLineFromCharTextBox : TextBox
    {
        public IntPtr LineFromCharResult { get; set; }

        protected override unsafe void WndProc(ref Message m)
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

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void TextBoxBase_GetPositionFromCharIndex_InvokeEmpty_ReturnsEmpty(int index)
    {
        using TextBox control = new();
        Assert.Equal(Point.Empty, control.GetPositionFromCharIndex(index));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void TextBoxBase_GetPositionFromCharIndex_InvokeValidIndexNotEmpty_ReturnsExpected(int index)
    {
        using TextBox control = new()
        {
            Text = "text"
        };
        Point pt = control.GetPositionFromCharIndex(index);
        Assert.True(pt.X > 0);
        Assert.Equal(0, pt.Y);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(4)]
    [InlineData(5)]
    public void TextBoxBase_GetPositionFromCharIndex_InvokeInvalidIndexNotEmpty_ReturnsEmpty(int index)
    {
        using TextBox control = new()
        {
            Text = "text"
        };
        Assert.Equal(Point.Empty, control.GetPositionFromCharIndex(index));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void TextBoxBase_GetPositionFromCharIndex_InvokeEmptyWithHandle_ReturnsEmpty(int index)
    {
        using TextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(Point.Empty, control.GetPositionFromCharIndex(index));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void TextBoxBase_GetPositionFromCharIndex_InvokeValidIndexNotEmptyWithHandle_ReturnsExpected(int index)
    {
        using TextBox control = new()
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

        Point pt = control.GetPositionFromCharIndex(index);
        Assert.True(pt.X > 0);
        Assert.Equal(0, pt.Y);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(4)]
    [InlineData(5)]
    public void TextBoxBase_GetPositionFromCharIndex_InvokeInvalidIndexNotEmptyWithHandle_ReturnsEmpty(int index)
    {
        using TextBox control = new()
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

        Assert.Equal(Point.Empty, control.GetPositionFromCharIndex(index));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> GetPositionFromCharIndex_CustomPosFromChar_TestData()
    {
        yield return new object[] { IntPtr.Zero, Point.Empty };
        yield return new object[] { (IntPtr)1, new Point(1, 0) };
        yield return new object[] { PARAM.FromLowHigh(1, 2), new Point(1, 2) };
        yield return new object[] { PARAM.FromLowHigh(-1, -2), new Point(-1, -2) };
        yield return new object[] { PARAM.FromLowHigh(int.MaxValue, int.MaxValue), new Point(-1, -1) };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetPositionFromCharIndex_CustomPosFromChar_TestData))]
    public void TextBoxBase_GetPositionFromCharIndex_CustomPosFromChar_Success(IntPtr result, Point expected)
    {
        using CustomPosFromCharTextBox control = new()
        {
            Text = "text",
            PosFromCharResult = result
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(expected, control.GetPositionFromCharIndex(1));
    }

    private class CustomPosFromCharTextBox : TextBox
    {
        public IntPtr PosFromCharResult { get; set; }

        protected override unsafe void WndProc(ref Message m)
        {
            if (m.Msg == (int)PInvokeCore.EM_POSFROMCHAR)
            {
                Assert.Equal(1, m.WParam);
                Assert.Equal(IntPtr.Zero, m.LParam);
                m.Result = PosFromCharResult;
                return;
            }

            base.WndProc(ref m);
        }
    }

    public static IEnumerable<object[]> GetPreferredSize_TestData()
    {
        foreach (bool multiline in new bool[] { true, false })
        {
            foreach (bool wordWrap in new bool[] { true, false })
            {
                yield return new object[] { multiline, wordWrap, BorderStyle.Fixed3D, Size.Empty, new Size(4, s_preferredHeight) };
                yield return new object[] { multiline, wordWrap, BorderStyle.Fixed3D, new Size(-1, -2), new Size(4, s_preferredHeight) };
                yield return new object[] { multiline, wordWrap, BorderStyle.Fixed3D, new Size(10, 20), new Size(4, s_preferredHeight) };
                yield return new object[] { multiline, wordWrap, BorderStyle.Fixed3D, new Size(30, 40), new Size(4, s_preferredHeight) };
                yield return new object[] { multiline, wordWrap, BorderStyle.Fixed3D, new Size(int.MaxValue, int.MaxValue), new Size(4, s_preferredHeight) };

                yield return new object[] { multiline, wordWrap, BorderStyle.FixedSingle, Size.Empty, new Size(4, s_preferredHeight) };
                yield return new object[] { multiline, wordWrap, BorderStyle.FixedSingle, new Size(-1, -2), new Size(4, s_preferredHeight) };
                yield return new object[] { multiline, wordWrap, BorderStyle.FixedSingle, new Size(10, 20), new Size(4, s_preferredHeight) };
                yield return new object[] { multiline, wordWrap, BorderStyle.FixedSingle, new Size(30, 40), new Size(4, s_preferredHeight) };
                yield return new object[] { multiline, wordWrap, BorderStyle.FixedSingle, new Size(int.MaxValue, int.MaxValue), new Size(4, s_preferredHeight) };

                yield return new object[] { multiline, wordWrap, BorderStyle.None, Size.Empty, new Size(0, Control.DefaultFont.Height) };
                yield return new object[] { multiline, wordWrap, BorderStyle.None, new Size(-1, -2), new Size(0, Control.DefaultFont.Height) };
                yield return new object[] { multiline, wordWrap, BorderStyle.None, new Size(10, 20), new Size(0, Control.DefaultFont.Height) };
                yield return new object[] { multiline, wordWrap, BorderStyle.None, new Size(30, 40), new Size(0, Control.DefaultFont.Height) };
                yield return new object[] { multiline, wordWrap, BorderStyle.None, new Size(int.MaxValue, int.MaxValue), new Size(0, Control.DefaultFont.Height) };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(GetPreferredSize_TestData))]
    public void TextBox_GetPreferredSize_Invoke_ReturnsExpected(bool multiline, bool wordWrap, BorderStyle borderStyle, Size proposedSize, Size expected)
    {
        using SubTextBox control = new()
        {
            Multiline = multiline,
            WordWrap = wordWrap,
            BorderStyle = borderStyle
        };
        Assert.Equal(expected, control.GetPreferredSize(proposedSize));
        Assert.False(control.IsHandleCreated);

        // Call again.
        Assert.Equal(expected, control.GetPreferredSize(proposedSize));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(GetPreferredSize_TestData))]
    public void TextBox_GetPreferredSize_InvokeWithPadding_ReturnsExpected(bool multiline, bool wordWrap, BorderStyle borderStyle, Size proposedSize, Size expected)
    {
        Padding padding = new(1, 2, 3, 4);
        using SubTextBox control = new()
        {
            Padding = padding,
            Multiline = multiline,
            WordWrap = wordWrap,
            BorderStyle = borderStyle
        };
        Assert.Equal(expected + padding.Size, control.GetPreferredSize(proposedSize));
        Assert.False(control.IsHandleCreated);

        // Call again.
        Assert.Equal(expected + padding.Size, control.GetPreferredSize(proposedSize));
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> GetPreferredSize_WithText_TestData()
    {
        yield return new object[] { BorderStyle.Fixed3D, Size.Empty, 4, s_preferredHeight };
        yield return new object[] { BorderStyle.Fixed3D, new Size(-1, -2), 4, s_preferredHeight };
        yield return new object[] { BorderStyle.Fixed3D, new Size(10, 20), 4, s_preferredHeight };
        yield return new object[] { BorderStyle.Fixed3D, new Size(30, 40), 4, s_preferredHeight };
        yield return new object[] { BorderStyle.Fixed3D, new Size(int.MaxValue, int.MaxValue), 4, s_preferredHeight };

        yield return new object[] { BorderStyle.FixedSingle, Size.Empty, 4, s_preferredHeight };
        yield return new object[] { BorderStyle.FixedSingle, new Size(-1, -2), 4, s_preferredHeight };
        yield return new object[] { BorderStyle.FixedSingle, new Size(10, 20), 4, s_preferredHeight };
        yield return new object[] { BorderStyle.FixedSingle, new Size(30, 40), 4, s_preferredHeight };
        yield return new object[] { BorderStyle.FixedSingle, new Size(int.MaxValue, int.MaxValue), 4, s_preferredHeight };

        yield return new object[] { BorderStyle.None, Size.Empty, 0, Control.DefaultFont.Height };
        yield return new object[] { BorderStyle.None, new Size(-1, -2), 0, Control.DefaultFont.Height };
        yield return new object[] { BorderStyle.None, new Size(10, 20), 0, Control.DefaultFont.Height };
        yield return new object[] { BorderStyle.None, new Size(30, 40), 0, Control.DefaultFont.Height };
        yield return new object[] { BorderStyle.None, new Size(int.MaxValue, int.MaxValue), 0, Control.DefaultFont.Height };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetPreferredSize_WithText_TestData))]
    public void TextBox_GetPreferredSize_InvokeWithText_ReturnsExpected(BorderStyle borderStyle, Size proposedSize, int expectedAdditionalWidth, int expectedHeight)
    {
        using SubTextBox control = new()
        {
            Text = "Text",
            BorderStyle = borderStyle
        };
        int width = TextRenderer.MeasureText(control.Text, control.Font).Width;

        Assert.Equal(new Size(width + expectedAdditionalWidth, expectedHeight), control.GetPreferredSize(proposedSize));
        Assert.False(control.IsHandleCreated);

        // Call again.
        Assert.Equal(new Size(width + expectedAdditionalWidth, expectedHeight), control.GetPreferredSize(proposedSize));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(GetPreferredSize_WithText_TestData))]
    public void TextBox_GetPreferredSize_InvokeWithNewLineMultiline_ReturnsExpected(BorderStyle borderStyle, Size proposedSize, int expectedAdditionalWidth, int expectedMinimumHeight)
    {
        using SubTextBox control = new()
        {
            Text = "a\rn\nb",
            Multiline = true,
            BorderStyle = borderStyle
        };
        int width = TextRenderer.MeasureText(control.Text, control.Font).Width;

        Size result = control.GetPreferredSize(proposedSize);
        Assert.Equal(width + expectedAdditionalWidth, result.Width);
        Assert.True(result.Height >= expectedMinimumHeight);
        Assert.False(control.IsHandleCreated);

        // Call again.
        result = control.GetPreferredSize(proposedSize);
        Assert.Equal(width + expectedAdditionalWidth, result.Width);
        Assert.True(result.Height >= expectedMinimumHeight);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(GetPreferredSize_WithText_TestData))]
    public void TextBox_GetPreferredSize_InvokeWithNewLineNotMultiline_ReturnsExpected(BorderStyle borderStyle, Size proposedSize, int expectedAdditionalWidth, int expectedHeight)
    {
        using SubTextBox control = new()
        {
            Text = "a\rn\nb",
            BorderStyle = borderStyle
        };
        int width = TextRenderer.MeasureText(control.Text, control.Font, Size.Empty, TextFormatFlags.SingleLine).Width;

        Size result = control.GetPreferredSize(proposedSize);
        Assert.Equal(new Size(width + expectedAdditionalWidth, expectedHeight), control.GetPreferredSize(proposedSize));
        Assert.False(control.IsHandleCreated);

        // Call again.
        result = control.GetPreferredSize(proposedSize);
        Assert.Equal(new Size(width + expectedAdditionalWidth, expectedHeight), control.GetPreferredSize(proposedSize));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(ControlStyles.ContainerControl, false)]
    [InlineData(ControlStyles.UserPaint, false)]
    [InlineData(ControlStyles.Opaque, false)]
    [InlineData(ControlStyles.ResizeRedraw, false)]
    [InlineData(ControlStyles.FixedWidth, false)]
    [InlineData(ControlStyles.FixedHeight, true)]
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
    public void TextBoxBase_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
    {
        using SubTextBox control = new();
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    [WinFormsFact]
    public void TextBoxBase_GetTopLevel_Invoke_ReturnsExpected()
    {
        using SubTextBox control = new();
        Assert.False(control.GetTopLevel());
    }

    public static IEnumerable<object[]> IsInputKey_TestData()
    {
        foreach (bool multiline in new bool[] { true, false })
        {
            foreach (bool acceptsTab in new bool[] { true, false })
            {
                foreach (bool readOnly in new bool[] { true, false })
                {
                    yield return new object[] { multiline, acceptsTab, readOnly, Keys.Tab, multiline && acceptsTab };
                    yield return new object[] { multiline, acceptsTab, readOnly, Keys.Tab | Keys.Control, false };
                    yield return new object[] { multiline, acceptsTab, readOnly, Keys.Tab | Keys.Alt, false };
                    yield return new object[] { multiline, acceptsTab, readOnly, Keys.Escape, false };
                    yield return new object[] { multiline, acceptsTab, readOnly, Keys.Escape | Keys.Alt, false };
                    yield return new object[] { multiline, acceptsTab, readOnly, Keys.Back | Keys.Alt, false };
                    yield return new object[] { multiline, acceptsTab, readOnly, Keys.PageUp, true };
                    yield return new object[] { multiline, acceptsTab, readOnly, Keys.PageUp | Keys.Alt, false };
                    yield return new object[] { multiline, acceptsTab, readOnly, Keys.PageDown, true };
                    yield return new object[] { multiline, acceptsTab, readOnly, Keys.PageDown | Keys.Alt, false };
                    yield return new object[] { multiline, acceptsTab, readOnly, Keys.Home, true };
                    yield return new object[] { multiline, acceptsTab, readOnly, Keys.Home | Keys.Alt, false };
                    yield return new object[] { multiline, acceptsTab, readOnly, Keys.End, true };
                    yield return new object[] { multiline, acceptsTab, readOnly, Keys.End | Keys.Alt, false };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(IsInputKey_TestData))]
    [InlineData(true, true, true, Keys.A, false)]
    [InlineData(true, true, true, Keys.Back, false)]
    [InlineData(true, true, false, Keys.Back, true)]
    [InlineData(true, true, true, Keys.Left, false)]
    [InlineData(true, true, false, Keys.Left, false)]
    [InlineData(true, true, true, Keys.Right, false)]
    [InlineData(true, true, false, Keys.Right, false)]
    [InlineData(true, true, true, Keys.Up, false)]
    [InlineData(true, true, false, Keys.Up, false)]
    [InlineData(false, true, true, Keys.Up, false)]
    [InlineData(false, true, false, Keys.Up, false)]
    [InlineData(true, true, true, Keys.Down, false)]
    [InlineData(true, true, false, Keys.Down, false)]
    [InlineData(false, true, true, Keys.Down, false)]
    [InlineData(false, true, false, Keys.Down, false)]
    [InlineData(false, true, true, Keys.Down | Keys.Alt, false)]
    [InlineData(false, true, false, Keys.Down | Keys.Alt, false)]
    public void TextBoxBase_IsInputKey_Invoke_ReturnsExpected(bool multiline, bool acceptsTab, bool readOnly, Keys keyData, bool expected)
    {
        using SubTextBox control = new()
        {
            Multiline = multiline,
            AcceptsTab = acceptsTab,
            ReadOnly = readOnly
        };
        Assert.Equal(expected, control.IsInputKey(keyData));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(IsInputKey_TestData))]
    [InlineData(true, true, true, Keys.A, true)]
    [InlineData(true, true, true, Keys.Back, true)]
    [InlineData(true, true, false, Keys.Back, true)]
    [InlineData(true, true, true, Keys.Left, true)]
    [InlineData(true, true, false, Keys.Left, true)]
    [InlineData(true, true, true, Keys.Right, true)]
    [InlineData(true, true, false, Keys.Right, true)]
    [InlineData(true, true, true, Keys.Up, true)]
    [InlineData(true, true, false, Keys.Up, true)]
    [InlineData(false, true, true, Keys.Up, true)]
    [InlineData(false, true, false, Keys.Up, true)]
    [InlineData(true, true, true, Keys.Down, true)]
    [InlineData(true, true, false, Keys.Down, true)]
    [InlineData(false, true, true, Keys.Down, true)]
    [InlineData(false, true, false, Keys.Down, true)]
    [InlineData(false, true, true, Keys.Down | Keys.Alt, false)]
    [InlineData(false, true, false, Keys.Down | Keys.Alt, false)]
    public void TextBoxBase_IsInputKey_InvokeWithHandle_ReturnsExpected(bool multiline, bool acceptsTab, bool readOnly, Keys keyData, bool expected)
    {
        using SubTextBox control = new()
        {
            Multiline = multiline,
            AcceptsTab = acceptsTab,
            ReadOnly = readOnly
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(expected, control.IsInputKey(keyData));
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void TextBoxBase_OnAcceptsTabChanged_Invoke_CallsAcceptsTabChanged(EventArgs eventArgs)
    {
        using SubTextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.AcceptsTabChanged += handler;
        control.OnAcceptsTabChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.AcceptsTabChanged -= handler;
        control.OnAcceptsTabChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void TextBoxBase_OnBorderStyleChanged_Invoke_CallsBorderStyleChanged(EventArgs eventArgs)
    {
        using SubTextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.BorderStyleChanged += handler;
        control.OnBorderStyleChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.BorderStyleChanged -= handler;
        control.OnBorderStyleChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void TextBoxBase_OnClick_Invoke_CallsClick(EventArgs eventArgs)
    {
        using SubTextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.Click += handler;
        control.OnClick(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.Click -= handler;
        control.OnClick(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void TextBoxBase_OnFontChanged_Invoke_CallsFontChanged(EventArgs eventArgs)
    {
        using SubTextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.FontChanged += handler;
        control.OnFontChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(s_preferredHeight, control.Height);

        // Remove handler.
        control.FontChanged -= handler;
        control.OnFontChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(s_preferredHeight, control.Height);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void TextBoxBase_OnHandleCreated_Invoke_CallsHandleCreated(EventArgs eventArgs)
    {
        using SubTextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.HandleCreated += handler;
        control.OnHandleCreated(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(s_preferredHeight, control.Height);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.HandleCreated -= handler;
        control.OnHandleCreated(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(s_preferredHeight, control.Height);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void TextBoxBase_OnHandleCreated_InvokeWithHandle_CallsHandleCreated(EventArgs eventArgs)
    {
        using SubTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.HandleCreated += handler;
        control.OnHandleCreated(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(s_preferredHeight, control.Height);
        Assert.True(control.IsHandleCreated);

        // Remove handler.
        control.HandleCreated -= handler;
        control.OnHandleCreated(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(s_preferredHeight, control.Height);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void TextBoxBase_OnHandleDestroyed_Invoke_CallsHandleDestroyed(EventArgs eventArgs)
    {
        using SubTextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.HandleDestroyed += handler;
        control.OnHandleDestroyed(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.HandleDestroyed -= handler;
        control.OnHandleDestroyed(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnHandleDestroyed_TestData()
    {
        foreach (bool modified in new bool[] { true, false })
        {
            yield return new object[] { modified, null };
            yield return new object[] { modified, new EventArgs() };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnHandleDestroyed_TestData))]
    public void TextBoxBase_OnHandleDestroyed_InvokeWithHandle_CallsHandleDestroyed(bool modified, EventArgs eventArgs)
    {
        using SubTextBox control = new()
        {
            Text = "Text",
            SelectionStart = 1,
            SelectionLength = 2,
            Modified = modified
        };
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
        control.HandleDestroyed += handler;
        control.OnHandleDestroyed(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(s_preferredHeight, control.Height);
        Assert.Equal(1, control.SelectionStart);
        Assert.Equal(2, control.SelectionLength);
        Assert.Equal(modified, control.Modified);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.HandleDestroyed -= handler;
        control.OnHandleDestroyed(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(s_preferredHeight, control.Height);
        Assert.Equal(1, control.SelectionStart);
        Assert.Equal(2, control.SelectionLength);
        Assert.Equal(modified, control.Modified);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void TextBoxBase_OnHideSelectionChanged_Invoke_CallsHideSelectionChanged(EventArgs eventArgs)
    {
        using SubTextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.HideSelectionChanged += handler;
        control.OnHideSelectionChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.HideSelectionChanged -= handler;
        control.OnHideSelectionChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void TextBoxBase_OnModifiedChanged_Invoke_CallsModifiedChanged(EventArgs eventArgs)
    {
        using SubTextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.ModifiedChanged += handler;
        control.OnModifiedChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.ModifiedChanged -= handler;
        control.OnModifiedChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetMouseEventArgsTheoryData))]
    public void TextBoxBase_OnMouseClick_Invoke_CallsMouseClick(MouseEventArgs eventArgs)
    {
        using SubTextBox control = new();
        int callCount = 0;
        MouseEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.MouseClick += handler;
        control.OnMouseClick(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.MouseClick -= handler;
        control.OnMouseClick(eventArgs);
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> OnMouseUp_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0) };
        yield return new object[] { new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0) };
        yield return new object[] { new MouseEventArgs(MouseButtons.Right, 0, 0, 0, 0) };
        yield return new object[] { new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0) };
        yield return new object[] { new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0) };
        yield return new object[] { new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0) };
        yield return new object[] { new MouseEventArgs(MouseButtons.Right, 2, 0, 0, 0) };
        yield return new object[] { new MouseEventArgs(MouseButtons.Left, 3, 0, 0, 0) };
        yield return new object[] { new MouseEventArgs(MouseButtons.Right, 3, 0, 0, 0) };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseUp_TestData))]
    public void TextBoxBase_OnMouseUp_Invoke_CallsMouseUp(MouseEventArgs eventArgs)
    {
        using SubTextBox control = new();
        int clickCallCount = 0;
        control.Click += (sender, e) => clickCallCount++;
        int mouseClickCallCount = 0;
        control.MouseClick += (sender, e) => mouseClickCallCount++;
        int callCount = 0;
        MouseEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.MouseUp += handler;
        control.OnMouseUp(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, clickCallCount);
        Assert.Equal(0, mouseClickCallCount);
        Assert.Equal(eventArgs is not null && eventArgs.Button == MouseButtons.Left, control.IsHandleCreated);

        // Remove handler.
        control.MouseUp -= handler;
        control.OnMouseUp(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, clickCallCount);
        Assert.Equal(0, mouseClickCallCount);
        Assert.Equal(eventArgs is not null && eventArgs.Button == MouseButtons.Left, control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseUp_TestData))]
    public void TextBoxBase_OnMouseUp_InvokeWithHandle_CallsMouseUp(MouseEventArgs eventArgs)
    {
        using SubTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int clickCallCount = 0;
        control.Click += (sender, e) => clickCallCount++;
        int mouseClickCallCount = 0;
        control.MouseClick += (sender, e) => mouseClickCallCount++;
        int callCount = 0;
        MouseEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.MouseUp += handler;
        control.OnMouseUp(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, clickCallCount);
        Assert.Equal(0, mouseClickCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.MouseUp -= handler;
        control.OnMouseUp(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, clickCallCount);
        Assert.Equal(0, mouseClickCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void TextBoxBase_OnMultilineChanged_Invoke_CallsMultilineChanged(EventArgs eventArgs)
    {
        using SubTextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.MultilineChanged += handler;
        control.OnMultilineChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.MultilineChanged -= handler;
        control.OnMultilineChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void TextBoxBase_OnPaddingChanged_Invoke_CallsPaddingChanged(EventArgs eventArgs)
    {
        using SubTextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.PaddingChanged += handler;
        control.OnPaddingChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(s_preferredHeight, control.Height);

        // Remove handler.
        control.PaddingChanged -= handler;
        control.OnPaddingChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(s_preferredHeight, control.Height);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaintEventArgsTheoryData))]
    public void TextBoxBase_OnPaint_Invoke_CallsPaint(PaintEventArgs eventArgs)
    {
        using SubTextBox control = new();
        int callCount = 0;
        PaintEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.Paint += handler;
        control.OnPaint(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.Paint -= handler;
        control.OnPaint(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void TextBoxBase_OnReadOnlyChanged_Invoke_CallsReadOnlyChanged(EventArgs eventArgs)
    {
        using SubTextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.ReadOnlyChanged += handler;
        control.OnReadOnlyChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.ReadOnlyChanged -= handler;
        control.OnReadOnlyChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void TextBoxBase_OnTextChanged_Invoke_CallsTextChanged(EventArgs eventArgs)
    {
        using SubTextBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.TextChanged += handler;
        control.OnTextChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.TextChanged -= handler;
        control.OnTextChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> ProcessCmdKey_TestData()
    {
        foreach (bool shortcutsEnabled in new bool[] { true, false })
        {
            foreach (bool readOnly in new bool[] { true, false })
            {
                yield return new object[] { shortcutsEnabled, readOnly, Keys.None, false };
                yield return new object[] { shortcutsEnabled, readOnly, Keys.A, false };

                yield return new object[] { shortcutsEnabled, readOnly, (Keys)Shortcut.CtrlZ, !shortcutsEnabled };
                yield return new object[] { shortcutsEnabled, readOnly, (Keys)Shortcut.CtrlC, !shortcutsEnabled };
                yield return new object[] { shortcutsEnabled, readOnly, (Keys)Shortcut.CtrlX, !shortcutsEnabled };
                yield return new object[] { shortcutsEnabled, readOnly, (Keys)Shortcut.CtrlV, !shortcutsEnabled };
                yield return new object[] { shortcutsEnabled, readOnly, (Keys)Shortcut.CtrlA, true };
                yield return new object[] { shortcutsEnabled, readOnly, (Keys)Shortcut.CtrlL, !shortcutsEnabled || readOnly };
                yield return new object[] { shortcutsEnabled, readOnly, (Keys)Shortcut.CtrlR, !shortcutsEnabled || readOnly };
                yield return new object[] { shortcutsEnabled, readOnly, (Keys)Shortcut.CtrlE, !shortcutsEnabled || readOnly };
                yield return new object[] { shortcutsEnabled, readOnly, (Keys)Shortcut.CtrlY, !shortcutsEnabled };
                yield return new object[] { shortcutsEnabled, readOnly, (Keys)Shortcut.CtrlDel, !shortcutsEnabled };
                yield return new object[] { shortcutsEnabled, readOnly, (Keys)Shortcut.ShiftDel, !shortcutsEnabled };
                yield return new object[] { shortcutsEnabled, readOnly, (Keys)Shortcut.ShiftIns, !shortcutsEnabled };
                yield return new object[] { shortcutsEnabled, readOnly, (Keys)Shortcut.CtrlJ, !shortcutsEnabled || readOnly };

                yield return new object[] { shortcutsEnabled, readOnly, (Keys)Shortcut.CtrlZ | Keys.Shift, !shortcutsEnabled };
                yield return new object[] { shortcutsEnabled, readOnly, (Keys)Shortcut.CtrlC | Keys.Shift, !shortcutsEnabled };
                yield return new object[] { shortcutsEnabled, readOnly, (Keys)Shortcut.CtrlX | Keys.Shift, !shortcutsEnabled };
                yield return new object[] { shortcutsEnabled, readOnly, (Keys)Shortcut.CtrlV | Keys.Shift, !shortcutsEnabled };
                yield return new object[] { shortcutsEnabled, readOnly, (Keys)Shortcut.CtrlA | Keys.Shift, !shortcutsEnabled };
                yield return new object[] { shortcutsEnabled, readOnly, (Keys)Shortcut.CtrlL | Keys.Shift, !shortcutsEnabled };
                yield return new object[] { shortcutsEnabled, readOnly, (Keys)Shortcut.CtrlR | Keys.Shift, !shortcutsEnabled };
                yield return new object[] { shortcutsEnabled, readOnly, (Keys)Shortcut.CtrlE | Keys.Shift, !shortcutsEnabled };
                yield return new object[] { shortcutsEnabled, readOnly, (Keys)Shortcut.CtrlY | Keys.Shift, !shortcutsEnabled };
                yield return new object[] { shortcutsEnabled, readOnly, (Keys)Shortcut.CtrlDel | Keys.Shift, !shortcutsEnabled };
                yield return new object[] { shortcutsEnabled, readOnly, (Keys)Shortcut.CtrlJ | Keys.Shift, !shortcutsEnabled };

                yield return new object[] { shortcutsEnabled, readOnly, Keys.Control | Keys.Back, !shortcutsEnabled || !readOnly };
                yield return new object[] { shortcutsEnabled, readOnly, Keys.Control | Keys.Shift | Keys.Back, !shortcutsEnabled || !readOnly };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ProcessCmdKey_TestData))]
    public void TextBoxBase_ProcessCmdKey_Invoke_ReturnsExpected(bool shortcutsEnabled, bool readOnly, Keys keyData, bool expected)
    {
        using SubTextBox control = new()
        {
            ShortcutsEnabled = shortcutsEnabled,
            ReadOnly = readOnly
        };
        Message m = default;
        Assert.Equal(expected, control.ProcessCmdKey(ref m, keyData));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ProcessCmdKey_TestData))]
    public void TextBoxBase_ProcessCmdKey_InvokeWithParent_ReturnsFalse(bool shortcutsEnabled, bool readOnly, Keys keyData, bool expected)
    {
        using Control parent = new();
        using SubTextBox control = new()
        {
            Parent = parent,
            ShortcutsEnabled = shortcutsEnabled,
            ReadOnly = readOnly
        };
        Message msg = default;
        Assert.Equal(expected, control.ProcessCmdKey(ref msg, keyData));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(Keys.A)]
    public void TextBoxBase_ProcessCmdKey_InvokeWithoutParent_ReturnsFalse(Keys keyData)
    {
        using SubTextBox control = new();
        Message m = default;
        Assert.False(control.ProcessCmdKey(ref m, keyData));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, false, (Keys)Shortcut.CtrlA, true, true)]
    [InlineData(true, false, (Keys)Shortcut.CtrlA, false, true)]
    [InlineData(false, false, (Keys)Shortcut.CtrlA, true, true)]
    [InlineData(false, false, (Keys)Shortcut.CtrlA, false, true)]
    [InlineData(true, true, (Keys)Shortcut.CtrlL, true, true)]
    [InlineData(true, true, (Keys)Shortcut.CtrlL, false, true)]
    [InlineData(true, false, (Keys)Shortcut.CtrlL, true, true)]
    [InlineData(true, false, (Keys)Shortcut.CtrlL, false, false)]
    [InlineData(true, false, Keys.A, true, true)]
    [InlineData(true, false, Keys.A, false, false)]
    public void TextBoxBase_ProcessCmdKey_InvokeWithCustomParent_ReturnsExpected(bool shortcutsEnabled, bool readOnly, Keys keyData, bool result, bool expected)
    {
        using SubTextBox control = new()
        {
            ShortcutsEnabled = shortcutsEnabled,
            ReadOnly = readOnly
        };
        Message msg = new()
        {
            Msg = 1
        };
        int callCount = 0;
        bool action(Message actualMsg, Keys actualKeyData)
        {
            Assert.Equal(1, actualMsg.Msg);
            Assert.Equal(keyData, actualKeyData);
            callCount++;
            return result;
        }

        using CustomProcessControl parent = new()
        {
            ProcessCmdKeyAction = action
        };
        control.Parent = parent;

        Assert.Equal(expected, control.ProcessCmdKey(ref msg, keyData));
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    private class CustomProcessControl : Control
    {
        public Func<Message, Keys, bool> ProcessCmdKeyAction { get; set; }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) => ProcessCmdKeyAction(msg, keyData);

        public Func<Keys, bool> ProcessDialogKeyAction { get; set; }

        protected override bool ProcessDialogKey(Keys keyData) => ProcessDialogKeyAction(keyData);
    }

    [WinFormsFact]
    public void TextBoxBase_ProcessCmdKey_CtrlBackspaceTextEmpty_RemainsEmpty()
    {
        using SubTextBox control = new();

        Message message = default;
        Assert.True(control.ProcessCmdKey(ref message, Keys.Control | Keys.Back));
        Assert.Empty(control.Text);
    }

    [WinFormsFact]
    public void TextBoxBase_ProcessCmdKey_CtrlBackspaceReadOnly_Nop()
    {
        using SubTextBox control = new()
        {
            Text = "text",
            ReadOnly = true
        };

        Message message = default;
        Assert.False(control.ProcessCmdKey(ref message, Keys.Control | Keys.Back));
        Assert.Equal("text", control.Text);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetCtrlBackspaceData))]
    public void TextBoxBase_ProcessCmdKey_CtrlBackspace_ClearsSelection(string text, string expected, int cursorRelativeToEnd)
    {
        using SubTextBox control = new()
        {
            Text = text,
            SelectionStart = text.Length + cursorRelativeToEnd
        };

        Message message = default;
        Assert.True(control.ProcessCmdKey(ref message, Keys.Control | Keys.Back));
        Assert.Equal(expected, control.Text);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetCtrlBackspaceRepeatedData))]
    public void TextBoxBase_ProcessCmdKey_CtrlBackspaceRepeated_ClearsSelection(string text, string expected, int repeats)
    {
        using SubTextBox control = new()
        {
            Text = text,
            SelectionStart = text.Length,
            SelectionLength = 0
        };

        for (int i = 0; i < repeats; i++)
        {
            Message message = default;
            Assert.True(control.ProcessCmdKey(ref message, Keys.Control | Keys.Back));
        }

        Assert.Equal(expected, control.Text);
    }

    [WinFormsFact]
    public void TextBoxBase_ProcessCmdKey_CtrlBackspaceWithSelection_ClearsSelection()
    {
        using SubTextBox control = new()
        {
            Text = "123-5-7-9",
            SelectionStart = 2,
            SelectionLength = 5
        };

        Message message = default;
        Assert.True(control.ProcessCmdKey(ref message, Keys.Control | Keys.Back));
        Assert.Equal("12-9", control.Text);
    }

    [WinFormsTheory]
    [InlineData(true, Keys.A)]
    [InlineData(false, Keys.A)]
    [InlineData(true, Keys.Tab)]
    [InlineData(false, Keys.Tab)]
    [InlineData(true, Keys.Control | Keys.Tab)]
    [InlineData(false, Keys.Control | Keys.Tab)]
    [InlineData(true, Keys.Control | Keys.A)]
    [InlineData(false, Keys.Control | Keys.A)]
    public void TextBox_ProcessDialogKey_InvokeWithoutParent_ReturnsFalse(bool acceptsTab, Keys keyData)
    {
        using SubTextBox control = new()
        {
            AcceptsTab = acceptsTab
        };
        Assert.False(control.ProcessDialogKey(keyData));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, Keys.A)]
    [InlineData(false, Keys.A)]
    [InlineData(true, Keys.Tab)]
    [InlineData(false, Keys.Tab)]
    [InlineData(true, Keys.Control | Keys.Tab)]
    [InlineData(false, Keys.Control | Keys.Tab)]
    [InlineData(true, Keys.Control | Keys.A)]
    [InlineData(false, Keys.Control | Keys.A)]
    public void TextBox_ProcessDialogKey_InvokeWithParent_ReturnsFalse(bool acceptsTab, Keys keyData)
    {
        using Control parent = new();
        using SubTextBox control = new()
        {
            Parent = parent,
            AcceptsTab = acceptsTab
        };
        Assert.False(control.ProcessDialogKey(keyData));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, Keys.A, Keys.A, true)]
    [InlineData(false, Keys.A, Keys.A, false)]
    [InlineData(true, Keys.Tab, Keys.Tab, true)]
    [InlineData(false, Keys.Tab, Keys.Tab, false)]
    [InlineData(true, Keys.Control | Keys.Tab, Keys.Tab, true)]
    [InlineData(false, Keys.Control | Keys.Tab, Keys.Control | Keys.Tab, false)]
    [InlineData(true, Keys.Control | Keys.A, Keys.Control | Keys.A, true)]
    [InlineData(false, Keys.Control | Keys.A, Keys.Control | Keys.A, false)]
    public void TextBox_ProcessDialogKey_InvokeWithCustomParent_ReturnsExpected(bool acceptsTab, Keys keyData, Keys expectedKeyData, bool result)
    {
        int callCount = 0;
        bool action(Keys actualKeyData)
        {
            Assert.Equal(expectedKeyData, actualKeyData);
            callCount++;
            return result;
        }

        using CustomProcessControl parent = new()
        {
            ProcessDialogKeyAction = action
        };
        using SubTextBox control = new()
        {
            Parent = parent,
            AcceptsTab = acceptsTab
        };
        Assert.Equal(result, control.ProcessDialogKey(keyData));
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, "")]
    [InlineData(false, "")]
    [InlineData(true, "Text")]
    [InlineData(false, "Text")]
    [InlineData(true, "Text\r\nMoreText")]
    [InlineData(false, "Text\r\nMoreText")]
    public void TextBoxBase_ScrollToCaret_InvokeWithoutHandleRecreate_Success(bool multiline, string text)
    {
        using SubTextBox control = new()
        {
            Multiline = multiline,
            Text = text
        };
        control.ScrollToCaret();
        Assert.False(control.IsHandleCreated);

        // Create Handle.
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.True(control.IsHandleCreated);

        // Recreate Handle.
        control.RecreateHandle();
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, "")]
    [InlineData(false, "")]
    [InlineData(true, "Text")]
    [InlineData(false, "Text")]
    [InlineData(true, "Text\r\nMoreText")]
    [InlineData(false, "Text\r\nMoreText")]
    public void TextBoxBase_ScrollToCaret_InvokeWithHandle_Success(bool multiline, string text)
    {
        using TextBox control = new()
        {
            Multiline = multiline,
            Text = text
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ScrollToCaret();
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> ScrollToCaret_NoGetOleInterface_TestData()
    {
        yield return new object[] { IntPtr.Zero };
        yield return new object[] { Marshal.GetIUnknownForObject(new object()) };
    }

    [WinFormsTheory]
    [MemberData(nameof(ScrollToCaret_NoGetOleInterface_TestData))]
    public void TextBoxBase_ScrollToCaret_InvokeWithHandleCustomGetOleInterfaceTextBox_Success(IntPtr lParam)
    {
        using CustomGetOleInterfaceTextBox control = new()
        {
            GetOleInterfaceResult = IntPtr.Zero,
            GetOleInterfaceLParam = lParam,
            Text = "Text"
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ScrollToCaret();
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TextBoxBase_ScrollToCaret_InvokeWithHandleInvalidGetOleInterfaceTextBox_DoesNotThrow()
    {
        IntPtr pUnk = Marshal.GetIUnknownForObject(new object());
        using CustomGetOleInterfaceTextBox control = new()
        {
            GetOleInterfaceResult = 1,
            GetOleInterfaceLParam = pUnk,
            Text = "Text"
        };

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.ScrollToCaret();
    }

    private class CustomGetOleInterfaceTextBox : TextBox
    {
        public IntPtr GetOleInterfaceResult { get; set; }

        public IntPtr GetOleInterfaceLParam { get; set; }

        protected override unsafe void WndProc(ref Message m)
        {
            if (m.Msg == (int)PInvokeCore.EM_GETOLEINTERFACE)
            {
                IntPtr* pParam = (IntPtr*)m.LParam;
                *pParam = GetOleInterfaceLParam;
                m.Result = GetOleInterfaceResult;
                return;
            }

            base.WndProc(ref m);
        }
    }

    public static IEnumerable<object[]> Select_TestData()
    {
        yield return new object[] { string.Empty, 0, -1, 0, 0, string.Empty };
        yield return new object[] { string.Empty, 0, 0, 0, 0, string.Empty };
        yield return new object[] { string.Empty, 0, 1, 0, 0, string.Empty };
        yield return new object[] { string.Empty, 1, 1, 0, 0, string.Empty };

        yield return new object[] { "text", 0, -1, 0, 0, string.Empty };
        yield return new object[] { "text", 0, 0, 0, 0, string.Empty };
        yield return new object[] { "text", 0, 1, 0, 1, "t" };
        yield return new object[] { "text", 1, 2, 1, 2, "ex" };
        yield return new object[] { "text", 3, 1, 3, 1, "t" };
        yield return new object[] { "text", 3, 2, 3, 1, "t" };
        yield return new object[] { "text", 4, 1, 4, 0, string.Empty };
        yield return new object[] { "text", 5, 1, 4, 0, string.Empty };
        yield return new object[] { "text", 0, 4, 0, 4, "text" };
        yield return new object[] { "text", 0, 5, 0, 4, "text" };
    }

    [WinFormsTheory]
    [MemberData(nameof(Select_TestData))]
    public void TextBoxBase_Select_Invoke_Success(string text, int start, int length, int expectedSelectionStart, int expectedSelectionLength, string expectedSelectedText)
    {
        using SubTextBox control = new()
        {
            Text = text
        };
        control.Select(start, length);
        Assert.Equal(expectedSelectionStart, control.SelectionStart);
        Assert.Equal(expectedSelectionLength, control.SelectionLength);
        Assert.Equal(expectedSelectedText, control.SelectedText);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.Select(start, length);
        Assert.Equal(expectedSelectionStart, control.SelectionStart);
        Assert.Equal(expectedSelectionLength, control.SelectionLength);
        Assert.Equal(expectedSelectedText, control.SelectedText);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Select_TestData))]
    public void TextBoxBase_Select_InvokeWithHandle_Success(string text, int start, int length, int expectedSelectionStart, int expectedSelectionLength, string expectedSelectedText)
    {
        using SubTextBox control = new()
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

        control.Select(start, length);
        Assert.Equal(expectedSelectionStart, control.SelectionStart);
        Assert.Equal(expectedSelectionLength, control.SelectionLength);
        Assert.Equal(expectedSelectedText, control.SelectedText);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.Select(start, length);
        Assert.Equal(expectedSelectionStart, control.SelectionStart);
        Assert.Equal(expectedSelectionLength, control.SelectionLength);
        Assert.Equal(expectedSelectedText, control.SelectedText);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(0, 0, 0, 0)]
    [InlineData(0, -1, 0, 0)]
    [InlineData(1, 2, 1, 3)]
    [InlineData(5, 2, 4, 4)]
    [InlineData(0, 4, 0, 4)]
    [InlineData(0, 5, 0, 4)]
    public unsafe void TextBoxBase_Select_GetSel_Success(int start, int length, int expectedSelectionStart, int expectedEnd)
    {
        using SubTextBox control = new()
        {
            Text = "Text"
        };

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.Select(start, length);
        int selectionStart = 0;
        int selectionEnd = 0;
        LRESULT result = PInvokeCore.SendMessage(
            control,
            PInvokeCore.EM_GETSEL,
            (WPARAM)(&selectionStart),
            (LPARAM)(&selectionEnd));
        Assert.Equal(expectedSelectionStart, result.LOWORD);
        Assert.Equal(expectedEnd, result.HIWORD);
        Assert.Equal(expectedSelectionStart, selectionStart);
        Assert.Equal(expectedEnd, selectionEnd);
    }

    [WinFormsFact]
    public void TextBoxBase_Select_NegativeStart_ThrowsArgumentOutOfRangeException()
    {
        using SubTextBox control = new();
        Assert.Throws<ArgumentOutOfRangeException>("start", () => control.Select(-1, 0));
    }

    [WinFormsFact]
    public void TextBoxBase_SelectAll_InvokeEmpty_Success()
    {
        using SubTextBox control = new();
        control.SelectAll();
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.SelectAll();
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBase_SelectAll_InvokeNotEmpty_Success()
    {
        using SubTextBox control = new()
        {
            Text = "text"
        };
        control.SelectAll();
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(4, control.SelectionLength);
        Assert.Equal("text", control.SelectedText);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.SelectAll();
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(4, control.SelectionLength);
        Assert.Equal("text", control.SelectedText);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBase_SelectAll_InvokeEmptyWithHandle_Success()
    {
        using SubTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SelectAll();
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.SelectAll();
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(0, control.SelectionLength);
        Assert.Empty(control.SelectedText);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TextBoxBase_SelectAll_InvokeNotEmptyWithHandle_Success()
    {
        using SubTextBox control = new()
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

        control.SelectAll();
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(4, control.SelectionLength);
        Assert.Equal("text", control.SelectedText);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.SelectAll();
        Assert.Equal(0, control.SelectionStart);
        Assert.Equal(4, control.SelectionLength);
        Assert.Equal("text", control.SelectedText);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public unsafe void TextBoxBase_SelectAll_GetSel_Success()
    {
        using SubTextBox control = new()
        {
            Text = "Text"
        };

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.SelectAll();
        int selectionStart = 0;
        int selectionEnd = 0;
        LRESULT result = PInvokeCore.SendMessage(
            control,
            PInvokeCore.EM_GETSEL,
            (WPARAM)(&selectionStart),
            (LPARAM)(&selectionEnd));
        Assert.Equal(0, result.LOWORD);
        Assert.Equal(4, result.HIWORD);
        Assert.Equal(0, selectionStart);
        Assert.Equal(4, selectionEnd);
    }

    public static IEnumerable<object[]> SetBoundsCore_TestData()
    {
        foreach (BoundsSpecified specified in Enum.GetValues(typeof(BoundsSpecified)))
        {
            yield return new object[] { 0, 0, 0, 0, specified, 0, 1 };
            yield return new object[] { -1, -2, -3, -4, specified, 1, 1 };
            yield return new object[] { 1, 0, 0, 0, specified, 1, 1 };
            yield return new object[] { 0, 2, 0, 0, specified, 1, 1 };
            yield return new object[] { 1, 2, 0, 0, specified, 1, 1 };
            yield return new object[] { 0, 0, 1, 0, specified, 0, 1 };
            yield return new object[] { 0, 0, 0, 2, specified, 0, 1 };
            yield return new object[] { 0, 0, 1, 2, specified, 0, 1 };
            yield return new object[] { 1, 2, 30, 40, specified, 1, 1 };
            yield return new object[] { 0, 0, 100, s_preferredHeight, specified, 0, 0 };
            yield return new object[] { 1, 2, 100, s_preferredHeight, specified, 1, 0 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBoundsCore_TestData))]
    public void TextBoxBase_SetBoundsCore_InvokeAutoSize_Success(int x, int y, int width, int height, BoundsSpecified specified, int expectedLocationChangedCallCount, int expectedLayoutCallCount)
    {
        using SubTextBox control = new();
        int moveCallCount = 0;
        int locationChangedCallCount = 0;
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Move += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(locationChangedCallCount, moveCallCount);
            Assert.Equal(layoutCallCount, moveCallCount);
            Assert.Equal(resizeCallCount, moveCallCount);
            Assert.Equal(sizeChangedCallCount, moveCallCount);
            Assert.Equal(clientSizeChangedCallCount, moveCallCount);
            moveCallCount++;
        };
        control.LocationChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(moveCallCount - 1, locationChangedCallCount);
            Assert.Equal(layoutCallCount, locationChangedCallCount);
            Assert.Equal(resizeCallCount, locationChangedCallCount);
            Assert.Equal(sizeChangedCallCount, locationChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, locationChangedCallCount);
            locationChangedCallCount++;
        };
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };

        control.SetBoundsCore(x, y, width, height, specified);
        Assert.Equal(new Size(width - 4, s_preferredHeight - 4), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, width - 4, s_preferredHeight - 4), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, width - 4, s_preferredHeight - 4), control.DisplayRectangle);
        Assert.Equal(new Size(width, s_preferredHeight), control.Size);
        Assert.Equal(x, control.Left);
        Assert.Equal(x + width, control.Right);
        Assert.Equal(y, control.Top);
        Assert.Equal(y + s_preferredHeight, control.Bottom);
        Assert.Equal(width, control.Width);
        Assert.Equal(s_preferredHeight, control.Height);
        Assert.Equal(new Rectangle(x, y, width, s_preferredHeight), control.Bounds);
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.SetBoundsCore(x, y, width, height, specified);
        Assert.Equal(new Size(width - 4, s_preferredHeight - 4), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, width - 4, s_preferredHeight - 4), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, width - 4, s_preferredHeight - 4), control.DisplayRectangle);
        Assert.Equal(new Size(width, s_preferredHeight), control.Size);
        Assert.Equal(x, control.Left);
        Assert.Equal(x + width, control.Right);
        Assert.Equal(y, control.Top);
        Assert.Equal(y + s_preferredHeight, control.Bottom);
        Assert.Equal(width, control.Width);
        Assert.Equal(s_preferredHeight, control.Height);
        Assert.Equal(new Rectangle(x, y, width, s_preferredHeight), control.Bounds);
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> SetBoundsCore_NoAutoSize_TestData()
    {
        foreach (BoundsSpecified specified in Enum.GetValues(typeof(BoundsSpecified)))
        {
            yield return new object[] { 0, 0, 0, 0, specified, 0, 1 };
            yield return new object[] { -1, -2, -3, -4, specified, 1, 1 };
            yield return new object[] { 1, 0, 0, 0, specified, 1, 1 };
            yield return new object[] { 0, 2, 0, 0, specified, 1, 1 };
            yield return new object[] { 1, 2, 0, 0, specified, 1, 1 };
            yield return new object[] { 0, 0, 1, 0, specified, 0, 1 };
            yield return new object[] { 0, 0, 0, 2, specified, 0, 1 };
            yield return new object[] { 0, 0, 1, 2, specified, 0, 1 };
            yield return new object[] { 1, 2, 30, 40, specified, 1, 1 };
            yield return new object[] { 0, 0, 100, s_preferredHeight, specified, 0, 0 };
            yield return new object[] { 1, 2, 100, s_preferredHeight, specified, 1, 0 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBoundsCore_TestData))]
    public void TextBoxBase_SetBoundsCore_InvokeMultiline_Success(int x, int y, int width, int height, BoundsSpecified specified, int expectedLocationChangedCallCount, int expectedLayoutCallCount)
    {
        using SubTextBox control = new()
        {
            Multiline = true
        };
        int moveCallCount = 0;
        int locationChangedCallCount = 0;
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Move += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(locationChangedCallCount, moveCallCount);
            Assert.Equal(layoutCallCount, moveCallCount);
            Assert.Equal(resizeCallCount, moveCallCount);
            Assert.Equal(sizeChangedCallCount, moveCallCount);
            Assert.Equal(clientSizeChangedCallCount, moveCallCount);
            moveCallCount++;
        };
        control.LocationChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(moveCallCount - 1, locationChangedCallCount);
            Assert.Equal(layoutCallCount, locationChangedCallCount);
            Assert.Equal(resizeCallCount, locationChangedCallCount);
            Assert.Equal(sizeChangedCallCount, locationChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, locationChangedCallCount);
            locationChangedCallCount++;
        };
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };

        control.SetBoundsCore(x, y, width, height, specified);
        Assert.Equal(new Size(width - 4, height - 4), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, width - 4, height - 4), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, width - 4, height - 4), control.DisplayRectangle);
        Assert.Equal(new Size(width, height), control.Size);
        Assert.Equal(x, control.Left);
        Assert.Equal(x + width, control.Right);
        Assert.Equal(y, control.Top);
        Assert.Equal(y + height, control.Bottom);
        Assert.Equal(width, control.Width);
        Assert.Equal(height, control.Height);
        Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.SetBoundsCore(x, y, width, height, specified);
        Assert.Equal(new Size(width - 4, height - 4), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, width - 4, height - 4), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, width - 4, height - 4), control.DisplayRectangle);
        Assert.Equal(new Size(width, height), control.Size);
        Assert.Equal(x, control.Left);
        Assert.Equal(x + width, control.Right);
        Assert.Equal(y, control.Top);
        Assert.Equal(y + height, control.Bottom);
        Assert.Equal(width, control.Width);
        Assert.Equal(height, control.Height);
        Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBoundsCore_TestData))]
    public void TextBoxBase_SetBoundsCore_InvokeNotAutoSize_Success(int x, int y, int width, int height, BoundsSpecified specified, int expectedLocationChangedCallCount, int expectedLayoutCallCount)
    {
        using SubTextBox control = new()
        {
            AutoSize = false
        };
        int moveCallCount = 0;
        int locationChangedCallCount = 0;
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Move += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(locationChangedCallCount, moveCallCount);
            Assert.Equal(layoutCallCount, moveCallCount);
            Assert.Equal(resizeCallCount, moveCallCount);
            Assert.Equal(sizeChangedCallCount, moveCallCount);
            Assert.Equal(clientSizeChangedCallCount, moveCallCount);
            moveCallCount++;
        };
        control.LocationChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(moveCallCount - 1, locationChangedCallCount);
            Assert.Equal(layoutCallCount, locationChangedCallCount);
            Assert.Equal(resizeCallCount, locationChangedCallCount);
            Assert.Equal(sizeChangedCallCount, locationChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, locationChangedCallCount);
            locationChangedCallCount++;
        };
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };

        control.SetBoundsCore(x, y, width, height, specified);
        Assert.Equal(new Size(width - 4, height - 4), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, width - 4, height - 4), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, width - 4, height - 4), control.DisplayRectangle);
        Assert.Equal(new Size(width, height), control.Size);
        Assert.Equal(x, control.Left);
        Assert.Equal(x + width, control.Right);
        Assert.Equal(y, control.Top);
        Assert.Equal(y + height, control.Bottom);
        Assert.Equal(width, control.Width);
        Assert.Equal(height, control.Height);
        Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.SetBoundsCore(x, y, width, height, specified);
        Assert.Equal(new Size(width - 4, height - 4), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, width - 4, height - 4), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, width - 4, height - 4), control.DisplayRectangle);
        Assert.Equal(new Size(width, height), control.Size);
        Assert.Equal(x, control.Left);
        Assert.Equal(x + width, control.Right);
        Assert.Equal(y, control.Top);
        Assert.Equal(y + height, control.Bottom);
        Assert.Equal(width, control.Width);
        Assert.Equal(height, control.Height);
        Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBase_ToString_Invoke_ReturnsExpected()
    {
        using SubTextBox control = new();
        Assert.Equal("System.Windows.Forms.Tests.TextBoxBaseTests+SubTextBox, Text: ", control.ToString());
    }

    [WinFormsFact]
    public void TextBoxBase_ToString_InvokeShortText_ReturnsExpected()
    {
        using SubTextBox control = new()
        {
            Text = "Text"
        };
        Assert.Equal("System.Windows.Forms.Tests.TextBoxBaseTests+SubTextBox, Text: Text", control.ToString());
    }

    [WinFormsFact]
    public void TextBoxBase_ToString_InvokeLongText_ReturnsExpected()
    {
        using SubTextBox control = new()
        {
            Text = new string('a', 41)
        };
        Assert.Equal("System.Windows.Forms.Tests.TextBoxBaseTests+SubTextBox, Text: aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa...", control.ToString());
    }

    [WinFormsFact]
    public void TextBoxBase_Undo_InvokeEmpty_Success()
    {
        using SubTextBox control = new();
        control.Undo();
        Assert.Empty(control.Text);
        Assert.False(control.CanUndo);
        Assert.False(control.Modified);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBase_Undo_InvokeNotEmpty_Success()
    {
        using SubTextBox control = new()
        {
            Text = "abc",
            SelectionStart = 1,
            SelectionLength = 2
        };
        control.Undo();
        Assert.Equal("abc", control.Text);
        Assert.False(control.CanUndo);
        Assert.False(control.Modified);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TextBoxBase_Undo_InvokeEmptyWithHandle_Success()
    {
        using SubTextBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Undo();
        Assert.Empty(control.Text);
        Assert.False(control.CanUndo);
        Assert.False(control.Modified);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TextBoxBase_Undo_InvokeNotEmptyWithHandle_Success()
    {
        using SubTextBox control = new()
        {
            Text = "abc",
            SelectionStart = 1,
            SelectionLength = 2
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Undo();
        Assert.Equal("abc", control.Text);
        Assert.False(control.CanUndo);
        Assert.False(control.Modified);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> WndProc_ContextMenuWithoutContextMenuStrip_TestData()
    {
        foreach (bool shortcutsEnabled in new bool[] { true, false })
        {
            IntPtr expectedResult = shortcutsEnabled ? IntPtr.Zero : 250;
            yield return new object[] { new Size(10, 20), shortcutsEnabled, (IntPtr)(-1), expectedResult };
            yield return new object[] { new Size(10, 20), shortcutsEnabled, PARAM.FromLowHigh(0, 0), expectedResult };
            yield return new object[] { new Size(10, 20), shortcutsEnabled, PARAM.FromLowHigh(1, 2), expectedResult };
            yield return new object[] { new Size(10, 20), shortcutsEnabled, PARAM.FromLowHigh(-1, -2), expectedResult };

            yield return new object[] { Size.Empty, shortcutsEnabled, (IntPtr)(-1), expectedResult };
            yield return new object[] { Size.Empty, shortcutsEnabled, PARAM.FromLowHigh(0, 0), expectedResult };
            yield return new object[] { Size.Empty, shortcutsEnabled, PARAM.FromLowHigh(1, 2), expectedResult };
            yield return new object[] { Size.Empty, shortcutsEnabled, PARAM.FromLowHigh(-1, -2), expectedResult };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_ContextMenuWithoutContextMenuStrip_TestData))]
    public void TextBoxBase_WndProc_InvokeContextMenuWithoutContextMenuStripWithoutHandle_Success(Size size, bool shortcutsEnabled, IntPtr lParam, IntPtr expectedResult)
    {
        using (new NoAssertContext())
        {
            using SubTextBox control = new()
            {
                Size = size,
                ShortcutsEnabled = shortcutsEnabled
            };
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_CONTEXTMENU,
                LParam = lParam,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(expectedResult, m.Result);
            Assert.False(control.IsHandleCreated);
        }
    }

    public static IEnumerable<object[]> WndProc_ContextMenuWithContextMenuStripWithoutHandle_TestData()
    {
        using Control control = new();
        Point p = control.PointToScreen(new Point(5, 5));

        foreach (bool shortcutsEnabled in new bool[] { true, false })
        {
            IntPtr expectedResult = shortcutsEnabled ? IntPtr.Zero : 250;

            yield return new object[] { new Size(10, 20), shortcutsEnabled, (IntPtr)(-1), (IntPtr)250, true, true };
            yield return new object[] { new Size(10, 20), shortcutsEnabled, PARAM.FromLowHigh(0, 0), expectedResult, false, true };
            yield return new object[] { new Size(10, 20), shortcutsEnabled, PARAM.FromLowHigh(1, 2), expectedResult, false, true };
            yield return new object[] { new Size(10, 20), shortcutsEnabled, PARAM.FromLowHigh(p.X, p.Y), (IntPtr)250, true, true };
            yield return new object[] { new Size(10, 20), shortcutsEnabled, PARAM.FromLowHigh(-1, -2), expectedResult, false, true };

            yield return new object[] { Size.Empty, shortcutsEnabled, (IntPtr)(-1), expectedResult, false, false };
            yield return new object[] { Size.Empty, shortcutsEnabled, PARAM.FromLowHigh(0, 0), expectedResult, false, true };
            yield return new object[] { Size.Empty, shortcutsEnabled, PARAM.FromLowHigh(1, 2), expectedResult, false, true };
            yield return new object[] { Size.Empty, shortcutsEnabled, PARAM.FromLowHigh(p.X, p.Y), expectedResult, false, true };
            yield return new object[] { Size.Empty, shortcutsEnabled, PARAM.FromLowHigh(-1, -2), expectedResult, false, true };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_ContextMenuWithContextMenuStripWithoutHandle_TestData))]
    public void TextBoxBase_WndProc_InvokeContextMenuWithContextMenuStripWithoutHandle_Success(Size size, bool shortcutsEnabled, IntPtr lParam, IntPtr expectedResult, bool expectedHasSourceControl, bool expectedHandleCreated)
    {
        using (new NoAssertContext())
        {
            using ContextMenuStrip menu = new();
            using SubTextBox control = new()
            {
                ContextMenuStrip = menu,
                Size = size,
                ShortcutsEnabled = shortcutsEnabled
            };
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_CONTEXTMENU,
                LParam = lParam,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(expectedResult, m.Result);
            Assert.False(menu.Visible);
            Assert.Equal(expectedHasSourceControl, menu.SourceControl == control);
            Assert.Equal(expectedHandleCreated, control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_ContextMenuWithoutContextMenuStrip_TestData))]
    public void TextBoxBase_WndProc_InvokeContextMenuWithoutContextMenuStripWithHandle_Success(Size size, bool shortcutsEnabled, IntPtr lParam, IntPtr expectedResult)
    {
        using SubTextBox control = new()
        {
            Size = size,
            ShortcutsEnabled = shortcutsEnabled
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
            Msg = (int)PInvokeCore.WM_CONTEXTMENU,
            LParam = lParam,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(expectedResult, m.Result);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> WndProc_ContextMenuWithContextMenuStripWithHandle_TestData()
    {
        using Control control = new();
        Point p = control.PointToScreen(new Point(5, 5));

        foreach (bool shortcutsEnabled in new bool[] { true, false })
        {
            IntPtr expectedResult = shortcutsEnabled ? IntPtr.Zero : 250;

            yield return new object[] { new Size(10, 20), shortcutsEnabled, (IntPtr)(-1), (IntPtr)250, true };
            yield return new object[] { new Size(10, 20), shortcutsEnabled, PARAM.FromLowHigh(0, 0), expectedResult, false };
            yield return new object[] { new Size(10, 20), shortcutsEnabled, PARAM.FromLowHigh(1, 2), expectedResult, false };
            yield return new object[] { new Size(10, 20), shortcutsEnabled, PARAM.FromLowHigh(p.X, p.Y), (IntPtr)250, true };
            yield return new object[] { new Size(10, 20), shortcutsEnabled, PARAM.FromLowHigh(-1, -2), expectedResult, false };

            yield return new object[] { Size.Empty, shortcutsEnabled, (IntPtr)(-1), expectedResult, false };
            yield return new object[] { Size.Empty, shortcutsEnabled, PARAM.FromLowHigh(0, 0), expectedResult, false };
            yield return new object[] { Size.Empty, shortcutsEnabled, PARAM.FromLowHigh(1, 2), expectedResult, false };
            yield return new object[] { Size.Empty, shortcutsEnabled, PARAM.FromLowHigh(p.X, p.Y), expectedResult, false };
            yield return new object[] { Size.Empty, shortcutsEnabled, PARAM.FromLowHigh(-1, -2), expectedResult, false };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_ContextMenuWithContextMenuStripWithHandle_TestData))]
    public void TextBoxBase_WndProc_InvokeContextMenuWithContextMenuStripWithHandle_Success(Size size, bool shortcutsEnabled, IntPtr lParam, IntPtr expectedResult, bool expectedHasSourceControl)
    {
        using ContextMenuStrip menu = new();
        using SubTextBox control = new()
        {
            ContextMenuStrip = menu,
            Size = size,
            ShortcutsEnabled = shortcutsEnabled
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
            Msg = (int)PInvokeCore.WM_CONTEXTMENU,
            LParam = lParam,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(expectedResult, m.Result);
        Assert.False(menu.Visible);
        Assert.Equal(expectedHasSourceControl, menu.SourceControl == control);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> WndProc_GetDlgCode_TestData()
    {
        yield return new object[] { true, (IntPtr)2 };
        yield return new object[] { false, IntPtr.Zero };
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_GetDlgCode_TestData))]
    public void TextBoxBase_WndProc_InvokeGetDlgCodeWithoutHandle_ReturnsExpected(bool acceptsTabs, IntPtr expectedResult)
    {
        using (new NoAssertContext())
        {
            using SubTextBox control = new()
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
    public void TextBoxBase_WndProc_InvokeGetDlgCodeWithHandle_ReturnsExpected(bool acceptsTabs, IntPtr expectedResult)
    {
        using SubTextBox control = new()
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

    public static IEnumerable<object[]> WndProc_MouseDown_TestData()
    {
        yield return new object[] { true, (int)PInvokeCore.WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero, (IntPtr)250, MouseButtons.Left, 1, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_LBUTTONDOWN, PARAM.FromLowHigh(1, 2), IntPtr.Zero, (IntPtr)250, MouseButtons.Left, 1, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_LBUTTONDOWN, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, (IntPtr)250, MouseButtons.Left, 1, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, MouseButtons.Left, 1, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_LBUTTONDOWN, PARAM.FromLowHigh(1, 2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Left, 1, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_LBUTTONDOWN, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Left, 1, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_LBUTTONDBLCLK, IntPtr.Zero, IntPtr.Zero, (IntPtr)250, MouseButtons.Left, 2, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_LBUTTONDBLCLK, PARAM.FromLowHigh(1, 2), IntPtr.Zero, (IntPtr)250, MouseButtons.Left, 2, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_LBUTTONDBLCLK, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, (IntPtr)250, MouseButtons.Left, 2, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_LBUTTONDBLCLK, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, MouseButtons.Left, 2, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_LBUTTONDBLCLK, PARAM.FromLowHigh(1, 2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Left, 2, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_LBUTTONDBLCLK, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Left, 2, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_MBUTTONDOWN, IntPtr.Zero, IntPtr.Zero, (IntPtr)250, MouseButtons.Middle, 1, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_MBUTTONDOWN, PARAM.FromLowHigh(1, 2), IntPtr.Zero, (IntPtr)250, MouseButtons.Middle, 1, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_MBUTTONDOWN, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, (IntPtr)250, MouseButtons.Middle, 1, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_MBUTTONDOWN, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, MouseButtons.Middle, 1, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_MBUTTONDOWN, PARAM.FromLowHigh(1, 2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Middle, 1, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_MBUTTONDOWN, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Middle, 1, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_MBUTTONDBLCLK, IntPtr.Zero, IntPtr.Zero, (IntPtr)250, MouseButtons.Middle, 2, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_MBUTTONDBLCLK, PARAM.FromLowHigh(1, 2), IntPtr.Zero, (IntPtr)250, MouseButtons.Middle, 2, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_MBUTTONDBLCLK, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, (IntPtr)250, MouseButtons.Middle, 2, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_MBUTTONDBLCLK, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, MouseButtons.Middle, 2, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_MBUTTONDBLCLK, PARAM.FromLowHigh(1, 2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Middle, 2, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_MBUTTONDBLCLK, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Middle, 2, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_RBUTTONDOWN, IntPtr.Zero, IntPtr.Zero, (IntPtr)250, MouseButtons.Right, 1, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_RBUTTONDOWN, PARAM.FromLowHigh(1, 2), IntPtr.Zero, (IntPtr)250, MouseButtons.Right, 1, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_RBUTTONDOWN, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, (IntPtr)250, MouseButtons.Right, 1, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_RBUTTONDOWN, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, MouseButtons.Right, 1, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_RBUTTONDOWN, PARAM.FromLowHigh(1, 2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Right, 1, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_RBUTTONDOWN, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Right, 1, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_RBUTTONDBLCLK, IntPtr.Zero, IntPtr.Zero, (IntPtr)250, MouseButtons.Right, 2, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_RBUTTONDBLCLK, PARAM.FromLowHigh(1, 2), IntPtr.Zero, (IntPtr)250, MouseButtons.Right, 2, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_RBUTTONDBLCLK, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, (IntPtr)250, MouseButtons.Right, 2, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_RBUTTONDBLCLK, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, MouseButtons.Right, 2, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_RBUTTONDBLCLK, PARAM.FromLowHigh(1, 2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Right, 2, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_RBUTTONDBLCLK, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Right, 2, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDOWN, IntPtr.Zero, IntPtr.Zero, (IntPtr)250, MouseButtons.None, 1, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDOWN, PARAM.FromLowHigh(1, 2), IntPtr.Zero, (IntPtr)250, MouseButtons.None, 1, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDOWN, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, (IntPtr)250, MouseButtons.None, 1, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDOWN, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, MouseButtons.None, 1, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDOWN, PARAM.FromLowHigh(1, 2), IntPtr.Zero, IntPtr.Zero, MouseButtons.None, 1, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDOWN, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, IntPtr.Zero, MouseButtons.None, 1, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDOWN, IntPtr.Zero, PARAM.FromLowHigh(2, 1), (IntPtr)250, MouseButtons.XButton1, 1, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDOWN, PARAM.FromLowHigh(1, 2), PARAM.FromLowHigh(2, 1), (IntPtr)250, MouseButtons.XButton1, 1, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDOWN, PARAM.FromLowHigh(-1, -2), PARAM.FromLowHigh(2, 1), (IntPtr)250, MouseButtons.XButton1, 1, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDOWN, IntPtr.Zero, PARAM.FromLowHigh(2, 1), IntPtr.Zero, MouseButtons.XButton1, 1, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDOWN, PARAM.FromLowHigh(1, 2), PARAM.FromLowHigh(2, 1), IntPtr.Zero, MouseButtons.XButton1, 1, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDOWN, PARAM.FromLowHigh(-1, -2), PARAM.FromLowHigh(2, 1), IntPtr.Zero, MouseButtons.XButton1, 1, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDOWN, IntPtr.Zero, PARAM.FromLowHigh(1, 2), (IntPtr)250, MouseButtons.XButton2, 1, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDOWN, PARAM.FromLowHigh(1, 2), PARAM.FromLowHigh(1, 2), (IntPtr)250, MouseButtons.XButton2, 1, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDOWN, PARAM.FromLowHigh(-1, -2), PARAM.FromLowHigh(1, 2), (IntPtr)250, MouseButtons.XButton2, 1, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDOWN, IntPtr.Zero, PARAM.FromLowHigh(1, 2), IntPtr.Zero, MouseButtons.XButton2, 1, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDOWN, PARAM.FromLowHigh(1, 2), PARAM.FromLowHigh(1, 2), IntPtr.Zero, MouseButtons.XButton2, 1, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDOWN, PARAM.FromLowHigh(-1, -2), PARAM.FromLowHigh(1, 2), IntPtr.Zero, MouseButtons.XButton2, 1, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDBLCLK, IntPtr.Zero, IntPtr.Zero, (IntPtr)250, MouseButtons.None, 2, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDBLCLK, PARAM.FromLowHigh(1, 2), IntPtr.Zero, (IntPtr)250, MouseButtons.None, 2, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDBLCLK, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, (IntPtr)250, MouseButtons.None, 2, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDBLCLK, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, MouseButtons.None, 2, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDBLCLK, PARAM.FromLowHigh(1, 2), IntPtr.Zero, IntPtr.Zero, MouseButtons.None, 2, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDBLCLK, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, IntPtr.Zero, MouseButtons.None, 2, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDBLCLK, IntPtr.Zero, PARAM.FromLowHigh(2, 1), (IntPtr)250, MouseButtons.XButton1, 2, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDBLCLK, PARAM.FromLowHigh(1, 2), PARAM.FromLowHigh(2, 1), (IntPtr)250, MouseButtons.XButton1, 2, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDBLCLK, PARAM.FromLowHigh(-1, -2), PARAM.FromLowHigh(2, 1), (IntPtr)250, MouseButtons.XButton1, 2, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDBLCLK, IntPtr.Zero, PARAM.FromLowHigh(2, 1), IntPtr.Zero, MouseButtons.XButton1, 2, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDBLCLK, PARAM.FromLowHigh(1, 2), PARAM.FromLowHigh(2, 1), IntPtr.Zero, MouseButtons.XButton1, 2, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDBLCLK, PARAM.FromLowHigh(-1, -2), PARAM.FromLowHigh(2, 1), IntPtr.Zero, MouseButtons.XButton1, 2, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDBLCLK, IntPtr.Zero, PARAM.FromLowHigh(1, 2), (IntPtr)250, MouseButtons.XButton2, 2, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDBLCLK, PARAM.FromLowHigh(1, 2), PARAM.FromLowHigh(1, 2), (IntPtr)250, MouseButtons.XButton2, 2, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDBLCLK, PARAM.FromLowHigh(-1, -2), PARAM.FromLowHigh(1, 2), (IntPtr)250, MouseButtons.XButton2, 2, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDBLCLK, IntPtr.Zero, PARAM.FromLowHigh(1, 2), IntPtr.Zero, MouseButtons.XButton2, 2, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDBLCLK, PARAM.FromLowHigh(1, 2), PARAM.FromLowHigh(1, 2), IntPtr.Zero, MouseButtons.XButton2, 2, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDBLCLK, PARAM.FromLowHigh(-1, -2), PARAM.FromLowHigh(1, 2), IntPtr.Zero, MouseButtons.XButton2, 2, -1, -2 };
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_MouseDown_TestData))]
    public void TextBoxBase_WndProc_InvokeMouseDownWithoutHandle_Success(bool userMouse, int msg, IntPtr lParam, IntPtr wParam, IntPtr expectedResult, MouseButtons expectedButton, int expectedClicks, int expectedX, int expectedY)
    {
        using (new NoAssertContext())
        {
            using SubTextBox control = new();
            control.SetStyle(ControlStyles.UserMouse, userMouse);
            int callCount = 0;
            control.MouseDown += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(expectedButton, e.Button);
                Assert.Equal(expectedClicks, e.Clicks);
                Assert.Equal(expectedX, e.X);
                Assert.Equal(expectedY, e.Y);
                Assert.Equal(0, e.Delta);
                callCount++;
            };
            Message m = new()
            {
                Msg = msg,
                LParam = lParam,
                WParam = wParam,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(expectedResult, m.Result);
            Assert.Equal(1, callCount);
            Assert.True(control.Capture);
            Assert.False(control.Focused);
            Assert.True(control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_MouseDown_TestData))]
    public void TextBoxBase_WndProc_InvokeMouseDownWithoutHandleNotSelectable_Success(bool userMouse, int msg, IntPtr lParam, IntPtr wParam, IntPtr expectedResult, MouseButtons expectedButton, int expectedClicks, int expectedX, int expectedY)
    {
        using (new NoAssertContext())
        {
            using SubTextBox control = new();
            control.SetStyle(ControlStyles.UserMouse, userMouse);
            control.SetStyle(ControlStyles.Selectable, false);
            int callCount = 0;
            control.MouseDown += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(expectedButton, e.Button);
                Assert.Equal(expectedClicks, e.Clicks);
                Assert.Equal(expectedX, e.X);
                Assert.Equal(expectedY, e.Y);
                Assert.Equal(0, e.Delta);
                callCount++;
            };
            Message m = new()
            {
                Msg = msg,
                LParam = lParam,
                WParam = wParam,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(expectedResult, m.Result);
            Assert.Equal(1, callCount);
            Assert.True(control.Capture);
            Assert.False(control.Focused);
            Assert.True(control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [InlineData((int)PInvokeCore.WM_LBUTTONDOWN)]
    [InlineData((int)PInvokeCore.WM_LBUTTONDBLCLK)]
    [InlineData((int)PInvokeCore.WM_MBUTTONDOWN)]
    [InlineData((int)PInvokeCore.WM_MBUTTONDBLCLK)]
    [InlineData((int)PInvokeCore.WM_RBUTTONDOWN)]
    [InlineData((int)PInvokeCore.WM_RBUTTONDBLCLK)]
    [InlineData((int)PInvokeCore.WM_XBUTTONDOWN)]
    [InlineData((int)PInvokeCore.WM_XBUTTONDBLCLK)]
    public void TextBoxBase_WndProc_InvokeMouseDownWithoutHandleNotEnabled_DoesNotCallMouseDown(int msg)
    {
        using (new NoAssertContext())
        {
            using SubTextBox control = new()
            {
                Enabled = false
            };
            int callCount = 0;
            control.MouseDown += (sender, e) => callCount++;
            Message m = new()
            {
                Msg = msg,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(0, callCount);
            Assert.True(control.Capture);
            Assert.False(control.Focused);
            Assert.True(control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_MouseDown_TestData))]
    public void TextBoxBase_WndProc_InvokeMouseDownWithHandle_Success(bool userMouse, int msg, IntPtr lParam, IntPtr wParam, IntPtr expectedResult, MouseButtons expectedButton, int expectedClicks, int expectedX, int expectedY)
    {
        using SubTextBox control = new();
        control.SetStyle(ControlStyles.UserMouse, userMouse);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.MouseDown += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(expectedButton, e.Button);
            Assert.Equal(expectedClicks, e.Clicks);
            Assert.Equal(expectedX, e.X);
            Assert.Equal(expectedY, e.Y);
            Assert.Equal(0, e.Delta);
            callCount++;
        };
        Message m = new()
        {
            Msg = msg,
            LParam = lParam,
            WParam = wParam,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(expectedResult, m.Result);
        Assert.Equal(1, callCount);
        Assert.True(control.Capture);
        Assert.False(control.Focused);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_MouseDown_TestData))]
    public void TextBoxBase_WndProc_InvokeMouseDownWithHandleNotSelectable_DoesNotCallMouseDown(bool userMouse, int msg, IntPtr lParam, IntPtr wParam, IntPtr expectedResult, MouseButtons expectedButton, int expectedClicks, int expectedX, int expectedY)
    {
        using SubTextBox control = new();
        control.SetStyle(ControlStyles.UserMouse, userMouse);
        control.SetStyle(ControlStyles.Selectable, false);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.MouseDown += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(expectedButton, e.Button);
            Assert.Equal(expectedClicks, e.Clicks);
            Assert.Equal(expectedX, e.X);
            Assert.Equal(expectedY, e.Y);
            Assert.Equal(0, e.Delta);
            callCount++;
        };
        Message m = new()
        {
            Msg = msg,
            LParam = lParam,
            WParam = wParam,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(expectedResult, m.Result);
        Assert.Equal(1, callCount);
        Assert.True(control.Capture);
        Assert.False(control.Focused);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData((int)PInvokeCore.WM_LBUTTONDOWN)]
    [InlineData((int)PInvokeCore.WM_LBUTTONDBLCLK)]
    [InlineData((int)PInvokeCore.WM_MBUTTONDOWN)]
    [InlineData((int)PInvokeCore.WM_MBUTTONDBLCLK)]
    [InlineData((int)PInvokeCore.WM_RBUTTONDOWN)]
    [InlineData((int)PInvokeCore.WM_RBUTTONDBLCLK)]
    [InlineData((int)PInvokeCore.WM_XBUTTONDOWN)]
    [InlineData((int)PInvokeCore.WM_XBUTTONDBLCLK)]
    public void TextBoxBase_WndProc_InvokeMouseDownWithHandleNotEnabled_DoesNotCallMouseDown(int msg)
    {
        using SubTextBox control = new()
        {
            Enabled = false
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.MouseDown += (sender, e) => callCount++;
        Message m = new()
        {
            Msg = msg,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(0, callCount);
        Assert.True(control.Capture);
        Assert.False(control.Focused);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void TextBoxBase_WndProc_InvokeMouseHoverWithHandle_Success()
    {
        using SubTextBox control = new();
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
    public void TextBoxBase_WndProc_InvokeReflectCommandWithoutHandle_Success(IntPtr wParam, IntPtr lParam, int expectedTextChangedCallCount)
    {
        using SubTextBox control = new();

        int textChangedCallCount = 0;
        control.TextChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            textChangedCallCount++;
        };
        int modifiedCallCount = 0;
        control.ModifiedChanged += (sender, e) => modifiedCallCount++;
        Message m = new()
        {
            Msg = (int)(MessageId.WM_REFLECT | PInvokeCore.WM_COMMAND),
            WParam = wParam,
            LParam = lParam,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(250, m.Result);
        Assert.Equal(expectedTextChangedCallCount, textChangedCallCount);
        Assert.Equal(0, modifiedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_ReflectCommand_TestData))]
    public void TextBoxBase_WndProc_InvokeReflectCommandWithHandle_Success(IntPtr wParam, IntPtr lParam, int expectedTextChangedCallCount)
    {
        using SubTextBox control = new();
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
        int modifiedCallCount = 0;
        control.ModifiedChanged += (sender, e) => modifiedCallCount++;
        Message m = new()
        {
            Msg = (int)(MessageId.WM_REFLECT | PInvokeCore.WM_COMMAND),
            WParam = wParam,
            LParam = lParam,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(250, m.Result);
        Assert.Equal(expectedTextChangedCallCount, textChangedCallCount);
        Assert.Equal(0, modifiedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(true, 3)]
    [InlineData(false, 0)]
    public void TextBoxBase_WndProc_InvokeSetFontWithoutHandle_ReturnsExpected(bool multiline, int expectedMargin)
    {
        using (new NoAssertContext())
        {
            using SubTextBox control = new()
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
            Assert.Equal(!multiline, control.IsHandleCreated);
            Assert.Equal(0, textChangedCallCount);
            control.CreateControl();
            nint result = PInvokeCore.SendMessage(control, PInvokeCore.EM_GETMARGINS);
            Assert.Equal(expectedMargin, PARAM.HIWORD(result));
            Assert.Equal(expectedMargin, PARAM.LOWORD(result));
        }
    }

    [WinFormsTheory]
    [InlineData(true, 1, 2)]
    [InlineData(false, 0, 0)]
    public void TextBoxBase_WndProc_InvokeSetFontWithHandle_ReturnsExpected(bool multiline, int expectedLeft, int expectedRight)
    {
        using SubTextBox control = new()
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
        PInvokeCore.SendMessage(
            control,
            PInvokeCore.EM_SETMARGINS,
            (WPARAM)(PInvoke.EC_LEFTMARGIN | PInvoke.EC_RIGHTMARGIN),
            LPARAM.MAKELPARAM(1, 2));
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

    private class CantCreateHandleTextBox : TextBox
    {
        protected override void CreateHandle()
        {
            // Nop.
        }
    }

    private class SubTextBox : TextBox
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

        public new void CreateHandle() => base.CreateHandle();

        public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

        public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

        public new bool GetTopLevel() => base.GetTopLevel();

        public new bool IsInputKey(Keys keyData) => base.IsInputKey(keyData);

        public new void OnAcceptsTabChanged(EventArgs e) => base.OnAcceptsTabChanged(e);

        public new void OnBorderStyleChanged(EventArgs e) => base.OnBorderStyleChanged(e);

        public new void OnClick(EventArgs e) => base.OnClick(e);

        public new void OnFontChanged(EventArgs e) => base.OnFontChanged(e);

        public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

        public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);

        public new void OnHideSelectionChanged(EventArgs e) => base.OnHideSelectionChanged(e);

        public new void OnModifiedChanged(EventArgs e) => base.OnModifiedChanged(e);

        public new void OnMouseClick(MouseEventArgs e) => base.OnMouseClick(e);

        public new void OnMouseUp(MouseEventArgs mevent) => base.OnMouseUp(mevent);

        public new void OnMultilineChanged(EventArgs e) => base.OnMultilineChanged(e);

        public new void OnPaddingChanged(EventArgs e) => base.OnPaddingChanged(e);

        public new void OnPaint(PaintEventArgs e) => base.OnPaint(e);

        public new void OnReadOnlyChanged(EventArgs e) => base.OnReadOnlyChanged(e);

        public new void OnTextChanged(EventArgs e) => base.OnTextChanged(e);

        public new bool ProcessCmdKey(ref Message m, Keys keyData) => base.ProcessCmdKey(ref m, keyData);

        public new bool ProcessDialogKey(Keys keyData) => base.ProcessDialogKey(keyData);

        public new void RecreateHandle() => base.RecreateHandle();

        public new void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) => base.SetBoundsCore(x, y, width, height, specified);

        public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);

        public new void WndProc(ref Message m) => base.WndProc(ref m);
    }

    private class SubTextBoxBase : TextBoxBase
    {
    }

    public static IEnumerable<object[]> TextBoxBase_GetLineFromCharIndex_TestData()
    {
        yield return new object[] { new Size(50, 20), false, 0, 0 };
        yield return new object[] { new Size(50, 20), false, 50, 0 };
        yield return new object[] { new Size(100, 50), true, 50, 3 };
        yield return new object[] { new Size(50, 50), true, 50, 8 };
    }

    [WinFormsTheory]
    [MemberData(nameof(TextBoxBase_GetLineFromCharIndex_TestData))]
    public void TextBoxBase_GetLineFromCharIndex_ReturnsCorrectValue(Size size, bool multiline, int charIndex, int expectedLine)
    {
        using SubTextBoxBase textBoxBase = new() { Size = size, Multiline = multiline };
        textBoxBase.Text = "Some test text for testing GetLineFromCharIndex method";
        int actualLine = textBoxBase.GetLineFromCharIndex(charIndex);
        Assert.Equal(expectedLine, actualLine);
    }

    public static IEnumerable<object[]> TextBoxBase_GetPositionFromCharIndex_TestData()
    {
        yield return new object[] { new Size(50, 20), false, 0, new Point(1, 0) };
        yield return new object[] { new Size(50, 20), false, 15, new Point(79, 0) };
        yield return new object[] { new Size(50, 50), true, 12, new Point(14, 31) };
        yield return new object[] { new Size(100, 50), true, 22, new Point(37, 16) };
        yield return new object[] { new Size(50, 50), true, 100, Point.Empty };
        yield return new object[] { new Size(50, 50), true, -1, Point.Empty };
    }

    [WinFormsTheory]
    [MemberData(nameof(TextBoxBase_GetPositionFromCharIndex_TestData))]
    public void TextBoxBase_GetPositionFromCharIndex_ReturnsCorrectValue(Size size, bool multiline, int charIndex, Point expectedPoint)
    {
        using SubTextBoxBase textBoxBase = new() { Size = size, Multiline = multiline };
        textBoxBase.Text = "Some test text for testing GetPositionFromCharIndex method";
        Point actualPoint = textBoxBase.GetPositionFromCharIndex(charIndex);
        Assert.True(actualPoint.X >= expectedPoint.X - 1 || actualPoint.X <= expectedPoint.X + 1);
        Assert.True(actualPoint.Y >= expectedPoint.Y - 1 || actualPoint.Y <= expectedPoint.Y + 1);
    }
}
