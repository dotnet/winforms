// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Automation;
using Microsoft.DotNet.RemoteExecutor;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ListViewItem;

using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests;

public class ListViewTests
{
    [WinFormsFact]
    public void ListView_Ctor_Default()
    {
        using SubListView control = new();
        Assert.Null(control.AccessibleDefaultActionDescription);
        Assert.Null(control.AccessibleDescription);
        Assert.Null(control.AccessibleName);
        Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
        Assert.Equal(ItemActivation.Standard, control.Activation);
        Assert.Equal(ListViewAlignment.Top, control.Alignment);
        Assert.False(control.AllowColumnReorder);
        Assert.False(control.AllowDrop);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        Assert.True(control.AutoArrange);
        Assert.False(control.AutoSize);
        Assert.Equal(SystemColors.Window, control.BackColor);
        Assert.Null(control.BackgroundImage);
        Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
        Assert.False(control.BackgroundImageTiled);
        Assert.Null(control.BindingContext);
        Assert.Equal(BorderStyle.Fixed3D, control.BorderStyle);
        Assert.Equal(97, control.Bottom);
        Assert.Equal(new Rectangle(0, 0, 121, 97), control.Bounds);
        Assert.True(control.CanEnableIme);
        Assert.False(control.CanFocus);
        Assert.True(control.CanRaiseEvents);
        Assert.True(control.CanSelect);
        Assert.False(control.Capture);
        Assert.True(control.CausesValidation);
        Assert.False(control.CheckBoxes);
        Assert.Empty(control.CheckedIndices);
        Assert.Same(control.CheckedIndices, control.CheckedIndices);
        Assert.Empty(control.CheckedItems);
        Assert.Same(control.CheckedItems, control.CheckedItems);
        Assert.Equal(new Size(117, 93), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 117, 93), control.ClientRectangle);
        Assert.Empty(control.Columns);
        Assert.Same(control.Columns, control.Columns);
        Assert.Null(control.Container);
        Assert.False(control.ContainsFocus);
        Assert.Null(control.ContextMenuStrip);
        Assert.Empty(control.Controls);
        Assert.Same(control.Controls, control.Controls);
        Assert.False(control.Created);
        Assert.Equal(Cursors.Default, control.Cursor);
        Assert.Same(Cursors.Default, control.DefaultCursor);
        Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
        Assert.Equal(new Padding(3), control.DefaultMargin);
        Assert.Equal(Size.Empty, control.DefaultMaximumSize);
        Assert.Equal(Size.Empty, control.DefaultMinimumSize);
        Assert.Equal(Padding.Empty, control.DefaultPadding);
        Assert.Equal(new Size(121, 97), control.DefaultSize);
        Assert.False(control.DesignMode);
        Assert.Equal(new Rectangle(0, 0, 117, 93), control.DisplayRectangle);
        Assert.Equal(DockStyle.None, control.Dock);
        Assert.False(control.DoubleBuffered);
        Assert.True(control.Enabled);
        Assert.NotNull(control.Events);
        Assert.Same(control.Events, control.Events);
        Assert.False(control.Focused);
        Assert.Null(control.FocusedItem);
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.Equal(SystemColors.WindowText, control.ForeColor);
        Assert.False(control.FullRowSelect);
        Assert.False(control.GridLines);
        Assert.Empty(control.Groups);
        Assert.Same(control.Groups, control.Groups);
        Assert.Null(control.GroupImageList);
        Assert.False(control.HasChildren);
        Assert.Equal(ColumnHeaderStyle.Clickable, control.HeaderStyle);
        Assert.Equal(97, control.Height);
        Assert.False(control.HideSelection);
        Assert.False(control.HotTracking);
        Assert.False(control.HoverSelection);
        Assert.Equal(ImeMode.NoControl, control.ImeMode);
        Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
        Assert.NotNull(control.InsertionMark);
        Assert.Same(control.InsertionMark, control.InsertionMark);
        Assert.False(control.IsAccessible);
        Assert.False(control.IsMirrored);
        Assert.Empty(control.Items);
        Assert.Same(control.Items, control.Items);
        Assert.False(control.LabelEdit);
        Assert.True(control.LabelWrap);
        Assert.Null(control.LargeImageList);
        Assert.NotNull(control.LayoutEngine);
        Assert.Same(control.LayoutEngine, control.LayoutEngine);
        Assert.Equal(0, control.Left);
        Assert.Null(control.ListViewItemSorter);
        Assert.Equal(Point.Empty, control.Location);
        Assert.Equal(new Padding(3), control.Margin);
        Assert.Equal(Size.Empty, control.MaximumSize);
        Assert.Equal(Size.Empty, control.MinimumSize);
        Assert.True(control.MultiSelect);
        Assert.False(control.OwnerDraw);
        Assert.Equal(Padding.Empty, control.Padding);
        Assert.Null(control.Parent);
        Assert.Equal(new Size(121, 97), control.PreferredSize);
        Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
        Assert.False(control.RecreatingHandle);
        Assert.Null(control.Region);
        Assert.False(control.ResizeRedraw);
        Assert.Equal(121, control.Right);
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.False(control.RightToLeftLayout);
        Assert.True(control.Scrollable);
        Assert.Empty(control.SelectedIndices);
        Assert.Same(control.SelectedIndices, control.SelectedIndices);
        Assert.Empty(control.SelectedItems);
        Assert.Same(control.SelectedItems, control.SelectedItems);
        Assert.True(control.ShowFocusCues);
        Assert.True(control.ShowGroups);
        Assert.False(control.ShowItemToolTips);
        Assert.True(control.ShowKeyboardCues);
        Assert.Null(control.SmallImageList);
        Assert.Null(control.Site);
        Assert.Equal(new Size(121, 97), control.Size);
        Assert.Equal(SortOrder.None, control.Sorting);
        Assert.Null(control.StateImageList);
        Assert.Equal(0, control.TabIndex);
        Assert.True(control.TabStop);
        Assert.Empty(control.Text);
        Assert.Equal(Size.Empty, control.TileSize);
        Assert.Equal(0, control.Top);
        Assert.Throws<InvalidOperationException>(() => control.TopItem);
        Assert.Null(control.TopLevelControl);
        Assert.True(control.UseCompatibleStateImageBehavior);
        Assert.False(control.UseWaitCursor);
        Assert.Equal(View.LargeIcon, control.View);
        Assert.Equal(0, control.VirtualListSize);
        Assert.False(control.VirtualMode);
        Assert.True(control.Visible);
        Assert.Equal(121, control.Width);

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListView_CreateParams_GetDefault_ReturnsExpected()
    {
        using SubListView control = new();
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("SysListView32", createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0x200, createParams.ExStyle);
        Assert.Equal(97, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(0x56010148, createParams.Style);
        Assert.Equal(121, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [EnumData<ItemActivation>]
    public void ListView_Activation_Set_GetReturnsExpected(ItemActivation value)
    {
        using ListView listView = new()
        {
            Activation = value
        };
        Assert.Equal(value, listView.Activation);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.Activation = value;
        Assert.Equal(value, listView.Activation);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(ItemActivation.Standard, 0)]
    [InlineData(ItemActivation.OneClick, 1)]
    [InlineData(ItemActivation.TwoClick, 1)]
    public void ListView_Activation_SetWithHandle_GetReturnsExpected(ItemActivation value, int expectedInvalidatedCallCount)
    {
        using ListView listView = new();
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.Activation = value;
        Assert.Equal(value, listView.Activation);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        listView.Activation = value;
        Assert.Equal(value, listView.Activation);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ListView_Activation_SetHotTrackingOneClick_Nop()
    {
        using ListView listView = new()
        {
            HotTracking = true,
            Activation = ItemActivation.OneClick
        };
        Assert.Equal(ItemActivation.OneClick, listView.Activation);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.Activation = ItemActivation.OneClick;
        Assert.Equal(ItemActivation.OneClick, listView.Activation);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InvalidEnumData<ItemActivation>]
    public void ListView_Activation_SetInvalidValue_ThrowsInvalidEnumArgumentException(ItemActivation value)
    {
        using ListView listView = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => listView.Activation = value);
    }

    [WinFormsTheory]
    [InvalidEnumData<ItemActivation>]
    public void ListView_Activation_SetHotTrackingInvalidValue_ThrowsInvalidEnumArgumentException(ItemActivation value)
    {
        using ListView listView = new()
        {
            HotTracking = true
        };
        Assert.Throws<InvalidEnumArgumentException>("value", () => listView.Activation = value);
    }

    [WinFormsTheory]
    [InlineData(ItemActivation.Standard)]
    [InlineData(ItemActivation.TwoClick)]
    public void ListView_Activation_SetHotTrackingNotOneClick_ThrowsArgumentException(ItemActivation value)
    {
        using ListView listView = new()
        {
            HotTracking = true
        };
        Assert.Throws<ArgumentException>("value", () => listView.Activation = value);
        Assert.Equal(ItemActivation.OneClick, listView.Activation);
    }

    [WinFormsTheory]
    [EnumData<ListViewAlignment>]
    public void ListView_Alignment_Set_GetReturnsExpected(ListViewAlignment value)
    {
        using ListView listView = new()
        {
            Alignment = value
        };
        Assert.Equal(value, listView.Alignment);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.Alignment = value;
        Assert.Equal(value, listView.Alignment);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(ListViewAlignment.Default, 2, 1)]
    [InlineData(ListViewAlignment.Top, 0, 0)]
    [InlineData(ListViewAlignment.Left, 2, 1)]
    [InlineData(ListViewAlignment.SnapToGrid, 2, 1)]
    public void ListView_Alignment_SetWithHandle_GetReturnsExpected(ListViewAlignment value, int expectedInvalidatedCallCount, int expectedCreatedCallCount)
    {
        using ListView listView = new();
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.Alignment = value;
        Assert.Equal(value, listView.Alignment);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        listView.Alignment = value;
        Assert.Equal(value, listView.Alignment);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<ListViewAlignment>]
    public void ListView_Alignment_SetInvalidValue_ThrowsInvalidEnumArgumentException(ListViewAlignment value)
    {
        using ListView listView = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => listView.Alignment = value);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_AllowColumnReorder_Set_GetReturnsExpected(bool value)
    {
        using ListView listView = new()
        {
            AllowColumnReorder = value
        };
        Assert.Equal(value, listView.AllowColumnReorder);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.AllowColumnReorder = value;
        Assert.Equal(value, listView.AllowColumnReorder);
        Assert.False(listView.IsHandleCreated);

        // Set different.
        listView.AllowColumnReorder = !value;
        Assert.Equal(!value, listView.AllowColumnReorder);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void ListView_AllowColumnReorder_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
    {
        using ListView listView = new();
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.AllowColumnReorder = value;
        Assert.Equal(value, listView.AllowColumnReorder);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        listView.AllowColumnReorder = value;
        Assert.Equal(value, listView.AllowColumnReorder);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        listView.AllowColumnReorder = !value;
        Assert.Equal(!value, listView.AllowColumnReorder);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_AutoArrange_Set_GetReturnsExpected(bool value)
    {
        using ListView listView = new()
        {
            AutoArrange = value
        };
        Assert.Equal(value, listView.AutoArrange);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.AutoArrange = value;
        Assert.Equal(value, listView.AutoArrange);
        Assert.False(listView.IsHandleCreated);

        // Set different.
        listView.AutoArrange = !value;
        Assert.Equal(!value, listView.AutoArrange);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 0)]
    [InlineData(false, 1)]
    public void ListView_AutoArrange_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
    {
        using ListView listView = new();
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.AutoArrange = value;
        Assert.Equal(value, listView.AutoArrange);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        listView.AutoArrange = value;
        Assert.Equal(value, listView.AutoArrange);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        listView.AutoArrange = !value;
        Assert.Equal(!value, listView.AutoArrange);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
        Assert.Equal(expectedInvalidatedCallCount + 1, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> BackColor_Set_TestData()
    {
        yield return new object[] { Color.Empty, SystemColors.Window };
        yield return new object[] { Color.Red, Color.Red };
    }

    [WinFormsTheory]
    [MemberData(nameof(BackColor_Set_TestData))]
    public void ListView_BackColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using ListView control = new()
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
        yield return new object[] { Color.Empty, SystemColors.Window, 0 };
        yield return new object[] { Color.Red, Color.Red, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(BackColor_SetWithHandle_TestData))]
    public void ListView_BackColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
    {
        using ListView control = new();
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
    public void ListView_BackColor_GetBkColor_Success()
    {
        using ListView control = new();

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.BackColor = Color.FromArgb(0xFF, 0x12, 0x34, 0x56);
        Assert.Equal(0x563412, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETBKCOLOR));
    }

    [WinFormsFact]
    public void ListView_BackColor_SetWithHandler_CallsBackColorChanged()
    {
        using ListView control = new();
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
    [EnumData<ImageLayout>]
    public void ListView_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
    {
        using SubListView control = new()
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
    public void ListView_BackgroundImageLayout_SetWithHandler_CallsBackgroundImageLayoutChanged()
    {
        using ListView control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
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
    [InvalidEnumData<ImageLayout>]
    public void ListView_BackgroundImageLayout_SetInvalid_ThrowsInvalidEnumArgumentException(ImageLayout value)
    {
        using ListView control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.BackgroundImageLayout = value);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_BackgroundImageTiled_Set_GetReturnsExpected(bool value)
    {
        using ListView listView = new()
        {
            BackgroundImageTiled = value
        };
        Assert.Equal(value, listView.BackgroundImageTiled);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.BackgroundImageTiled = value;
        Assert.Equal(value, listView.BackgroundImageTiled);
        Assert.False(listView.IsHandleCreated);

        // Set different.
        listView.BackgroundImageTiled = !value;
        Assert.Equal(!value, listView.BackgroundImageTiled);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_BackgroundImageTiled_SetWithBackgroundImage_GetReturnsExpected(bool value)
    {
        using ListView listView = new()
        {
            BackgroundImage = new Bitmap(10, 10),
            BackgroundImageTiled = value
        };
        Assert.Equal(value, listView.BackgroundImageTiled);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.BackgroundImageTiled = value;
        Assert.Equal(value, listView.BackgroundImageTiled);
        Assert.False(listView.IsHandleCreated);

        // Set different.
        listView.BackgroundImageTiled = !value;
        Assert.Equal(!value, listView.BackgroundImageTiled);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_BackgroundImageTiled_SetWithHandle_GetReturnsExpected(bool value)
    {
        using ListView listView = new();
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.BackgroundImageTiled = value;
        Assert.Equal(value, listView.BackgroundImageTiled);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        listView.BackgroundImageTiled = value;
        Assert.Equal(value, listView.BackgroundImageTiled);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        listView.BackgroundImageTiled = !value;
        Assert.Equal(!value, listView.BackgroundImageTiled);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_BackgroundImageTiled_SetWithBackgroundImageWithHandle_GetReturnsExpected(bool value)
    {
        using ListView listView = new()
        {
            BackgroundImage = new Bitmap(10, 10)
        };
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.BackgroundImageTiled = value;
        Assert.Equal(value, listView.BackgroundImageTiled);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        listView.BackgroundImageTiled = value;
        Assert.Equal(value, listView.BackgroundImageTiled);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        listView.BackgroundImageTiled = !value;
        Assert.Equal(!value, listView.BackgroundImageTiled);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [EnumData<BorderStyle>]
    public void ListView_BorderStyle_Set_GetReturnsExpected(BorderStyle value)
    {
        using ListView listView = new()
        {
            BorderStyle = value
        };
        Assert.Equal(value, listView.BorderStyle);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.BorderStyle = value;
        Assert.Equal(value, listView.BorderStyle);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(BorderStyle.Fixed3D, 0)]
    [InlineData(BorderStyle.FixedSingle, 1)]
    [InlineData(BorderStyle.None, 1)]
    public void ListView_BorderStyle_SetWithHandle_GetReturnsExpected(BorderStyle value, int expectedInvalidatedCallCount)
    {
        using ListView listView = new();
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.BorderStyle = value;
        Assert.Equal(value, listView.BorderStyle);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        listView.BorderStyle = value;
        Assert.Equal(value, listView.BorderStyle);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<BorderStyle>]
    public void ListView_BorderStyle_SetInvalid_ThrowsInvalidEnumArgumentException(BorderStyle value)
    {
        using ListView listView = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => listView.BorderStyle = value);
    }

    public static IEnumerable<object[]> CheckBoxes_Set_TestData()
    {
        foreach (bool useCompatibleStateImageBehavior in new bool[] { true, false })
        {
            foreach (ListViewAlignment alignment in Enum.GetValues(typeof(ListViewAlignment)))
            {
                foreach (Func<ImageList> imageListFactory in new Func<ImageList>[] { () => new ImageList(), () => null })
                {
                    foreach (bool value in new bool[] { true, false })
                    {
                        yield return new object[] { useCompatibleStateImageBehavior, View.Details, alignment, imageListFactory(), value };
                        yield return new object[] { useCompatibleStateImageBehavior, View.LargeIcon, alignment, imageListFactory(), value };
                        yield return new object[] { useCompatibleStateImageBehavior, View.List, alignment, imageListFactory(), value };
                        yield return new object[] { useCompatibleStateImageBehavior, View.SmallIcon, alignment, imageListFactory(), value };
                    }
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(CheckBoxes_Set_TestData))]
    public void ListView_CheckBoxes_Set_GetReturnsExpected(bool useCompatibleStateImageBehavior, View view, ListViewAlignment alignment, ImageList stateImageList, bool value)
    {
        using ListView listView = new()
        {
            UseCompatibleStateImageBehavior = useCompatibleStateImageBehavior,
            View = view,
            Alignment = alignment,
            StateImageList = stateImageList,
            CheckBoxes = value
        };
        Assert.Equal(value, listView.CheckBoxes);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.CheckBoxes = value;
        Assert.Equal(value, listView.CheckBoxes);
        Assert.False(listView.IsHandleCreated);

        // Set different.
        listView.CheckBoxes = !value;
        Assert.Equal(!value, listView.CheckBoxes);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(CheckBoxes_Set_TestData))]
    public void ListView_CheckBoxes_SetAutoArrange_GetReturnsExpected(bool useCompatibleStateImageBehavior, View view, ListViewAlignment alignment, ImageList stateImageList, bool value)
    {
        using ListView listView = new()
        {
            AutoArrange = true,
            UseCompatibleStateImageBehavior = useCompatibleStateImageBehavior,
            View = view,
            Alignment = alignment,
            StateImageList = stateImageList,
            CheckBoxes = value
        };
        Assert.Equal(value, listView.CheckBoxes);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.CheckBoxes = value;
        Assert.Equal(value, listView.CheckBoxes);
        Assert.False(listView.IsHandleCreated);

        // Set different.
        listView.CheckBoxes = !value;
        Assert.Equal(!value, listView.CheckBoxes);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_CheckBoxes_SetWithCheckedItems_Success(bool value)
    {
        ListViewItem item1 = new()
        {
            Checked = true
        };
        ListViewItem item2 = new();
        using ListView listView = new();
        listView.Items.Add(item1);
        listView.Items.Add(item2);
        Assert.Equal(new ListViewItem[] { item1, item2 }, listView.Items.Cast<ListViewItem>());

        listView.CheckBoxes = value;
        Assert.Equal(value, listView.CheckBoxes);
        Assert.True(item1.Checked);
        Assert.False(item2.Checked);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.CheckBoxes = value;
        Assert.Equal(value, listView.CheckBoxes);
        Assert.True(item1.Checked);
        Assert.False(item2.Checked);
        Assert.False(listView.IsHandleCreated);

        // Set different.
        listView.CheckBoxes = !value;
        Assert.Equal(!value, listView.CheckBoxes);
        Assert.True(item1.Checked);
        Assert.False(item2.Checked);
        Assert.False(listView.IsHandleCreated);
    }

    public static IEnumerable<object[]> CheckBoxes_SetWithHandle_TestData()
    {
        foreach (ListViewAlignment alignment in Enum.GetValues(typeof(ListViewAlignment)))
        {
            foreach (Func<ImageList> imageListFactory in new Func<ImageList>[] { () => new ImageList(), () => null })
            {
                yield return new object[] { true, View.Details, alignment, imageListFactory(), true, 1, 0, 2, 0 };
                yield return new object[] { true, View.LargeIcon, alignment, imageListFactory(), true, 1, 0, 2, 0 };
                yield return new object[] { true, View.List, alignment, imageListFactory(), true, 1, 0, 2, 0 };
                yield return new object[] { true, View.SmallIcon, alignment, imageListFactory(), true, 1, 0, 2, 0 };
                yield return new object[] { true, View.Details, alignment, imageListFactory(), false, 0, 0, 1, 0 };
                yield return new object[] { true, View.LargeIcon, alignment, imageListFactory(), false, 0, 0, 1, 0 };
                yield return new object[] { true, View.List, alignment, imageListFactory(), false, 0, 0, 1, 0 };
                yield return new object[] { true, View.SmallIcon, alignment, imageListFactory(), false, 0, 0, 1, 0 };
            }

            if (alignment != ListViewAlignment.Left)
            {
                yield return new object[] { false, View.Details, alignment, null, true, 1, 0, 2, 0 };
                yield return new object[] { false, View.LargeIcon, alignment, null, true, 2, 1, 3, 1 };
                yield return new object[] { false, View.List, alignment, null, true, 1, 1, 2, 1 };
                yield return new object[] { false, View.SmallIcon, alignment, null, true, 2, 1, 3, 1 };
                yield return new object[] { false, View.Details, alignment, null, false, 0, 0, 1, 0 };
                yield return new object[] { false, View.LargeIcon, alignment, null, false, 0, 0, 2, 1 };
                yield return new object[] { false, View.List, alignment, null, false, 0, 0, 1, 1 };
                yield return new object[] { false, View.SmallIcon, alignment, null, false, 0, 0, 2, 1 };

                yield return new object[] { false, View.Details, alignment, new ImageList(), true, 1, 0, 2, 1 };
                yield return new object[] { false, View.LargeIcon, alignment, new ImageList(), true, 2, 1, 4, 2 };
                yield return new object[] { false, View.List, alignment, new ImageList(), true, 1, 1, 2, 2 };
                yield return new object[] { false, View.SmallIcon, alignment, new ImageList(), true, 2, 1, 4, 2 };
                yield return new object[] { false, View.Details, alignment, new ImageList(), false, 0, 0, 1, 0 };
                yield return new object[] { false, View.LargeIcon, alignment, new ImageList(), false, 0, 0, 2, 1 };
                yield return new object[] { false, View.List, alignment, new ImageList(), false, 0, 0, 1, 1 };
                yield return new object[] { false, View.SmallIcon, alignment, new ImageList(), false, 0, 0, 2, 1 };
            }
        }

        foreach (Func<ImageList> imageListFactory in new Func<ImageList>[] { () => new ImageList(), () => null })
        {
            yield return new object[] { false, View.Details, ListViewAlignment.Left, imageListFactory(), true, 1, 0, 2, 1 };
            yield return new object[] { false, View.LargeIcon, ListViewAlignment.Left, imageListFactory(), true, 2, 1, 4, 2 };
            yield return new object[] { false, View.List, ListViewAlignment.Left, imageListFactory(), true, 1, 1, 2, 2 };
            yield return new object[] { false, View.SmallIcon, ListViewAlignment.Left, imageListFactory(), true, 2, 1, 4, 2 };
            yield return new object[] { false, View.Details, ListViewAlignment.Left, imageListFactory(), false, 0, 0, 1, 0 };
            yield return new object[] { false, View.LargeIcon, ListViewAlignment.Left, imageListFactory(), false, 0, 0, 2, 1 };
            yield return new object[] { false, View.List, ListViewAlignment.Left, imageListFactory(), false, 0, 0, 1, 1 };
            yield return new object[] { false, View.SmallIcon, ListViewAlignment.Left, imageListFactory(), false, 0, 0, 2, 1 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(CheckBoxes_SetWithHandle_TestData))]
    public void ListView_CheckBoxes_SetWithHandle_GetReturnsExpected(bool useCompatibleStateImageBehavior, View view, ListViewAlignment alignment, ImageList stateImageList, bool value, int expectedInvalidatedCallCount1, int expectedCreatedCallCount1, int expectedInvalidatedCallCount2, int expectedCreatedCallCount2)
    {
        using ListView listView = new()
        {
            UseCompatibleStateImageBehavior = useCompatibleStateImageBehavior,
            View = view,
            Alignment = alignment,
            StateImageList = stateImageList
        };
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.CheckBoxes = value;
        Assert.Equal(value, listView.CheckBoxes);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount1, createdCallCount);

        // Set same.
        listView.CheckBoxes = value;
        Assert.Equal(value, listView.CheckBoxes);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount1, createdCallCount);

        // Set different.
        listView.CheckBoxes = !value;
        Assert.Equal(!value, listView.CheckBoxes);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount2, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(CheckBoxes_SetWithHandle_TestData))]
    public void ListView_CheckBoxes_SetAutoArrangeWithHandle_GetReturnsExpected(bool useCompatibleStateImageBehavior, View view, ListViewAlignment alignment, ImageList stateImageList, bool value, int expectedInvalidatedCallCount1, int expectedCreatedCallCount1, int expectedInvalidatedCallCount2, int expectedCreatedCallCount2)
    {
        using ListView listView = new()
        {
            AutoArrange = true,
            UseCompatibleStateImageBehavior = useCompatibleStateImageBehavior,
            View = view,
            Alignment = alignment,
            StateImageList = stateImageList
        };
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.CheckBoxes = value;
        Assert.Equal(value, listView.CheckBoxes);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount1, createdCallCount);

        // Set same.
        listView.CheckBoxes = value;
        Assert.Equal(value, listView.CheckBoxes);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount1, createdCallCount);

        // Set different.
        listView.CheckBoxes = !value;
        Assert.Equal(!value, listView.CheckBoxes);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount2, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_CheckBoxes_SetWithCheckedItemsWithHandle_Success(bool value)
    {
        ListViewItem item1 = new()
        {
            Checked = true
        };
        ListViewItem item2 = new();
        using ListView listView = new();
        listView.Items.Add(item1);
        listView.Items.Add(item2);
        Assert.Equal(new ListViewItem[] { item1, item2 }, listView.Items.Cast<ListViewItem>());
        Assert.NotEqual(IntPtr.Zero, listView.Handle);

        listView.CheckBoxes = value;
        Assert.Equal(value, listView.CheckBoxes);
        Assert.True(item1.Checked);
        Assert.False(item2.Checked);
        Assert.True(listView.IsHandleCreated);

        // Set same.
        listView.CheckBoxes = value;
        Assert.Equal(value, listView.CheckBoxes);
        Assert.True(item1.Checked);
        Assert.False(item2.Checked);
        Assert.True(listView.IsHandleCreated);

        // Set different.
        listView.CheckBoxes = !value;
        Assert.Equal(!value, listView.CheckBoxes);
        Assert.True(item1.Checked);
        Assert.False(item2.Checked);
        Assert.True(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_CheckBoxes_SetTile_ThrowsNotSupportedException(bool useCompatibleStateImageBehavior)
    {
        using ListView listView = new()
        {
            UseCompatibleStateImageBehavior = useCompatibleStateImageBehavior,
            View = View.Tile
        };
        Assert.Throws<NotSupportedException>(() => listView.CheckBoxes = true);
        Assert.False(listView.CheckBoxes);

        listView.CheckBoxes = false;
        Assert.False(listView.CheckBoxes);
    }

    [WinFormsFact]
    public void ListView_DisposeWithReferencedImageListDoesNotLeak()
    {
        // must be separate function because GC of local variables is not precise
        static WeakReference CreateAndDisposeListViewWithImageListReference(ImageList imageList)
        {
            // short lived test code, whatever you need to trigger the leak
            using ListView listView = new();
            listView.LargeImageList = imageList;

            // return a weak reference to whatever you want to track GC of
            // creating a long weak reference to make sure finalizer does not resurrect the ListView
            return new WeakReference(listView, true);
        }

        // simulate a long-living ImageList by keeping it alive for the test
        using ImageList imageList = new();

        // simulate a short-living ListView by disposing it (returning a WeakReference to track finalization)
        var listViewRef = CreateAndDisposeListViewWithImageListReference(imageList);

        GC.Collect(); // mark for finalization (also would clear normal weak references)
        GC.WaitForPendingFinalizers(); // wait until finalizer is executed
        GC.Collect(); // wait for long weak reference to be cleared

        // at this point the WeakReference is cleared if -and only if- the finalizer was called and did not resurrect the object
        // (if the test ever fails you can set a breakpoint here, debug the test, and make heap snapshot in VS;
        // then search for the ListView in the heap snapshot UI and look who is referencing it, usually you
        // can derive from types referencing the ListView who is to blame)
        Assert.False(listViewRef.IsAlive);
    }

#if DEBUG
    [WinFormsFact]
    public void ListView_Dispose_shared_ImageList_doesnt_assert()
    {
        using ListView listView = new();
        ImageList imageList = new();
        listView.LargeImageList = imageList;
        listView.SmallImageList = imageList;

        // Initiate DetachImageList sequence
        imageList.Dispose();

        // Unless we track whether an imagelist was disposed, we would hit Debug.Fail assertion
        // and never reach this line
        Assert.True(true);
    }
#endif

    [WinFormsTheory]
    [BoolData]
    public void ListView_DoubleBuffered_Get_ReturnsExpected(bool value)
    {
        using SubListView control = new();
        control.SetStyle(ControlStyles.OptimizedDoubleBuffer, value);
        Assert.Equal(value, control.DoubleBuffered);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_DoubleBuffered_Set_GetReturnsExpected(bool value)
    {
        using SubListView control = new()
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
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void ListView_DoubleBuffered_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
    {
        using SubListView control = new();
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
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.DoubleBuffered = value;
        Assert.Equal(value, control.DoubleBuffered);
        Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.DoubleBuffered = !value;
        Assert.Equal(!value, control.DoubleBuffered);
        Assert.Equal(!value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> FocusedItem_Set_TestData()
    {
        yield return new object[] { new ListViewItem(), false };
        yield return new object[] { null, null };
    }

    [WinFormsTheory]
    [MemberData(nameof(FocusedItem_Set_TestData))]
    public void ListView_FocusedItem_Set_GetReturnsExpected(ListViewItem value, bool? expectedFocused)
    {
        using SubListView control = new()
        {
            FocusedItem = value
        };
        Assert.Null(control.FocusedItem);
        Assert.False(control.IsHandleCreated);
        Assert.Equal(expectedFocused, value?.Focused);

        // Set same.
        control.FocusedItem = value;
        Assert.Null(control.FocusedItem);
        Assert.False(control.IsHandleCreated);
        Assert.Equal(expectedFocused, value?.Focused);
    }

    [WinFormsFact]
    public void ListView_FocusedItem_SetChild_GetReturnsExpected()
    {
        ListViewItem value = new();
        using SubListView control = new();
        control.Items.Add(value);

        control.FocusedItem = value;
        Assert.Null(control.FocusedItem);
        Assert.False(control.Focused);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.FocusedItem = value;
        Assert.Null(control.FocusedItem);
        Assert.False(control.Focused);
        Assert.False(control.IsHandleCreated);

        // Set null.
        control.FocusedItem = null;
        Assert.Null(control.FocusedItem);
        Assert.False(control.Focused);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(FocusedItem_Set_TestData))]
    public void ListView_FocusedItem_SetWithHandle_GetReturnsExpected(ListViewItem value, bool? expectedFocused)
    {
        using SubListView control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.FocusedItem = value;
        Assert.Null(control.FocusedItem);
        Assert.Equal(expectedFocused, value?.Focused);

        // Set same.
        control.FocusedItem = value;
        Assert.Null(control.FocusedItem);
        Assert.Equal(expectedFocused, value?.Focused);
    }

    [WinFormsFact]
    public void ListView_FocusedItem_SetChildWithHandle_GetReturnsExpected()
    {
        ListViewItem value = new();
        using SubListView control = new();
        control.Items.Add(value);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.FocusedItem = value;
        Assert.Same(value, control.FocusedItem);
        Assert.True(value.Focused);

        // Set same.
        control.FocusedItem = value;
        Assert.Same(value, control.FocusedItem);
        Assert.True(value.Focused);

        // Set null.
        control.FocusedItem = null;
        Assert.Same(value, control.FocusedItem);
        Assert.True(value.Focused);
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
    public void ListView_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using ListView control = new()
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
    public void ListView_ForeColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
    {
        using ListView control = new();
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
    public void ListView_ForeColor_GetTxtColor_Success()
    {
        using ListView control = new();

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.ForeColor = Color.FromArgb(0x12, 0x34, 0x56, 0x78);
        Assert.Equal(0x785634, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETTEXTCOLOR));
    }

    [WinFormsFact]
    public void ListView_ForeColor_SetWithHandler_CallsForeColorChanged()
    {
        using ListView control = new();
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
    [BoolData]
    public void ListView_FullRowSelect_Set_GetReturnsExpected(bool value)
    {
        using ListView listView = new()
        {
            FullRowSelect = value
        };
        Assert.Equal(value, listView.FullRowSelect);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.FullRowSelect = value;
        Assert.Equal(value, listView.FullRowSelect);
        Assert.False(listView.IsHandleCreated);

        // Set different.
        listView.FullRowSelect = !value;
        Assert.Equal(!value, listView.FullRowSelect);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void ListView_FullRowSelect_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
    {
        using ListView listView = new();
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.FullRowSelect = value;
        Assert.Equal(value, listView.FullRowSelect);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        listView.FullRowSelect = value;
        Assert.Equal(value, listView.FullRowSelect);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        listView.FullRowSelect = !value;
        Assert.Equal(!value, listView.FullRowSelect);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_GridLines_Set_GetReturnsExpected(bool value)
    {
        using ListView listView = new()
        {
            GridLines = value
        };
        Assert.Equal(value, listView.GridLines);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.GridLines = value;
        Assert.Equal(value, listView.GridLines);
        Assert.False(listView.IsHandleCreated);

        // Set different.
        listView.GridLines = !value;
        Assert.Equal(!value, listView.GridLines);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void ListView_GridLines_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
    {
        using ListView listView = new();
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.GridLines = value;
        Assert.Equal(value, listView.GridLines);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        listView.GridLines = value;
        Assert.Equal(value, listView.GridLines);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        listView.GridLines = !value;
        Assert.Equal(!value, listView.GridLines);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> GroupImageList_Set_GetReturnsExpected()
    {
        foreach (bool autoArrange in new bool[] { true, false })
        {
            foreach (bool virtualMode in new bool[] { true, false })
            {
                foreach (View view in new View[] { View.Details, View.LargeIcon, View.List, View.SmallIcon })
                {
                    yield return new object[] { autoArrange, virtualMode, view, null };
                    yield return new object[] { autoArrange, virtualMode, view, new ImageList() };
                    yield return new object[] { autoArrange, virtualMode, view, CreateImageListNonEmpty() };
                }
            }

            yield return new object[] { autoArrange, false, View.Tile, null };
            yield return new object[] { autoArrange, false, View.Tile, new ImageList() };
            yield return new object[] { autoArrange, false, View.Tile, CreateImageListNonEmpty() };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(GroupImageList_Set_GetReturnsExpected))]
    public void ListView_GroupImageList_Set_GetReturnsExpected(bool autoArrange, bool virtualMode, View view, ImageList value)
    {
        using ListView listView = new()
        {
            AutoArrange = autoArrange,
            VirtualMode = virtualMode,
            View = view,
            GroupImageList = value
        };

        Assert.Same(value, listView.GroupImageList);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.GroupImageList = value;
        Assert.Same(value, listView.GroupImageList);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(GroupImageList_Set_GetReturnsExpected))]
    public void ListView_GroupImageList_SetWithNonNullOldValue_GetReturnsExpected(bool autoArrange, bool virtualMode, View view, ImageList value)
    {
        using ImageList imageList = new();
        using ListView listView = new()
        {
            AutoArrange = autoArrange,
            VirtualMode = virtualMode,
            View = view,
            GroupImageList = imageList
        };

        listView.GroupImageList = value;
        Assert.Same(value, listView.GroupImageList);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.GroupImageList = value;
        Assert.Same(value, listView.GroupImageList);
        Assert.False(listView.IsHandleCreated);
    }

    public static IEnumerable<object[]> GroupImageList_SetWithHandle_GetReturnsExpected()
    {
        yield return new object[] { true, false, View.Details, null };
        yield return new object[] { true, false, View.Details, new ImageList() };
        yield return new object[] { true, false, View.Details, CreateImageListNonEmpty() };
        yield return new object[] { true, false, View.LargeIcon, null };
        yield return new object[] { true, false, View.LargeIcon, new ImageList() };
        yield return new object[] { true, false, View.LargeIcon, CreateImageListNonEmpty() };
        yield return new object[] { true, false, View.List, null };
        yield return new object[] { true, false, View.List, new ImageList() };
        yield return new object[] { true, false, View.List, CreateImageListNonEmpty() };
        yield return new object[] { true, false, View.SmallIcon, null };
        yield return new object[] { true, false, View.SmallIcon, new ImageList() };
        yield return new object[] { true, false, View.SmallIcon, CreateImageListNonEmpty() };
        yield return new object[] { true, false, View.Tile, null };
        yield return new object[] { true, false, View.Tile, new ImageList() };
        yield return new object[] { true, false, View.Tile, CreateImageListNonEmpty() };

        foreach (bool autoArrange in new bool[] { true, false })
        {
            yield return new object[] { autoArrange, true, View.Details, null };
            yield return new object[] { autoArrange, true, View.Details, new ImageList() };
            yield return new object[] { autoArrange, true, View.Details, CreateImageListNonEmpty() };
            yield return new object[] { autoArrange, true, View.LargeIcon, null };
            yield return new object[] { autoArrange, true, View.LargeIcon, new ImageList() };
            yield return new object[] { autoArrange, true, View.LargeIcon, CreateImageListNonEmpty() };
            yield return new object[] { autoArrange, true, View.List, null };
            yield return new object[] { autoArrange, true, View.List, new ImageList() };
            yield return new object[] { autoArrange, true, View.List, CreateImageListNonEmpty() };
            yield return new object[] { autoArrange, true, View.SmallIcon, null };
            yield return new object[] { autoArrange, true, View.SmallIcon, new ImageList() };
            yield return new object[] { autoArrange, true, View.SmallIcon, CreateImageListNonEmpty() };
        }

        yield return new object[] { false, false, View.Details, null };
        yield return new object[] { false, false, View.Details, new ImageList() };
        yield return new object[] { false, false, View.Details, CreateImageListNonEmpty() };
        yield return new object[] { false, false, View.LargeIcon, null };
        yield return new object[] { false, false, View.LargeIcon, new ImageList() };
        yield return new object[] { false, false, View.LargeIcon, CreateImageListNonEmpty() };
        yield return new object[] { false, false, View.List, null };
        yield return new object[] { false, false, View.List, new ImageList() };
        yield return new object[] { false, false, View.List, CreateImageListNonEmpty() };
        yield return new object[] { false, false, View.SmallIcon, null };
        yield return new object[] { false, false, View.SmallIcon, new ImageList() };
        yield return new object[] { false, false, View.SmallIcon, CreateImageListNonEmpty() };
        yield return new object[] { false, false, View.Tile, null };
        yield return new object[] { false, false, View.Tile, new ImageList() };
        yield return new object[] { false, false, View.Tile, CreateImageListNonEmpty() };
    }

    [WinFormsTheory]
    [MemberData(nameof(GroupImageList_SetWithHandle_GetReturnsExpected))]
    public void ListView_GroupImageList_SetWithHandle_GetReturnsExpected(bool autoArrange, bool virtualMode, View view, ImageList value)
    {
        using ListView listView = new()
        {
            AutoArrange = autoArrange,
            VirtualMode = virtualMode,
            View = view
        };

        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.GroupImageList = value;
        Assert.Same(value, listView.GroupImageList);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        listView.GroupImageList = value;
        Assert.Same(value, listView.GroupImageList);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> GroupImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected()
    {
        yield return new object[] { true, false, View.Details, null };
        yield return new object[] { true, false, View.Details, new ImageList() };
        yield return new object[] { true, false, View.Details, CreateImageListNonEmpty() };
        yield return new object[] { true, false, View.LargeIcon, null };
        yield return new object[] { true, false, View.LargeIcon, new ImageList() };
        yield return new object[] { true, false, View.LargeIcon, CreateImageListNonEmpty() };
        yield return new object[] { true, false, View.List, null };
        yield return new object[] { true, false, View.List, new ImageList() };
        yield return new object[] { true, false, View.List, CreateImageListNonEmpty() };
        yield return new object[] { true, false, View.SmallIcon, null };
        yield return new object[] { true, false, View.SmallIcon, new ImageList() };
        yield return new object[] { true, false, View.SmallIcon, CreateImageListNonEmpty() };
        yield return new object[] { true, false, View.Tile, null };
        yield return new object[] { true, false, View.Tile, new ImageList() };
        yield return new object[] { true, false, View.Tile, CreateImageListNonEmpty() };

        foreach (bool autoArrange in new bool[] { true, false })
        {
            yield return new object[] { autoArrange, true, View.Details, null };
            yield return new object[] { autoArrange, true, View.Details, new ImageList() };
            yield return new object[] { autoArrange, true, View.Details, CreateImageListNonEmpty() };
            yield return new object[] { autoArrange, true, View.LargeIcon, null };
            yield return new object[] { autoArrange, true, View.LargeIcon, new ImageList() };
            yield return new object[] { autoArrange, true, View.LargeIcon, CreateImageListNonEmpty() };
            yield return new object[] { autoArrange, true, View.List, null };
            yield return new object[] { autoArrange, true, View.List, new ImageList() };
            yield return new object[] { autoArrange, true, View.List, CreateImageListNonEmpty() };
            yield return new object[] { autoArrange, true, View.SmallIcon, null };
            yield return new object[] { autoArrange, true, View.SmallIcon, new ImageList() };
            yield return new object[] { autoArrange, true, View.SmallIcon, CreateImageListNonEmpty() };
        }

        yield return new object[] { false, false, View.Details, null };
        yield return new object[] { false, false, View.Details, new ImageList() };
        yield return new object[] { false, false, View.Details, CreateImageListNonEmpty() };
        yield return new object[] { false, false, View.LargeIcon, null };
        yield return new object[] { false, false, View.LargeIcon, new ImageList() };
        yield return new object[] { false, false, View.LargeIcon, CreateImageListNonEmpty() };
        yield return new object[] { false, false, View.List, null };
        yield return new object[] { false, false, View.List, new ImageList() };
        yield return new object[] { false, false, View.List, CreateImageListNonEmpty() };
        yield return new object[] { false, false, View.SmallIcon, null };
        yield return new object[] { false, false, View.SmallIcon, new ImageList() };
        yield return new object[] { false, false, View.SmallIcon, CreateImageListNonEmpty() };
        yield return new object[] { false, false, View.Tile, null };
        yield return new object[] { false, false, View.Tile, new ImageList() };
        yield return new object[] { false, false, View.Tile, CreateImageListNonEmpty() };
    }

    [WinFormsTheory]
    [MemberData(nameof(GroupImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected))]
    public void ListView_GroupImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected(bool autoArrange, bool virtualMode, View view, ImageList value)
    {
        using ListView listView = new()
        {
            AutoArrange = autoArrange,
            VirtualMode = virtualMode,
            View = view,
        };

        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.GroupImageList = value;
        Assert.Same(value, listView.GroupImageList);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        listView.GroupImageList = value;
        Assert.Same(value, listView.GroupImageList);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_GroupImageList_Dispose_DetachesFromListView(bool autoArrange)
    {
        using ImageList imageList1 = new();
        using ImageList imageList2 = new();
        using ListView listView = new()
        {
            AutoArrange = autoArrange,
            GroupImageList = imageList1
        };

        Assert.Same(imageList1, listView.GroupImageList);

        imageList1.Dispose();
        Assert.Null(listView.GroupImageList);
        Assert.False(listView.IsHandleCreated);

        // Make sure we detached the setter.
        listView.GroupImageList = imageList2;
        imageList1.Dispose();
        Assert.Same(imageList2, listView.GroupImageList);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void ListView_GroupImageList_DisposeWithHandle_DetachesFromListView(bool autoArrange, int expectedInvalidatedCallCount)
    {
        using ImageList imageList1 = new();
        using ImageList imageList2 = new();
        using ListView listView = new()
        {
            AutoArrange = autoArrange
        };

        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.GroupImageList = imageList1;
        Assert.Same(imageList1, listView.GroupImageList);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        imageList1.Dispose();
        Assert.Null(listView.GroupImageList);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Make sure we detached the setter.
        listView.GroupImageList = imageList2;
        imageList1.Dispose();
        Assert.Same(imageList2, listView.GroupImageList);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ListView_Handle_GetWithBackColor_Success()
    {
        using ListView control = new()
        {
            BackColor = Color.FromArgb(0xFF, 0x12, 0x34, 0x56)
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(0x563412, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETBKCOLOR));
    }

    [WinFormsFact]
    public void ListView_Handle_GetWithForeColor_Success()
    {
        using ListView control = new()
        {
            ForeColor = Color.FromArgb(0x12, 0x34, 0x56, 0x78)
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(0x785634, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETTEXTCOLOR));
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_Handle_GetWithoutGroups_Success(bool showGroups)
    {
        using ListView listView = new()
        {
            ShowGroups = showGroups
        };
        Assert.Equal(0, (int)PInvokeCore.SendMessage(listView, PInvoke.LVM_GETGROUPCOUNT));
    }

    public static IEnumerable<object[]> Handle_GetWithGroups_TestData()
    {
        foreach (bool showGroups in new bool[] { true, false })
        {
            yield return new object[] { showGroups, null, HorizontalAlignment.Left, null, HorizontalAlignment.Right, string.Empty, string.Empty, 0x00000021 };
            yield return new object[] { showGroups, null, HorizontalAlignment.Center, null, HorizontalAlignment.Center, string.Empty, string.Empty, 0x00000012 };
            yield return new object[] { showGroups, null, HorizontalAlignment.Right, null, HorizontalAlignment.Left, string.Empty, string.Empty, 0x0000000C };

            yield return new object[] { showGroups, string.Empty, HorizontalAlignment.Left, string.Empty, HorizontalAlignment.Right, string.Empty, string.Empty, 0x00000021 };
            yield return new object[] { showGroups, string.Empty, HorizontalAlignment.Center, string.Empty, HorizontalAlignment.Center, string.Empty, string.Empty, 0x00000012 };
            yield return new object[] { showGroups, string.Empty, HorizontalAlignment.Right, string.Empty, HorizontalAlignment.Left, string.Empty, string.Empty, 0x0000000C };

            yield return new object[] { showGroups, "header", HorizontalAlignment.Left, "footer", HorizontalAlignment.Right, "header", "footer", 0x00000021 };
            yield return new object[] { showGroups, "header", HorizontalAlignment.Center, "footer", HorizontalAlignment.Center, "header", "footer", 0x00000012 };
            yield return new object[] { showGroups, "header", HorizontalAlignment.Right, "footer", HorizontalAlignment.Left, "header", "footer", 0x0000000C };

            yield return new object[] { showGroups, "he\0der", HorizontalAlignment.Left, "fo\0oter", HorizontalAlignment.Right, "he", "fo", 0x00000021 };
            yield return new object[] { showGroups, "he\0der", HorizontalAlignment.Center, "fo\0oter", HorizontalAlignment.Center, "he", "fo", 0x00000012 };
            yield return new object[] { showGroups, "he\0der", HorizontalAlignment.Right, "fo\0oter", HorizontalAlignment.Left, "he", "fo", 0x0000000C };
        }
    }

    [WinFormsFact(Skip = "Crash with AbandonedMutexException. See: https://github.com/dotnet/arcade/issues/5325")]
    public unsafe void ListView_Handle_GetWithGroups_Success()
    {
        // Run this from another thread as we call Application.EnableVisualStyles.
        using RemoteInvokeHandle invokerHandle = RemoteExecutor.Invoke(() =>
        {
            char* headerBuffer = stackalloc char[256];
            char* footerBuffer = stackalloc char[256];

            foreach (object[] data in Handle_GetWithGroups_TestData())
            {
                bool showGroups = (bool)data[0];
                string header = (string)data[1];
                HorizontalAlignment headerAlignment = (HorizontalAlignment)data[2];
                string footer = (string)data[3];
                HorizontalAlignment footerAlignment = (HorizontalAlignment)data[4];
                string expectedHeaderText = (string)data[5];
                string expectedFooterText = (string)data[6];
                int expectedAlign = (int)data[7];

                Application.EnableVisualStyles();

                using ListView listView = new()
                {
                    ShowGroups = showGroups
                };
                ListViewGroup group1 = new();
                ListViewGroup group2 = new()
                {
                    Header = header,
                    HeaderAlignment = headerAlignment,
                    Footer = footer,
                    FooterAlignment = footerAlignment
                };
                listView.Groups.Add(group1);
                listView.Groups.Add(group2);

                Assert.Equal(2, (int)PInvokeCore.SendMessage(listView, PInvoke.LVM_GETGROUPCOUNT));

                LVGROUP lvgroup1 = new()
                {
                    cbSize = (uint)sizeof(LVGROUP),
                    mask = LVGROUP_MASK.LVGF_HEADER | LVGROUP_MASK.LVGF_FOOTER | LVGROUP_MASK.LVGF_GROUPID | LVGROUP_MASK.LVGF_ALIGN,
                    pszHeader = headerBuffer,
                    cchHeader = 256,
                    pszFooter = footerBuffer,
                    cchFooter = 256,
                };
                Assert.Equal(1, (int)PInvokeCore.SendMessage(listView, PInvoke.LVM_GETGROUPINFOBYINDEX, 0, ref lvgroup1));
                Assert.Equal("ListViewGroup", new string(lvgroup1.pszHeader));
                Assert.Empty(new string(lvgroup1.pszFooter));
                Assert.True(lvgroup1.iGroupId >= 0);
                Assert.Equal(0x00000009, (int)lvgroup1.uAlign);

                LVGROUP lvgroup2 = new()
                {
                    cbSize = (uint)sizeof(LVGROUP),
                    mask = LVGROUP_MASK.LVGF_HEADER | LVGROUP_MASK.LVGF_FOOTER | LVGROUP_MASK.LVGF_GROUPID | LVGROUP_MASK.LVGF_ALIGN,
                    pszHeader = headerBuffer,
                    cchHeader = 256,
                    pszFooter = footerBuffer,
                    cchFooter = 256,
                };
                Assert.Equal(1, (int)PInvokeCore.SendMessage(listView, PInvoke.LVM_GETGROUPINFOBYINDEX, 1, ref lvgroup2));
                Assert.Equal(expectedHeaderText, new string(lvgroup2.pszHeader));
                Assert.Equal(expectedFooterText, new string(lvgroup2.pszFooter));
                Assert.True(lvgroup2.iGroupId > 0);
                Assert.Equal(expectedAlign, (int)lvgroup2.uAlign);
                Assert.True(lvgroup2.iGroupId > lvgroup1.iGroupId);
            }
        });

        // verify the remote process succeeded
        Assert.Equal(RemoteExecutor.SuccessExitCode, invokerHandle.ExitCode);
    }

    [WinFormsFact]
    public void ListView_Handle_GetTextBackColor_Success()
    {
        using ListView control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        nint expected = unchecked((nint)0xFFFFFFFF);
        Assert.Equal(expected, (nint)PInvokeCore.SendMessage(control, PInvoke.LVM_GETTEXTBKCOLOR));
    }

    [WinFormsFact]
    public void ListView_Handle_GetVersion_ReturnsExpected()
    {
        using ListView control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int version = Application.UseVisualStyles ? 6 : 5;
        Assert.Equal(version, (int)PInvokeCore.SendMessage(control, PInvoke.CCM_GETVERSION));
    }

    public static IEnumerable<object[]> Handle_CustomGetVersion_TestData()
    {
        yield return new object[] { IntPtr.Zero, 1 };
        yield return new object[] { (IntPtr)4, 1 };
        yield return new object[] { (IntPtr)5, 0 };
        yield return new object[] { (IntPtr)6, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Handle_CustomGetVersion_TestData))]
    public void ListView_Handle_CustomGetVersion_Success(IntPtr getVersionResult, int expectedSetVersionCallCount)
    {
        using CustomGetVersionListView control = new()
        {
            GetVersionResult = getVersionResult
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(expectedSetVersionCallCount, control.SetVersionCallCount);
    }

    private class CustomGetVersionListView : ListView
    {
        public IntPtr GetVersionResult { get; set; }
        public int SetVersionCallCount { get; set; }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)PInvoke.CCM_GETVERSION)
            {
                Assert.Equal(IntPtr.Zero, m.WParam);
                Assert.Equal(IntPtr.Zero, m.LParam);
                m.Result = GetVersionResult;
                return;
            }
            else if (m.Msg == (int)PInvoke.CCM_SETVERSION)
            {
                Assert.Equal(5, m.WParam);
                Assert.Equal(IntPtr.Zero, m.LParam);
                SetVersionCallCount++;
                return;
            }

            base.WndProc(ref m);
        }
    }

    [WinFormsTheory]
    [EnumData<ColumnHeaderStyle>]
    public void ListView_HeaderStyle_Set_GetReturnsExpected(ColumnHeaderStyle value)
    {
        using ListView listView = new()
        {
            HeaderStyle = value
        };
        Assert.Equal(value, listView.HeaderStyle);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.HeaderStyle = value;
        Assert.Equal(value, listView.HeaderStyle);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(ColumnHeaderStyle.Clickable, 0, 0, 0)]
    [InlineData(ColumnHeaderStyle.Nonclickable, 2, 0, 1)]
    [InlineData(ColumnHeaderStyle.None, 1, 1, 0)]
    public void ListView_HeaderStyle_SetClickableWithHandle_GetReturnsExpected(ColumnHeaderStyle value, int expectedInvalidatedCallCount, int expectedStyleChangedCallCount, int expectedCreatedCallCount)
    {
        using ListView listView = new();
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.HeaderStyle = value;
        Assert.Equal(value, listView.HeaderStyle);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        listView.HeaderStyle = value;
        Assert.Equal(value, listView.HeaderStyle);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(ColumnHeaderStyle.Clickable, 2, 0, 1)]
    [InlineData(ColumnHeaderStyle.Nonclickable, 0, 0, 0)]
    [InlineData(ColumnHeaderStyle.None, 1, 1, 0)]
    public void ListView_HeaderStyle_SetNonClickableWithHandle_GetReturnsExpected(ColumnHeaderStyle value, int expectedInvalidatedCallCount, int expectedStyleChangedCallCount, int expectedCreatedCallCount)
    {
        using ListView listView = new()
        {
            HeaderStyle = ColumnHeaderStyle.Nonclickable
        };
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.HeaderStyle = value;
        Assert.Equal(value, listView.HeaderStyle);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        listView.HeaderStyle = value;
        Assert.Equal(value, listView.HeaderStyle);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<ColumnHeaderStyle>]
    public void ListView_HeaderStyle_SetInvalidValue_ThrowsInvalidEnumArgumentException(ColumnHeaderStyle value)
    {
        using ListView listView = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => listView.HeaderStyle = value);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_HideSelection_Set_GetReturnsExpected(bool value)
    {
        using ListView listView = new()
        {
            HideSelection = value
        };
        Assert.Equal(value, listView.HideSelection);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.HideSelection = value;
        Assert.Equal(value, listView.HideSelection);
        Assert.False(listView.IsHandleCreated);

        // Set different.
        listView.HideSelection = !value;
        Assert.Equal(!value, listView.HideSelection);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void ListView_HideSelection_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
    {
        using ListView listView = new();
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.HideSelection = value;
        Assert.Equal(value, listView.HideSelection);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        listView.HideSelection = value;
        Assert.Equal(value, listView.HideSelection);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        listView.HideSelection = !value;
        Assert.Equal(!value, listView.HideSelection);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
        Assert.Equal(expectedInvalidatedCallCount + 1, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_HotTracking_Set_GetReturnsExpected(bool value)
    {
        using ListView listView = new()
        {
            HotTracking = value
        };
        Assert.Equal(value, listView.HotTracking);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.HotTracking = value;
        Assert.Equal(value, listView.HotTracking);
        Assert.False(listView.IsHandleCreated);

        // Set different.
        listView.HotTracking = !value;
        Assert.Equal(!value, listView.HotTracking);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 3, 4)]
    [InlineData(false, 0, 3)]
    public void ListView_HotTracking_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount1, int expectedInvalidatedCallCount2)
    {
        using ListView listView = new();
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.HotTracking = value;
        Assert.Equal(value, listView.HotTracking);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        listView.HotTracking = value;
        Assert.Equal(value, listView.HotTracking);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        listView.HotTracking = !value;
        Assert.Equal(!value, listView.HotTracking);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ListView_HotTracking_Set_SetsHoverSelectionAndActivationIfTrue()
    {
        using ListView listView = new()
        {
            HotTracking = true
        };
        Assert.True(listView.HotTracking);
        Assert.True(listView.HoverSelection);
        Assert.Equal(ItemActivation.OneClick, listView.Activation);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.HotTracking = true;
        Assert.True(listView.HotTracking);
        Assert.True(listView.HoverSelection);
        Assert.Equal(ItemActivation.OneClick, listView.Activation);
        Assert.False(listView.IsHandleCreated);

        // Set different.
        listView.HotTracking = false;
        Assert.False(listView.HotTracking);
        Assert.True(listView.HoverSelection);
        Assert.Equal(ItemActivation.OneClick, listView.Activation);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_HoverSelection_Set_GetReturnsExpected(bool value)
    {
        using ListView listView = new()
        {
            HoverSelection = value
        };
        Assert.Equal(value, listView.HoverSelection);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.HoverSelection = value;
        Assert.Equal(value, listView.HoverSelection);
        Assert.False(listView.IsHandleCreated);

        // Set different.
        listView.HoverSelection = !value;
        Assert.Equal(!value, listView.HoverSelection);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void ListView_HoverSelection_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
    {
        using ListView listView = new();
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.HoverSelection = value;
        Assert.Equal(value, listView.HoverSelection);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        listView.HoverSelection = value;
        Assert.Equal(value, listView.HoverSelection);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        listView.HoverSelection = !value;
        Assert.Equal(!value, listView.HoverSelection);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ListView_HoverSelection_SetHotTrackingTrue_Nop()
    {
        using ListView listView = new()
        {
            HotTracking = true,
            HoverSelection = true
        };
        Assert.True(listView.HoverSelection);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.HoverSelection = true;
        Assert.True(listView.HoverSelection);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListView_HoverSelection_SetHotTrackingFalse_ThrowsArgumentException()
    {
        using ListView listView = new()
        {
            HotTracking = true
        };
        Assert.Throws<ArgumentException>("value", () => listView.HoverSelection = false);
        Assert.True(listView.HoverSelection);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_LabelEdit_Set_GetReturnsExpected(bool value)
    {
        using ListView listView = new()
        {
            LabelEdit = value
        };
        Assert.Equal(value, listView.LabelEdit);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.LabelEdit = value;
        Assert.Equal(value, listView.LabelEdit);
        Assert.False(listView.IsHandleCreated);

        // Set different.
        listView.LabelEdit = !value;
        Assert.Equal(!value, listView.LabelEdit);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void ListView_LabelEdit_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
    {
        using ListView listView = new();
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.LabelEdit = value;
        Assert.Equal(value, listView.LabelEdit);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        listView.LabelEdit = value;
        Assert.Equal(value, listView.LabelEdit);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        listView.LabelEdit = !value;
        Assert.Equal(!value, listView.LabelEdit);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
        Assert.Equal(expectedInvalidatedCallCount + 1, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_LabelWrap_Set_GetReturnsExpected(bool value)
    {
        using ListView listView = new()
        {
            LabelWrap = value
        };
        Assert.Equal(value, listView.LabelWrap);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.LabelWrap = value;
        Assert.Equal(value, listView.LabelWrap);
        Assert.False(listView.IsHandleCreated);

        // Set different.
        listView.LabelWrap = !value;
        Assert.Equal(!value, listView.LabelWrap);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 0)]
    [InlineData(false, 1)]
    public void ListView_LabelWrap_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
    {
        using ListView listView = new();
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.LabelWrap = value;
        Assert.Equal(value, listView.LabelWrap);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        listView.LabelWrap = value;
        Assert.Equal(value, listView.LabelWrap);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        listView.LabelWrap = !value;
        Assert.Equal(!value, listView.LabelWrap);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
        Assert.Equal(expectedInvalidatedCallCount + 1, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> LargeImageList_Set_GetReturnsExpected()
    {
        foreach (bool autoArrange in new bool[] { true, false })
        {
            foreach (bool virtualMode in new bool[] { true, false })
            {
                foreach (View view in new View[] { View.Details, View.LargeIcon, View.List, View.SmallIcon })
                {
                    yield return new object[] { autoArrange, virtualMode, view, null };
                    yield return new object[] { autoArrange, virtualMode, view, new ImageList() };
                    yield return new object[] { autoArrange, virtualMode, view, CreateImageListNonEmpty() };
                }
            }

            yield return new object[] { autoArrange, false, View.Tile, null };
            yield return new object[] { autoArrange, false, View.Tile, new ImageList() };
            yield return new object[] { autoArrange, false, View.Tile, CreateImageListNonEmpty() };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(LargeImageList_Set_GetReturnsExpected))]
    public void ListView_LargeImageList_Set_GetReturnsExpected(bool autoArrange, bool virtualMode, View view, ImageList value)
    {
        using ListView listView = new()
        {
            AutoArrange = autoArrange,
            VirtualMode = virtualMode,
            View = view,
            LargeImageList = value
        };
        Assert.Same(value, listView.LargeImageList);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.LargeImageList = value;
        Assert.Same(value, listView.LargeImageList);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(LargeImageList_Set_GetReturnsExpected))]
    public void ListView_LargeImageList_SetWithNonNullOldValue_GetReturnsExpected(bool autoArrange, bool virtualMode, View view, ImageList value)
    {
        using ImageList imageList = new();
        using ListView listView = new()
        {
            AutoArrange = autoArrange,
            VirtualMode = virtualMode,
            View = view,
            LargeImageList = imageList
        };

        listView.LargeImageList = value;
        Assert.Same(value, listView.LargeImageList);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.LargeImageList = value;
        Assert.Same(value, listView.LargeImageList);
        Assert.False(listView.IsHandleCreated);
    }

    public static IEnumerable<object[]> LargeImageList_SetWithHandle_GetReturnsExpected()
    {
        yield return new object[] { true, false, View.Details, null, 0 };
        yield return new object[] { true, false, View.Details, new ImageList(), 0 };
        yield return new object[] { true, false, View.Details, CreateImageListNonEmpty(), 0 };
        yield return new object[] { true, false, View.LargeIcon, null, 0 };
        yield return new object[] { true, false, View.LargeIcon, new ImageList(), 1 };
        yield return new object[] { true, false, View.LargeIcon, CreateImageListNonEmpty(), 1 };
        yield return new object[] { true, false, View.List, null, 0 };
        yield return new object[] { true, false, View.List, new ImageList(), 0 };
        yield return new object[] { true, false, View.List, CreateImageListNonEmpty(), 0 };
        yield return new object[] { true, false, View.SmallIcon, null, 0 };
        yield return new object[] { true, false, View.SmallIcon, new ImageList(), 1 };
        yield return new object[] { true, false, View.SmallIcon, CreateImageListNonEmpty(), 1 };
        yield return new object[] { true, false, View.Tile, null, 0 };
        yield return new object[] { true, false, View.Tile, new ImageList(), 0 };
        yield return new object[] { true, false, View.Tile, CreateImageListNonEmpty(), 0 };

        foreach (bool autoArrange in new bool[] { true, false })
        {
            yield return new object[] { autoArrange, true, View.Details, null, 0 };
            yield return new object[] { autoArrange, true, View.Details, new ImageList(), 0 };
            yield return new object[] { autoArrange, true, View.Details, CreateImageListNonEmpty(), 0 };
            yield return new object[] { autoArrange, true, View.LargeIcon, null, 0 };
            yield return new object[] { autoArrange, true, View.LargeIcon, new ImageList(), 0 };
            yield return new object[] { autoArrange, true, View.LargeIcon, CreateImageListNonEmpty(), 0 };
            yield return new object[] { autoArrange, true, View.List, null, 0 };
            yield return new object[] { autoArrange, true, View.List, new ImageList(), 0 };
            yield return new object[] { autoArrange, true, View.List, CreateImageListNonEmpty(), 0 };
            yield return new object[] { autoArrange, true, View.SmallIcon, null, 0 };
            yield return new object[] { autoArrange, true, View.SmallIcon, new ImageList(), 0 };
            yield return new object[] { autoArrange, true, View.SmallIcon, CreateImageListNonEmpty(), 0 };
        }

        yield return new object[] { false, false, View.Details, null, 0 };
        yield return new object[] { false, false, View.Details, new ImageList(), 0 };
        yield return new object[] { false, false, View.Details, CreateImageListNonEmpty(), 0 };
        yield return new object[] { false, false, View.LargeIcon, null, 0 };
        yield return new object[] { false, false, View.LargeIcon, new ImageList(), 0 };
        yield return new object[] { false, false, View.LargeIcon, CreateImageListNonEmpty(), 0 };
        yield return new object[] { false, false, View.List, null, 0 };
        yield return new object[] { false, false, View.List, new ImageList(), 0 };
        yield return new object[] { false, false, View.List, CreateImageListNonEmpty(), 0 };
        yield return new object[] { false, false, View.SmallIcon, null, 0 };
        yield return new object[] { false, false, View.SmallIcon, new ImageList(), 0 };
        yield return new object[] { false, false, View.SmallIcon, CreateImageListNonEmpty(), 0 };
        yield return new object[] { false, false, View.Tile, null, 0 };
        yield return new object[] { false, false, View.Tile, new ImageList(), 0 };
        yield return new object[] { false, false, View.Tile, CreateImageListNonEmpty(), 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(LargeImageList_SetWithHandle_GetReturnsExpected))]
    public void ListView_LargeImageList_SetWithHandle_GetReturnsExpected(bool autoArrange, bool virtualMode, View view, ImageList value, int expectedInvalidatedCallCount)
    {
        using ListView listView = new()
        {
            AutoArrange = autoArrange,
            VirtualMode = virtualMode,
            View = view
        };
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.LargeImageList = value;
        Assert.Same(value, listView.LargeImageList);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        listView.LargeImageList = value;
        Assert.Same(value, listView.LargeImageList);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> LargeImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected()
    {
        yield return new object[] { true, false, View.Details, null, 0 };
        yield return new object[] { true, false, View.Details, new ImageList(), 0 };
        yield return new object[] { true, false, View.Details, CreateImageListNonEmpty(), 0 };
        yield return new object[] { true, false, View.LargeIcon, null, 1 };
        yield return new object[] { true, false, View.LargeIcon, new ImageList(), 1 };
        yield return new object[] { true, false, View.LargeIcon, CreateImageListNonEmpty(), 1 };
        yield return new object[] { true, false, View.List, null, 0 };
        yield return new object[] { true, false, View.List, new ImageList(), 0 };
        yield return new object[] { true, false, View.List, CreateImageListNonEmpty(), 0 };
        yield return new object[] { true, false, View.SmallIcon, null, 1 };
        yield return new object[] { true, false, View.SmallIcon, new ImageList(), 1 };
        yield return new object[] { true, false, View.SmallIcon, CreateImageListNonEmpty(), 1 };
        yield return new object[] { true, false, View.Tile, null, 0 };
        yield return new object[] { true, false, View.Tile, new ImageList(), 0 };
        yield return new object[] { true, false, View.Tile, CreateImageListNonEmpty(), 0 };

        foreach (bool autoArrange in new bool[] { true, false })
        {
            yield return new object[] { autoArrange, true, View.Details, null, 0 };
            yield return new object[] { autoArrange, true, View.Details, new ImageList(), 0 };
            yield return new object[] { autoArrange, true, View.Details, CreateImageListNonEmpty(), 0 };
            yield return new object[] { autoArrange, true, View.LargeIcon, null, 0 };
            yield return new object[] { autoArrange, true, View.LargeIcon, new ImageList(), 0 };
            yield return new object[] { autoArrange, true, View.LargeIcon, CreateImageListNonEmpty(), 0 };
            yield return new object[] { autoArrange, true, View.List, null, 0 };
            yield return new object[] { autoArrange, true, View.List, new ImageList(), 0 };
            yield return new object[] { autoArrange, true, View.List, CreateImageListNonEmpty(), 0 };
            yield return new object[] { autoArrange, true, View.SmallIcon, null, 0 };
            yield return new object[] { autoArrange, true, View.SmallIcon, new ImageList(), 0 };
            yield return new object[] { autoArrange, true, View.SmallIcon, CreateImageListNonEmpty(), 0 };
        }

        yield return new object[] { false, false, View.Details, null, 0 };
        yield return new object[] { false, false, View.Details, new ImageList(), 0 };
        yield return new object[] { false, false, View.Details, CreateImageListNonEmpty(), 0 };
        yield return new object[] { false, false, View.LargeIcon, null, 0 };
        yield return new object[] { false, false, View.LargeIcon, new ImageList(), 0 };
        yield return new object[] { false, false, View.LargeIcon, CreateImageListNonEmpty(), 0 };
        yield return new object[] { false, false, View.List, null, 0 };
        yield return new object[] { false, false, View.List, new ImageList(), 0 };
        yield return new object[] { false, false, View.List, CreateImageListNonEmpty(), 0 };
        yield return new object[] { false, false, View.SmallIcon, null, 0 };
        yield return new object[] { false, false, View.SmallIcon, new ImageList(), 0 };
        yield return new object[] { false, false, View.SmallIcon, CreateImageListNonEmpty(), 0 };
        yield return new object[] { false, false, View.Tile, null, 0 };
        yield return new object[] { false, false, View.Tile, new ImageList(), 0 };
        yield return new object[] { false, false, View.Tile, CreateImageListNonEmpty(), 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(LargeImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected))]
    public void ListView_LargeImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected(bool autoArrange, bool virtualMode, View view, ImageList value, int expectedInvalidatedCallCount)
    {
        using ImageList imageList = new();
        using ListView listView = new()
        {
            AutoArrange = autoArrange,
            VirtualMode = virtualMode,
            View = view,
            LargeImageList = imageList
        };
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.LargeImageList = value;
        Assert.Same(value, listView.LargeImageList);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        listView.LargeImageList = value;
        Assert.Same(value, listView.LargeImageList);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_LargeImageList_Dispose_DetachesFromListView(bool autoArrange)
    {
        using ImageList imageList1 = new();
        using ImageList imageList2 = new();
        using ListView listView = new()
        {
            AutoArrange = autoArrange,
            LargeImageList = imageList1
        };
        Assert.Same(imageList1, listView.LargeImageList);

        imageList1.Dispose();
        Assert.Null(listView.LargeImageList);
        Assert.False(listView.IsHandleCreated);

        // Make sure we detached the setter.
        listView.LargeImageList = imageList2;
        imageList1.Dispose();
        Assert.Same(imageList2, listView.LargeImageList);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void ListView_LargeImageList_DisposeWithHandle_DetachesFromListView(bool autoArrange, int expectedInvalidatedCallCount)
    {
        using ImageList imageList1 = new();
        using ImageList imageList2 = new();
        using ListView listView = new()
        {
            AutoArrange = autoArrange
        };
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.LargeImageList = imageList1;
        Assert.Same(imageList1, listView.LargeImageList);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        imageList1.Dispose();
        Assert.Null(listView.LargeImageList);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Make sure we detached the setter.
        listView.LargeImageList = imageList2;
        imageList1.Dispose();
        Assert.Same(imageList2, listView.LargeImageList);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount * 3, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_MultiSelect_Set_GetReturnsExpected(bool value)
    {
        using ListView listView = new()
        {
            MultiSelect = value
        };
        Assert.Equal(value, listView.MultiSelect);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.MultiSelect = value;
        Assert.Equal(value, listView.MultiSelect);
        Assert.False(listView.IsHandleCreated);

        // Set different.
        listView.MultiSelect = !value;
        Assert.Equal(!value, listView.MultiSelect);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 0)]
    [InlineData(false, 1)]
    public void ListView_MultiSelect_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
    {
        using ListView listView = new();
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.MultiSelect = value;
        Assert.Equal(value, listView.MultiSelect);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        listView.MultiSelect = value;
        Assert.Equal(value, listView.MultiSelect);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        listView.MultiSelect = !value;
        Assert.Equal(!value, listView.MultiSelect);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
        Assert.Equal(expectedInvalidatedCallCount + 1, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_OwnerDraw_Set_GetReturnsExpected(bool value)
    {
        using ListView listView = new()
        {
            OwnerDraw = value
        };
        Assert.Equal(value, listView.OwnerDraw);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.OwnerDraw = value;
        Assert.Equal(value, listView.OwnerDraw);
        Assert.False(listView.IsHandleCreated);

        // Set different.
        listView.OwnerDraw = !value;
        Assert.Equal(!value, listView.OwnerDraw);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void ListView_OwnerDraw_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
    {
        using ListView listView = new();
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.OwnerDraw = value;
        Assert.Equal(value, listView.OwnerDraw);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        listView.OwnerDraw = value;
        Assert.Equal(value, listView.OwnerDraw);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        listView.OwnerDraw = !value;
        Assert.Equal(!value, listView.OwnerDraw);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_Scrollable_Set_GetReturnsExpected(bool value)
    {
        using ListView listView = new()
        {
            Scrollable = value
        };
        Assert.Equal(value, listView.Scrollable);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.Scrollable = value;
        Assert.Equal(value, listView.Scrollable);
        Assert.False(listView.IsHandleCreated);

        // Set different.
        listView.Scrollable = !value;
        Assert.Equal(!value, listView.Scrollable);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 0, 0)]
    [InlineData(false, 2, 1)]
    public void ListView_Scrollable_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount, int expectedCreatedCallCount)
    {
        using ListView listView = new();
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.Scrollable = value;
        Assert.Equal(value, listView.Scrollable);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        listView.Scrollable = value;
        Assert.Equal(value, listView.Scrollable);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set different.
        listView.Scrollable = !value;
        Assert.Equal(!value, listView.Scrollable);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount + 2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount + 1, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_ShowGroups_Set_GetReturnsExpected(bool value)
    {
        using ListView listView = new()
        {
            ShowGroups = value
        };
        Assert.Equal(value, listView.ShowGroups);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.ShowGroups = value;
        Assert.Equal(value, listView.ShowGroups);
        Assert.False(listView.IsHandleCreated);

        // Set different.
        listView.ShowGroups = !value;
        Assert.Equal(!value, listView.ShowGroups);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_ShowGroups_VirtualMode_Set_GetReturnsExpected(bool value)
    {
        using ListView listView = new()
        {
            ShowGroups = value,
            VirtualMode = true,
        };
        Assert.Equal(value, listView.ShowGroups);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.ShowGroups = value;
        Assert.Equal(value, listView.ShowGroups);
        Assert.False(listView.IsHandleCreated);

        // Set different.
        listView.ShowGroups = !value;
        Assert.Equal(!value, listView.ShowGroups);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_ShowGroups_SetWithHandle_GetReturnsExpected(bool value)
    {
        using ListView listView = new();
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.ShowGroups = value;
        Assert.Equal(value, listView.ShowGroups);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        listView.ShowGroups = value;
        Assert.Equal(value, listView.ShowGroups);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        listView.ShowGroups = !value;
        Assert.Equal(!value, listView.ShowGroups);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_ShowGroups_VirtualMode_SetWithHandle_GetReturnsExpected(bool value)
    {
        using ListView listView = new()
        {
            VirtualMode = true,
        };

        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.ShowGroups = value;
        Assert.Equal(value, listView.ShowGroups);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        listView.ShowGroups = value;
        Assert.Equal(value, listView.ShowGroups);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        listView.ShowGroups = !value;
        Assert.Equal(!value, listView.ShowGroups);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_ShowItemToolTips_Set_GetReturnsExpected(bool value)
    {
        using ListView listView = new()
        {
            ShowItemToolTips = value
        };
        Assert.Equal(value, listView.ShowItemToolTips);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.ShowItemToolTips = value;
        Assert.Equal(value, listView.ShowItemToolTips);
        Assert.False(listView.IsHandleCreated);

        // Set different.
        listView.ShowItemToolTips = !value;
        Assert.Equal(!value, listView.ShowItemToolTips);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 2, 1)]
    [InlineData(false, 0, 0)]
    public void ListView_ShowItemToolTips_SetWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount, int expectedCreatedCallCount)
    {
        using ListView listView = new();
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.ShowItemToolTips = value;
        Assert.Equal(value, listView.ShowItemToolTips);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        listView.ShowItemToolTips = value;
        Assert.Equal(value, listView.ShowItemToolTips);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set different.
        listView.ShowItemToolTips = !value;
        Assert.Equal(!value, listView.ShowItemToolTips);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount + 2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount + 1, createdCallCount);
    }

    public static IEnumerable<object[]> SmallImageList_Set_GetReturnsExpected()
    {
        foreach (bool autoArrange in new bool[] { true, false })
        {
            foreach (bool virtualMode in new bool[] { true, false })
            {
                foreach (View view in new View[] { View.Details, View.LargeIcon, View.List, View.SmallIcon })
                {
                    yield return new object[] { autoArrange, virtualMode, view, null };
                    yield return new object[] { autoArrange, virtualMode, view, new ImageList() };
                    yield return new object[] { autoArrange, virtualMode, view, CreateImageListNonEmpty() };
                }
            }

            yield return new object[] { autoArrange, false, View.Tile, null };
            yield return new object[] { autoArrange, false, View.Tile, new ImageList() };
            yield return new object[] { autoArrange, false, View.Tile, CreateImageListNonEmpty() };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(SmallImageList_Set_GetReturnsExpected))]
    public void ListView_SmallImageList_Set_GetReturnsExpected(bool autoArrange, bool virtualMode, View view, ImageList value)
    {
        using ListView listView = new()
        {
            AutoArrange = autoArrange,
            VirtualMode = virtualMode,
            View = view,
            SmallImageList = value
        };
        Assert.Same(value, listView.SmallImageList);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.SmallImageList = value;
        Assert.Same(value, listView.SmallImageList);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SmallImageList_Set_GetReturnsExpected))]
    public void ListView_SmallImageList_SetWithNonNullOldValue_GetReturnsExpected(bool autoArrange, bool virtualMode, View view, ImageList value)
    {
        using ImageList imageList = new();
        using ListView listView = new()
        {
            AutoArrange = autoArrange,
            VirtualMode = virtualMode,
            View = view,
            SmallImageList = imageList
        };

        listView.SmallImageList = value;
        Assert.Same(value, listView.SmallImageList);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.SmallImageList = value;
        Assert.Same(value, listView.SmallImageList);
        Assert.False(listView.IsHandleCreated);
    }

    public static IEnumerable<object[]> SmallImageList_SetWithHandle_GetReturnsExpected()
    {
        yield return new object[] { true, false, View.Details, null, 0, 0 };
        yield return new object[] { true, false, View.Details, new ImageList(), 1, 0 };
        yield return new object[] { true, false, View.Details, CreateImageListNonEmpty(), 1, 0 };
        yield return new object[] { true, false, View.LargeIcon, null, 0, 0 };
        yield return new object[] { true, false, View.LargeIcon, new ImageList(), 1, 0 };
        yield return new object[] { true, false, View.LargeIcon, CreateImageListNonEmpty(), 1, 0 };
        yield return new object[] { true, false, View.List, null, 0, 0 };
        yield return new object[] { true, false, View.List, new ImageList(), 0, 0 };
        yield return new object[] { true, false, View.List, CreateImageListNonEmpty(), 0, 0 };
        yield return new object[] { true, false, View.Tile, null, 0, 0 };
        yield return new object[] { true, false, View.Tile, new ImageList(), 0, 0 };
        yield return new object[] { true, false, View.Tile, CreateImageListNonEmpty(), 0, 0 };

        foreach (bool autoArrange in new bool[] { true, false })
        {
            yield return new object[] { autoArrange, true, View.Details, null, 0, 0 };
            yield return new object[] { autoArrange, true, View.Details, new ImageList(), 1, 0 };
            yield return new object[] { autoArrange, true, View.Details, CreateImageListNonEmpty(), 1, 0 };
            yield return new object[] { autoArrange, true, View.LargeIcon, null, 0, 0 };
            yield return new object[] { autoArrange, true, View.LargeIcon, new ImageList(), 0, 0 };
            yield return new object[] { autoArrange, true, View.LargeIcon, CreateImageListNonEmpty(), 0, 0 };
            yield return new object[] { autoArrange, true, View.List, null, 0, 0 };
            yield return new object[] { autoArrange, true, View.List, new ImageList(), 0, 0 };
            yield return new object[] { autoArrange, true, View.List, CreateImageListNonEmpty(), 0, 0 };
        }

        yield return new object[] { false, false, View.Details, null, 0, 0 };
        yield return new object[] { false, false, View.Details, new ImageList(), 1, 0 };
        yield return new object[] { false, false, View.Details, CreateImageListNonEmpty(), 1, 0 };
        yield return new object[] { false, false, View.LargeIcon, null, 0, 0 };
        yield return new object[] { false, false, View.LargeIcon, new ImageList(), 0, 0 };
        yield return new object[] { false, false, View.LargeIcon, CreateImageListNonEmpty(), 0, 0 };
        yield return new object[] { false, false, View.List, null, 0, 0 };
        yield return new object[] { false, false, View.List, new ImageList(), 0, 0 };
        yield return new object[] { false, false, View.List, CreateImageListNonEmpty(), 0, 0 };
        yield return new object[] { false, false, View.Tile, null, 0, 0 };
        yield return new object[] { false, false, View.Tile, new ImageList(), 0, 0 };
        yield return new object[] { false, false, View.Tile, CreateImageListNonEmpty(), 0, 0 };

        if (Application.UseVisualStyles)
        {
            yield return new object[] { true, false, View.SmallIcon, null, 0, 0 };
            yield return new object[] { true, false, View.SmallIcon, new ImageList(), 2, 0 };
            yield return new object[] { true, false, View.SmallIcon, CreateImageListNonEmpty(), 2, 0 };

            foreach (bool autoArrange in new bool[] { true, false })
            {
                yield return new object[] { autoArrange, true, View.SmallIcon, null, 0, 0 };
                yield return new object[] { autoArrange, true, View.SmallIcon, new ImageList(), 0, 0 };
                yield return new object[] { autoArrange, true, View.SmallIcon, CreateImageListNonEmpty(), 0, 0 };
            }

            yield return new object[] { false, false, View.SmallIcon, null, 0, 0 };
            yield return new object[] { false, false, View.SmallIcon, new ImageList(), 0, 0 };
            yield return new object[] { false, false, View.SmallIcon, CreateImageListNonEmpty(), 0, 0 };
        }
        else
        {
            yield return new object[] { true, false, View.SmallIcon, null, 0, 0 };
            yield return new object[] { true, false, View.SmallIcon, new ImageList(), 4, 2 };
            yield return new object[] { true, false, View.SmallIcon, CreateImageListNonEmpty(), 4, 2 };

            foreach (bool autoArrange in new bool[] { true, false })
            {
                yield return new object[] { autoArrange, true, View.SmallIcon, null, 0, 0 };
                yield return new object[] { autoArrange, true, View.SmallIcon, new ImageList(), 2, 2 };
                yield return new object[] { autoArrange, true, View.SmallIcon, CreateImageListNonEmpty(), 2, 2 };
            }

            yield return new object[] { false, false, View.SmallIcon, null, 0, 0 };
            yield return new object[] { false, false, View.SmallIcon, new ImageList(), 2, 2 };
            yield return new object[] { false, false, View.SmallIcon, CreateImageListNonEmpty(), 2, 2 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(SmallImageList_SetWithHandle_GetReturnsExpected))]
    public void ListView_SmallImageList_SetWithHandle_GetReturnsExpected(bool autoArrange, bool virtualMode, View view, ImageList value, int expectedInvalidatedCallCount, int expectedStyleChangedCallCount)
    {
        using ListView listView = new()
        {
            AutoArrange = autoArrange,
            VirtualMode = virtualMode,
            View = view
        };
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.SmallImageList = value;
        Assert.Same(value, listView.SmallImageList);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        listView.SmallImageList = value;
        Assert.Same(value, listView.SmallImageList);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> SmallImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected()
    {
        yield return new object[] { true, false, View.Details, null, 1, 0 };
        yield return new object[] { true, false, View.Details, new ImageList(), 1, 0 };
        yield return new object[] { true, false, View.Details, CreateImageListNonEmpty(), 1, 0 };
        yield return new object[] { true, false, View.LargeIcon, null, 1, 0 };
        yield return new object[] { true, false, View.LargeIcon, new ImageList(), 1, 0 };
        yield return new object[] { true, false, View.LargeIcon, CreateImageListNonEmpty(), 1, 0 };
        yield return new object[] { true, false, View.List, null, 0, 0 };
        yield return new object[] { true, false, View.List, new ImageList(), 0, 0 };
        yield return new object[] { true, false, View.List, CreateImageListNonEmpty(), 0, 0 };
        yield return new object[] { true, false, View.Tile, null, 0, 0 };
        yield return new object[] { true, false, View.Tile, new ImageList(), 0, 0 };
        yield return new object[] { true, false, View.Tile, CreateImageListNonEmpty(), 0, 0 };

        foreach (bool autoArrange in new bool[] { true, false })
        {
            yield return new object[] { autoArrange, true, View.Details, null, 1, 0 };
            yield return new object[] { autoArrange, true, View.Details, new ImageList(), 1, 0 };
            yield return new object[] { autoArrange, true, View.Details, CreateImageListNonEmpty(), 1, 0 };
            yield return new object[] { autoArrange, true, View.LargeIcon, null, 0, 0 };
            yield return new object[] { autoArrange, true, View.LargeIcon, new ImageList(), 0, 0 };
            yield return new object[] { autoArrange, true, View.LargeIcon, CreateImageListNonEmpty(), 0, 0 };
            yield return new object[] { autoArrange, true, View.List, null, 0, 0 };
            yield return new object[] { autoArrange, true, View.List, new ImageList(), 0, 0 };
            yield return new object[] { autoArrange, true, View.List, CreateImageListNonEmpty(), 0, 0 };
        }

        yield return new object[] { false, false, View.Details, null, 1, 0 };
        yield return new object[] { false, false, View.Details, new ImageList(), 1, 0 };
        yield return new object[] { false, false, View.Details, CreateImageListNonEmpty(), 1, 0 };
        yield return new object[] { false, false, View.LargeIcon, null, 0, 0 };
        yield return new object[] { false, false, View.LargeIcon, new ImageList(), 0, 0 };
        yield return new object[] { false, false, View.LargeIcon, CreateImageListNonEmpty(), 0, 0 };
        yield return new object[] { false, false, View.List, null, 0, 0 };
        yield return new object[] { false, false, View.List, new ImageList(), 0, 0 };
        yield return new object[] { false, false, View.List, CreateImageListNonEmpty(), 0, 0 };
        yield return new object[] { false, false, View.Tile, null, 0, 0 };
        yield return new object[] { false, false, View.Tile, new ImageList(), 0, 0 };
        yield return new object[] { false, false, View.Tile, CreateImageListNonEmpty(), 0, 0 };

        if (Application.UseVisualStyles)
        {
            yield return new object[] { true, false, View.SmallIcon, null, 2, 0 };
            yield return new object[] { true, false, View.SmallIcon, new ImageList(), 2, 0 };
            yield return new object[] { true, false, View.SmallIcon, CreateImageListNonEmpty(), 2, 0 };

            foreach (bool autoArrange in new bool[] { true, false })
            {
                yield return new object[] { autoArrange, true, View.SmallIcon, null, 0, 0 };
                yield return new object[] { autoArrange, true, View.SmallIcon, new ImageList(), 0, 0 };
                yield return new object[] { autoArrange, true, View.SmallIcon, CreateImageListNonEmpty(), 0, 0 };
            }

            yield return new object[] { false, false, View.SmallIcon, null, 0, 0 };
            yield return new object[] { false, false, View.SmallIcon, new ImageList(), 0, 0 };
            yield return new object[] { false, false, View.SmallIcon, CreateImageListNonEmpty(), 0, 0 };
        }
        else
        {
            yield return new object[] { true, false, View.SmallIcon, null, 4, 2 };
            yield return new object[] { true, false, View.SmallIcon, new ImageList(), 4, 2 };
            yield return new object[] { true, false, View.SmallIcon, CreateImageListNonEmpty(), 4, 2 };

            foreach (bool autoArrange in new bool[] { true, false })
            {
                yield return new object[] { autoArrange, true, View.SmallIcon, null, 2, 2 };
                yield return new object[] { autoArrange, true, View.SmallIcon, new ImageList(), 2, 2 };
                yield return new object[] { autoArrange, true, View.SmallIcon, CreateImageListNonEmpty(), 2, 2 };
            }

            yield return new object[] { false, false, View.SmallIcon, null, 2, 2 };
            yield return new object[] { false, false, View.SmallIcon, new ImageList(), 2, 2 };
            yield return new object[] { false, false, View.SmallIcon, CreateImageListNonEmpty(), 2, 2 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(SmallImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected))]
    public void ListView_SmallImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected(bool autoArrange, bool virtualMode, View view, ImageList value, int expectedInvalidatedCallCount, int expectedStyleChangedCallCount)
    {
        using ImageList imageList = new();
        using ListView listView = new()
        {
            AutoArrange = autoArrange,
            VirtualMode = virtualMode,
            View = view,
            SmallImageList = imageList
        };
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.SmallImageList = value;
        Assert.Same(value, listView.SmallImageList);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        listView.SmallImageList = value;
        Assert.Same(value, listView.SmallImageList);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_SmallImageList_Dispose_DetachesFromListView(bool autoArrange)
    {
        using ImageList imageList1 = new();
        using ImageList imageList2 = new();
        using ListView listView = new()
        {
            AutoArrange = autoArrange,
            SmallImageList = imageList1
        };
        Assert.Same(imageList1, listView.SmallImageList);

        imageList1.Dispose();
        Assert.Null(listView.SmallImageList);
        Assert.False(listView.IsHandleCreated);

        // Make sure we detached the setter.
        listView.SmallImageList = imageList2;
        imageList1.Dispose();
        Assert.Same(imageList2, listView.SmallImageList);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void ListView_SmallImageList_DisposeWithHandle_DetachesFromListView(bool autoArrange, int expectedInvalidatedCallCount)
    {
        using ImageList imageList1 = new();
        using ImageList imageList2 = new();
        using ListView listView = new()
        {
            AutoArrange = autoArrange
        };
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.SmallImageList = imageList1;
        Assert.Same(imageList1, listView.SmallImageList);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        imageList1.Dispose();
        Assert.Null(listView.SmallImageList);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Make sure we detached the setter.
        listView.SmallImageList = imageList2;
        imageList1.Dispose();
        Assert.Same(imageList2, listView.SmallImageList);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount * 3, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> StateImageList_Set_GetReturnsExpected()
    {
        foreach (bool useCompatibleStateImageBehavior in new bool[] { true, false })
        {
            foreach (bool checkBoxes in new bool[] { true, false })
            {
                foreach (bool autoArrange in new bool[] { true, false })
                {
                    foreach (bool virtualMode in new bool[] { true, false })
                    {
                        foreach (View view in new View[] { View.Details, View.LargeIcon, View.List, View.SmallIcon })
                        {
                            yield return new object[] { useCompatibleStateImageBehavior, checkBoxes, autoArrange, virtualMode, view, null };
                            yield return new object[] { useCompatibleStateImageBehavior, checkBoxes, autoArrange, virtualMode, view, new ImageList() };
                            yield return new object[] { useCompatibleStateImageBehavior, checkBoxes, autoArrange, virtualMode, view, CreateImageListNonEmpty() };
                        }
                    }
                }
            }

            yield return new object[] { useCompatibleStateImageBehavior, false, true, false, View.Tile, null };
            yield return new object[] { useCompatibleStateImageBehavior, false, true, false, View.Tile, new ImageList() };
            yield return new object[] { useCompatibleStateImageBehavior, false, true, false, View.Tile, CreateImageListNonEmpty() };

            yield return new object[] { useCompatibleStateImageBehavior, false, false, false, View.Tile, null };
            yield return new object[] { useCompatibleStateImageBehavior, false, false, false, View.Tile, new ImageList() };
            yield return new object[] { useCompatibleStateImageBehavior, false, false, false, View.Tile, CreateImageListNonEmpty() };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(StateImageList_Set_GetReturnsExpected))]
    public void ListView_StateImageList_Set_GetReturnsExpected(bool useCompatibleStateImageBehavior, bool checkBoxes, bool autoArrange, bool virtualMode, View view, ImageList value)
    {
        using ListView listView = new()
        {
            UseCompatibleStateImageBehavior = useCompatibleStateImageBehavior,
            CheckBoxes = checkBoxes,
            AutoArrange = autoArrange,
            VirtualMode = virtualMode,
            View = view,
            StateImageList = value
        };
        Assert.Same(value, listView.StateImageList);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.StateImageList = value;
        Assert.Same(value, listView.StateImageList);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(StateImageList_Set_GetReturnsExpected))]
    public void ListView_StateImageList_SetWithNonNullOldValue_GetReturnsExpected(bool useCompatibleStateImageBehavior, bool checkBoxes, bool autoArrange, bool virtualMode, View view, ImageList value)
    {
        using ImageList imageList = new();
        using ListView listView = new()
        {
            UseCompatibleStateImageBehavior = useCompatibleStateImageBehavior,
            CheckBoxes = checkBoxes,
            AutoArrange = autoArrange,
            VirtualMode = virtualMode,
            View = view,
            StateImageList = imageList
        };

        listView.StateImageList = value;
        Assert.Same(value, listView.StateImageList);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.StateImageList = value;
        Assert.Same(value, listView.StateImageList);
        Assert.False(listView.IsHandleCreated);
    }

    public static IEnumerable<object[]> StateImageList_SetWithHandle_GetReturnsExpected()
    {
        // UseCompatibleStateImageBehavior true
        foreach (bool checkBoxes in new bool[] { true, false })
        {
            yield return new object[] { true, checkBoxes, true, false, View.Details, null, 0, 0 };
            yield return new object[] { true, checkBoxes, true, false, View.Details, new ImageList(), 0, 0 };
            yield return new object[] { true, checkBoxes, true, false, View.Details, CreateImageListNonEmpty(), 0, 0 };
            yield return new object[] { true, checkBoxes, true, false, View.LargeIcon, null, 0, 0 };
            yield return new object[] { true, checkBoxes, true, false, View.LargeIcon, new ImageList(), 0, 0 };
            yield return new object[] { true, checkBoxes, true, false, View.LargeIcon, CreateImageListNonEmpty(), 0, 0 };
            yield return new object[] { true, checkBoxes, true, false, View.List, null, 0, 0 };
            yield return new object[] { true, checkBoxes, true, false, View.List, new ImageList(), 0, 0 };
            yield return new object[] { true, checkBoxes, true, false, View.List, CreateImageListNonEmpty(), 0, 0 };
            yield return new object[] { true, checkBoxes, true, false, View.SmallIcon, null, 0, 0 };
            yield return new object[] { true, checkBoxes, true, false, View.SmallIcon, new ImageList(), 0, 0 };
            yield return new object[] { true, checkBoxes, true, false, View.SmallIcon, CreateImageListNonEmpty(), 0, 0 };

            foreach (bool autoArrange in new bool[] { true, false })
            {
                yield return new object[] { true, checkBoxes, autoArrange, true, View.Details, null, 0, 0 };
                yield return new object[] { true, checkBoxes, autoArrange, true, View.Details, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, autoArrange, true, View.Details, CreateImageListNonEmpty(), 0, 0 };
                yield return new object[] { true, checkBoxes, autoArrange, true, View.LargeIcon, null, 0, 0 };
                yield return new object[] { true, checkBoxes, autoArrange, true, View.LargeIcon, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, autoArrange, true, View.LargeIcon, CreateImageListNonEmpty(), 0, 0 };
                yield return new object[] { true, checkBoxes, autoArrange, true, View.List, null, 0, 0 };
                yield return new object[] { true, checkBoxes, autoArrange, true, View.List, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, autoArrange, true, View.List, CreateImageListNonEmpty(), 0, 0 };
                yield return new object[] { true, checkBoxes, autoArrange, true, View.SmallIcon, null, 0, 0 };
                yield return new object[] { true, checkBoxes, autoArrange, true, View.SmallIcon, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, autoArrange, true, View.SmallIcon, CreateImageListNonEmpty(), 0, 0 };
            }

            yield return new object[] { true, checkBoxes, false, false, View.Details, null, 0, 0 };
            yield return new object[] { true, checkBoxes, false, false, View.Details, new ImageList(), 0, 0 };
            yield return new object[] { true, checkBoxes, false, false, View.Details, CreateImageListNonEmpty(), 0, 0 };
            yield return new object[] { true, checkBoxes, false, false, View.LargeIcon, null, 0, 0 };
            yield return new object[] { true, checkBoxes, false, false, View.LargeIcon, new ImageList(), 0, 0 };
            yield return new object[] { true, checkBoxes, false, false, View.LargeIcon, CreateImageListNonEmpty(), 0, 0 };
            yield return new object[] { true, checkBoxes, false, false, View.List, null, 0, 0 };
            yield return new object[] { true, checkBoxes, false, false, View.List, new ImageList(), 0, 0 };
            yield return new object[] { true, checkBoxes, false, false, View.List, CreateImageListNonEmpty(), 0, 0 };
            yield return new object[] { true, checkBoxes, false, false, View.SmallIcon, null, 0, 0 };
            yield return new object[] { true, checkBoxes, false, false, View.SmallIcon, new ImageList(), 0, 0 };
            yield return new object[] { true, checkBoxes, false, false, View.SmallIcon, CreateImageListNonEmpty(), 0, 0 };
        }

        yield return new object[] { true, false, true, false, View.Tile, null, 0, 0 };
        yield return new object[] { true, false, true, false, View.Tile, new ImageList(), 0, 0 };
        yield return new object[] { true, false, true, false, View.Tile, CreateImageListNonEmpty(), 0, 0 };

        yield return new object[] { true, false, false, false, View.Tile, null, 0, 0 };
        yield return new object[] { true, false, false, false, View.Tile, new ImageList(), 0, 0 };
        yield return new object[] { true, false, false, false, View.Tile, CreateImageListNonEmpty(), 0, 0 };

        // UseCompatibleStateImageBehavior false, CheckBoxes true
        yield return new object[] { false, true, true, false, View.Details, null, 0, 0 };
        yield return new object[] { false, true, true, false, View.Details, new ImageList(), 1, 1 };
        yield return new object[] { false, true, true, false, View.Details, CreateImageListNonEmpty(), 1, 1 };
        yield return new object[] { false, true, true, false, View.LargeIcon, null, 0, 0 };
        yield return new object[] { false, true, true, false, View.LargeIcon, new ImageList(), 3, 1 };
        yield return new object[] { false, true, true, false, View.LargeIcon, CreateImageListNonEmpty(), 3, 1 };
        yield return new object[] { false, true, true, false, View.List, null, 0, 0 };
        yield return new object[] { false, true, true, false, View.List, new ImageList(), 1, 1 };
        yield return new object[] { false, true, true, false, View.List, CreateImageListNonEmpty(), 1, 1 };
        yield return new object[] { false, true, true, false, View.SmallIcon, null, 0, 0 };
        yield return new object[] { false, true, true, false, View.SmallIcon, new ImageList(), 3, 1 };
        yield return new object[] { false, true, true, false, View.SmallIcon, CreateImageListNonEmpty(), 3, 1 };

        foreach (bool autoArrange in new bool[] { true, false })
        {
            yield return new object[] { false, true, autoArrange, true, View.Details, null, 0, 0 };
            yield return new object[] { false, true, autoArrange, true, View.Details, new ImageList(), 1, 1 };
            yield return new object[] { false, true, autoArrange, true, View.Details, CreateImageListNonEmpty(), 1, 1 };
            yield return new object[] { false, true, autoArrange, true, View.LargeIcon, null, 0, 0 };
            yield return new object[] { false, true, autoArrange, true, View.LargeIcon, new ImageList(), 1, 1 };
            yield return new object[] { false, true, autoArrange, true, View.LargeIcon, CreateImageListNonEmpty(), 1, 1 };
            yield return new object[] { false, true, autoArrange, true, View.List, null, 0, 0 };
            yield return new object[] { false, true, autoArrange, true, View.List, new ImageList(), 1, 1 };
            yield return new object[] { false, true, autoArrange, true, View.List, CreateImageListNonEmpty(), 1, 1 };
            yield return new object[] { false, true, autoArrange, true, View.SmallIcon, null, 0, 0 };
            yield return new object[] { false, true, autoArrange, true, View.SmallIcon, new ImageList(), 1, 1 };
            yield return new object[] { false, true, autoArrange, true, View.SmallIcon, CreateImageListNonEmpty(), 1, 1 };
        }

        yield return new object[] { false, true, false, false, View.Details, null, 0, 0 };
        yield return new object[] { false, true, false, false, View.Details, new ImageList(), 1, 1 };
        yield return new object[] { false, true, false, false, View.Details, CreateImageListNonEmpty(), 1, 1 };
        yield return new object[] { false, true, false, false, View.LargeIcon, null, 0, 0 };
        yield return new object[] { false, true, false, false, View.LargeIcon, new ImageList(), 1, 1 };
        yield return new object[] { false, true, false, false, View.LargeIcon, CreateImageListNonEmpty(), 1, 1 };
        yield return new object[] { false, true, false, false, View.List, null, 0, 0 };
        yield return new object[] { false, true, false, false, View.List, new ImageList(), 1, 1 };
        yield return new object[] { false, true, false, false, View.List, CreateImageListNonEmpty(), 1, 1 };
        yield return new object[] { false, true, false, false, View.SmallIcon, null, 0, 0 };
        yield return new object[] { false, true, false, false, View.SmallIcon, new ImageList(), 1, 1 };
        yield return new object[] { false, true, false, false, View.SmallIcon, CreateImageListNonEmpty(), 1, 1 };

        // UseCompatibleStateImageBehavior false, CheckBoxes false
        yield return new object[] { false, false, true, false, View.Details, null, 0, 0 };
        yield return new object[] { false, false, true, false, View.Details, new ImageList(), 0, 0 };
        yield return new object[] { false, false, true, false, View.Details, CreateImageListNonEmpty(), 0, 0 };
        yield return new object[] { false, false, true, false, View.LargeIcon, null, 0, 0 };
        yield return new object[] { false, false, true, false, View.LargeIcon, new ImageList(), 1, 0 };
        yield return new object[] { false, false, true, false, View.LargeIcon, CreateImageListNonEmpty(), 1, 0 };
        yield return new object[] { false, false, true, false, View.List, null, 0, 0 };
        yield return new object[] { false, false, true, false, View.List, new ImageList(), 0, 0 };
        yield return new object[] { false, false, true, false, View.List, CreateImageListNonEmpty(), 0, 0 };
        yield return new object[] { false, false, true, false, View.SmallIcon, null, 0, 0 };
        yield return new object[] { false, false, true, false, View.SmallIcon, new ImageList(), 1, 0 };
        yield return new object[] { false, false, true, false, View.SmallIcon, CreateImageListNonEmpty(), 1, 0 };
        yield return new object[] { false, false, true, false, View.Tile, null, 0, 0 };
        yield return new object[] { false, false, true, false, View.Tile, new ImageList(), 0, 0 };
        yield return new object[] { false, false, true, false, View.Tile, CreateImageListNonEmpty(), 0, 0 };

        foreach (bool autoArrange in new bool[] { true, false })
        {
            yield return new object[] { false, false, autoArrange, true, View.Details, null, 0, 0 };
            yield return new object[] { false, false, autoArrange, true, View.Details, new ImageList(), 0, 0 };
            yield return new object[] { false, false, autoArrange, true, View.Details, CreateImageListNonEmpty(), 0, 0 };
            yield return new object[] { false, false, autoArrange, true, View.LargeIcon, null, 0, 0 };
            yield return new object[] { false, false, autoArrange, true, View.LargeIcon, new ImageList(), 0, 0 };
            yield return new object[] { false, false, autoArrange, true, View.LargeIcon, CreateImageListNonEmpty(), 0, 0 };
            yield return new object[] { false, false, autoArrange, true, View.List, null, 0, 0 };
            yield return new object[] { false, false, autoArrange, true, View.List, new ImageList(), 0, 0 };
            yield return new object[] { false, false, autoArrange, true, View.List, CreateImageListNonEmpty(), 0, 0 };
            yield return new object[] { false, false, autoArrange, true, View.SmallIcon, null, 0, 0 };
            yield return new object[] { false, false, autoArrange, true, View.SmallIcon, new ImageList(), 0, 0 };
            yield return new object[] { false, false, autoArrange, true, View.SmallIcon, CreateImageListNonEmpty(), 0, 0 };
        }

        yield return new object[] { false, false, false, false, View.Details, null, 0, 0 };
        yield return new object[] { false, false, false, false, View.Details, new ImageList(), 0, 0 };
        yield return new object[] { false, false, false, false, View.Details, CreateImageListNonEmpty(), 0, 0 };
        yield return new object[] { false, false, false, false, View.LargeIcon, null, 0, 0 };
        yield return new object[] { false, false, false, false, View.LargeIcon, new ImageList(), 0, 0 };
        yield return new object[] { false, false, false, false, View.LargeIcon, CreateImageListNonEmpty(), 0, 0 };
        yield return new object[] { false, false, false, false, View.List, null, 0, 0 };
        yield return new object[] { false, false, false, false, View.List, new ImageList(), 0, 0 };
        yield return new object[] { false, false, false, false, View.List, CreateImageListNonEmpty(), 0, 0 };
        yield return new object[] { false, false, false, false, View.SmallIcon, null, 0, 0 };
        yield return new object[] { false, false, false, false, View.SmallIcon, new ImageList(), 0, 0 };
        yield return new object[] { false, false, false, false, View.SmallIcon, CreateImageListNonEmpty(), 0, 0 };
        yield return new object[] { false, false, false, false, View.Tile, null, 0, 0 };
        yield return new object[] { false, false, false, false, View.Tile, new ImageList(), 0, 0 };
        yield return new object[] { false, false, false, false, View.Tile, CreateImageListNonEmpty(), 0, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(StateImageList_SetWithHandle_GetReturnsExpected))]
    public void ListView_StateImageList_SetWithHandle_GetReturnsExpected(bool useCompatibleStateImageBehavior, bool checkBoxes, bool autoArrange, bool virtualMode, View view, ImageList value, int expectedInvalidatedCallCount, int expectedCreatedCallCount)
    {
        using ListView listView = new()
        {
            UseCompatibleStateImageBehavior = useCompatibleStateImageBehavior,
            CheckBoxes = checkBoxes,
            AutoArrange = autoArrange,
            VirtualMode = virtualMode,
            View = view
        };
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.StateImageList = value;
        Assert.Same(value, listView.StateImageList);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        listView.StateImageList = value;
        Assert.Same(value, listView.StateImageList);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    public static IEnumerable<object[]> StateImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected()
    {
        // UseCompatibleStateImageBehavior true
        foreach (bool checkBoxes in new bool[] { true, false })
        {
            yield return new object[] { true, checkBoxes, true, false, View.Details, null, 0, 0 };
            yield return new object[] { true, checkBoxes, true, false, View.Details, new ImageList(), 0, 0 };
            yield return new object[] { true, checkBoxes, true, false, View.Details, CreateImageListNonEmpty(), 0, 0 };
            yield return new object[] { true, checkBoxes, true, false, View.LargeIcon, null, 0, 0 };
            yield return new object[] { true, checkBoxes, true, false, View.LargeIcon, new ImageList(), 0, 0 };
            yield return new object[] { true, checkBoxes, true, false, View.LargeIcon, CreateImageListNonEmpty(), 0, 0 };
            yield return new object[] { true, checkBoxes, true, false, View.List, null, 0, 0 };
            yield return new object[] { true, checkBoxes, true, false, View.List, new ImageList(), 0, 0 };
            yield return new object[] { true, checkBoxes, true, false, View.List, CreateImageListNonEmpty(), 0, 0 };
            yield return new object[] { true, checkBoxes, true, false, View.SmallIcon, null, 0, 0 };
            yield return new object[] { true, checkBoxes, true, false, View.SmallIcon, new ImageList(), 0, 0 };
            yield return new object[] { true, checkBoxes, true, false, View.SmallIcon, CreateImageListNonEmpty(), 0, 0 };

            foreach (bool autoArrange in new bool[] { true, false })
            {
                yield return new object[] { true, checkBoxes, autoArrange, true, View.Details, null, 0, 0 };
                yield return new object[] { true, checkBoxes, autoArrange, true, View.Details, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, autoArrange, true, View.Details, CreateImageListNonEmpty(), 0, 0 };
                yield return new object[] { true, checkBoxes, autoArrange, true, View.LargeIcon, null, 0, 0 };
                yield return new object[] { true, checkBoxes, autoArrange, true, View.LargeIcon, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, autoArrange, true, View.LargeIcon, CreateImageListNonEmpty(), 0, 0 };
                yield return new object[] { true, checkBoxes, autoArrange, true, View.List, null, 0, 0 };
                yield return new object[] { true, checkBoxes, autoArrange, true, View.List, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, autoArrange, true, View.List, CreateImageListNonEmpty(), 0, 0 };
                yield return new object[] { true, checkBoxes, autoArrange, true, View.SmallIcon, null, 0, 0 };
                yield return new object[] { true, checkBoxes, autoArrange, true, View.SmallIcon, new ImageList(), 0, 0 };
                yield return new object[] { true, checkBoxes, autoArrange, true, View.SmallIcon, CreateImageListNonEmpty(), 0, 0 };
            }

            yield return new object[] { true, checkBoxes, false, false, View.Details, null, 0, 0 };
            yield return new object[] { true, checkBoxes, false, false, View.Details, new ImageList(), 0, 0 };
            yield return new object[] { true, checkBoxes, false, false, View.Details, CreateImageListNonEmpty(), 0, 0 };
            yield return new object[] { true, checkBoxes, false, false, View.LargeIcon, null, 0, 0 };
            yield return new object[] { true, checkBoxes, false, false, View.LargeIcon, new ImageList(), 0, 0 };
            yield return new object[] { true, checkBoxes, false, false, View.LargeIcon, CreateImageListNonEmpty(), 0, 0 };
            yield return new object[] { true, checkBoxes, false, false, View.List, null, 0, 0 };
            yield return new object[] { true, checkBoxes, false, false, View.List, new ImageList(), 0, 0 };
            yield return new object[] { true, checkBoxes, false, false, View.List, CreateImageListNonEmpty(), 0, 0 };
            yield return new object[] { true, checkBoxes, false, false, View.SmallIcon, null, 0, 0 };
            yield return new object[] { true, checkBoxes, false, false, View.SmallIcon, new ImageList(), 0, 0 };
            yield return new object[] { true, checkBoxes, false, false, View.SmallIcon, CreateImageListNonEmpty(), 0, 0 };
        }

        yield return new object[] { true, false, true, false, View.Tile, null, 0, 0 };
        yield return new object[] { true, false, true, false, View.Tile, new ImageList(), 0, 0 };
        yield return new object[] { true, false, true, false, View.Tile, CreateImageListNonEmpty(), 0, 0 };

        yield return new object[] { true, false, false, false, View.Tile, null, 0, 0 };
        yield return new object[] { true, false, false, false, View.Tile, new ImageList(), 0, 0 };
        yield return new object[] { true, false, false, false, View.Tile, CreateImageListNonEmpty(), 0, 0 };

        // UseCompatibleStateImageBehavior false, CheckBoxes true
        yield return new object[] { false, true, true, false, View.Details, null, 1, 1 };
        yield return new object[] { false, true, true, false, View.Details, new ImageList(), 1, 1 };
        yield return new object[] { false, true, true, false, View.Details, CreateImageListNonEmpty(), 1, 1 };
        yield return new object[] { false, true, true, false, View.LargeIcon, null, 3, 1 };
        yield return new object[] { false, true, true, false, View.LargeIcon, new ImageList(), 3, 1 };
        yield return new object[] { false, true, true, false, View.LargeIcon, CreateImageListNonEmpty(), 3, 1 };
        yield return new object[] { false, true, true, false, View.List, null, 1, 1 };
        yield return new object[] { false, true, true, false, View.List, new ImageList(), 1, 1 };
        yield return new object[] { false, true, true, false, View.List, CreateImageListNonEmpty(), 1, 1 };
        yield return new object[] { false, true, true, false, View.SmallIcon, null, 3, 1 };
        yield return new object[] { false, true, true, false, View.SmallIcon, new ImageList(), 3, 1 };
        yield return new object[] { false, true, true, false, View.SmallIcon, CreateImageListNonEmpty(), 3, 1 };

        foreach (bool autoArrange in new bool[] { true, false })
        {
            yield return new object[] { false, true, autoArrange, true, View.Details, null, 1, 1 };
            yield return new object[] { false, true, autoArrange, true, View.Details, new ImageList(), 1, 1 };
            yield return new object[] { false, true, autoArrange, true, View.Details, CreateImageListNonEmpty(), 1, 1 };
            yield return new object[] { false, true, autoArrange, true, View.LargeIcon, null, 1, 1 };
            yield return new object[] { false, true, autoArrange, true, View.LargeIcon, new ImageList(), 1, 1 };
            yield return new object[] { false, true, autoArrange, true, View.LargeIcon, CreateImageListNonEmpty(), 1, 1 };
            yield return new object[] { false, true, autoArrange, true, View.List, null, 1, 1 };
            yield return new object[] { false, true, autoArrange, true, View.List, new ImageList(), 1, 1 };
            yield return new object[] { false, true, autoArrange, true, View.List, CreateImageListNonEmpty(), 1, 1 };
            yield return new object[] { false, true, autoArrange, true, View.SmallIcon, null, 1, 1 };
            yield return new object[] { false, true, autoArrange, true, View.SmallIcon, new ImageList(), 1, 1 };
            yield return new object[] { false, true, autoArrange, true, View.SmallIcon, CreateImageListNonEmpty(), 1, 1 };
        }

        yield return new object[] { false, true, false, false, View.Details, null, 1, 1 };
        yield return new object[] { false, true, false, false, View.Details, new ImageList(), 1, 1 };
        yield return new object[] { false, true, false, false, View.Details, CreateImageListNonEmpty(), 1, 1 };
        yield return new object[] { false, true, false, false, View.LargeIcon, null, 1, 1 };
        yield return new object[] { false, true, false, false, View.LargeIcon, new ImageList(), 1, 1 };
        yield return new object[] { false, true, false, false, View.LargeIcon, CreateImageListNonEmpty(), 1, 1 };
        yield return new object[] { false, true, false, false, View.List, null, 1, 1 };
        yield return new object[] { false, true, false, false, View.List, new ImageList(), 1, 1 };
        yield return new object[] { false, true, false, false, View.List, CreateImageListNonEmpty(), 1, 1 };
        yield return new object[] { false, true, false, false, View.SmallIcon, null, 1, 1 };
        yield return new object[] { false, true, false, false, View.SmallIcon, new ImageList(), 1, 1 };
        yield return new object[] { false, true, false, false, View.SmallIcon, CreateImageListNonEmpty(), 1, 1 };

        // UseCompatibleStateImageBehavior false, CheckBoxes false
        yield return new object[] { false, false, true, false, View.Details, null, 0, 0 };
        yield return new object[] { false, false, true, false, View.Details, new ImageList(), 0, 0 };
        yield return new object[] { false, false, true, false, View.Details, CreateImageListNonEmpty(), 0, 0 };
        yield return new object[] { false, false, true, false, View.LargeIcon, null, 1, 0 };
        yield return new object[] { false, false, true, false, View.LargeIcon, new ImageList(), 1, 0 };
        yield return new object[] { false, false, true, false, View.LargeIcon, CreateImageListNonEmpty(), 1, 0 };
        yield return new object[] { false, false, true, false, View.List, null, 0, 0 };
        yield return new object[] { false, false, true, false, View.List, new ImageList(), 0, 0 };
        yield return new object[] { false, false, true, false, View.List, CreateImageListNonEmpty(), 0, 0 };
        yield return new object[] { false, false, true, false, View.SmallIcon, null, 1, 0 };
        yield return new object[] { false, false, true, false, View.SmallIcon, new ImageList(), 1, 0 };
        yield return new object[] { false, false, true, false, View.SmallIcon, CreateImageListNonEmpty(), 1, 0 };
        yield return new object[] { false, false, true, false, View.Tile, null, 0, 0 };
        yield return new object[] { false, false, true, false, View.Tile, new ImageList(), 0, 0 };
        yield return new object[] { false, false, true, false, View.Tile, CreateImageListNonEmpty(), 0, 0 };

        foreach (bool autoArrange in new bool[] { true, false })
        {
            yield return new object[] { false, false, autoArrange, true, View.Details, null, 0, 0 };
            yield return new object[] { false, false, autoArrange, true, View.Details, new ImageList(), 0, 0 };
            yield return new object[] { false, false, autoArrange, true, View.Details, CreateImageListNonEmpty(), 0, 0 };
            yield return new object[] { false, false, autoArrange, true, View.LargeIcon, null, 0, 0 };
            yield return new object[] { false, false, autoArrange, true, View.LargeIcon, new ImageList(), 0, 0 };
            yield return new object[] { false, false, autoArrange, true, View.LargeIcon, CreateImageListNonEmpty(), 0, 0 };
            yield return new object[] { false, false, autoArrange, true, View.List, null, 0, 0 };
            yield return new object[] { false, false, autoArrange, true, View.List, new ImageList(), 0, 0 };
            yield return new object[] { false, false, autoArrange, true, View.List, CreateImageListNonEmpty(), 0, 0 };
            yield return new object[] { false, false, autoArrange, true, View.SmallIcon, null, 0, 0 };
            yield return new object[] { false, false, autoArrange, true, View.SmallIcon, new ImageList(), 0, 0 };
            yield return new object[] { false, false, autoArrange, true, View.SmallIcon, CreateImageListNonEmpty(), 0, 0 };
        }

        yield return new object[] { false, false, false, false, View.Details, null, 0, 0 };
        yield return new object[] { false, false, false, false, View.Details, new ImageList(), 0, 0 };
        yield return new object[] { false, false, false, false, View.Details, CreateImageListNonEmpty(), 0, 0 };
        yield return new object[] { false, false, false, false, View.LargeIcon, null, 0, 0 };
        yield return new object[] { false, false, false, false, View.LargeIcon, new ImageList(), 0, 0 };
        yield return new object[] { false, false, false, false, View.LargeIcon, CreateImageListNonEmpty(), 0, 0 };
        yield return new object[] { false, false, false, false, View.List, null, 0, 0 };
        yield return new object[] { false, false, false, false, View.List, new ImageList(), 0, 0 };
        yield return new object[] { false, false, false, false, View.List, CreateImageListNonEmpty(), 0, 0 };
        yield return new object[] { false, false, false, false, View.SmallIcon, null, 0, 0 };
        yield return new object[] { false, false, false, false, View.SmallIcon, new ImageList(), 0, 0 };
        yield return new object[] { false, false, false, false, View.SmallIcon, CreateImageListNonEmpty(), 0, 0 };
        yield return new object[] { false, false, false, false, View.Tile, null, 0, 0 };
        yield return new object[] { false, false, false, false, View.Tile, new ImageList(), 0, 0 };
        yield return new object[] { false, false, false, false, View.Tile, CreateImageListNonEmpty(), 0, 0 };
    }

    [WinFormsTheory(Skip = "Leads to random AccessViolationException. See: https://github.com/dotnet/winforms/issues/3358")]
    [ActiveIssue("https://github.com/dotnet/winforms/issues/3358")]
    [MemberData(nameof(StateImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected))]
    public void ListView_StateImageList_SetWithHandleWithNonNullOldValue_GetReturnsExpected(bool useCompatibleStateImageBehavior, bool checkBoxes, bool autoArrange, bool virtualMode, View view, ImageList value, int expectedInvalidatedCallCount, int expectedCreatedCallCount)
    {
        using ImageList imageList = new();
        using ListView listView = new()
        {
            UseCompatibleStateImageBehavior = useCompatibleStateImageBehavior,
            CheckBoxes = checkBoxes,
            AutoArrange = autoArrange,
            VirtualMode = virtualMode,
            View = view,
        };

        listView.StateImageList = imageList;

        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.StateImageList = value;
        Assert.Same(value, listView.StateImageList);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        listView.StateImageList = value;
        Assert.Same(value, listView.StateImageList);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public void ListView_StateImageList_Dispose_DetachesFromListView(bool useCompatibleStateImageBehavior, bool autoArrange)
    {
        using ImageList imageList1 = new();
        using ImageList imageList2 = new();
        using ListView listView = new()
        {
            UseCompatibleStateImageBehavior = useCompatibleStateImageBehavior,
            AutoArrange = autoArrange,
            StateImageList = imageList1
        };
        Assert.Same(imageList1, listView.StateImageList);

        imageList1.Dispose();
        Assert.Null(listView.StateImageList);
        Assert.False(listView.IsHandleCreated);

        // Make sure we detached the setter.
        listView.StateImageList = imageList2;
        imageList1.Dispose();
        Assert.Same(imageList2, listView.StateImageList);
    }

    [WinFormsTheory]
    [InlineData(true, true, 0, 1, 1)]
    [InlineData(false, true, 1, 2, 3)]
    [InlineData(true, false, 0, 0, 0)]
    [InlineData(false, false, 0, 0, 0)]
    public void ListView_StateImageList_DisposeWithHandle_DetachesFromListView(bool useCompatibleStateImageBehavior, bool autoArrange, int expectedInvalidatedCallCount1, int expectedInvalidatedCallCount2, int expectedInvalidatedCallCount3)
    {
        using ImageList imageList1 = new();
        using ImageList imageList2 = new();
        using ListView listView = new()
        {
            UseCompatibleStateImageBehavior = useCompatibleStateImageBehavior,
            AutoArrange = autoArrange
        };
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.StateImageList = imageList1;
        Assert.Same(imageList1, listView.StateImageList);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        imageList1.Dispose();
        Assert.Null(listView.StateImageList);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Make sure we detached the setter.
        listView.StateImageList = imageList2;
        imageList1.Dispose();
        Assert.Same(imageList2, listView.StateImageList);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount3, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_UseCompatibleStateImageBehavior_Set_GetReturnsExpected(bool value)
    {
        using ListView listView = new()
        {
            UseCompatibleStateImageBehavior = value
        };
        Assert.Equal(value, listView.UseCompatibleStateImageBehavior);
        Assert.False(listView.IsHandleCreated);

        // Set same.
        listView.UseCompatibleStateImageBehavior = value;
        Assert.Equal(value, listView.UseCompatibleStateImageBehavior);
        Assert.False(listView.IsHandleCreated);

        // Set different.
        listView.UseCompatibleStateImageBehavior = !value;
        Assert.Equal(!value, listView.UseCompatibleStateImageBehavior);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListView_UseCompatibleStateImageBehavior_SetWithHandle_GetReturnsExpected(bool value)
    {
        using ListView listView = new();
        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        int invalidatedCallCount = 0;
        listView.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        listView.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        listView.HandleCreated += (sender, e) => createdCallCount++;

        listView.UseCompatibleStateImageBehavior = value;
        Assert.Equal(value, listView.UseCompatibleStateImageBehavior);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        listView.UseCompatibleStateImageBehavior = value;
        Assert.Equal(value, listView.UseCompatibleStateImageBehavior);
        Assert.True(listView.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ListView_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using SubListView control = new();
        Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
    }

    [WinFormsFact]
    public void ListView_GetItemRect_InvokeWithoutHandle_ReturnsExpectedAndCreatedHandle()
    {
        using ListView control = new();
        ListViewItem item1 = new();
        ListViewItem item2 = new();
        control.Items.Add(item1);
        control.Items.Add(item2);

        Rectangle rect1 = control.GetItemRect(0);
        Assert.True(rect1.X >= 0);
        Assert.True(rect1.Y >= 0);
        Assert.True(rect1.Width > 0);
        Assert.True(rect1.Height > 0);
        Assert.Equal(rect1, control.GetItemRect(0));
        Assert.True(control.IsHandleCreated);

        Rectangle rect2 = control.GetItemRect(1);
        Assert.True((rect2.X >= rect1.Right && rect2.Y == rect1.Y) || (rect2.X == rect1.X && rect2.Y >= rect1.Bottom));
        Assert.True(rect2.Width > 0);
        Assert.True(rect2.Height > 0);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListView_GetItemRect_InvokeWithHandle_ReturnsExpected()
    {
        using ListView control = new();
        ListViewItem item1 = new();
        ListViewItem item2 = new();
        control.Items.Add(item1);
        control.Items.Add(item2);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Rectangle rect1 = control.GetItemRect(0);
        Assert.True(rect1.X >= 0);
        Assert.True(rect1.Y >= 0);
        Assert.True(rect1.Width > 0);
        Assert.True(rect1.Height > 0);
        Assert.Equal(rect1, control.GetItemRect(0));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        Rectangle rect2 = control.GetItemRect(1);
        Assert.True((rect2.X >= rect1.Right && rect2.Y == rect1.Y) || (rect2.X == rect1.X && rect2.Y >= rect1.Bottom));
        Assert.True(rect2.Width > 0);
        Assert.True(rect2.Height > 0);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> GetItemRect_InvokeCustomGetItemRect_TestData()
    {
        yield return new object[] { default(RECT), Rectangle.Empty };
        yield return new object[] { new RECT(1, 2, 3, 4), new Rectangle(1, 2, 2, 2) };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetItemRect_InvokeCustomGetItemRect_TestData))]
    public void ListView_GetItemRect_InvokeCustomGetItemRect_ReturnsExpected(object getItemRectResult, Rectangle expected)
    {
        using CustomGetItemRectListView control = new()
        {
            GetItemRectResult = (RECT)getItemRectResult
        };
        ListViewItem item = new();
        control.Items.Add(item);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        Assert.Equal(expected, control.GetItemRect(0));
    }

    private class CustomGetItemRectListView : ListView
    {
        public RECT GetItemRectResult { get; set; }

        protected override unsafe void WndProc(ref Message m)
        {
            if (m.Msg == (int)PInvoke.LVM_GETITEMRECT)
            {
                RECT* pRect = (RECT*)m.LParam;
                *pRect = GetItemRectResult;
                m.Result = 1;
                return;
            }

            base.WndProc(ref m);
        }
    }

    [WinFormsFact]
    public void ListView_GetItemRect_InvokeInvalidGetItemRect_ThrowsArgumentOutOfRangeException()
    {
        using InvalidGetItemRectListView control = new();
        ListViewItem item = new();
        control.Items.Add(item);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.MakeInvalid = true;
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRect(0));
    }

    private class InvalidGetItemRectListView : ListView
    {
        public bool MakeInvalid { get; set; }

        protected override unsafe void WndProc(ref Message m)
        {
            if (MakeInvalid && m.Msg == (int)PInvoke.LVM_GETITEMRECT)
            {
                RECT* pRect = (RECT*)m.LParam;
                *pRect = new RECT(1, 2, 3, 4);
                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);
        }
    }

    [WinFormsFact]
    public void ListView_GetItemRect_InvalidIndexEmpty_ThrowsArgumentOutOfRangeException()
    {
        using ListView control = new();
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRect(-1));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRect(0));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRect(1));
    }

    [WinFormsFact]
    public void ListView_GetItemRect_InvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException()
    {
        using ListView control = new();
        ListViewItem item1 = new();
        control.Items.Add(item1);

        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRect(-1));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRect(1));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRect(2));
    }

    [WinFormsFact]
    public void ListView_GetItemRect_InvalidIndexWithHandleEmpty_ThrowsArgumentOutOfRangeException()
    {
        using ListView control = new();
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRect(-1));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRect(0));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRect(1));
    }

    [WinFormsFact]
    public void ListView_GetItemRect_InvalidIndexWithHandleNotEmpty_ThrowsArgumentOutOfRangeException()
    {
        using ListView control = new();
        ListViewItem item1 = new();
        control.Items.Add(item1);
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRect(-1));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRect(1));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => control.GetItemRect(2));
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
    [InlineData(ControlStyles.StandardDoubleClick, true)]
    [InlineData(ControlStyles.AllPaintingInWmPaint, true)]
    [InlineData(ControlStyles.CacheText, false)]
    [InlineData(ControlStyles.EnableNotifyMessage, false)]
    [InlineData(ControlStyles.DoubleBuffer, false)]
    [InlineData(ControlStyles.OptimizedDoubleBuffer, false)]
    [InlineData(ControlStyles.UseTextForAccessibility, false)]
    [InlineData((ControlStyles)0, true)]
    [InlineData((ControlStyles)int.MaxValue, false)]
    [InlineData((ControlStyles)(-1), false)]
    public void ListView_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
    {
        using SubListView control = new();
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    [WinFormsFact]
    public void ListView_GetTopLevel_Invoke_ReturnsExpected()
    {
        using SubListView control = new();
        Assert.False(control.GetTopLevel());
    }

    private static ImageList CreateImageListNonEmpty()
    {
        ImageList nonEmptyImageList = new();
        nonEmptyImageList.Images.Add(new Bitmap(10, 10));
        return nonEmptyImageList;
    }

    public static IEnumerable<object[]> ListView_InvokeOnSelectedIndexChanged_TestData()
    {
        foreach (View view in Enum.GetValues(typeof(View)))
        {
            foreach (bool showGroups in new[] { true, false })
            {
                foreach (bool focused in new[] { true, false })
                {
                    foreach (bool selected in new[] { true, false })
                    {
                        // Updating Focused property of ListViewItem always calls RaiseAutomationEvent.
                        // If ListViewItem is focused and selected then RaiseAutomationEvent is also called.
                        int expectedCallCount = focused && selected ? 2 : 1;
                        yield return new object[] { view, showGroups, focused, selected, expectedCallCount };
                    }
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ListView_InvokeOnSelectedIndexChanged_TestData))]
    public void ListView_OnSelectedIndexChanged_Invoke(View view, bool showGroups, bool focused, bool selected, int expectedCallCount)
    {
        using SubListView listView = new()
        {
            View = view,
            VirtualMode = false,
            ShowGroups = showGroups
        };

        listView.CreateControl();

        SubListViewItem testItem = new("Test 1");

        listView.Items.Add(testItem);

        SubListViewItemAccessibleObject customAccessibleObject = new(testItem);
        testItem.CustomAccessibleObject = customAccessibleObject;

        // Enforce accessible object creation
        _ = listView.AccessibilityObject;

        listView.Items[0].Focused = focused;
        listView.Items[0].Selected = selected;

        Assert.Equal(expectedCallCount, customAccessibleObject.RaiseAutomationEventCalls);
    }

    public static IEnumerable<object[]> ListView_InvokeOnSelectedIndexChanged_VirtualMode_TestData()
    {
        foreach (View view in Enum.GetValues(typeof(View)))
        {
            // View.Tile is not supported by ListView in virtual mode
            if (view == View.Tile)
            {
                continue;
            }

            foreach (bool showGroups in new[] { true, false })
            {
                foreach (bool focused in new[] { true, false })
                {
                    foreach (bool selected in new[] { true, false })
                    {
                        // Updating Focused property of ListViewItem always calls RaiseAutomationEvent.
                        // If ListViewItem is focused and selected then RaiseAutomationEvent is also called.
                        int expectedCallCount = focused && selected ? 2 : 1;
                        yield return new object[] { view, showGroups, focused, selected, expectedCallCount };
                    }
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ListView_InvokeOnSelectedIndexChanged_VirtualMode_TestData))]
    public void ListView_OnSelectedIndexChanged_VirtualMode_Invoke(View view, bool showGroups, bool focused, bool selected, int expectedCallCount)
    {
        SubListViewItem listItem1 = new("Test 1");

        using ListView listView = new ListView
        {
            View = view,
            VirtualMode = true,
            ShowGroups = showGroups,
            VirtualListSize = 1
        };

        listView.RetrieveVirtualItem += (s, e) =>
        {
            e.Item = e.ItemIndex switch
            {
                0 => listItem1,
                _ => throw new NotImplementedException()
            };
        };

        listView.CreateControl();
        listItem1.SetItemIndex(listView, 0);

        Assert.NotNull(listView.AccessibilityObject);

        SubListViewItemAccessibleObject customAccessibleObject = new(listItem1);
        listItem1.CustomAccessibleObject = customAccessibleObject;

        listView.Items[0].Focused = focused;
        listView.Items[0].Selected = selected;

        Assert.Equal(expectedCallCount, customAccessibleObject.RaiseAutomationEventCalls);
    }

    [WinFormsFact]
    // Regression test for https://github.com/dotnet/winforms/issues/11658
    public void ListView_OnItemChecked_VirtualMode()
    {
        ListViewItem listItem1 = new("Test 1");

        using SubListView listView = new SubListView
        {
            VirtualMode = true,
            VirtualListSize = 1,
            CheckBoxes = true
        };

        listView.RetrieveVirtualItem += (s, e) =>
        {
            e.Item = e.ItemIndex switch
            {
                0 => listItem1,
                _ => throw new NotImplementedException()
            };
        };
        listView.CreateControl();

        AccessibleObject accessibleObject = listView.AccessibilityObject;
        listView.Items[0].Focused = true;
        listView.OnGotFocus(new EventArgs());
        var clone = (ListViewItem)listView.Items[0].Clone();
        clone.Checked = true;
        Action action = () => listView.OnItemChecked(new ItemCheckedEventArgs(clone));
        action.Should().NotThrow();
    }

    public static IEnumerable<object[]> ListView_Checkboxes_VirtualMode_Disabling_TestData()
    {
        foreach (View view in Enum.GetValues(typeof(View)))
        {
            // View.Tile is not supported by ListView in virtual mode
            if (view == View.Tile)
            {
                continue;
            }

            foreach (bool showGroups in new[] { true, false })
            {
                foreach (bool useCompatibleStateImageBehavior in new[] { true, false })
                {
                    yield return new object[] { view, showGroups, useCompatibleStateImageBehavior };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ListView_Checkboxes_VirtualMode_Disabling_TestData))]
    public void ListView_Checkboxes_VirtualMode_Disabling_Succeeds(View view, bool showGroups, bool useCompatibleStateImageBehavior)
    {
        using SubListView listView = new()
        {
            View = view,
            VirtualMode = true,
            ShowGroups = showGroups,
            UseCompatibleStateImageBehavior = useCompatibleStateImageBehavior
        };

        listView.CheckBoxes = true;
        listView.CheckBoxes = false; // This would throw an InvalidOperationException prior to fix of #4042
    }

    [WinFormsFact]
    public void ListView_WmReflectNotify_LVN_KEYDOWN_WithoutGroups_and_CheckBoxes_DoesntHaveSelectedItems()
    {
        using ListView control = new();
        control.Items.Add(new ListViewItem());
        control.Items.Add(new ListViewItem());
        control.CreateControl();
        PInvokeCore.SendMessage(control, PInvokeCore.WM_KEYDOWN);
        Assert.Empty(control.SelectedItems);
    }

    [WinFormsTheory]
    [InlineData(true, true, true)]
    [InlineData(true, false, true)]
    [InlineData(false, true, true)]
    [InlineData(false, false, true)]
    [InlineData(true, true, false)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, false)]
    public unsafe void ListView_WmReflectNotify_LVN_KEYDOWN_SpaceKey_HasCheckBoxes_WithoutGroups_CheckedExpected(bool focusItem, bool checkItem, bool selectItems)
    {
        using ListView control = new();
        control.CheckBoxes = true;
        ListViewItem item1 = new()
        {
            Text = "First"
        };
        ListViewItem item2 = new()
        {
            Text = "Second"
        };

        control.Items.Add(item1);
        control.Items.Add(item2);
        control.CreateControl();
        control.VirtualMode = false;

        item1.Focused = focusItem;
        item1.Checked = checkItem;
        item1.Selected = selectItems;
        item2.Selected = selectItems;

        KeyboardSimulator.KeyDown(control, Keys.Space);

        Assert.Equal(selectItems ? 2 : 0, control.SelectedItems.Count);
        Assert.Equal(!checkItem && selectItems && focusItem, item2.Checked);
    }

    [WinFormsTheory]
    [InlineData(Keys.Down)]
    [InlineData(Keys.Up)]
    public unsafe void ListView_WmReflectNotify_LVN_KEYDOWN_WithGroups_WithoutSelection_DoesntFocusGroup(Keys key)
    {
        using ListView control = new();
        ListViewItem item1 = new()
        {
            Text = "First"
        };
        ListViewItem item2 = new()
        {
            Text = "Second"
        };

        ListViewGroup group = new("Test group");
        group.Items.Add(item1);
        group.Items.Add(item2);

        control.VirtualMode = false;
        control.Groups.Add(group);
        control.CreateControl();

        KeyboardSimulator.KeyDown(control, key);

        Assert.Empty(control.SelectedIndices);
        Assert.Null(control.FocusedItem);
        Assert.Null(control.FocusedGroup);
    }

    [WinFormsTheory(Skip = "Crash with unexpected invokerHandle ExitCode")]
    [InlineData("Keys.Down", "2")]
    [InlineData("Keys.Up", "1")]
    public unsafe void ListView_WmReflectNotify_LVN_KEYDOWN_WithGroups_and_SelectedItems_FocusedGroupIsExpected(string keyString, string expectedGroupIndexString)
    {
        // Run this from another thread as we call Application.EnableVisualStyles.
        using RemoteInvokeHandle invokerHandle = RemoteExecutor.Invoke((key_s, expectedGroupIndex_s) =>
        {
            Application.EnableVisualStyles();

            using ListView control = new();
            ListViewGroup group1 = new("Test group1");
            ListViewGroup group2 = new("Test group2");
            ListViewGroup group3 = new("Test group3");
            ListViewItem item1 = new(group1)
            {
                Text = "First"
            };
            ListViewItem item2 = new(group2)
            {
                Text = "Second"
            };
            ListViewItem item3 = new(group3)
            {
                Text = "Third"
            };
            control.Items.Add(item1);
            control.Items.Add(item2);
            control.Items.Add(item3);
            control.Groups.Add(group1);
            control.Groups.Add(group2);
            control.Groups.Add(group3);
            control.VirtualMode = false;
            control.CreateControl();

            item2.Selected = true;

            var key = key_s == "Keys.Down" ? Keys.Down : Keys.Up;
            KeyboardSimulator.KeyDown(control, key);

            Assert.False(control.GroupsEnabled);
            Assert.True(control.Items.Count > 0);
            int expectedGroupIndex = int.Parse(expectedGroupIndex_s);
            Assert.Equal(control.Groups[expectedGroupIndex], control.FocusedGroup);
        }, keyString, expectedGroupIndexString);

        // verify the remote process succeeded
        Assert.Equal(RemoteExecutor.SuccessExitCode, invokerHandle.ExitCode);
    }

    [WinFormsTheory]
    [InlineData(Keys.Down)]
    [InlineData(Keys.Up)]
    public unsafe void ListView_VirtualMode_WmReflectNotify_LVN_KEYDOWN_WithGroups_DoesNotFocusGroups(Keys key)
    {
        using ListView control = new ListView
        {
            ShowGroups = true,
            CheckBoxes = false,
            VirtualListSize = 2 // we can't add items, just indicate how many we have
        };

        ListViewGroup group = new("Test group");
        control.Groups.Add(group);
        control.VirtualMode = true;
        control.RetrieveVirtualItem += (s, e) =>
        {
            e.Item = e.ItemIndex switch
            {
                0 => new ListViewItem(group) { Selected = true },
                _ => new ListViewItem(group),
            };
        };

        control.CreateControl();

        KeyboardSimulator.KeyDown(control, key);

        Assert.Null(control.FocusedGroup);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public unsafe void ListView_VirtualMode_WmReflectNotify_LVN_KEYDOWN_EnabledCheckBoxes_WithoutGroups_DoesNotCheckItems(bool checkedItem)
    {
        using ListView control = new ListView
        {
            ShowGroups = true,
            CheckBoxes = true,
            VirtualMode = true,
            VirtualListSize = 2 // we can't add items, just indicate how many we have
        };

        ListViewItem item1 = new();
        ListViewItem item2 = new();

        control.RetrieveVirtualItem += (s, e) =>
        {
            e.Item = e.ItemIndex switch
            {
                0 => item1,
                _ => item2,
            };
        };

        control.CreateControl();
        item1.Checked = checkedItem;
        item2.Checked = false;
        control.FocusedItem = item1;

        KeyboardSimulator.KeyDown(control, Keys.Space);

        Assert.False(item2.Checked);
    }

    [WinFormsFact]
    public void ListView_KeyUp_Event_Triggers()
    {
        using ListView control = new();
        int callCount = 0;
        control.KeyUp += (_, _) => callCount++;

        KeyboardSimulator.KeyPress(control, Keys.Enter);

        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> ListView_SelectedIndexies_Contains_Invoke_TestData()
    {
        foreach (bool virtualMode in new[] { true, false })
        {
            foreach (View view in Enum.GetValues(typeof(View)))
            {
                // View.Tile is not supported by ListView in virtual mode
                if (virtualMode && view == View.Tile)
                {
                    continue;
                }

                foreach (bool showGroups in new[] { true, false })
                {
                    foreach (bool createHandle in new[] { true, false })
                    {
                        yield return new object[] { view, showGroups, createHandle, virtualMode };
                    }
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ListView_SelectedIndexies_Contains_Invoke_TestData))]
    public void ListView_SelectedIndexies_Contains_Invoke_ReturnExpected(View view, bool showGroups, bool createHandle, bool virtualMode)
    {
        using ListView listView = new ListView
        {
            ShowGroups = showGroups,
            VirtualMode = virtualMode,
            View = view,
            VirtualListSize = 1
        };

        ListViewItem listItem = new();

        if (virtualMode)
        {
            listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    0 => listItem,
                    _ => throw new NotImplementedException()
                };
            };
        }
        else
        {
            listView.Items.Add(listItem);
        }

        if (createHandle)
        {
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
        }

        listView.Items[0].Selected = true;

        Assert.False(listView.SelectedIndices.Contains(-1));
        Assert.False(listView.SelectedIndices.Contains(1));
        Assert.True(listView.SelectedIndices.Contains(0));
        Assert.Equal(createHandle, listView.IsHandleCreated);
    }

    public static IEnumerable<object[]> ListView_OnGotFocus_Invoke_TestData()
    {
        foreach (bool virtualMode in new[] { true, false })
        {
            foreach (View view in Enum.GetValues(typeof(View)))
            {
                // View.Tile is not supported by ListView in virtual mode
                if (virtualMode && view == View.Tile)
                {
                    continue;
                }

                foreach (bool showGroups in new[] { true, false })
                {
                    foreach (bool createHandle in new[] { true, false })
                    {
                        foreach (bool focused in new[] { true, false })
                        {
                            int expectedCount = createHandle
                                ? focused
                                    ? 2 // "RaiseAutomationEvent" method is called when test updates "Focused" property and calls "OnGotFocus" method
                                    : 1 // "RaiseAutomationEvent" method is called when test updates "Focused" property
                                : 0; // "RaiseAutomationEvent method" is not called if handle is not created

                            yield return new object[] { view, virtualMode, showGroups, createHandle, focused, expectedCount };
                        }
                    }
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ListView_OnGotFocus_Invoke_TestData))]
    public void ListView_OnGotFocus_Invoke(View view, bool virtualMode, bool showGroups, bool createHandle, bool focused, int expectedCount)
    {
        using SubListView listView = new()
        {
            View = view,
            VirtualMode = virtualMode,
            ShowGroups = showGroups,
            VirtualListSize = 1
        };

        SubListViewItem listItem = new("Test 1");

        if (virtualMode)
        {
            listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    0 => listItem,
                    _ => throw new NotImplementedException()
                };
            };

            listItem.SetItemIndex(listView, 0);
        }
        else
        {
            listView.Items.Add(listItem);
        }

        if (createHandle)
        {
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
        }

        Assert.NotNull(listView.AccessibilityObject);

        SubListViewItemAccessibleObject customAccessibleObject = new(listItem);
        listItem.CustomAccessibleObject = customAccessibleObject;
        listView.Items[0].Focused = focused;
        listView.OnGotFocus(new EventArgs());

        Assert.Equal(expectedCount, customAccessibleObject.RaiseAutomationEventCalls);
        Assert.Equal(createHandle, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public unsafe void ListView_InvokeGetToolInfoWrapper_ReturnsExpected(bool showItemToolTips, bool useKeyboardToolTip)
    {
        using ListView listView = new();
        listView.ShowItemToolTips = showItemToolTips;
        ToolTip toolTip = useKeyboardToolTip ? listView.KeyboardToolTip : new ToolTip();
        ToolInfoWrapper<Control> wrapper = listView.GetToolInfoWrapper(TOOLTIP_FLAGS.TTF_ABSOLUTE, "Test caption", toolTip);

        Assert.Equal("Test caption", wrapper.Text);
        // Assert.Equal method does not work because char* cannot be used as an argument to it
        Assert.Equal(string.Empty, new string(wrapper.Info.lpszText));
    }

    [WinFormsFact]
    public unsafe void ListView_ShowNodesEnabled_ExternalToolTip_InvokeGetToolInfoWrapper_ReturnsExpected()
    {
        using ListView listView = new();
        listView.ShowItemToolTips = true;
        ToolTip toolTip = new();
        ToolInfoWrapper<Control> wrapper = listView.GetToolInfoWrapper(TOOLTIP_FLAGS.TTF_ABSOLUTE, "Test caption", toolTip);
        char* expected = (char*)(-1);

        Assert.Null(wrapper.Text);
        // Assert.Equal method does not work because char* cannot be used as an argument to it
        Assert.True(wrapper.Info.lpszText == expected);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ListView_InvokeAdd_AddListViewItemToTrackList(bool showItemToolTips)
    {
        using ListView listView = new();
        listView.ShowItemToolTips = showItemToolTips;
        ListViewItem listViewItem = new();
        listView.Items.Add(listViewItem);

        Assert.True((bool)KeyboardToolTipStateMachine.Instance.TestAccessor().Dynamic.IsToolTracked(listViewItem));
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ListView_InvokeAddRange_AddlistViewItemsToTrackList(bool showItemToolTips)
    {
        using ListView listView = new();
        listView.ShowItemToolTips = showItemToolTips;
        ListViewItem listViewItem1 = new();
        ListViewItem listViewItem2 = new();
        ListViewItem listViewItem3 = new();
        var accessor = KeyboardToolTipStateMachine.Instance.TestAccessor();

        listView.Items.AddRange((ListViewItem[])[listViewItem1, listViewItem2, listViewItem3]);

        Assert.True(accessor.IsToolTracked(listViewItem1));
        Assert.True(accessor.IsToolTracked(listViewItem2));
        Assert.True(accessor.IsToolTracked(listViewItem3));
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ListView_InvokeInsert_AddlistViewItemToTrackList(bool showItemToolTips)
    {
        using ListView listView = new();
        listView.ShowItemToolTips = showItemToolTips;
        ListViewItem listViewItem = new();
        listView.Items.Insert(0, listViewItem);

        Assert.True((bool)KeyboardToolTipStateMachine.Instance.TestAccessor().Dynamic.IsToolTracked(listViewItem));
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ListView_InvokeRemove_RemoveListViewItemFromTrackList(bool showItemToolTips)
    {
        using ListView listView = new();
        listView.ShowItemToolTips = showItemToolTips;
        ListViewItem listViewItem = new();
        var accessor = KeyboardToolTipStateMachine.Instance.TestAccessor();
        listView.Items.Add(listViewItem);

        Assert.True(accessor.IsToolTracked(listViewItem));

        listView.Items.Remove(listViewItem);
        Assert.False(accessor.IsToolTracked(listViewItem));
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ListView_InvokeDispose_RemoveListViewItemFromTrackList(bool showItemToolTips)
    {
        using ListView listView = new();
        listView.ShowItemToolTips = showItemToolTips;
        ListViewItem listViewItem = new();
        var accessor = KeyboardToolTipStateMachine.Instance.TestAccessor();
        listView.Items.Add(listViewItem);

        Assert.True(accessor.Dynamic.IsToolTracked(listViewItem));

        listView.Dispose();
        Assert.False(accessor.IsToolTracked(listViewItem));
    }

    [WinFormsFact]
    public void ListView_NormalMode_InvokeNotifyAboutGotFocus_DoesNotAddListViewItemToTrackList()
    {
        using ListView listView = new();
        ListViewItem listViewItem = new();
        listView.TestAccessor().Dynamic.NotifyAboutGotFocus(listViewItem);
        Assert.False((bool)KeyboardToolTipStateMachine.Instance.TestAccessor().Dynamic.IsToolTracked(listViewItem));
    }

    [WinFormsFact]
    public void ListView_VirtualMode_InvokeNotifyAboutGotFocus_AddListViewItemToTrackList()
    {
        using ListView listView = new() { VirtualMode = true };
        ListViewItem listViewItem = new();
        listView.TestAccessor().Dynamic.NotifyAboutGotFocus(listViewItem);
        Assert.True((bool)KeyboardToolTipStateMachine.Instance.TestAccessor().Dynamic.IsToolTracked(listViewItem));
    }

    [WinFormsFact]
    public void ListView_NormalMode_InvokeNotifyAboutLostFocus_DoesNotRemoveListViewItemFromTrackList()
    {
        using ListView listView = new();
        ListViewItem listViewItem = new();
        var accessor = KeyboardToolTipStateMachine.Instance.TestAccessor();
        listView.Items.Add(listViewItem);

        Assert.True(accessor.IsToolTracked(listViewItem));

        listView.TestAccessor().Dynamic.NotifyAboutLostFocus(listViewItem);
        Assert.True(accessor.IsToolTracked(listViewItem));
    }

    [WinFormsFact]
    public void ListView_VirtualMode_InvokeNotifyAboutLostFocus_RemoveListViewItemFromTrackList()
    {
        using ListView listView = new() { VirtualMode = true };
        ListViewItem listViewItem = new();
        var accessor = KeyboardToolTipStateMachine.Instance.TestAccessor();

        listView.TestAccessor().Dynamic.NotifyAboutGotFocus(listViewItem);
        Assert.True(accessor.IsToolTracked(listViewItem));

        listView.TestAccessor().Dynamic.NotifyAboutLostFocus(listViewItem);
        Assert.False(accessor.IsToolTracked(listViewItem));
    }

    public static IEnumerable<object[]> ListView_FindNearestItem_Invoke_TestData()
    {
        yield return new object[] { 0, null, null, 1, 3 };
        yield return new object[] { 1, 0, null, 2, 4 };
        yield return new object[] { 2, 1, null, null, 5 };
        yield return new object[] { 3, null, 0, 4, 6 };
        yield return new object[] { 4, 3, 1, 5, 7 };
        yield return new object[] { 5, 4, 2, null, 8 };
        yield return new object[] { 6, null, 3, 7, null };
        yield return new object[] { 7, 6, 4, 8, null };
        yield return new object[] { 8, 7, 5, null, null };
    }

    [WinFormsTheory]
    [MemberData(nameof(ListView_FindNearestItem_Invoke_TestData))]
    public void ListView_FindNearestItem(int item, int? leftitem, int? upitem, int? rightitem, int? downitem)
    {
        using ListView listView = new();
        ListViewItem listViewItem1 = new("1");
        ListViewItem listViewItem2 = new("2");
        ListViewItem listViewItem3 = new("3");
        ListViewItem listViewItem4 = new("4");
        ListViewItem listViewItem5 = new("5");
        ListViewItem listViewItem6 = new("6");
        ListViewItem listViewItem7 = new("7");
        ListViewItem listViewItem8 = new("8");
        ListViewItem listViewItem9 = new("9");

        using ColumnHeader columnHeader1 = new();
        using ColumnHeader columnHeader2 = new();

        listView.Columns.AddRange(
        [
        columnHeader1,
        columnHeader2
        ]);
        listView.HideSelection = false;
        var listItems = new ListViewItem[]
        {
        listViewItem1,
        listViewItem2,
        listViewItem3,
        listViewItem4,
        listViewItem5,
        listViewItem6,
        listViewItem7,
        listViewItem8,
        listViewItem9
        };
        listView.Items.AddRange(listItems);
        listView.View = View.SmallIcon;
        listView.Size = new Size(200, 200);

        var listViewItemToTest = listItems[item];
        ListView_FindNearestItem_Check_Result(listItems, listViewItemToTest, SearchDirectionHint.Left, leftitem);
        ListView_FindNearestItem_Check_Result(listItems, listViewItemToTest, SearchDirectionHint.Up, upitem);
        ListView_FindNearestItem_Check_Result(listItems, listViewItemToTest, SearchDirectionHint.Right, rightitem);
        ListView_FindNearestItem_Check_Result(listItems, listViewItemToTest, SearchDirectionHint.Down, downitem);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListView_FindNearestItem_Invoke_TestData))]
    public void ListView_FindNearestItem_With_Images(int item, int? leftitem, int? upitem, int? rightitem, int? downitem)
    {
        using ImageList imagecollection = new();
        imagecollection.Images.Add(Form.DefaultIcon);
        imagecollection.Images.Add(Form.DefaultIcon);

        imagecollection.TransparentColor = Color.Transparent;
        imagecollection.Images.SetKeyName(0, "SmallA.bmp");
        imagecollection.Images.SetKeyName(1, "SmallABlue.bmp");

        using ListView listView = new();
        listView.SmallImageList = imagecollection;
        ListViewItem listViewItem1 = new("Item1");
        ListViewItem listViewItem2 = new("item2") { ImageKey = "SmallABlue.bmp" };
        ListViewItem listViewItem3 = new("item3");
        ListViewItem listViewItem4 = new("Items 4") { ImageKey = "SmallA.bmp" };
        ListViewItem listViewItem5 = new("Items 5");
        ListViewItem listViewItem6 = new("Items 6") { ImageKey = "SmallABlue.bmp" };
        ListViewItem listViewItem7 = new("Items 7") { ImageKey = "SmallA.bmp" };
        ListViewItem listViewItem8 = new("Items 8");
        ListViewItem listViewItem9 = new("Items 9");

        using ColumnHeader columnHeader1 = new();
        using ColumnHeader columnHeader2 = new();

        listView.Columns.AddRange(
        [
        columnHeader1,
        columnHeader2
        ]);
        listView.HideSelection = false;
        var listItems = new ListViewItem[]
        {
        listViewItem1,
        listViewItem2,
        listViewItem3,
        listViewItem4,
        listViewItem5,
        listViewItem6,
        listViewItem7,
        listViewItem8,
        listViewItem9
        };
        listView.Items.AddRange(listItems);
        listView.View = View.SmallIcon;
        listView.Size = new Size(200, 200);

        var listViewItemToTest = listItems[item];
        ListView_FindNearestItem_Check_Result(listItems, listViewItemToTest, SearchDirectionHint.Left, leftitem);
        ListView_FindNearestItem_Check_Result(listItems, listViewItemToTest, SearchDirectionHint.Up, upitem);
        ListView_FindNearestItem_Check_Result(listItems, listViewItemToTest, SearchDirectionHint.Right, rightitem);
        ListView_FindNearestItem_Check_Result(listItems, listViewItemToTest, SearchDirectionHint.Down, downitem);
    }

    private void ListView_FindNearestItem_Check_Result(ListViewItem[] listItems, ListViewItem item, SearchDirectionHint direction, int? resultItem)
    {
        if (!resultItem.HasValue)
        {
            Assert.Null(item.FindNearestItem(direction));
        }
        else
        {
            Assert.Equal(listItems[resultItem.Value], item.FindNearestItem(direction));
        }
    }

    [WinFormsFact]
    public void ListView_Invokes_SetToolTip_IfExternalToolTipIsSet()
    {
        using ListView listView = new();
        using ToolTip toolTip = new();
        listView.CreateControl();

        dynamic listViewDynamic = listView.TestAccessor().Dynamic;
        string actual = listViewDynamic._toolTipCaption;

        Assert.Empty(actual);
        Assert.NotEqual(IntPtr.Zero, toolTip.Handle); // A workaround to create the toolTip native window Handle

        string text = "Some test text";
        toolTip.SetToolTip(listView, text); // Invokes ListView's SetToolTip inside
        actual = listViewDynamic._toolTipCaption;

        Assert.Equal(text, actual);
    }

    [WinFormsTheory]
    [InlineData(View.Details)]
    [InlineData(View.LargeIcon)]
    [InlineData(View.List)]
    [InlineData(View.SmallIcon)]
    [InlineData(View.Tile)]
    public void ListView_AnnounceColumnHeader_DoesNotWork_WithoutHandle(View view)
    {
        using ListView listView = new()
        {
            Size = new Size(300, 200),
            View = view
        };

        listView.Columns.Add(new ColumnHeader() { Text = "Column 1", Width = 100 });
        listView.Columns.Add(new ColumnHeader() { Text = "Column 2", Width = 100 });
        listView.Columns.Add(new ColumnHeader() { Text = "Column 3", Width = 100 });
        listView.Items.Add(new ListViewItem("Test"));
        SubListViewAccessibleObject accessibleObject = new(listView);

        int accessibilityProperty = listView.TestAccessor().Dynamic.s_accessibilityProperty;
        listView.Properties.AddValue(accessibilityProperty, accessibleObject);
        listView.AnnounceColumnHeader(new Point(15, 40));
        Assert.Equal(0, accessibleObject.RaiseAutomationNotificationCallCount);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(View.Details)]
    [InlineData(View.LargeIcon)]
    [InlineData(View.List)]
    [InlineData(View.SmallIcon)]
    [InlineData(View.Tile)]
    public void ListView_AnnounceColumnHeader_DoesNotWork_WithoutHeader(View view)
    {
        using ListView listView = new()
        {
            Size = new Size(300, 200),
            View = view
        };

        listView.CreateControl();
        listView.Items.Add(new ListViewItem("Test"));
        SubListViewAccessibleObject accessibleObject = new(listView);

        int accessibilityProperty = listView.TestAccessor().Dynamic.s_accessibilityProperty;
        listView.Properties.AddValue(accessibilityProperty, accessibleObject);
        listView.AnnounceColumnHeader(new Point(15, 40));
        Assert.Equal(0, accessibleObject.RaiseAutomationNotificationCallCount);
        Assert.True(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(View.Details)]
    [InlineData(View.LargeIcon)]
    [InlineData(View.List)]
    [InlineData(View.SmallIcon)]
    [InlineData(View.Tile)]
    public void ListView_AnnounceColumnHeader_DoesNotWork_InvalidPoint(View view)
    {
        using ListView listView = new()
        {
            Size = new Size(300, 200),
            View = view
        };

        listView.CreateControl();
        listView.Items.Add(new ListViewItem("Test"));
        SubListViewAccessibleObject accessibleObject = new(listView);

        int accessibilityProperty = listView.TestAccessor().Dynamic.s_accessibilityProperty;
        listView.Properties.AddValue(accessibilityProperty, accessibleObject);
        listView.AnnounceColumnHeader(new Point(10, 20));
        Assert.Equal(0, accessibleObject.RaiseAutomationNotificationCallCount);
        Assert.True(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(15, 40, "Column 1")]
    [InlineData(150, 40, "Column 2")]
    [InlineData(250, 40, "Column 3")]
    public void ListView_AnnounceColumnHeader_WorksCorrectly(int x, int y, string expectedColumnName)
    {
        using ListView listView = new()
        {
            Size = new Size(300, 200),
            View = View.Details
        };

        listView.CreateControl();

        listView.Columns.Add(new ColumnHeader() { Text = "Column 1", Width = 100 });
        listView.Columns.Add(new ColumnHeader() { Text = "Column 2", Width = 100 });
        listView.Columns.Add(new ColumnHeader() { Text = "Column 3", Width = 100 });
        listView.Items.Add(new ListViewItem("Test"));
        SubListViewAccessibleObject accessibleObject = new(listView);

        int accessibilityProperty = listView.TestAccessor().Dynamic.s_accessibilityProperty;
        listView.Properties.AddValue(accessibilityProperty, accessibleObject);
        listView.AnnounceColumnHeader(new Point(x, y));

        Assert.Equal(1, accessibleObject.RaiseAutomationNotificationCallCount);
        Assert.Equal(expectedColumnName, accessibleObject.AnnouncedColumn);
        Assert.True(listView.IsHandleCreated);
    }

    public static TheoryData<ListViewItem> GetListViewItemTheoryData() => new()
    {
        { new("Item 1") },
        { null }
    };

    [WinFormsTheory]
    [MemberData(nameof(GetListViewItemTheoryData))]
    // Regression test for https://github.com/dotnet/winforms/issues/11663.
    public void ListView_VirtualMode_ReleaseUiaProvider_Success(ListViewItem listItem)
    {
        using ListView listView = new()
        {
            VirtualMode = true,
            VirtualListSize = 1
        };

        listView.RetrieveVirtualItem += (s, e) =>
        {
            e.Item = e.ItemIndex switch
            {
                0 => listItem,
                _ => throw new NotImplementedException()
            };
        };

        listView.AccessibilityObject.Should().NotBeNull();

        Action action = () => listView.ReleaseUiaProvider(listView.InternalHandle);
        action.Should().NotThrow();
        listView.IsAccessibilityObjectCreated.Should().BeFalse();
        listView.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsFact]
    public void ListView_VirtualMode_GetListViewItemAsExpected()
    {
        using ListView listView = new()
        {
            VirtualMode = true,
            VirtualListSize = 2
        };

        ListViewItem listItem1 = new("Item 1");
        ListViewItem listItem2 = null;
        listView.RetrieveVirtualItem += (s, e) =>
        {
            e.Item = e.ItemIndex switch
            {
                0 => listItem1,
                1 => listItem2,
                _ => throw new NotImplementedException()
            };
        };
        listView.Items.GetItemByIndex(0).Should().Be(listView.Items[0]);
        listView.Items.GetItemByIndex(1).Should().BeNull();
        Action action = () => listView.Items[1].ToString();
        action.Should().Throw<InvalidOperationException>(SR.ListViewVirtualItemRequired);
    }

    private class SubListViewAccessibleObject : ListView.ListViewAccessibleObject
    {
        internal string AnnouncedColumn { get; private set; }

        internal int RaiseAutomationNotificationCallCount { get; private set; }

        internal SubListViewAccessibleObject(ListView listView) : base(listView)
        {
        }

        internal override bool InternalRaiseAutomationNotification(AutomationNotificationKind notificationKind, AutomationNotificationProcessing notificationProcessing, string notificationText)
        {
            AnnouncedColumn = notificationText;
            RaiseAutomationNotificationCallCount++;
            return true;
        }
    }

    public static IEnumerable<object[]> ListView_OnSelectedIndexChanged_TestData()
    {
        foreach (View view in Enum.GetValues(typeof(View)))
        {
            foreach (bool virtualMode in new[] { true, false })
            {
                // View.Tile is not supported by ListView in virtual mode
                if (view == View.Tile)
                {
                    continue;
                }

                foreach (bool showGroups in new[] { true, false })
                {
                    foreach (bool withinGroup in new[] { true, false })
                    {
                        yield return new object[] { view, virtualMode, showGroups, withinGroup };
                    }
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ListView_OnSelectedIndexChanged_TestData))]
    public void ListView_OnSelectedIndexChanged_DoesNotInvoke_RaiseAutomationEvent_SecondTime(View view, bool virtualMode, bool showGroups, bool withinGroup)
    {
        using SubListView listView = GetSubListViewWithData(view, virtualMode, showGroups, withinGroup, createControl: true);
        ListViewItem listViewItem = listView.Items[0];

        Assert.NotNull(listView.AccessibilityObject);
        Assert.NotNull(listViewItem.AccessibilityObject);

        SubListViewItemAccessibleObject accessibleObject = new(listViewItem);
        listViewItem.TestAccessor().Dynamic._accessibilityObject = accessibleObject;
        listView.CreateControl();
        listViewItem.Focused = true;
        listViewItem.Selected = true;

        Assert.Equal(2, accessibleObject.RaiseAutomationEventCalls);

        listView.CallSelectedIndexChanged();

        Assert.Equal(2, accessibleObject.RaiseAutomationEventCalls);
    }

    public static IEnumerable<object[]> ListView_OnGroupCollapsedStateChanged_InvokeRaiseAutomationEvent_TestData()
    {
        foreach (View view in Enum.GetValues(typeof(View)))
        {
            foreach (bool virtualMode in new[] { true, false })
            {
                // View.Tile is not supported by ListView in virtual mode
                if (view == View.Tile)
                {
                    continue;
                }

                foreach (bool showGroups in new[] { true, false })
                {
                    foreach (bool withinGroup in new[] { true, false })
                    {
                        foreach (bool createControl in new[] { true, false })
                        {
                            foreach (int groupId in new[] { -1, 0, 1 })
                            {
                                yield return new object[] { view, virtualMode, showGroups, withinGroup, createControl, groupId };
                            }
                        }
                    }
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ListView_OnGroupCollapsedStateChanged_InvokeRaiseAutomationEvent_TestData))]
    public void ListView_OnGroupCollapsedStateChanged_InvokeRaiseAutomationEvent_AsExpected(
        View view,
        bool virtualMode,
        bool showGroups,
        bool withinGroup,
        bool createControl,
        int groupId)
    {
        using SubListView listView = GetSubListViewWithData(view, virtualMode, showGroups, withinGroup, createControl);
        SubListViewAccessibleObject accessibleObject = new(listView);
        int accessibilityProperty = listView.TestAccessor().Dynamic.s_accessibilityProperty;
        listView.Properties.AddValue(accessibilityProperty, accessibleObject);

        listView.OnGroupCollapsedStateChanged(new ListViewGroupEventArgs(groupId));

        int expectedCount = listView.GroupsEnabled && groupId == 0 ? 1 : 0;

        Assert.Equal(expectedCount, accessibleObject.RaiseAutomationNotificationCallCount);
        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0, true)]  // Click on the item results in item editing.
    [InlineData(1, false)]  // Click on sub-items does not result in editing.
    [InlineData(2, false)]
    public void ListView_OnMouseClick_EditLabel_AsExpected(int subItemIndex, bool isEditControlCreated)
    {
        using ListView listView = new()
        {
            Size = new Size(300, 200),
            View = View.Details,
            LabelEdit = true,
            FullRowSelect = true
        };

        listView.Columns.AddRange(
            [
                new() { Text = "Column 1", Width = 100 },
                new() { Text = "Column 2", Width = 100 },
                new() { Text = "Column 3", Width = 100 }
            ]);

        ListViewItem item = new("Test");
        item.SubItems.Add("Sub1");
        item.SubItems.Add("Sub2");
        listView.Items.Add(item);

        listView.CreateControl();

        PInvoke.SetFocus(listView);
        listView.Items[0].Selected = true;

        // Add a pixel both to x and y as the left-upper corner is not a part of subitem
        Point subItemLocation = listView.Items[0].SubItems[subItemIndex].Bounds.Location + new Size(1, 1);
        // The mouse down handler will wait for mouse up event, so we need to put it on the message queue
        // before invoking mouse down.
        PInvokeCore.PostMessage(listView, PInvokeCore.WM_LBUTTONUP, 0, PARAM.FromPoint(subItemLocation));
        PInvokeCore.SendMessage(listView, PInvokeCore.WM_LBUTTONDOWN, 1, PARAM.FromPoint(subItemLocation));

        // Start editing immediately (if it was queued).
        PInvokeCore.SendMessage(listView, PInvokeCore.WM_TIMER, (WPARAM)(nint)listView.TestAccessor().Dynamic.LVLABELEDITTIMER);

        nint editControlHandle = PInvokeCore.SendMessage(listView, PInvoke.LVM_GETEDITCONTROL);

        // End the edit because this more closely resembles real live usage. Additionally
        // when edit box is open, the native ListView will move focus to items being removed.
        PInvokeCore.SendMessage(listView, PInvoke.LVM_CANCELEDITLABEL);

        if (isEditControlCreated)
        {
            Assert.NotEqual(0, editControlHandle);
        }
        else
        {
            Assert.Equal(0, editControlHandle);
        }
    }

    [WinFormsTheory]
    [InlineData(Keys.Right)]
    [InlineData(Keys.Left)]
    public void ListView_LeftRightArrow_DoesNotThrowException(Keys key)
    {
        using ListView listView = new() { ShowGroups = true };
        listView.Items.Add(new ListViewItem("Group Item 0"));
        listView.CreateControl();
        listView.Items[0].Selected = true;
        listView.Items[0].Focused = true;

        // https://docs.microsoft.com/windows/win32/inputdev/wm-keyup
        // The MSDN page tells us what bits of lParam to use for each of the parameters.
        // All we need to do is some bit shifting to assemble lParam
        // lParam = repeatCount | (scanCode << 16)
        nint keyCode = (nint)key;
        nint lParam = 0x00000001 | keyCode << 16;
        PInvokeCore.SendMessage(listView, PInvokeCore.WM_KEYUP, (WPARAM)keyCode, (LPARAM)lParam);

        Assert.True(listView.IsHandleCreated);
    }

    public static IEnumerable<object[]> ListView_View_ShowGroup_TestData()
    {
        foreach (View view in Enum.GetValues(typeof(View)))
        {
            foreach (bool showGroups in new[] { true, false })
            {
                yield return new object[] { view, showGroups };
            }
        }
    }

    public static IEnumerable<object[]> ListView_View_ShowGroup_Checked_TestData()
    {
        foreach (View view in Enum.GetValues(typeof(View)))
        {
            // CheckBoxes are not supported in Tile view.
            if (view == View.Tile)
            {
                continue;
            }

            foreach (bool showGroups in new[] { true, false })
            {
                yield return new object[] { view, showGroups };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ListView_View_ShowGroup_TestData))]
    public void ListView_Remove_NotSelectedItems(View view, bool showGroups)
    {
        using ListView listView = new() { View = view, ShowGroups = showGroups };
        listView.CreateControl();
        listView.Items.AddRange((ListViewItem[])[new("test 1"), new("test 2"), new("test 3")]);

        listView.Items[0].Selected = true;

        Assert.True(listView.Items[0].Selected);
        Assert.Equal(3, listView.Items.Count);
        Assert.Single(listView.SelectedItems);
        Assert.Single(listView.SelectedIndices);

        listView.Items.Remove(listView.Items[1]);

        Assert.True(listView.Items[0].Selected);
        Assert.Equal(2, listView.Items.Count);
        Assert.Single(listView.SelectedItems);
        Assert.Single(listView.SelectedIndices);
        Assert.True(listView.Items[0].Selected);

        listView.Items.Remove(listView.Items[1]);

        Assert.True(listView.Items[0].Selected);
        Assert.Single(listView.Items);
        Assert.Single(listView.SelectedItems);
        Assert.Single(listView.SelectedIndices);
        Assert.True(listView.Items[0].Selected);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListView_View_ShowGroup_TestData))]
    public void ListView_Remove_SelectedItems(View view, bool showGroups)
    {
        using ListView listView = new() { View = view, ShowGroups = showGroups };
        listView.CreateControl();
        listView.Items.AddRange((ListViewItem[])[new("test 1"), new("test 2"), new("test 3")]);

        for (int count = listView.Items.Count; count > 1; count -= 1)
        {
            ListViewItem item = listView.Items[0];
            item.Selected = true;

            Assert.True(item.Selected);
            Assert.Equal(count, listView.Items.Count);
            Assert.Equal(listView, item.ListView);
            Assert.Single(listView.SelectedItems);
            Assert.Single(listView.SelectedIndices);

            listView.Items.Remove(item);
            count -= 1;

            Assert.True(item.Selected);
            Assert.Equal(count, listView.Items.Count);
            Assert.Null(item.ListView);
            Assert.Empty(listView.SelectedItems);
            Assert.Empty(listView.SelectedIndices);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ListView_View_ShowGroup_Checked_TestData))]
    public void ListView_Remove_NotCheckedItems(View view, bool showGroups)
    {
        using ListView listView = new() { View = view, ShowGroups = showGroups, CheckBoxes = true };
        listView.CreateControl();
        listView.Items.AddRange((ListViewItem[])[new("test 1"), new("test 2"), new("test 3")]);

        listView.Items[0].Checked = true;

        Assert.True(listView.Items[0].Checked);
        Assert.Equal(3, listView.Items.Count);
        Assert.Single(listView.CheckedItems);
        Assert.Single(listView.CheckedIndices);

        listView.Items.Remove(listView.Items[1]);

        Assert.True(listView.Items[0].Checked);
        Assert.Equal(2, listView.Items.Count);
        Assert.Single(listView.CheckedItems);
        Assert.Single(listView.CheckedIndices);

        listView.Items.Remove(listView.Items[1]);

        Assert.True(listView.Items[0].Checked);
        Assert.Single(listView.Items);
        Assert.Single(listView.CheckedItems);
        Assert.Single(listView.CheckedIndices);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListView_View_ShowGroup_Checked_TestData))]
    public void ListView_Remove_CheckedItems(View view, bool showGroups)
    {
        using ListView listView = new() { View = view, ShowGroups = showGroups, CheckBoxes = true };
        listView.CreateControl();
        listView.Items.AddRange((ListViewItem[])[new("test 1"), new("test 2"), new("test 3")]);

        for (int count = listView.Items.Count; count > 1; count -= 1)
        {
            ListViewItem item = listView.Items[0];
            item.Checked = true;

            Assert.True(item.Checked);
            Assert.Equal(count, listView.Items.Count);
            Assert.Equal(listView, item.ListView);
            Assert.Single(listView.CheckedItems);
            Assert.Single(listView.CheckedIndices);

            listView.Items.Remove(item);
            count -= 1;

            Assert.True(item.Checked);
            Assert.Equal(count, listView.Items.Count);
            Assert.Null(item.ListView);
            Assert.Empty(listView.CheckedItems);
            Assert.Empty(listView.CheckedIndices);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ListView_View_ShowGroup_TestData))]
    public void ListView_Remove_Group_WithNotSelectedItems(View view, bool showGroups)
    {
        using ListView listView = new() { View = view, ShowGroups = showGroups };
        listView.CreateControl();

        var groups = new ListViewGroup[] { new("Group 1"), new("Group 2"), new("Group 3") };
        listView.Groups.AddRange(groups);
        listView.Items.AddRange((ListViewItem[])[new("test 1", groups[0]), new("test 2", groups[1]), new("test 3", groups[2])]);

        listView.Items[0].Selected = true;

        Assert.True(listView.Items[0].Selected);
        Assert.Equal(3, listView.Items.Count);
        Assert.Equal(3, listView.Groups.Count);
        Assert.Single(listView.SelectedItems);
        Assert.Single(listView.SelectedIndices);

        listView.Groups.Remove(listView.Groups[2]);

        Assert.True(listView.Items[0].Selected);
        Assert.Equal(3, listView.Items.Count);
        Assert.Equal(2, listView.Groups.Count);
        Assert.Single(listView.SelectedItems);
        Assert.Single(listView.SelectedIndices);
        Assert.True(listView.Items[0].Selected);

        listView.Groups.Remove(listView.Groups[1]);

        Assert.True(listView.Items[0].Selected);
        Assert.Equal(3, listView.Items.Count);
        Assert.Single(listView.Groups);
        Assert.Single(listView.SelectedItems);
        Assert.Single(listView.SelectedIndices);
        Assert.True(listView.Items[0].Selected);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListView_View_ShowGroup_TestData))]
    public void ListView_Remove_Group_WithSelectedItems(View view, bool showGroups)
    {
        using ListView listView = new() { View = view, ShowGroups = showGroups };
        listView.CreateControl();

        var groups = new ListViewGroup[] { new("Group 1"), new("Group 2"), new("Group 3") };
        listView.Groups.AddRange(groups);
        listView.Items.AddRange((ListViewItem[])[new("test 1", groups[0]), new("test 2", groups[1]), new("test 3", groups[2])]);

        int count = 3;
        for (int i = 0; i < 2; i++)
        {
            ListViewItem item = listView.Items[i];
            item.Selected = true;
            int selectedCount = i + 1;

            Assert.True(item.Selected);
            Assert.Equal(3, listView.Items.Count);
            Assert.Equal(count, listView.Groups.Count);
            Assert.Equal(listView, item.ListView);
            Assert.Equal(selectedCount, listView.SelectedItems.Count);
            Assert.Equal(selectedCount, listView.SelectedIndices.Count);

            listView.Groups.Remove(listView.Groups[0]);
            count -= 1;

            Assert.True(item.Selected);
            Assert.Equal(3, listView.Items.Count);
            Assert.Equal(count, listView.Groups.Count);
            Assert.Equal(listView, item.ListView);
            Assert.Equal(selectedCount, listView.SelectedItems.Count);
            Assert.Equal(selectedCount, listView.SelectedIndices.Count);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ListView_View_ShowGroup_Checked_TestData))]
    public void ListView_Remove_Group_WithNotCheckedItems(View view, bool showGroups)
    {
        using ListView listView = new() { View = view, ShowGroups = showGroups, CheckBoxes = true };
        listView.CreateControl();

        var groups = new ListViewGroup[] { new("Group 1"), new("Group 2"), new("Group 3") };
        listView.Groups.AddRange(groups);
        listView.Items.AddRange((ListViewItem[])[new("test 1", groups[0]), new("test 2", groups[1]), new("test 3", groups[2])]);

        listView.Items[0].Checked = true;

        Assert.True(listView.Items[0].Checked);
        Assert.Equal(3, listView.Items.Count);
        Assert.Equal(3, listView.Items.Count);
        Assert.Single(listView.CheckedItems);
        Assert.Single(listView.CheckedIndices);

        listView.Groups.Remove(listView.Groups[2]);

        Assert.True(listView.Items[0].Checked);
        Assert.Equal(3, listView.Items.Count);
        Assert.Equal(2, listView.Groups.Count);
        Assert.Single(listView.CheckedItems);
        Assert.Single(listView.CheckedIndices);

        listView.Groups.Remove(listView.Groups[1]);

        Assert.True(listView.Items[0].Checked);
        Assert.Equal(3, listView.Items.Count);
        Assert.Single(listView.Groups);
        Assert.Single(listView.CheckedItems);
        Assert.Single(listView.CheckedIndices);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListView_View_ShowGroup_Checked_TestData))]
    public void ListView_Remove_Group_WithCheckedItems(View view, bool showGroups)
    {
        using ListView listView = new() { View = view, ShowGroups = showGroups, CheckBoxes = true };
        listView.CreateControl();

        var groups = new ListViewGroup[] { new("Group 1"), new("Group 2"), new("Group 3") };
        listView.Groups.AddRange(groups);
        listView.Items.AddRange((ListViewItem[])[new("test 1", groups[0]), new("test 2", groups[1]), new("test 3", groups[2])]);

        int count = 3;
        for (int i = 0; i < 2; i++)
        {
            ListViewItem item = listView.Items[i];
            item.Checked = true;
            int selectedCount = i + 1;

            Assert.True(item.Checked);
            Assert.Equal(3, listView.Items.Count);
            Assert.Equal(count, listView.Groups.Count);
            Assert.Equal(listView, item.ListView);
            Assert.Equal(selectedCount, listView.CheckedItems.Count);
            Assert.Equal(selectedCount, listView.CheckedIndices.Count);

            listView.Groups.Remove(listView.Groups[0]);
            count -= 1;

            Assert.True(item.Checked);
            Assert.Equal(3, listView.Items.Count);
            Assert.Equal(count, listView.Groups.Count);
            Assert.Equal(listView, item.ListView);
            Assert.Equal(selectedCount, listView.CheckedItems.Count);
            Assert.Equal(selectedCount, listView.CheckedIndices.Count);
        }
    }

    [WinFormsFact]
    public void ListView_FocusedItem_Reset_Remove()
    {
        using ListView listView = new();
        listView.CreateControl();
        listView.Items.Add(new ListViewItem("Test 1"));
        listView.Items.Add(new ListViewItem("Test 2"));
        listView.Items[0].Focused = true;

        Assert.NotNull(listView.FocusedItem);

        listView.Items.Remove(listView.Items[0]);

        Assert.Null(listView.FocusedItem);
    }

    [WinFormsFact]
    public void ListView_FocusedItem_Reset_RemoveAt()
    {
        using ListView listView = new();
        listView.CreateControl();
        listView.Items.Add(new ListViewItem("Test 1"));
        listView.Items.Add(new ListViewItem("Test 2"));
        listView.Items[0].Focused = true;

        Assert.NotNull(listView.FocusedItem);

        listView.Items.RemoveAt(0);

        Assert.Null(listView.FocusedItem);
    }

    [WinFormsFact]
    public void ListView_FocusedItem_Reset_RemoveByKey()
    {
        using ListView listView = new();
        listView.CreateControl();
        listView.Items.Add(new ListViewItem("Test 1") { Name = "Test 1" });
        listView.Items.Add(new ListViewItem("Test 2"));
        listView.Items[0].Focused = true;

        Assert.NotNull(listView.FocusedItem);

        listView.Items.RemoveByKey("Test 1");

        Assert.Null(listView.FocusedItem);
    }

    [WinFormsFact]
    public void ListView_FocusedItem_Reset_Clear()
    {
        using ListView listView = new();
        listView.CreateControl();
        listView.Items.Add(new ListViewItem("Test 1"));
        listView.Items.Add(new ListViewItem("Test 2"));
        listView.Items[0].Focused = true;

        Assert.NotNull(listView.FocusedItem);

        listView.Items.Clear();

        Assert.Null(listView.FocusedItem);
    }

    [WinFormsFact]
    public void ListView_ReleaseUiaProvider_DoesNotForceDefaultGroupCreation()
    {
        using ListView listView = new();
        _ = listView.AccessibilityObject;

        listView.ReleaseUiaProvider(listView.HWND);

        Assert.Null(listView.TestAccessor().Dynamic._defaultGroup);
        Assert.True(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListView_RightToLeftLayoutChanged_AddRemove_Invoke_Success()
    {
        using SubListView listView = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(listView);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };

        listView.RightToLeftLayoutChanged += handler;
        listView.RightToLeftLayoutChanged -= handler;
        callCount.Should().Be(0);

        listView.RightToLeftLayoutChanged += handler;
        listView.OnRightToLeftLayoutChanged(EventArgs.Empty);
        callCount.Should().Be(1);

        listView.RightToLeftLayoutChanged -= handler;
        listView.OnRightToLeftLayoutChanged(EventArgs.Empty);
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void ListView_TextChanged_AddRemove_Success()
    {
        using SubListView listView = new();
        int callCount = 0;
        EventHandler handler = (_, _) => callCount++;

        listView.TextChanged += handler;
        listView.Text = "New Text";
        callCount.Should().Be(1);
        listView.Text.Should().Be("New Text");

        listView.TextChanged -= handler;
        listView.Text = "Another Text";
        callCount.Should().Be(1);
        listView.Text.Should().Be("Another Text");
    }

    [WinFormsFact]
    public void ListView_LabelEditEvents_AddRemove_Invoke()
    {
        using SubListView listView = new();
        int beforeLabelEditCallCount = 0;
        int afterLabelEditCallCount = 0;

        LabelEditEventHandler beforeLabelEditHandler = (sender, e) =>
        {
            sender.Should().BeSameAs(listView);
            e.Should().NotBeNull();
            beforeLabelEditCallCount++;
        };

        LabelEditEventHandler afterLabelEditHandler = (sender, e) =>
        {
            sender.Should().BeSameAs(listView);
            e.Should().NotBeNull();
            afterLabelEditCallCount++;
        };

        listView.BeforeLabelEdit += beforeLabelEditHandler;

        listView.OnBeforeLabelEdit(new LabelEditEventArgs(1));
        beforeLabelEditCallCount.Should().Be(1);

        listView.BeforeLabelEdit -= beforeLabelEditHandler;

        listView.OnBeforeLabelEdit(new LabelEditEventArgs(1));
        beforeLabelEditCallCount.Should().Be(1);

        listView.AfterLabelEdit += afterLabelEditHandler;

        listView.OnAfterLabelEdit(new LabelEditEventArgs(1));
        afterLabelEditCallCount.Should().Be(1);

        listView.AfterLabelEdit -= afterLabelEditHandler;

        listView.OnAfterLabelEdit(new LabelEditEventArgs(1));
        afterLabelEditCallCount.Should().Be(1);
    }

    [WinFormsFact]
    public void ListView_CacheVirtualItemsEvent_Test()
    {
        using SubListView listView = new();
        int callCount = 0;

        CacheVirtualItemsEventHandler handler = (sender, e) =>
        {
            sender.Should().BeSameAs(listView);
            e.Should().NotBeNull();
            callCount++;
        };

        listView.CacheVirtualItems += handler;

        listView.OnCacheVirtualItems(new CacheVirtualItemsEventArgs(1, 2));
        callCount.Should().Be(1);

        listView.CacheVirtualItems -= handler;

        listView.OnCacheVirtualItems(new CacheVirtualItemsEventArgs(1, 2));
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void ListView_ColumnClick_AddRemoveHandlers_ShouldCorrectlyInvokeOrNotInvokeEvent()
    {
        using SubListView listView = new();
        int firstHandlerInvokeCount = 0;
        int secondHandlerInvokeCount = 0;
        ColumnClickEventArgs eventArgs = null;

        ColumnClickEventHandler firstHandler = (sender, e) =>
        {
            firstHandlerInvokeCount++;
            eventArgs = e;
        };

        ColumnClickEventHandler secondHandler = (sender, e) => { secondHandlerInvokeCount++; };

        // Add first handler and simulate the column click event
        listView.ColumnClick += firstHandler;
        ColumnClickEventArgs args = new(1);
        listView.TestAccessor().Dynamic.OnColumnClick(args);

        firstHandlerInvokeCount.Should().Be(1);
        secondHandlerInvokeCount.Should().Be(0);
        eventArgs.Should().NotBeNull();
        eventArgs.Column.Should().Be(1);

        // Add second handler and simulate the event again
        listView.ColumnClick += secondHandler;
        listView.TestAccessor().Dynamic.OnColumnClick(args);

        firstHandlerInvokeCount.Should().Be(2); // First handler called again
        secondHandlerInvokeCount.Should().Be(1); // Second handler called for the first time

        // Remove first handler and simulate the event again
        listView.ColumnClick -= firstHandler;
        listView.TestAccessor().Dynamic.OnColumnClick(args);

        // First handler should not be called again, second handler should be called again
        firstHandlerInvokeCount.Should().Be(2);
        secondHandlerInvokeCount.Should().Be(2);
    }

    [WinFormsFact]
    public void ListView_GroupTaskLinkClick_EventHandling_ShouldBehaveAsExpected()
    {
        using SubListView listView = new();
        int callCount = 0;
        object eventSender = null;
        ListViewGroupEventArgs eventArgs = null;

        EventHandler<ListViewGroupEventArgs> handler1 = (sender, e) =>
        {
            callCount++;
            eventSender = sender;
            eventArgs = e;
        };

        EventHandler<ListViewGroupEventArgs> handler2 = (sender, e) => { callCount++; };

        // Test adding and invoking first handler.
        listView.GroupTaskLinkClick += handler1;
        ListViewGroupEventArgs expectedEventArgs = new(1);
        listView.TestAccessor().Dynamic.OnGroupTaskLinkClick(expectedEventArgs);

        callCount.Should().Be(1);
        eventSender.Should().Be(listView);
        eventArgs.Should().Be(expectedEventArgs);

        // Test adding and invoking both handlers.
        listView.GroupTaskLinkClick += handler2;
        listView.TestAccessor().Dynamic.OnGroupTaskLinkClick(new ListViewGroupEventArgs(2));

        // Expect callCount to be 3 because both handlers should be called.
        callCount.Should().Be(3);

        // Test removing first handler and invoking.
        listView.GroupTaskLinkClick -= handler1;
        listView.TestAccessor().Dynamic.OnGroupTaskLinkClick(new ListViewGroupEventArgs(3));

        // Expect callCount to be 4 because only second handler should be called.
        callCount.Should().Be(4);

        // Test removing second handler and ensuring no invocation.
        listView.GroupTaskLinkClick -= handler2;
        listView.TestAccessor().Dynamic.OnGroupTaskLinkClick(new ListViewGroupEventArgs(4));

        // Expect callCount to remain 4 because no handlers should be called.
        callCount.Should().Be(4);
    }

    private class SubListViewItem : ListViewItem
    {
        public AccessibleObject CustomAccessibleObject { get; set; }

        public SubListViewItem(string text) : base(text)
        {
        }

        internal override AccessibleObject AccessibilityObject => CustomAccessibleObject;
    }

    private class SubListViewItemAccessibleObject : ListViewItemBaseAccessibleObject
    {
        protected override View View => View.List;

        public int RaiseAutomationEventCalls;

        public SubListViewItemAccessibleObject(ListViewItem owningItem) : base(owningItem)
        {
        }

        internal override bool RaiseAutomationEvent(UIA_EVENT_ID eventId)
        {
            RaiseAutomationEventCalls++;
            return base.RaiseAutomationEvent(eventId);
        }
    }

    private class SubListView : ListView
    {
        internal void CallSelectedIndexChanged() => base.OnSelectedIndexChanged(new EventArgs());

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

        public new void OnGotFocus(EventArgs e) => base.OnGotFocus(e);

        public new void OnGroupCollapsedStateChanged(ListViewGroupEventArgs e) => base.OnGroupCollapsedStateChanged(e);

        public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);

        public new void OnRightToLeftLayoutChanged(EventArgs e) => base.OnRightToLeftLayoutChanged(e);

        public new void OnBeforeLabelEdit(LabelEditEventArgs e) => base.OnBeforeLabelEdit(e);

        public new void OnAfterLabelEdit(LabelEditEventArgs e) => base.OnAfterLabelEdit(e);

        public new void OnCacheVirtualItems(CacheVirtualItemsEventArgs e) => base.OnCacheVirtualItems(e);

        public new void OnItemChecked(ItemCheckedEventArgs e) => base.OnItemChecked(e);
    }

    private SubListView GetSubListViewWithData(View view, bool virtualMode, bool showGroups, bool withinGroup, bool createControl)
    {
        SubListView listView = new()
        {
            View = view,
            ShowGroups = showGroups,
            VirtualMode = virtualMode,
            VirtualListSize = 2
        };

        ListViewItem listItem1 = new("Test Item 1");
        ListViewItem listItem2 = new("Test Item 2");

        if (!virtualMode && withinGroup)
        {
            ListViewGroup listViewGroup = new("Test");
            listView.Groups.Add(listViewGroup);
            listItem2.Group = listViewGroup;
        }

        listView.Columns.Add(new ColumnHeader() { Name = "Column 1" });

        if (virtualMode)
        {
            listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    0 => listItem1,
                    1 => listItem2,
                    _ => throw new NotImplementedException()
                };
            };

            listItem1.SetItemIndex(listView, 0);
            listItem2.SetItemIndex(listView, 1);
        }
        else
        {
            listView.Items.Add(listItem1);
            listView.Items.Add(listItem2);
        }

        if (createControl)
        {
            listView.CreateControl();
        }

        return listView;
    }
}
