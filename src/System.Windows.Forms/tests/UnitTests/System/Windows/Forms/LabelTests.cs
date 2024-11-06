// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Automation;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests;

public class LabelTests
{
    [WinFormsFact]
    public void Label_Ctor_Default()
    {
        using SubLabel control = new();
        Assert.Null(control.AccessibleDefaultActionDescription);
        Assert.Null(control.AccessibleDescription);
        Assert.Null(control.AccessibleName);
        Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
        Assert.False(control.AllowDrop);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        Assert.False(control.AutoEllipsis);
        Assert.False(control.AutoSize);
        Assert.Equal(Control.DefaultBackColor, control.BackColor);
        Assert.Null(control.BackgroundImage);
        Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
        Assert.Null(control.BindingContext);
        Assert.Equal(BorderStyle.None, control.BorderStyle);
        Assert.Equal(23, control.Bottom);
        Assert.Equal(new Rectangle(0, 0, 100, 23), control.Bounds);
        Assert.False(control.CanEnableIme);
        Assert.False(control.CanFocus);
        Assert.True(control.CanRaiseEvents);
        Assert.False(control.CanSelect);
        Assert.False(control.Capture);
        Assert.True(control.CausesValidation);
        Assert.Equal(new Size(100, 23), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 100, 23), control.ClientRectangle);
        Assert.Null(control.Container);
        Assert.False(control.ContainsFocus);
        Assert.Null(control.ContextMenuStrip);
        Assert.Empty(control.Controls);
        Assert.Same(control.Controls, control.Controls);
        Assert.False(control.Created);
        Assert.Same(Cursors.Default, control.Cursor);
        Assert.Same(Cursors.Default, control.DefaultCursor);
        Assert.Equal(ImeMode.Disable, control.DefaultImeMode);
        Assert.Equal(new Padding(3, 0, 3, 0), control.DefaultMargin);
        Assert.Equal(Size.Empty, control.DefaultMaximumSize);
        Assert.Equal(Size.Empty, control.DefaultMinimumSize);
        Assert.Equal(Padding.Empty, control.DefaultPadding);
        Assert.Equal(new Size(100, 23), control.DefaultSize);
        Assert.False(control.DesignMode);
        Assert.Equal(new Rectangle(0, 0, 100, 23), control.DisplayRectangle);
        Assert.Equal(DockStyle.None, control.Dock);
        Assert.True(control.DoubleBuffered);
        Assert.True(control.Enabled);
        Assert.NotNull(control.Events);
        Assert.Same(control.Events, control.Events);
        Assert.Equal(FlatStyle.Standard, control.FlatStyle);
        Assert.False(control.Focused);
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.Equal(Control.DefaultForeColor, control.ForeColor);
        Assert.False(control.HasChildren);
        Assert.Equal(23, control.Height);
        Assert.Null(control.Image);
        Assert.Equal(ContentAlignment.MiddleCenter, control.ImageAlign);
        Assert.Equal(-1, control.ImageIndex);
        Assert.Empty(control.ImageKey);
        Assert.Null(control.ImageList);
        Assert.Equal(ImeMode.Disable, control.ImeMode);
        Assert.Equal(ImeMode.Disable, control.ImeModeBase);
        Assert.False(control.IsAccessible);
        Assert.False(control.IsMirrored);
        Assert.NotNull(control.LayoutEngine);
        Assert.Same(control.LayoutEngine, control.LayoutEngine);
        Assert.Equal(0, control.Left);
        Assert.Equal(Point.Empty, control.Location);
        Assert.Equal(new Padding(3, 0, 3, 0), control.Margin);
        Assert.Equal(Size.Empty, control.MaximumSize);
        Assert.Equal(Size.Empty, control.MinimumSize);
        Assert.Equal(Padding.Empty, control.Padding);
        Assert.Null(control.Parent);
        Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
        Assert.Equal(0, control.PreferredSize.Width);
        Assert.True(control.PreferredSize.Height > 0);
        Assert.True(control.PreferredHeight > 0);
        Assert.Equal(0, control.PreferredWidth);
        Assert.False(control.RecreatingHandle);
        Assert.Null(control.Region);
        Assert.False(control.RenderTransparent);
        Assert.True(control.ResizeRedraw);
        Assert.Equal(100, control.Right);
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.True(control.ShowFocusCues);
        Assert.True(control.ShowKeyboardCues);
        Assert.Null(control.Site);
        Assert.Equal(new Size(100, 23), control.Size);
        Assert.Equal(0, control.TabIndex);
        Assert.False(control.TabStop);
        Assert.Empty(control.Text);
        Assert.Equal(ContentAlignment.TopLeft, control.TextAlign);
        Assert.Equal(0, control.Top);
        Assert.Null(control.TopLevelControl);
        Assert.True(control.UseCompatibleTextRendering);
        Assert.True(control.UseMnemonic);
        Assert.False(control.UseWaitCursor);
        Assert.True(control.Visible);
        Assert.Equal(100, control.Width);

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Label_CreateParams_GetDefault_ReturnsExpected()
    {
        using SubLabel control = new();
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("Static", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0, createParams.ExStyle);
        Assert.Equal(23, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(0x5600000D, createParams.Style);
        Assert.Equal(100, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Label_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using SubLabel control = new();
        Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
    }

    [WinFormsTheory]
    [InlineData(ControlStyles.ContainerControl, false)]
    [InlineData(ControlStyles.UserPaint, true)]
    [InlineData(ControlStyles.Opaque, false)]
    [InlineData(ControlStyles.ResizeRedraw, true)]
    [InlineData(ControlStyles.FixedWidth, false)]
    [InlineData(ControlStyles.FixedHeight, false)]
    [InlineData(ControlStyles.StandardClick, true)]
    [InlineData(ControlStyles.Selectable, false)]
    [InlineData(ControlStyles.UserMouse, false)]
    [InlineData(ControlStyles.SupportsTransparentBackColor, true)]
    [InlineData(ControlStyles.StandardDoubleClick, true)]
    [InlineData(ControlStyles.AllPaintingInWmPaint, true)]
    [InlineData(ControlStyles.CacheText, false)]
    [InlineData(ControlStyles.EnableNotifyMessage, false)]
    [InlineData(ControlStyles.DoubleBuffer, false)]
    [InlineData(ControlStyles.OptimizedDoubleBuffer, true)]
    [InlineData(ControlStyles.UseTextForAccessibility, true)]
    [InlineData((ControlStyles)0, true)]
    [InlineData((ControlStyles)int.MaxValue, false)]
    [InlineData((ControlStyles)(-1), false)]
    public void Label_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
    {
        using SubLabel control = new();
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    [WinFormsFact]
    public void Label_GetTopLevel_Invoke_ReturnsExpected()
    {
        using SubLabel control = new();
        Assert.False(control.GetTopLevel());
    }

    [WinFormsFact]
    public void Label_ImageIndex_setting_minus_one_resets_ImageKey()
    {
        int index = -1;

        using SubLabel control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(index, control.ImageIndex);
        Assert.Equal(string.Empty, control.ImageKey);

        control.ImageKey = "key";
        control.ImageIndex = index;

        Assert.Equal(index, control.ImageIndex);
        Assert.Equal(string.Empty, control.ImageKey);
    }

    [WinFormsFact]
    public void Label_ImageKey_setting_empty_resets_ImageIndex()
    {
        string key = string.Empty;

        using SubLabel control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(key, control.ImageKey);
        Assert.Equal(-1, control.ImageIndex);

        control.ImageIndex = 2;
        control.ImageKey = key;

        Assert.Equal(key, control.ImageKey);
        Assert.Equal(-1, control.ImageIndex);
    }

    [WinFormsFact]
    public void Label_SupportsUiaProviders_returns_true()
    {
        using Label label = new();
        Assert.True(label.SupportsUiaProviders);
    }

    [WinFormsFact]
    public void Label_Invokes_SetToolTip_IfExternalToolTipIsSet()
    {
        using Label label = new();
        using ToolTip toolTip = new();
        label.CreateControl();

        dynamic labelDynamic = label.TestAccessor().Dynamic;
        bool actual = labelDynamic._controlToolTip;

        Assert.False(actual);
        Assert.NotEqual(IntPtr.Zero, toolTip.Handle); // A workaround to create the toolTip native window Handle

        toolTip.SetToolTip(label, "Some test text"); // Invokes Label's SetToolTip inside
        actual = labelDynamic._controlToolTip;

        Assert.True(actual);
    }

    [WinFormsTheory]
    [InvalidEnumData<ContentAlignment>]
    public void Label_ImageAlign_SetInvalidValue_ThrowsInvalidEnumArgumentException(ContentAlignment value)
    {
        using Label control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.ImageAlign = value);
    }

    public static IEnumerable<object[]> ImageAlign_Set_TestData()
    {
        foreach (bool autoSize in new bool[] { true, false })
        {
            foreach (ContentAlignment value in Enum.GetValues(typeof(ContentAlignment)))
            {
                yield return new object[] { autoSize, value };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ImageAlign_Set_TestData))]
    public void Label_ImageAlign_Set_GetReturnsExpected(bool autoSize, ContentAlignment value)
    {
        using Label control = new()
        {
            AutoSize = autoSize
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.ImageAlign = value;
        Assert.Equal(value, control.ImageAlign);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ImageAlign = value;
        Assert.Equal(value, control.ImageAlign);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void Label_AutoSize_BehavesExpected(bool autoSize, bool expected)
    {
        using Form form = new();
        using Label label = new()
        {
            AutoSize = autoSize,
            Size = new(10, 10),
            Text = "Hello",
        };
        Size oldSize = label.Size;
        form.Controls.Add(label);
        form.Show();
        label.Text = "Say Hello";
        Size newSize = label.Size;
        Assert.Equal(expected, newSize == oldSize);
    }

    public static IEnumerable<object[]> BorderStyles_Set_TestData()
    {
        foreach (bool autoSize in new bool[] { true, false })
        {
            foreach (BorderStyle style in Enum.GetValues(typeof(BorderStyle)))
            {
                yield return new object[] { autoSize, style };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(BorderStyles_Set_TestData))]
    public void Label_BorderStyle_Set_GetReturnsExpected(bool autoSize, BorderStyle style)
    {
        using Label control = new()
        {
            AutoSize = autoSize
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.BorderStyle = style;
        Assert.Equal(style, control.BorderStyle);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(autoSize, control.AutoSize);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BorderStyle = style;
        Assert.Equal(style, control.BorderStyle);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(autoSize, control.AutoSize);
        Assert.False(control.IsHandleCreated);

        // Set different.
        if (style != BorderStyle.None)
        {
            control.BorderStyle = BorderStyle.None;
            Assert.Equal(BorderStyle.None, control.BorderStyle);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(autoSize, control.AutoSize);
            Assert.False(control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [InlineData(FlatStyle.System)]
    [InlineData(FlatStyle.Popup)]
    [InlineData(FlatStyle.Standard)]
    [InlineData(FlatStyle.Flat)]
    public void Label_FlatStyle_Set_GetReturnsExpected(FlatStyle style)
    {
        using Label label = new();
        label.FlatStyle = style;
        label.CreateControl();

        Assert.True(label.IsHandleCreated);
        Assert.Equal(style, label.FlatStyle);

        // Set same.
        label.FlatStyle = style;
        label.CreateControl();

        Assert.True(label.IsHandleCreated);
        Assert.Equal(style, label.FlatStyle);

        // Set different.
        if (style != FlatStyle.Flat)
        {
            label.FlatStyle = FlatStyle.Flat;
            label.CreateControl();
            Assert.True(label.IsHandleCreated);
            Assert.Equal(FlatStyle.Flat, label.FlatStyle);
        }
    }

    [WinFormsTheory]
    [InlineData(ContentAlignment.TopLeft)]
    [InlineData(ContentAlignment.TopCenter)]
    [InlineData(ContentAlignment.TopRight)]
    [InlineData(ContentAlignment.MiddleLeft)]
    [InlineData(ContentAlignment.MiddleCenter)]
    [InlineData(ContentAlignment.MiddleRight)]
    [InlineData(ContentAlignment.BottomLeft)]
    [InlineData(ContentAlignment.BottomCenter)]
    [InlineData(ContentAlignment.BottomRight)]
    public void Label_TextAlign_Set_GetReturnsExpected(ContentAlignment alignment)
    {
        using Label label = new();
        label.TextAlign = alignment;

        Assert.Equal(alignment, label.TextAlign);
        Assert.True(label.OwnerDraw);
    }

    [WinFormsFact]
    public void Label_TextAlign_SetSameValue_DoesNotInvalidate()
    {
        using Label label = new();
        label.TextAlign = ContentAlignment.TopLeft;

        label.CreateControl();
        Assert.True(label.IsHandleCreated);

        label.TextAlign = ContentAlignment.TopLeft;
        Assert.True(label.IsHandleCreated);
    }

    [WinFormsFact]
    public void Label_TextAlign_SetDifferentValue_Invalidate()
    {
        using Label label = new();
        label.TextAlign = ContentAlignment.TopLeft;

        label.CreateControl();
        Assert.True(label.IsHandleCreated);

        label.TextAlign = ContentAlignment.MiddleCenter;
        Assert.True(label.IsHandleCreated);
    }

    [WinFormsFact]
    public void Label_AutoSizeChangedEvent_AddRemove_Success()
    {
        using Label label = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(label, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };

        label.AutoSizeChanged += handler;
        label.AutoSizeChanged -= handler;
        Assert.Equal(0, callCount);

        label.AutoSizeChanged += handler;
        label.AutoSize = label.AutoSize;
        Assert.Equal(0, callCount);

        label.AutoSize = !label.AutoSize;
        Assert.Equal(1, callCount);

        label.AutoSize = label.AutoSize;
        Assert.Equal(1, callCount);

        label.AutoSize = !label.AutoSize;
        Assert.Equal(2, callCount);

        label.AutoSizeChanged -= handler;
        label.AutoSize = !label.AutoSize;
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void Label_AutoEllipsis_SetWithHandle_GetReturnsExpected()
    {
        using Label label = new();
        Assert.NotEqual(0, label.Handle);

        int invalidatedCallCount = 0;
        label.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        label.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        label.HandleCreated += (sender, e) => createdCallCount++;

        // Set true.
        label.AutoEllipsis = true;
        Assert.True(label.AutoEllipsis);
        Assert.True(label.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        label.AutoEllipsis = true;
        Assert.True(label.AutoEllipsis);
        Assert.True(label.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        label.AutoEllipsis = false;
        Assert.False(label.AutoEllipsis);
        Assert.True(label.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Label_BackgroundImageGetSet()
    {
        using Label label = new();
        Assert.Null(label.BackgroundImage);  // Default value

        // Set image.
        using Bitmap image1 = new(10, 10);
        label.BackgroundImage = image1;
        Assert.Same(image1, label.BackgroundImage);

        // Set same.
        label.BackgroundImage = image1;
        Assert.Same(image1, label.BackgroundImage);

        // Set different.
        using Bitmap image2 = new(50, 10);
        label.BackgroundImage = image2;
        Assert.Same(image2, label.BackgroundImage);

        // Set null.
        label.BackgroundImage = null;
        Assert.Null(label.BackgroundImage);
    }

    [WinFormsFact]
    public void Label_BackgroundImageChangedEvent_AddRemove_Success()
    {
        using Label label = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(label, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };

        label.BackgroundImageChanged += handler;
        label.BackgroundImageChanged -= handler;
        Assert.Equal(0, callCount);

        label.BackgroundImageChanged += handler;
        label.BackgroundImage = label.BackgroundImage;
        Assert.Equal(0, callCount);

        using Bitmap image = new(10, 10);
        label.BackgroundImage = image;
        Assert.Equal(1, callCount);

        label.BackgroundImage = label.BackgroundImage;
        Assert.Equal(1, callCount);

        label.BackgroundImage = null;
        Assert.Equal(2, callCount);

        label.BackgroundImageChanged -= handler;
        label.BackgroundImage = image;
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void Label_BackgroundImageLayoutGetSet()
    {
        using Label label = new();
        Assert.Equal(ImageLayout.Tile, label.BackgroundImageLayout);  // Default value

        // Set valid value.
        label.BackgroundImageLayout = ImageLayout.Center;
        Assert.Equal(ImageLayout.Center, label.BackgroundImageLayout);

        // Set same.
        label.BackgroundImageLayout = ImageLayout.Center;
        Assert.Equal(ImageLayout.Center, label.BackgroundImageLayout);

        // Set different.
        label.BackgroundImageLayout = ImageLayout.Stretch;
        Assert.Equal(ImageLayout.Stretch, label.BackgroundImageLayout);
    }

    [WinFormsFact]
    public void Label_BackgroundImageLayoutChangedEvent_AddRemove_Success()
    {
        using Label label = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(label, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };

        label.BackgroundImageLayoutChanged += handler;
        label.BackgroundImageLayoutChanged -= handler;
        Assert.Equal(0, callCount);

        label.BackgroundImageLayoutChanged += handler;
        label.BackgroundImageLayout = label.BackgroundImageLayout;
        Assert.Equal(0, callCount);

        label.BackgroundImageLayout = ImageLayout.Center;
        Assert.Equal(1, callCount);

        label.BackgroundImageLayout = label.BackgroundImageLayout;
        Assert.Equal(1, callCount);

        label.BackgroundImageLayout = ImageLayout.Stretch;
        Assert.Equal(2, callCount);

        label.BackgroundImageLayoutChanged -= handler;
        label.BackgroundImageLayout = ImageLayout.Zoom;
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void Label_ImageIndex_GetSet_ImageList()
    {
        using ImageList imageList = new();
        using Bitmap bitmap1 = new(10, 10);
        using Bitmap bitmap2 = new(10, 10);

        imageList.Images.Add(bitmap1);
        imageList.Images.Add(bitmap2);

        using Label label = new();
        label.ImageList = imageList;

        // Set valid value.
        label.ImageIndex = 0;
        Assert.Equal(0, label.ImageIndex);

        // Set same.
        label.ImageIndex = 0;
        Assert.Equal(0, label.ImageIndex);

        // Set different.
        label.ImageIndex = 1;
        Assert.Equal(1, label.ImageIndex);

        // Set invalid value.
        Assert.Throws<ArgumentOutOfRangeException>(() => label.ImageIndex = -2);
        Assert.Equal(1, label.ImageIndex);  // Value should not have changed.

        // Set to default value.
        label.ImageIndex = -1;
        Assert.Equal(-1, label.ImageIndex);
    }

    [WinFormsFact]
    public void Label_ImageKey_GetSet_ImageList()
    {
        using ImageList imageList = new();
        using Bitmap bitmap1 = new(10, 10);
        using Bitmap bitmap2 = new(10, 10);

        imageList.Images.Add("key1", bitmap1);
        imageList.Images.Add("key2", bitmap2);

        using Label label = new();
        label.ImageList = imageList;

        // Set valid value exists in the ImageList.
        label.ImageKey = "key1";
        Assert.Equal("key1", label.ImageKey);

        // Set different value exists in the ImageList.
        label.ImageKey = "key2";
        Assert.Equal("key2", label.ImageKey);

        // Set value does not exist in the ImageList.
        label.ImageKey = "nonexistent";
        Assert.Equal("nonexistent", label.ImageKey);

        // Set null.
        label.ImageKey = null;
        Assert.Equal(ImageList.Indexer.DefaultKey, label.ImageKey);  // Should reset to default value.

        // Set empty.
        label.ImageKey = string.Empty;
        Assert.Equal(ImageList.Indexer.DefaultKey, label.ImageKey);  // Should reset to default value.
    }

    [WinFormsFact]
    public void Label_ImageList_GetSet_Image()
    {
        using Label label = new();
        using Bitmap image = new(10, 10);
        label.Image = image;

        // Set valid value.
        using ImageList imageList = new();
        label.ImageList = imageList;
        Assert.Same(imageList, label.ImageList);
        Assert.Null(label.Image);  // Image should be reset.
    }

    [WinFormsFact]
    public void Label_ImageList_Disposing_UnsetsImageList()
    {
        using Label label = new();
        using ImageList imageList = new();
        label.ImageList = imageList;

        imageList.Dispose();
        Assert.Null(label.ImageList);
    }

    [WinFormsFact]
    public void Label_ImageList_RecreateHandleRefreshesControl()
    {
        using SubLabel label = new();
        using ImageList imageList = new();
        using Bitmap bitmap = new(10, 10);

        label.ImageList = imageList;
        imageList.Images.Add(bitmap);

        label.CreateControl();
        Assert.True(label.IsHandleCreated);

        label.RecreateHandle();
        Assert.True(label.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(AutomationLiveSetting.Off)]
    [InlineData(AutomationLiveSetting.Polite)]
    [InlineData(AutomationLiveSetting.Assertive)]
    public void Label_LiveSetting_Set_GetReturnsExpected(AutomationLiveSetting value)
    {
        using Label label = new();
        label.LiveSetting = value;
        Assert.Equal(value, label.LiveSetting);
    }

    [WinFormsFact]
    public void Label_LiveSetting_SetInvalidValue_ThrowsInvalidEnumArgumentException()
    {
        using Label label = new();
        Assert.Throws<InvalidEnumArgumentException>(() => label.LiveSetting = (AutomationLiveSetting)999);
    }

    [WinFormsFact]
    public void Label_KeyUp_AddRemove_Success()
    {
        using SubLabel label = new();
        int callCount = 0;
        KeyEventHandler handler = (sender, e) =>
        {
            Assert.Same(label, sender);
            callCount++;
        };

        KeyEventArgs keyEventArgs = new KeyEventArgs(Keys.A);

        label.KeyUp += handler;
        label.OnKeyUp(keyEventArgs);
        Assert.Equal(1, callCount);

        label.KeyUp -= handler;
        label.OnKeyUp(keyEventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void Label_KeyDown_AddRemove_Success()
    {
        using SubLabel label = new();
        int callCount = 0;
        KeyEventHandler handler = (sender, e) =>
        {
            Assert.Same(label, sender);
            callCount++;
        };

        KeyEventArgs keyEventArgs = new KeyEventArgs(Keys.A);

        label.KeyDown += handler;
        label.OnKeyDown(keyEventArgs);
        Assert.Equal(1, callCount);

        label.KeyDown -= handler;
        label.OnKeyDown(keyEventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void Label_KeyPress_AddRemove_Success()
    {
        using SubLabel label = new();
        int callCount = 0;
        KeyPressEventHandler handler = (sender, e) =>
        {
            Assert.Same(label, sender);
            callCount++;
        };

        KeyPressEventArgs keyPressEventArgs = new KeyPressEventArgs('A');

        label.KeyPress += handler;
        label.OnKeyPress(keyPressEventArgs);
        Assert.Equal(1, callCount);

        label.KeyPress -= handler;
        label.OnKeyPress(keyPressEventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void Label_TabStopChanged_AddRemove_Success()
    {
        using Label label = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(label, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };

        label.TabStopChanged += handler;
        label.TabStop = !label.TabStop;
        Assert.Equal(1, callCount);

        label.TabStopChanged -= handler;
        label.TabStop = !label.TabStop;
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void Label_TextAlignChanged_AddRemove_Success()
    {
        using Label label = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(label, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };

        label.TextAlignChanged += handler;
        label.TextAlign = ContentAlignment.BottomCenter;
        Assert.Equal(1, callCount);

        label.TextAlignChanged -= handler;
        label.TextAlign = ContentAlignment.BottomLeft;
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void Label_UseCompatibleTextRendering_GetSet_ReturnsExpected()
    {
        using Label label = new();
        bool defaultValue = label.UseCompatibleTextRendering;

        // Set true.
        label.UseCompatibleTextRendering = true;
        Assert.True(label.UseCompatibleTextRendering);

        // Set false.
        label.UseCompatibleTextRendering = false;
        Assert.False(label.UseCompatibleTextRendering);

        // Set default.
        label.UseCompatibleTextRendering = defaultValue;
        Assert.Equal(defaultValue, label.UseCompatibleTextRendering);
    }

    [WinFormsFact]
    public void Label_UseCompatibleTextRendering_SetWithAutoSize_UpdatesPreferredSize()
    {
        using Label label = new()
        {
            AutoSize = true,
            Text = "Some text"
        };
        Size defaultSize = label.PreferredSize;

        label.UseCompatibleTextRendering = !label.UseCompatibleTextRendering;
        Assert.NotEqual(defaultSize, label.PreferredSize);
    }

    public class SubLabel : Label
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

#pragma warning disable 0618
        public new bool RenderTransparent
        {
            get => base.RenderTransparent;
            set => base.RenderTransparent = value;
        }
#pragma warning restore 0618

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

        public new void RecreateHandle() => base.RecreateHandle();

        public new void OnKeyUp(KeyEventArgs e) => base.OnKeyUp(e);

        public new void OnKeyDown(KeyEventArgs e) => base.OnKeyDown(e);

        public new void OnKeyPress(KeyPressEventArgs e) => base.OnKeyPress(e);
    }
}
