// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.TestUtilities;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests;

public class SplitterTests
{
    [WinFormsFact]
    public void Splitter_Ctor_Default()
    {
        using SubSplitter control = new();
        Assert.Null(control.AccessibleDefaultActionDescription);
        Assert.Null(control.AccessibleDescription);
        Assert.Null(control.AccessibleName);
        Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
        Assert.False(control.AllowDrop);
        Assert.Equal(AnchorStyles.None, control.Anchor);
        Assert.False(control.AutoSize);
        Assert.Equal(Control.DefaultBackColor, control.BackColor);
        Assert.Null(control.BackgroundImage);
        Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
        Assert.Null(control.BindingContext);
        Assert.Equal(BorderStyle.None, control.BorderStyle);
        Assert.Equal(3, control.Bottom);
        Assert.Equal(new Rectangle(0, 0, 3, 3), control.Bounds);
        Assert.False(control.CanEnableIme);
        Assert.False(control.CanFocus);
        Assert.True(control.CanRaiseEvents);
        Assert.False(control.CanSelect);
        Assert.False(control.Capture);
        Assert.True(control.CausesValidation);
        Assert.Equal(new Size(3, 3), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 3, 3), control.ClientRectangle);
        Assert.Null(control.Container);
        Assert.False(control.ContainsFocus);
        Assert.Null(control.ContextMenuStrip);
        Assert.Empty(control.Controls);
        Assert.Same(control.Controls, control.Controls);
        Assert.False(control.Created);
        Assert.Same(Cursors.VSplit, control.Cursor);
        Assert.Same(Cursors.VSplit, control.DefaultCursor);
        Assert.Equal(ImeMode.Disable, control.DefaultImeMode);
        Assert.Equal(new Padding(3), control.DefaultMargin);
        Assert.Equal(Size.Empty, control.DefaultMaximumSize);
        Assert.Equal(Size.Empty, control.DefaultMinimumSize);
        Assert.Equal(Padding.Empty, control.DefaultPadding);
        Assert.Equal(new Size(3, 3), control.DefaultSize);
        Assert.False(control.DesignMode);
        Assert.Equal(new Rectangle(0, 0, 3, 3), control.DisplayRectangle);
        Assert.Equal(DockStyle.Left, control.Dock);
        Assert.False(control.DoubleBuffered);
        Assert.True(control.Enabled);
        Assert.NotNull(control.Events);
        Assert.Same(control.Events, control.Events);
        Assert.False(control.Focused);
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.Equal(Control.DefaultForeColor, control.ForeColor);
        Assert.False(control.HasChildren);
        Assert.Equal(3, control.Height);
        Assert.Equal(ImeMode.Disable, control.ImeMode);
        Assert.Equal(ImeMode.Disable, control.ImeModeBase);
        Assert.False(control.IsAccessible);
        Assert.False(control.IsMirrored);
        Assert.NotNull(control.LayoutEngine);
        Assert.Same(control.LayoutEngine, control.LayoutEngine);
        Assert.Equal(0, control.Left);
        Assert.Equal(Point.Empty, control.Location);
        Assert.Equal(new Padding(3), control.Margin);
        Assert.Equal(Size.Empty, control.MaximumSize);
        Assert.Equal(Size.Empty, control.MinimumSize);
        Assert.Equal(25, control.MinExtra);
        Assert.Equal(25, control.MinSize);
        Assert.Equal(Padding.Empty, control.Padding);
        Assert.Null(control.Parent);
        Assert.Equal(new Size(3, 3), control.PreferredSize);
        Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
        Assert.False(control.RecreatingHandle);
        Assert.Null(control.Region);
        Assert.False(control.ResizeRedraw);
        Assert.Equal(3, control.Right);
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.Equal(-1, control.SplitPosition);
        Assert.True(control.ShowFocusCues);
        Assert.True(control.ShowKeyboardCues);
        Assert.Null(control.Site);
        Assert.Equal(new Size(3, 3), control.Size);
        Assert.Equal(0, control.TabIndex);
        Assert.False(control.TabStop);
        Assert.Empty(control.Text);
        Assert.Equal(0, control.Top);
        Assert.Null(control.TopLevelControl);
        Assert.False(control.UseWaitCursor);
        Assert.True(control.Visible);
        Assert.Equal(3, control.Width);

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Splitter_CreateParams_GetDefault_ReturnsExpected()
    {
        using SubSplitter control = new();
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Null(createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0, createParams.ExStyle);
        Assert.Equal(3, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(0x56000000, createParams.Style);
        Assert.Equal(3, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(BorderStyle.None, 0x56000000, 0)]
    [InlineData(BorderStyle.Fixed3D, 0x56000000, 0x200)]
    [InlineData(BorderStyle.FixedSingle, 0x56800000, 0)]
    public void Splitter_CreateParams_GetBorderStyle_ReturnsExpected(BorderStyle borderStyle, int expectedStyle, int expectedExStyle)
    {
        using SubSplitter control = new()
        {
            BorderStyle = borderStyle
        };

        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Null(createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(expectedExStyle, createParams.ExStyle);
        Assert.Equal(3, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(expectedStyle, createParams.Style);
        Assert.Equal(3, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void Splitter_AllowDrop_Set_GetReturnsExpected(bool value)
    {
        using Splitter control = new()
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
        control.AllowDrop = !value;
        Assert.Equal(!value, control.AllowDrop);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [EnumData<AnchorStyles>]
    [InvalidEnumData<AnchorStyles>]
    public void Splitter_Anchor_Set_GetReturnsExpected(AnchorStyles value)
    {
        using Splitter control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.Anchor = value;
        Assert.Equal(AnchorStyles.None, control.Anchor);
        Assert.Equal(DockStyle.Left, control.Dock);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Anchor = value;
        Assert.Equal(AnchorStyles.None, control.Anchor);
        Assert.Equal(DockStyle.Left, control.Dock);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetImageTheoryData))]
    public void Splitter_BackgroundImage_Set_GetReturnsExpected(Image value)
    {
        using Splitter control = new()
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
    public void Splitter_BackgroundImage_SetWithHandler_CallsBackgroundImageChanged()
    {
        using Splitter control = new();
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
    public void Splitter_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
    {
        using SubSplitter control = new()
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
    public void Splitter_BackgroundImageLayout_SetWithHandler_CallsBackgroundImageLayoutChanged()
    {
        using Splitter control = new();
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
    public void Splitter_BorderStyle_Set_GetReturnsExpected(BorderStyle value)
    {
        using Splitter control = new()
        {
            BorderStyle = value
        };
        Assert.Equal(value, control.BorderStyle);
        Assert.Equal(3, control.Height);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BorderStyle = value;
        Assert.Equal(value, control.BorderStyle);
        Assert.Equal(3, control.Height);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(BorderStyle.Fixed3D, 1)]
    [InlineData(BorderStyle.FixedSingle, 1)]
    [InlineData(BorderStyle.None, 0)]
    public void Splitter_BorderStyle_SetWithHandle_GetReturnsExpected(BorderStyle value, int expectedInvalidatedCallCount)
    {
        using Splitter control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.BorderStyle = value;
        Assert.Equal(value, control.BorderStyle);
        Assert.Equal(3, control.Height);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.BorderStyle = value;
        Assert.Equal(value, control.BorderStyle);
        Assert.Equal(3, control.Height);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<BorderStyle>]
    public void Splitter_BorderStyle_SetInvalid_ThrowsInvalidEnumArgumentException(BorderStyle value)
    {
        using Splitter control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.BorderStyle = value);
    }

    public static IEnumerable<object[]> DefaultCursor_TestData()
    {
        yield return new object[] { DockStyle.Top, Cursors.HSplit };
        yield return new object[] { DockStyle.Bottom, Cursors.HSplit };
        yield return new object[] { DockStyle.Left, Cursors.VSplit };
        yield return new object[] { DockStyle.Right, Cursors.VSplit };
    }

    [WinFormsTheory]
    [MemberData(nameof(DefaultCursor_TestData))]
    public void Splitter_DefaultCursor_GetWithDockStyle_ReturnsExpected(DockStyle dock, Cursor expected)
    {
        using SubSplitter control = new()
        {
            Dock = dock
        };
        Assert.Equal(expected, control.Cursor);
        Assert.Equal(expected, control.DefaultCursor);
    }

    [WinFormsTheory]
    [InlineData(DockStyle.Left)]
    [InlineData(DockStyle.Right)]
    [InlineData(DockStyle.Top)]
    [InlineData(DockStyle.Bottom)]
    public void Splitter_Dock_Set_GetReturnsExpected(DockStyle value)
    {
        using Splitter control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.Dock = value;
        Assert.Equal(value, control.Dock);
        Assert.Equal(AnchorStyles.None, control.Anchor);
        Assert.Equal(3, control.Width);
        Assert.Equal(3, control.Height);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Dock = value;
        Assert.Equal(value, control.Dock);
        Assert.Equal(AnchorStyles.None, control.Anchor);
        Assert.Equal(3, control.Width);
        Assert.Equal(3, control.Height);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(DockStyle.Left, 40, 0)]
    [InlineData(DockStyle.Right, 40, 0)]
    [InlineData(DockStyle.Top, 50, 1)]
    [InlineData(DockStyle.Bottom, 50, 1)]
    public void Splitter_Dock_SetCustomWidthHeight_GetReturnsExpected(DockStyle value, int expectedHeight, int expectedLayoutCallCount)
    {
        using Splitter control = new()
        {
            Width = 50,
            Height = 40
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.Dock = value;
        Assert.Equal(value, control.Dock);
        Assert.Equal(AnchorStyles.None, control.Anchor);
        Assert.Equal(50, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Dock = value;
        Assert.Equal(value, control.Dock);
        Assert.Equal(AnchorStyles.None, control.Anchor);
        Assert.Equal(50, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(DockStyle.Left)]
    [InlineData(DockStyle.Right)]
    [InlineData(DockStyle.Top)]
    [InlineData(DockStyle.Bottom)]
    public void Splitter_Dock_SetWithOldValue_GetReturnsExpected(DockStyle value)
    {
        using Splitter control = new()
        {
            Dock = DockStyle.Top
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.Dock = value;
        Assert.Equal(value, control.Dock);
        Assert.Equal(AnchorStyles.None, control.Anchor);
        Assert.Equal(3, control.Width);
        Assert.Equal(3, control.Height);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Dock = value;
        Assert.Equal(value, control.Dock);
        Assert.Equal(AnchorStyles.None, control.Anchor);
        Assert.Equal(3, control.Width);
        Assert.Equal(3, control.Height);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(DockStyle.Left, 40, 1)]
    [InlineData(DockStyle.Right, 40, 1)]
    [InlineData(DockStyle.Top, 50, 0)]
    [InlineData(DockStyle.Bottom, 50, 0)]
    public void Splitter_Dock_SetWithOldValueCustomWidthHeight_GetReturnsExpected(DockStyle value, int expectedWidth, int expectedLayoutCallCount)
    {
        using Splitter control = new()
        {
            Dock = DockStyle.Top,
            Width = 50,
            Height = 40
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.Dock = value;
        Assert.Equal(value, control.Dock);
        Assert.Equal(AnchorStyles.None, control.Anchor);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(40, control.Height);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Dock = value;
        Assert.Equal(value, control.Dock);
        Assert.Equal(AnchorStyles.None, control.Anchor);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(40, control.Height);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Dock_SetWithParent_TestData()
    {
        yield return new object[] { DockStyle.Bottom, 1 };
        yield return new object[] { DockStyle.Left, 0 };
        yield return new object[] { DockStyle.Right, 1 };
        yield return new object[] { DockStyle.Top, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Dock_SetWithParent_TestData))]
    public void Splitter_Dock_SetWithParent_GetReturnsExpected(DockStyle value, int expectedParentLayoutCallCount)
    {
        using Splitter parent = new();
        using Splitter control = new()
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
            Assert.Equal("Dock", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.Dock = value;
            Assert.Equal(value, control.Dock);
            Assert.Equal(AnchorStyles.None, control.Anchor);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Dock = value;
            Assert.Equal(value, control.Dock);
            Assert.Equal(AnchorStyles.None, control.Anchor);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> Splitter_Dock_SetWithOldValueWithParent_TestData()
    {
        yield return new object[] { DockStyle.Bottom, DockStyle.Bottom, 0 };
        yield return new object[] { DockStyle.Left, DockStyle.Bottom, 1 };
        yield return new object[] { DockStyle.Right, DockStyle.Bottom, 1 };
        yield return new object[] { DockStyle.Top, DockStyle.Bottom, 1 };

        yield return new object[] { DockStyle.Bottom, DockStyle.Left, 1 };
        yield return new object[] { DockStyle.Left, DockStyle.Left, 0 };
        yield return new object[] { DockStyle.Right, DockStyle.Left, 1 };
        yield return new object[] { DockStyle.Top, DockStyle.Left, 1 };

        yield return new object[] { DockStyle.Bottom, DockStyle.Right, 1 };
        yield return new object[] { DockStyle.Left, DockStyle.Right, 1 };
        yield return new object[] { DockStyle.Right, DockStyle.Right, 0 };
        yield return new object[] { DockStyle.Top, DockStyle.Right, 1 };

        yield return new object[] { DockStyle.Bottom, DockStyle.Top, 1 };
        yield return new object[] { DockStyle.Left, DockStyle.Top, 1 };
        yield return new object[] { DockStyle.Right, DockStyle.Top, 1 };
        yield return new object[] { DockStyle.Top, DockStyle.Top, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Splitter_Dock_SetWithOldValueWithParent_TestData))]
    public void Splitter_Dock_SetWithOldValueWithParent_GetReturnsExpected(DockStyle oldValue, DockStyle value, int expectedParentLayoutCallCount)
    {
        using Splitter parent = new();
        using Splitter control = new()
        {
            Dock = oldValue,
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Dock", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.Dock = value;
            Assert.Equal(value, control.Dock);
            Assert.Equal(AnchorStyles.None, control.Anchor);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Dock = value;
            Assert.Equal(value, control.Dock);
            Assert.Equal(AnchorStyles.None, control.Anchor);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(DockStyle.Left)]
    [InlineData(DockStyle.Right)]
    [InlineData(DockStyle.Top)]
    [InlineData(DockStyle.Bottom)]
    public void Splitter_Dock_SetWithHandle_GetReturnsExpected(DockStyle value)
    {
        using Splitter control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.Dock = value;
        Assert.Equal(value, control.Dock);
        Assert.Equal(AnchorStyles.None, control.Anchor);
        Assert.Equal(3, control.Width);
        Assert.Equal(3, control.Height);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Dock = value;
        Assert.Equal(value, control.Dock);
        Assert.Equal(AnchorStyles.None, control.Anchor);
        Assert.Equal(3, control.Width);
        Assert.Equal(3, control.Height);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Splitter_Dock_SetWithHandler_CallsDockChanged()
    {
        using Splitter control = new()
        {
            Dock = DockStyle.Bottom
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.DockChanged += handler;

        // Set different.
        control.Dock = DockStyle.Top;
        Assert.Equal(DockStyle.Top, control.Dock);
        Assert.Equal(1, callCount);

        // Set same.
        control.Dock = DockStyle.Top;
        Assert.Equal(DockStyle.Top, control.Dock);
        Assert.Equal(1, callCount);

        // Set different.
        control.Dock = DockStyle.Left;
        Assert.Equal(DockStyle.Left, control.Dock);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.DockChanged -= handler;
        control.Dock = DockStyle.Top;
        Assert.Equal(DockStyle.Top, control.Dock);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<DockStyle>]
    [InlineData(DockStyle.None)]
    [InlineData(DockStyle.Fill)]
    public void Splitter_Dock_SetInvalid_ThrowsInvalidEnumArgumentException(DockStyle value)
    {
        using Splitter control = new();
        Assert.Throws<ArgumentException>(() => control.Dock = value);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetFontTheoryData))]
    public void Splitter_Font_Set_GetReturnsExpected(Font value)
    {
        using SubSplitter control = new()
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

    [WinFormsFact]
    public void Splitter_Font_SetWithHandler_CallsFontChanged()
    {
        using Splitter control = new();
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

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetForeColorTheoryData))]
    public void Splitter_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using Splitter control = new()
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
        yield return new object[] { Color.Red, Color.Red, 1 };
        yield return new object[] { Color.FromArgb(254, 1, 2, 3), Color.FromArgb(254, 1, 2, 3), 1 };
        yield return new object[] { Color.Empty, Control.DefaultForeColor, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(ForeColor_SetWithHandle_TestData))]
    public void Splitter_ForeColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
    {
        using Splitter control = new();
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
    public void Splitter_ForeColor_SetWithHandler_CallsForeColorChanged()
    {
        using Splitter control = new();
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
        Assert.Equal(Control.DefaultForeColor, control.ForeColor);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.ForeColorChanged -= handler;
        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.Equal(2, callCount);
    }

    public static IEnumerable<object[]> ImeMode_Set_TestData()
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
        yield return new object[] { ImeMode.OnHalf, ImeMode.On };
    }

    [WinFormsTheory]
    [MemberData(nameof(ImeMode_Set_TestData))]
    public void Splitter_ImeMode_Set_GetReturnsExpected(ImeMode value, ImeMode expected)
    {
        using Splitter control = new()
        {
            ImeMode = value
        };
        Assert.Equal(expected, control.ImeMode);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ImeMode = value;
        Assert.Equal(expected, control.ImeMode);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ImeMode_Set_TestData))]
    public void Splitter_ImeMode_SetWithHandle_GetReturnsExpected(ImeMode value, ImeMode expected)
    {
        using Splitter control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ImeMode = value;
        Assert.Equal(expected, control.ImeMode);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ImeMode = value;
        Assert.Equal(expected, control.ImeMode);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Splitter_ImeMode_SetWithHandler_CallsImeModeChanged()
    {
        using Splitter control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.ImeModeChanged += handler;

        // Set different.
        control.ImeMode = ImeMode.On;
        Assert.Equal(ImeMode.On, control.ImeMode);
        Assert.Equal(0, callCount);

        // Set same.
        control.ImeMode = ImeMode.On;
        Assert.Equal(ImeMode.On, control.ImeMode);
        Assert.Equal(0, callCount);

        // Set different.
        control.ImeMode = ImeMode.Off;
        Assert.Equal(ImeMode.Off, control.ImeMode);
        Assert.Equal(0, callCount);

        // Remove handler.
        control.ImeModeChanged -= handler;
        control.ImeMode = ImeMode.Off;
        Assert.Equal(ImeMode.Off, control.ImeMode);
        Assert.Equal(0, callCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<ImeMode>]
    public void Splitter_ImeMode_SetInvalid_ThrowsInvalidEnumArgumentException(ImeMode value)
    {
        using Splitter control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.ImeMode = value);
    }

    [WinFormsTheory]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(25, 25)]
    [InlineData(50, 50)]
    public void Splitter_MinExtra_Set_GetReturnsExpected(int value, int expected)
    {
        using Splitter control = new()
        {
            MinExtra = value
        };
        Assert.Equal(expected, control.MinExtra);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.MinExtra = value;
        Assert.Equal(expected, control.MinExtra);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(25, 25)]
    [InlineData(50, 50)]
    public void Splitter_MinExtra_SetWithHandle_GetReturnsExpected(int value, int expected)
    {
        using Splitter control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.MinExtra = value;
        Assert.Equal(expected, control.MinExtra);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.MinExtra = value;
        Assert.Equal(expected, control.MinExtra);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(25, 25)]
    [InlineData(50, 50)]
    public void Splitter_MinSize_Set_GetReturnsExpected(int value, int expected)
    {
        using Splitter control = new()
        {
            MinSize = value
        };
        Assert.Equal(expected, control.MinSize);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.MinSize = value;
        Assert.Equal(expected, control.MinSize);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(25, 25)]
    [InlineData(50, 50)]
    public void Splitter_MinSize_SetWithHandle_GetReturnsExpected(int value, int expected)
    {
        using Splitter control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.MinSize = value;
        Assert.Equal(expected, control.MinSize);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.MinSize = value;
        Assert.Equal(expected, control.MinSize);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> SplitPosition_Set_TestData()
    {
        yield return new object[] { DockStyle.Right, -2 };
        yield return new object[] { DockStyle.Right, 1 };
        yield return new object[] { DockStyle.Right, 0 };
        yield return new object[] { DockStyle.Right, 25 };

        yield return new object[] { DockStyle.Left, -2 };
        yield return new object[] { DockStyle.Left, 1 };
        yield return new object[] { DockStyle.Left, 0 };
        yield return new object[] { DockStyle.Left, 25 };

        yield return new object[] { DockStyle.Top, -2 };
        yield return new object[] { DockStyle.Top, 1 };
        yield return new object[] { DockStyle.Top, 0 };
        yield return new object[] { DockStyle.Top, 25 };

        yield return new object[] { DockStyle.Bottom, -2 };
        yield return new object[] { DockStyle.Bottom, 1 };
        yield return new object[] { DockStyle.Bottom, 0 };
        yield return new object[] { DockStyle.Bottom, 25 };
    }

    [WinFormsTheory]
    [MemberData(nameof(SplitPosition_Set_TestData))]
    public void Splitter_SplitPosition_Set_GetReturnsExpected(DockStyle dock, int value)
    {
        using Splitter control = new()
        {
            Dock = dock
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.SplitPosition = value;
        Assert.Equal(-1, control.SplitPosition);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SplitPosition = value;
        Assert.Equal(-1, control.SplitPosition);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SplitPosition_Set_TestData))]
    public void Splitter_SplitPosition_SetWithParent_GetReturnsExpected(DockStyle dock, int value)
    {
        using Control parent = new()
        {
            Width = 100,
            Height = 100
        };
        using Splitter control = new()
        {
            Parent = parent,
            Dock = dock
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.SplitPosition = value;
        Assert.Equal(-1, control.SplitPosition);
        Assert.False(control.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);

        // Set same.
        control.SplitPosition = value;
        Assert.Equal(-1, control.SplitPosition);
        Assert.False(control.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);
    }

    public static IEnumerable<object[]> SplitPosition_SetWithParentNoChild_TestData()
    {
        foreach (DockStyle childDock in Enum.GetValues(typeof(DockStyle)))
        {
            yield return new object[] { childDock, DockStyle.Right, -2 };
            yield return new object[] { childDock, DockStyle.Right, 1 };
            yield return new object[] { childDock, DockStyle.Right, 0 };
            yield return new object[] { childDock, DockStyle.Right, 25 };

            yield return new object[] { childDock, DockStyle.Left, -2 };
            yield return new object[] { childDock, DockStyle.Left, 1 };
            yield return new object[] { childDock, DockStyle.Left, 0 };
            yield return new object[] { childDock, DockStyle.Left, 25 };

            yield return new object[] { childDock, DockStyle.Top, -2 };
            yield return new object[] { childDock, DockStyle.Top, 1 };
            yield return new object[] { childDock, DockStyle.Top, 0 };
            yield return new object[] { childDock, DockStyle.Top, 25 };

            yield return new object[] { childDock, DockStyle.Bottom, -2 };
            yield return new object[] { childDock, DockStyle.Bottom, 1 };
            yield return new object[] { childDock, DockStyle.Bottom, 0 };
            yield return new object[] { childDock, DockStyle.Bottom, 25 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(SplitPosition_SetWithParentNoChild_TestData))]
    public void Splitter_SplitPosition_SetWithParentNoChild_GetReturnsExpected(DockStyle childDock, DockStyle dock, int value)
    {
        using Control parent = new()
        {
            Width = 100,
            Height = 100
        };
        using Control child = new()
        {
            Parent = parent,
            Dock = childDock,
            Bounds = new Rectangle(10, 10, 50, 50)
        };
        using Splitter control = new()
        {
            Parent = parent,
            Dock = dock,
            Location = new Point(50, 0)
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.SplitPosition = value;
        Assert.Equal(-1, control.SplitPosition);
        Assert.False(control.IsHandleCreated);
        Assert.False(child.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);

        // Set same.
        control.SplitPosition = value;
        Assert.Equal(-1, control.SplitPosition);
        Assert.False(control.IsHandleCreated);
        Assert.False(child.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-2, 25)]
    [InlineData(-1, 25)]
    [InlineData(0, 25)]
    [InlineData(25, 25)]
    [InlineData(40, 40)]
    [InlineData(50, 50)]
    [InlineData(100, 72)]
    [InlineData(110, 72)]
    public void Splitter_SplitPosition_SetWithParentLeftChild_GetReturnsExpected(int value, int expected)
    {
        using Control parent = new()
        {
            Width = 100,
            Height = 100
        };
        using Control child = new()
        {
            Parent = parent,
            Bounds = new Rectangle(0, 0, 0, 100)
        };
        using Splitter control = new()
        {
            Parent = parent,
            Dock = DockStyle.Left,
            Bounds = new Rectangle(50, 0, 3, 3)
        };
        Assert.Equal(child.Right, control.Left);
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.SplitPosition = value;
        Assert.Equal(expected, control.SplitPosition);
        Assert.False(control.IsHandleCreated);
        Assert.False(child.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);

        // Set same.
        control.SplitPosition = value;
        Assert.Equal(-1, control.SplitPosition);
        Assert.False(control.IsHandleCreated);
        Assert.False(child.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SplitPosition_Set_TestData))]
    public void Splitter_SplitPosition_SetWithHandle_GetReturnsExpected(DockStyle dock, int value)
    {
        using Splitter control = new()
        {
            Dock = dock
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.SplitPosition = value;
        Assert.Equal(-1, control.SplitPosition);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SplitPosition = value;
        Assert.Equal(-1, control.SplitPosition);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(SplitPosition_Set_TestData))]
    public void Splitter_SplitPosition_SetWithParentHandle_GetReturnsExpected(DockStyle dock, int value)
    {
        using Control parent = new()
        {
            Width = 100,
            Height = 100
        };
        using Splitter control = new()
        {
            Parent = parent,
            Dock = dock
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.SplitPosition = value;
        Assert.Equal(-1, control.SplitPosition);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SplitPosition = value;
        Assert.Equal(-1, control.SplitPosition);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(-2, 25)]
    [InlineData(-1, 25)]
    [InlineData(0, 25)]
    [InlineData(25, 25)]
    [InlineData(40, 40)]
    [InlineData(50, 50)]
    [InlineData(100, 72)]
    [InlineData(110, 72)]
    public void Splitter_SplitPosition_SetWithParentLeftChildWithHandle_GetReturnsExpected(int value, int expected)
    {
        using Control parent = new()
        {
            Width = 100,
            Height = 100
        };
        using Control child = new()
        {
            Parent = parent,
            Bounds = new Rectangle(0, 0, 0, 100)
        };
        using Splitter control = new()
        {
            Parent = parent,
            Dock = DockStyle.Left,
            Bounds = new Rectangle(50, 0, 3, 3)
        };
        Assert.Equal(child.Right, control.Left);
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.SplitPosition = value;
        Assert.Equal(expected, control.SplitPosition);
        Assert.True(control.IsHandleCreated);
        Assert.True(child.IsHandleCreated);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.SplitPosition = value;
        Assert.Equal(-1, control.SplitPosition);
        Assert.True(control.IsHandleCreated);
        Assert.True(child.IsHandleCreated);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Splitter_SplitPosition_SetWithHandler_DoesNotCallsSplitterMoved()
    {
        using Splitter control = new()
        {
            SplitPosition = 0
        };
        int splitterMovingCallCount = 0;
        control.SplitterMoving += (sender, e) => splitterMovingCallCount++;
        int callCount = 0;
        SplitterEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(control.Left, e.X);
            Assert.Equal(control.Top, e.Y);
            Assert.Equal(control.Left + control.Bounds.Width / 2, e.SplitX);
            Assert.Equal(control.Top + control.Bounds.Height / 2, e.SplitY);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.SplitterMoved += handler;

        // Set different.
        control.SplitPosition = 1;
        Assert.Equal(-1, control.SplitPosition);
        Assert.Equal(0, splitterMovingCallCount);
        Assert.Equal(0, callCount);

        // Set same.
        control.SplitPosition = 1;
        Assert.Equal(-1, control.SplitPosition);
        Assert.Equal(0, splitterMovingCallCount);
        Assert.Equal(0, callCount);

        // Set different.
        control.SplitPosition = 2;
        Assert.Equal(-1, control.SplitPosition);
        Assert.Equal(0, splitterMovingCallCount);
        Assert.Equal(0, callCount);

        // Remove handler.
        control.SplitterMoved -= handler;
        control.SplitPosition = 1;
        Assert.Equal(-1, control.SplitPosition);
        Assert.Equal(0, splitterMovingCallCount);
        Assert.Equal(0, callCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void Splitter_TabStop_Set_GetReturnsExpected(bool value)
    {
        using Splitter control = new()
        {
            TabStop = value
        };
        Assert.Equal(value, control.TabStop);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.TabStop = value;
        Assert.Equal(value, control.TabStop);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.TabStop = value;
        Assert.Equal(value, control.TabStop);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void Splitter_TabStop_SetWithHandle_GetReturnsExpected(bool value)
    {
        using Splitter control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.TabStop = value;
        Assert.Equal(value, control.TabStop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.TabStop = value;
        Assert.Equal(value, control.TabStop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.TabStop = value;
        Assert.Equal(value, control.TabStop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Splitter_TabStop_SetWithHandler_CallsTabStopChanged()
    {
        using Splitter control = new()
        {
            TabStop = true
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.TabStopChanged += handler;

        // Set different.
        control.TabStop = false;
        Assert.False(control.TabStop);
        Assert.Equal(1, callCount);

        // Set same.
        control.TabStop = false;
        Assert.False(control.TabStop);
        Assert.Equal(1, callCount);

        // Set different.
        control.TabStop = true;
        Assert.True(control.TabStop);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.TabStopChanged -= handler;
        control.TabStop = false;
        Assert.False(control.TabStop);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void Splitter_Text_Set_GetReturnsExpected(string value, string expected)
    {
        using Splitter control = new()
        {
            Text = value
        };
        Assert.Equal(expected, control.Text);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void Splitter_Text_SetWithHandle_GetReturnsExpected(string value, string expected)
    {
        using Splitter control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Splitter_Text_SetWithHandler_CallsTextChanged()
    {
        using Splitter control = new();
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
        Assert.Same("text", control.Text);
        Assert.Equal(1, callCount);

        // Set same.
        control.Text = "text";
        Assert.Same("text", control.Text);
        Assert.Equal(1, callCount);

        // Set different.
        control.Text = null;
        Assert.Empty(control.Text);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.TextChanged -= handler;
        control.Text = "text";
        Assert.Same("text", control.Text);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void Splitter_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using SubSplitter control = new();
        Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
    }

    [WinFormsTheory]
    [InlineData(ControlStyles.ContainerControl, false)]
    [InlineData(ControlStyles.UserPaint, true)]
    [InlineData(ControlStyles.Opaque, false)]
    [InlineData(ControlStyles.ResizeRedraw, false)]
    [InlineData(ControlStyles.FixedWidth, false)]
    [InlineData(ControlStyles.FixedHeight, false)]
    [InlineData(ControlStyles.StandardClick, true)]
    [InlineData(ControlStyles.Selectable, false)]
    [InlineData(ControlStyles.UserMouse, false)]
    [InlineData(ControlStyles.SupportsTransparentBackColor, false)]
    [InlineData(ControlStyles.StandardDoubleClick, true)]
    [InlineData(ControlStyles.AllPaintingInWmPaint, true)]
    [InlineData(ControlStyles.CacheText, false)]
    [InlineData(ControlStyles.EnableNotifyMessage, false)]
    [InlineData(ControlStyles.DoubleBuffer, false)]
    [InlineData(ControlStyles.OptimizedDoubleBuffer, false)]
    [InlineData(ControlStyles.UseTextForAccessibility, true)]
    [InlineData((ControlStyles)0, true)]
    [InlineData((ControlStyles)int.MaxValue, false)]
    [InlineData((ControlStyles)(-1), false)]
    public void Splitter_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
    {
        using SubSplitter control = new();
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    [WinFormsFact]
    public void Splitter_GetTopLevel_Invoke_ReturnsExpected()
    {
        using SubSplitter control = new();
        Assert.False(control.GetTopLevel());
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void Splitter_OnEnter_Invoke_CallsEnter(EventArgs eventArgs)
    {
        using SubSplitter control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.Enter += handler;
        control.OnEnter(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.Enter -= handler;
        control.OnEnter(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void Splitter_OnHandleCreated_Invoke_CallsHandleCreated(EventArgs eventArgs)
    {
        using SubSplitter control = new();
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
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.HandleCreated -= handler;
        control.OnHandleCreated(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void Splitter_OnHandleCreated_InvokeWithHandle_CallsHandleCreated(EventArgs eventArgs)
    {
        using SubSplitter control = new();
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
        Assert.True(control.IsHandleCreated);

        // Remove handler.
        control.HandleCreated -= handler;
        control.OnHandleCreated(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void Splitter_OnHandleDestroyed_Invoke_CallsHandleDestroyed(EventArgs eventArgs)
    {
        using SubSplitter control = new();
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

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void Splitter_OnHandleDestroyed_InvokeWithHandle_CallsHandleDestroyed(EventArgs eventArgs)
    {
        using SubSplitter control = new();
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
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.HandleDestroyed -= handler;
        control.OnHandleDestroyed(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> KeyEventArgs_TestData()
    {
        yield return new object[] { new KeyEventArgs(Keys.None) };
        yield return new object[] { new KeyEventArgs(Keys.Cancel) };
        yield return new object[] { new KeyEventArgs(Keys.Escape) };
    }

    [WinFormsTheory]
    [MemberData(nameof(KeyEventArgs_TestData))]
    public void Splitter_OnKeyDown_Invoke_CallsKeyDown(KeyEventArgs eventArgs)
    {
        using SubSplitter control = new();
        int splitterMovingCallCount = 0;
        control.SplitterMoving += (sender, e) => splitterMovingCallCount++;
        int splitterMovedCallCount = 0;
        control.SplitterMoved += (sender, e) => splitterMovedCallCount++;
        int callCount = 0;
        KeyEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.KeyDown += handler;
        control.OnKeyDown(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, splitterMovingCallCount);
        Assert.Equal(0, splitterMovedCallCount);

        // Remove handler.
        control.KeyDown -= handler;
        control.OnKeyDown(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, splitterMovingCallCount);
        Assert.Equal(0, splitterMovedCallCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetKeyPressEventArgsTheoryData))]
    public void Splitter_OnKeyPress_Invoke_CallsKeyPress(KeyPressEventArgs eventArgs)
    {
        using SubSplitter control = new();
        int splitterMovingCallCount = 0;
        control.SplitterMoving += (sender, e) => splitterMovingCallCount++;
        int splitterMovedCallCount = 0;
        control.SplitterMoved += (sender, e) => splitterMovedCallCount++;
        int callCount = 0;
        KeyPressEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.KeyPress += handler;
        control.OnKeyPress(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, splitterMovingCallCount);
        Assert.Equal(0, splitterMovedCallCount);

        // Remove handler.
        control.KeyPress -= handler;
        control.OnKeyPress(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, splitterMovingCallCount);
        Assert.Equal(0, splitterMovedCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(KeyEventArgs_TestData))]
    public void Splitter_OnKeyUp_Invoke_CallsKeyUp(KeyEventArgs eventArgs)
    {
        using SubSplitter control = new();
        int splitterMovingCallCount = 0;
        control.SplitterMoving += (sender, e) => splitterMovingCallCount++;
        int splitterMovedCallCount = 0;
        control.SplitterMoved += (sender, e) => splitterMovedCallCount++;
        int callCount = 0;
        KeyEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.KeyUp += handler;
        control.OnKeyUp(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, splitterMovingCallCount);
        Assert.Equal(0, splitterMovedCallCount);

        // Remove handler.
        control.KeyUp -= handler;
        control.OnKeyUp(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, splitterMovingCallCount);
        Assert.Equal(0, splitterMovedCallCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void Splitter_OnLeave_Invoke_CallsLeave(EventArgs eventArgs)
    {
        using SubSplitter control = new();
        int splitterMovingCallCount = 0;
        control.SplitterMoving += (sender, e) => splitterMovingCallCount++;
        int splitterMovedCallCount = 0;
        control.SplitterMoved += (sender, e) => splitterMovedCallCount++;
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.Leave += handler;
        control.OnLeave(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, splitterMovingCallCount);
        Assert.Equal(0, splitterMovedCallCount);

        // Remove handler.
        control.Leave -= handler;
        control.OnLeave(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, splitterMovingCallCount);
        Assert.Equal(0, splitterMovedCallCount);
    }

    public static IEnumerable<object[]> OnMouseDown_TestData()
    {
        yield return new object[] { new MouseEventArgs(MouseButtons.Left, 0, 2, 3, 4) };
        yield return new object[] { new MouseEventArgs(MouseButtons.Left, 1, 2, 3, 4) };
        yield return new object[] { new MouseEventArgs(MouseButtons.Middle, 1, 2, 3, 4) };
        yield return new object[] { new HandledMouseEventArgs(MouseButtons.Left, 1, 2, 3, 4) };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseDown_TestData))]
    public void Splitter_OnMouseDown_Invoke_CallsMouseDown(MouseEventArgs eventArgs)
    {
        using SubSplitter control = new();
        int splitterMovingCallCount = 0;
        control.SplitterMoving += (sender, e) => splitterMovingCallCount++;
        int splitterMovedCallCount = 0;
        control.SplitterMoved += (sender, e) => splitterMovedCallCount++;
        int callCount = 0;
        MouseEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.MouseDown += handler;
        control.OnMouseDown(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, splitterMovingCallCount);
        Assert.Equal(0, splitterMovedCallCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.MouseDown -= handler;
        control.OnMouseDown(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, splitterMovingCallCount);
        Assert.Equal(0, splitterMovedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnMouseDown_WithTarget_TestData()
    {
        yield return new object[] { new MouseEventArgs(MouseButtons.Left, 0, 2, 3, 4), false };
        yield return new object[] { new MouseEventArgs(MouseButtons.Left, 1, 2, 3, 4), true };
        yield return new object[] { new MouseEventArgs(MouseButtons.Middle, 1, 2, 3, 4), false };
        yield return new object[] { new HandledMouseEventArgs(MouseButtons.Left, 1, 2, 3, 4), true };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseDown_WithTarget_TestData))]
    public void Splitter_OnMouseDown_InvokeWithTarget_CallsMouseDown(MouseEventArgs eventArgs, bool expectedIsHandleCreated)
    {
        using Control parent = new()
        {
            Width = 100,
            Height = 100
        };
        using Control child = new()
        {
            Parent = parent,
            Bounds = new Rectangle(0, 0, 0, 100)
        };
        using SubSplitter control = new()
        {
            Parent = parent,
            Dock = DockStyle.Left,
            Bounds = new Rectangle(50, 0, 3, 3)
        };
        Assert.Equal(child.Right, control.Left);
        int splitterMovingCallCount = 0;
        control.SplitterMoving += (sender, e) => splitterMovingCallCount++;
        int splitterMovedCallCount = 0;
        control.SplitterMoved += (sender, e) => splitterMovedCallCount++;
        int callCount = 0;
        MouseEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.MouseDown += handler;
        control.OnMouseDown(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, splitterMovingCallCount);
        Assert.Equal(0, splitterMovedCallCount);
        Assert.Equal(expectedIsHandleCreated, control.IsHandleCreated);
        Assert.Equal(expectedIsHandleCreated, child.IsHandleCreated);
        Assert.Equal(expectedIsHandleCreated, parent.IsHandleCreated);

        // Remove handler.
        control.MouseDown -= handler;
        control.OnMouseDown(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, splitterMovingCallCount);
        Assert.Equal(0, splitterMovedCallCount);
        Assert.Equal(expectedIsHandleCreated, control.IsHandleCreated);
        Assert.Equal(expectedIsHandleCreated, child.IsHandleCreated);
        Assert.Equal(expectedIsHandleCreated, parent.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseDown_TestData))]
    public void Splitter_OnMouseDown_InvokeWithHandle_CallsMouseDown(MouseEventArgs eventArgs)
    {
        using SubSplitter control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int splitterMovingCallCount = 0;
        control.SplitterMoving += (sender, e) => splitterMovingCallCount++;
        int splitterMovedCallCount = 0;
        control.SplitterMoved += (sender, e) => splitterMovedCallCount++;
        int callCount = 0;
        MouseEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.MouseDown += handler;
        control.OnMouseDown(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, splitterMovingCallCount);
        Assert.Equal(0, splitterMovedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.MouseDown -= handler;
        control.OnMouseDown(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, splitterMovingCallCount);
        Assert.Equal(0, splitterMovedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseDown_TestData))]
    public void Splitter_OnMouseDown_InvokeWithTargetWithHandle_CallsMouseDown(MouseEventArgs eventArgs)
    {
        using Control parent = new()
        {
            Width = 100,
            Height = 100
        };
        using Control child = new()
        {
            Parent = parent,
            Bounds = new Rectangle(0, 0, 0, 100)
        };
        using SubSplitter control = new()
        {
            Parent = parent,
            Dock = DockStyle.Left,
            Bounds = new Rectangle(50, 0, 3, 3)
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        Assert.Equal(child.Right, control.Left);
        int splitterMovingCallCount = 0;
        control.SplitterMoving += (sender, e) => splitterMovingCallCount++;
        int splitterMovedCallCount = 0;
        control.SplitterMoved += (sender, e) => splitterMovedCallCount++;
        int callCount = 0;
        MouseEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.MouseDown += handler;
        control.OnMouseDown(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, splitterMovingCallCount);
        Assert.Equal(0, splitterMovedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.True(child.IsHandleCreated);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.MouseDown -= handler;
        control.OnMouseDown(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, splitterMovingCallCount);
        Assert.Equal(0, splitterMovedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.True(child.IsHandleCreated);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Splitter_OnMouseDown_NullE_ThrowsNullReferenceException()
    {
        using SubSplitter control = new();
        Assert.Throws<NullReferenceException>(() => control.OnMouseDown(null));
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetMouseEventArgsTheoryData))]
    public void Splitter_OnMouseMove_Invoke_CallsMouseMove(MouseEventArgs eventArgs)
    {
        using SubSplitter control = new();
        int splitterMovingCallCount = 0;
        control.SplitterMoving += (sender, e) => splitterMovingCallCount++;
        int splitterMovedCallCount = 0;
        control.SplitterMoved += (sender, e) => splitterMovedCallCount++;
        int callCount = 0;
        MouseEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.MouseMove += handler;
        control.OnMouseMove(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, splitterMovingCallCount);
        Assert.Equal(0, splitterMovedCallCount);

        // Remove handler.
        control.MouseMove -= handler;
        control.OnMouseMove(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, splitterMovingCallCount);
        Assert.Equal(0, splitterMovedCallCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetMouseEventArgsTheoryData))]
    public void Splitter_OnMouseUp_Invoke_CallsMouseUp(MouseEventArgs eventArgs)
    {
        using SubSplitter control = new();
        int splitterMovingCallCount = 0;
        control.SplitterMoving += (sender, e) => splitterMovingCallCount++;
        int splitterMovedCallCount = 0;
        control.SplitterMoved += (sender, e) => splitterMovedCallCount++;
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
        Assert.Equal(0, splitterMovingCallCount);
        Assert.Equal(0, splitterMovedCallCount);

        // Remove handler.
        control.MouseUp -= handler;
        control.OnMouseUp(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(0, splitterMovingCallCount);
        Assert.Equal(0, splitterMovedCallCount);
    }

    public static IEnumerable<object[]> SplitterEventArgs_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new SplitterEventArgs(0, 0, 0, 0) };
    }

    [WinFormsTheory]
    [MemberData(nameof(SplitterEventArgs_TestData))]
    public void Splitter_OnSplitterMoved_Invoke_CallsSplitterMoved(SplitterEventArgs eventArgs)
    {
        using SubSplitter control = new();
        int callCount = 0;
        SplitterEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.SplitterMoved += handler;
        control.OnSplitterMoved(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.SplitterMoved -= handler;
        control.OnSplitterMoved(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(SplitterEventArgs_TestData))]
    public void Splitter_OnSplitterMoving_Invoke_CallsSplitterMoving(SplitterEventArgs eventArgs)
    {
        using SubSplitter control = new();
        int callCount = 0;
        SplitterEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.SplitterMoving += handler;
        control.OnSplitterMoving(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.SplitterMoving -= handler;
        control.OnSplitterMoving(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void Control_ToString_Invoke_ReturnsExpected()
    {
        using Splitter control = new();
        Assert.Equal("System.Windows.Forms.Splitter, MinExtra: 25, MinSize: 25", control.ToString());
    }

    private class SubSplitter : Splitter
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

        public new void OnEnter(EventArgs e) => base.OnEnter(e);

        public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

        public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);

        public new void OnKeyDown(KeyEventArgs e) => base.OnKeyDown(e);

        public new void OnKeyPress(KeyPressEventArgs e) => base.OnKeyPress(e);

        public new void OnKeyUp(KeyEventArgs e) => base.OnKeyUp(e);

        public new void OnLeave(EventArgs e) => base.OnLeave(e);

        public new void OnMouseDown(MouseEventArgs e) => base.OnMouseDown(e);

        public new void OnMouseMove(MouseEventArgs e) => base.OnMouseMove(e);

        public new void OnMouseUp(MouseEventArgs e) => base.OnMouseUp(e);

        public new void OnSplitterMoved(SplitterEventArgs sevent) => base.OnSplitterMoved(sevent);

        public new void OnSplitterMoving(SplitterEventArgs sevent) => base.OnSplitterMoving(sevent);
    }
}
