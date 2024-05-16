// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.TestUtilities;

namespace System.Windows.Forms.Tests;

public class DomainUpDownTests : IDisposable
{
    private readonly DomainUpDown _control;
    private readonly SubDomainUpDown _sub;

    public DomainUpDownTests()
    {
        _control = new DomainUpDown();
        _sub= new SubDomainUpDown();
    }

    public void Dispose()
    {
       _control.Items.Clear();
       _control.Dispose();
       _sub.Items.Clear();
       _sub.Dispose();
    }

    [WinFormsFact]
    public void DomainUpDown_Ctor_Default()
    {
        Assert.Null(_sub.ActiveControl);
        Assert.False(_sub.AllowDrop);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, _sub.Anchor);
        Assert.False(_sub.AutoScroll);
        Assert.Equal(SizeF.Empty, _sub.AutoScaleDimensions);
        Assert.Equal(new SizeF(1, 1), _sub.AutoScaleFactor);
        Assert.Equal(Size.Empty, _sub.AutoScrollMargin);
        Assert.Equal(AutoScaleMode.Inherit, _sub.AutoScaleMode);
        Assert.Equal(Size.Empty, _sub.AutoScrollMinSize);
        Assert.Equal(Point.Empty, _sub.AutoScrollPosition);
        Assert.False(_sub.AutoSize);
        Assert.Equal(SystemColors.Window, _sub.BackColor);
        Assert.Null(_sub.BackgroundImage);
        Assert.Equal(ImageLayout.Tile, _sub.BackgroundImageLayout);
        Assert.NotNull(_sub.BindingContext);
        Assert.Same(_sub.BindingContext, _sub.BindingContext);
        Assert.Equal(BorderStyle.Fixed3D, _sub.BorderStyle);
        Assert.Equal(_sub.PreferredHeight, _sub.Bottom);
        Assert.Equal(new Rectangle(0, 0, 120, _sub.PreferredHeight), _sub.Bounds);
        Assert.False(_sub.CanEnableIme);
        Assert.False(_sub.CanFocus);
        Assert.True(_sub.CanRaiseEvents);
        Assert.True(_sub.CausesValidation);
        Assert.False(_sub.ChangingText);
        if (Application.UseVisualStyles)
        {
            Assert.Equal(new Rectangle(0, 0, 120, Control.DefaultFont.Height + 7), _sub.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 120, Control.DefaultFont.Height + 7), _sub.DisplayRectangle);
            Assert.Equal(new Size(120, Control.DefaultFont.Height + 7), _sub.ClientSize);
            Assert.Equal(new Size(16, _sub.PreferredHeight), _sub.PreferredSize);
        }
        else
        {
            Assert.Equal(new Rectangle(0, 0, 116, Control.DefaultFont.Height + 3), _sub.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 116, Control.DefaultFont.Height + 3), _sub.DisplayRectangle);
            Assert.Equal(new Size(116, Control.DefaultFont.Height + 3), _sub.ClientSize);
            Assert.Equal(new Size(20, _sub.PreferredHeight), _sub.PreferredSize);
        }

        Assert.Null(_sub.Container);
        Assert.False(_sub.ContainsFocus);
        Assert.Null(_sub.ContextMenuStrip);
        Assert.NotEmpty(_sub.Controls);
        Assert.Same(_sub.Controls, _sub.Controls);
        Assert.False(_sub.Created);
        Assert.Equal(SizeF.Empty, _sub.CurrentAutoScaleDimensions);
        Assert.Equal(Cursors.Default, _sub.Cursor);
        Assert.Equal(Cursors.Default, _sub.DefaultCursor);
        Assert.Equal(ImeMode.Inherit, _sub.DefaultImeMode);
        Assert.Equal(new Padding(3), _sub.DefaultMargin);
        Assert.Equal(Size.Empty, _sub.DefaultMaximumSize);
        Assert.Equal(Size.Empty, _sub.DefaultMinimumSize);
        Assert.Equal(Padding.Empty, _sub.DefaultPadding);
        Assert.Equal(new Size(120, _sub.PreferredHeight), _sub.DefaultSize);
        Assert.False(_sub.DesignMode);
        Assert.Equal(DockStyle.None, _sub.Dock);
        Assert.NotNull(_sub.DockPadding);
        Assert.Same(_sub.DockPadding, _sub.DockPadding);
        Assert.Equal(0, _sub.DockPadding.Top);
        Assert.Equal(0, _sub.DockPadding.Bottom);
        Assert.Equal(0, _sub.DockPadding.Left);
        Assert.Equal(0, _sub.DockPadding.Right);
        Assert.False(_sub.DoubleBuffered);
        Assert.True(_sub.Enabled);
        Assert.NotNull(_sub.Events);
        Assert.Same(_sub.Events, _sub.Events);
        Assert.False(_sub.Focused);
        Assert.Equal(Control.DefaultFont, _sub.Font);
        Assert.Equal(_sub.Font.Height, _sub.FontHeight);
        Assert.Equal(SystemColors.WindowText, _sub.ForeColor);
        Assert.True(_sub.HasChildren);
        Assert.Equal(_sub.PreferredHeight, _sub.Height);
        Assert.NotNull(_sub.HorizontalScroll);
        Assert.Same(_sub.HorizontalScroll, _sub.HorizontalScroll);
        Assert.False(_sub.HScroll);
        Assert.Equal(ImeMode.NoControl, _sub.ImeMode);
        Assert.Equal(ImeMode.NoControl, _sub.ImeModeBase);
        Assert.True(_sub.InterceptArrowKeys);
        Assert.False(_sub.InvokeRequired);
        Assert.False(_sub.IsAccessible);
        Assert.False(_sub.IsMirrored);
        Assert.Empty(_sub.Items);
        Assert.Same(_sub.Items, _sub.Items);
        Assert.NotNull(_sub.LayoutEngine);
        Assert.Same(_sub.LayoutEngine, _sub.LayoutEngine);
        Assert.Equal(0, _sub.Left);
        Assert.Equal(Point.Empty, _sub.Location);
        Assert.Equal(new Padding(3), _sub.Margin);
        Assert.Equal(Size.Empty, _sub.MaximumSize);
        Assert.Equal(Size.Empty, _sub.MinimumSize);
        Assert.Equal(Padding.Empty, _sub.Padding);
        Assert.Null(_sub.Parent);
        Assert.Equal(Control.DefaultFont.Height + 7, _sub.PreferredHeight);
        Assert.Equal("Microsoft\u00AE .NET", _sub.ProductName);
        Assert.False(_sub.ReadOnly);
        Assert.False(_sub.RecreatingHandle);
        Assert.Null(_sub.Region);
        Assert.True(_sub.ResizeRedraw);
        Assert.Equal(120, _sub.Right);
        Assert.Equal(RightToLeft.No, _sub.RightToLeft);
        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.Null(_sub.SelectedItem);
        Assert.True(_sub.ShowFocusCues);
        Assert.True(_sub.ShowKeyboardCues);
        Assert.Null(_sub.Site);
        Assert.Equal(new Size(120, _sub.PreferredHeight), _sub.Size);
        Assert.Equal(0, _sub.TabIndex);
        Assert.True(_sub.TabStop);
        Assert.Empty(_sub.Text);
        Assert.Equal(HorizontalAlignment.Left, _sub.TextAlign);
        Assert.Equal(0, _sub.Top);
        Assert.Null(_sub.TopLevelControl);
        Assert.Equal(LeftRightAlignment.Right, _sub.UpDownAlign);
        Assert.False(_sub.UserEdit);
        Assert.False(_sub.UseWaitCursor);
        Assert.True(_sub.Visible);
        Assert.NotNull(_sub.VerticalScroll);
        Assert.Same(_sub.VerticalScroll, _sub.VerticalScroll);
        Assert.False(_sub.VScroll);
        Assert.Equal(120, _sub.Width);
        Assert.False(_sub.Wrap);

        Assert.False(_sub.IsHandleCreated);
    }

    [WinFormsFact]
    public void DomainUpDown_CreateParams_GetDefault_ReturnsExpected()
    {
        CreateParams createParams = _sub.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Null(createParams.ClassName);

        Assert.Equal(WNDCLASS_STYLES.CS_DBLCLKS, (WNDCLASS_STYLES)createParams.ClassStyle);
        Assert.Equal(WINDOW_STYLE.WS_MAXIMIZEBOX | WINDOW_STYLE.WS_CLIPCHILDREN | WINDOW_STYLE.WS_CLIPSIBLINGS
            | WINDOW_STYLE.WS_VISIBLE | WINDOW_STYLE.WS_CHILD, (WINDOW_STYLE)createParams.Style);

        if (Application.UseVisualStyles)
        {
            Assert.Equal(WINDOW_EX_STYLE.WS_EX_CONTROLPARENT, (WINDOW_EX_STYLE)createParams.ExStyle);
        }
        else
        {
            Assert.Equal(WINDOW_EX_STYLE.WS_EX_CLIENTEDGE | WINDOW_EX_STYLE.WS_EX_CONTROLPARENT, (WINDOW_EX_STYLE)createParams.ExStyle);
        }

        Assert.Equal(_sub.PreferredHeight, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(120, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, _sub.CreateParams);
        Assert.False(_sub.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaddingNormalizedTheoryData))]
    public void DomainUpDown_Padding_Set_GetReturnsExpected(Padding value, Padding expected)
    {
        _control.Padding = value;

        Assert.Equal(expected, _control.Padding);
        Assert.False(_control.IsHandleCreated);

        // Set same.
        _control.Padding = value;
        Assert.Equal(expected, _control.Padding);
        Assert.False(_control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaddingNormalizedTheoryData))]
    public void DomainUpDown_Padding_SetWithHandle_GetReturnsExpected(Padding value, Padding expected)
    {
        Assert.NotEqual(IntPtr.Zero, _control.Handle);
        int invalidatedCallCount = 0;
        _control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        _control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        _control.HandleCreated += (sender, e) => createdCallCount++;

        _control.Padding = value;
        Assert.Equal(expected, _control.Padding);
        Assert.True(_control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        _control.Padding = value;
        Assert.Equal(expected, _control.Padding);
        Assert.True(_control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void DomainUpDown_Padding_SetWithHandler_CallsPaddingChanged()
    {
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Equal(_control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        _control.PaddingChanged += handler;

        // Set different.
        Padding padding1 = new(1);
        _control.Padding = padding1;
        Assert.Equal(padding1, _control.Padding);
        Assert.Equal(1, callCount);

        // Set same.
        _control.Padding = padding1;
        Assert.Equal(padding1, _control.Padding);
        Assert.Equal(1, callCount);

        // Set different.
        Padding padding2 = new(2);
        _control.Padding = padding2;
        Assert.Equal(padding2, _control.Padding);
        Assert.Equal(2, callCount);

        // Remove handler.
        _control.PaddingChanged -= handler;
        _control.Padding = padding1;
        Assert.Equal(padding1, _control.Padding);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void DomainUpDown_SelectedIndex_SetEmpty_Nop()
    {
        _sub.SelectedIndex = -1;

        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.Null(_sub.SelectedItem);
        Assert.Empty(_sub.Text);
        Assert.False(_sub.UserEdit);
        Assert.False(_sub.ChangingText);
        Assert.False(_sub.IsHandleCreated);

        // Set same.
        _sub.SelectedIndex = -1;
        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.Null(_sub.SelectedItem);
        Assert.Empty(_sub.Text);
        Assert.False(_sub.UserEdit);
        Assert.False(_sub.ChangingText);
        Assert.False(_sub.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0, "Item1", "Item1", true)]
    [InlineData(1, "Item2", "Item2", true)]
    [InlineData(-1, null, "", false)]
    public void DomainUpDown_SelectedIndex_SetNotEmpty_GetReturnsExpected(int value, object expected, string expectedText, bool expectedUserEdit)
    {
        _sub.Items.Add("Item1");
        _sub.Items.Add("Item2");

        _sub.SelectedIndex = value;
        Assert.Equal(value, _sub.SelectedIndex);
        Assert.Equal(expected, _sub.SelectedItem);
        Assert.Equal(expectedText, _sub.Text);
        Assert.False(_sub.UserEdit);
        Assert.False(_sub.ChangingText);
        Assert.False(_sub.IsHandleCreated);

        // Set same.
        _sub.SelectedIndex = value;
        Assert.Equal(value, _sub.SelectedIndex);
        Assert.Equal(expected, _sub.SelectedItem);
        Assert.Equal(expectedText, _sub.Text);
        Assert.False(_sub.UserEdit);
        Assert.False(_sub.ChangingText);
        Assert.False(_sub.IsHandleCreated);

        // Set none.
        _sub.SelectedIndex = -1;
        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.Null(_sub.SelectedItem);
        Assert.Equal(expectedText, _sub.Text);
        Assert.Equal(expectedUserEdit, _sub.UserEdit);
        Assert.False(_sub.ChangingText);
        Assert.False(_sub.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0, "Item1", "Item1", true)]
    [InlineData(1, "Item2", "Item2", true)]
    [InlineData(-1, null, "", false)]
    public void DomainUpDown_SelectedIndex_SetUserEdit_GetReturnsExpected(int value, object expected, string expectedText, bool expectedUserEdit)
    {
        _sub.UserEdit=true;
        _sub.Items.Add("Item1");
        _sub.Items.Add("Item2");

        _sub.SelectedIndex = value;
        Assert.Equal(value, _sub.SelectedIndex);
        Assert.Equal(expected, _sub.SelectedItem);
        Assert.Equal(expectedText, _sub.Text);
        Assert.Equal(!expectedUserEdit, _sub.UserEdit);
        Assert.False(_sub.ChangingText);
        Assert.False(_sub.IsHandleCreated);

        // Set same.
        _sub.SelectedIndex = value;
        Assert.Equal(value, _sub.SelectedIndex);
        Assert.Equal(expected, _sub.SelectedItem);
        Assert.Equal(expectedText, _sub.Text);
        Assert.Equal(!expectedUserEdit, _sub.UserEdit);
        Assert.False(_sub.ChangingText);
        Assert.False(_sub.IsHandleCreated);

        // Set none.
        _sub.SelectedIndex = -1;
        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.Null(_sub.SelectedItem);
        Assert.Equal(expectedText, _sub.Text);
        Assert.True(_sub.UserEdit);
        Assert.False(_sub.ChangingText);
        Assert.False(_sub.IsHandleCreated);
    }

    [WinFormsFact]
    public void DomainUpDown_SelectedIndex_SetWithHandler_CallsSelectedItemChanged()
    {
        _control.Items.Add("Item1");
        _control.Items.Add("Item2");

        int textChangedCallCount = 0;
        int callCount = 0;
        EventHandler textChangedHandler = (sender, e) =>
        {
            Assert.Same(_control, sender);
            Assert.Same(EventArgs.Empty, e);
            textChangedCallCount++;
        };
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(_control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(textChangedCallCount - 1, callCount);
            callCount++;
        };
        _control.TextChanged += textChangedHandler;
        _control.SelectedItemChanged += handler;

        // Set different.
        _control.SelectedIndex = 0;
        Assert.Equal(0, _control.SelectedIndex);
        Assert.Equal(1, textChangedCallCount);
        Assert.Equal(1, callCount);

        // Set same.
        _control.SelectedIndex = 0;
        Assert.Equal(0, _control.SelectedIndex);
        Assert.Equal(1, textChangedCallCount);
        Assert.Equal(1, callCount);

        // Set different.
        _control.SelectedIndex = 1;
        Assert.Equal(1, _control.SelectedIndex);
        Assert.Equal(2, textChangedCallCount);
        Assert.Equal(2, callCount);

        // Set none.
        _control.SelectedIndex = -1;
        Assert.Equal(-1, _control.SelectedIndex);
        Assert.Equal(2, textChangedCallCount);
        Assert.Equal(2, callCount);

        // Remove handler.
        _control.TextChanged -= textChangedHandler;
        _control.SelectedItemChanged -= handler;
        _control.SelectedIndex = 0;
        Assert.Equal(0, _control.SelectedIndex);
        Assert.Equal(2, textChangedCallCount);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InlineData(-2)]
    [InlineData(0)]
    [InlineData(1)]
    public void DomainUpDown_SelectedIndex_SetInvalidValueEmpty_ThrowsArgumentOutOfRangeException(int value)
    {
        Assert.Throws<ArgumentOutOfRangeException>("value", () => _control.SelectedIndex = value);
    }

    [WinFormsTheory]
    [InlineData(-2)]
    [InlineData(1)]
    [InlineData(2)]
    public void DomainUpDown_SelectedIndex_SetInvalidValueNotEmpty_ThrowsArgumentOutOfRangeException(int value)
    {
        _control.Items.Add("Item");
        Assert.Throws<ArgumentOutOfRangeException>("value", () => _control.SelectedIndex = value);
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("NoSuchItem")]
    public void DomainUpDown_SelectedItem_SetEmpty_Nop(object value)
    {
        _sub.SelectedItem = value;
        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.Null(_sub.SelectedItem);
        Assert.Empty(_sub.Text);
        Assert.False(_sub.UserEdit);
        Assert.False(_sub.ChangingText);
        Assert.False(_sub.IsHandleCreated);

        // Set same.
        _sub.SelectedItem = value;
        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.Null(_sub.SelectedItem);
        Assert.Empty(_sub.Text);
        Assert.False(_sub.UserEdit);
        Assert.False(_sub.ChangingText);
        Assert.False(_sub.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData("Item1", 0, "Item1", "Item1", true)]
    [InlineData("Item2", 1, "Item2", "Item2", true)]
    [InlineData("NoSuchItem", -1, null, "", false)]
    [InlineData(null, -1, null, "", false)]
    public void DomainUpDown_SelectedItem_SetNotEmpty_GetReturnsExpected(object value, int expectedSelectedIndex, object expected, string expectedText, bool expectedUserEdit)
    {
        _sub.Items.Add("Item1");
        _sub.Items.Add("Item2");

        _sub.SelectedItem = value;
        Assert.Equal(expectedSelectedIndex, _sub.SelectedIndex);
        Assert.Equal(expected, _sub.SelectedItem);
        Assert.Equal(expectedText, _sub.Text);
        Assert.False(_sub.UserEdit);
        Assert.False(_sub.ChangingText);
        Assert.False(_sub.IsHandleCreated);

        // Set same.
        _sub.SelectedItem = value;
        Assert.Equal(expectedSelectedIndex, _sub.SelectedIndex);
        Assert.Equal(expected, _sub.SelectedItem);
        Assert.Equal(expectedText, _sub.Text);
        Assert.False(_sub.UserEdit);
        Assert.False(_sub.ChangingText);
        Assert.False(_sub.IsHandleCreated);

        // Set no such item.
        _sub.SelectedItem = "NoSuchItem";
        Assert.Equal(expectedSelectedIndex, _sub.SelectedIndex);
        Assert.Equal(expected, _sub.SelectedItem);
        Assert.Equal(expectedText, _sub.Text);
        Assert.False(_sub.UserEdit);
        Assert.False(_sub.ChangingText);
        Assert.False(_sub.IsHandleCreated);

        // Set none.
        _sub.SelectedItem = null;
        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.Null(_sub.SelectedItem);
        Assert.Equal(expectedText, _sub.Text);
        Assert.Equal(expectedUserEdit, _sub.UserEdit);
        Assert.False(_sub.ChangingText);
        Assert.False(_sub.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData("Item1", 0, "Item1", "Item1", true)]
    [InlineData("Item2", 1, "Item2", "Item2", true)]
    [InlineData("NoSuchItem", -1, null, "", false)]
    [InlineData(null, -1, null, "", false)]
    public void DomainUpDown_SelectedItem_SetUserEdit_GetReturnsExpected(object value, int expectedSelectedIndex, object expected, string expectedText, bool expectedUserEdit)
    {
        _sub.UserEdit = true;
        _sub.Items.Add("Item1");
        _sub.Items.Add("Item2");

        _sub.SelectedItem = value;
        Assert.Equal(expectedSelectedIndex, _sub.SelectedIndex);
        Assert.Equal(expected, _sub.SelectedItem);
        Assert.Equal(expectedText, _sub.Text);
        Assert.Equal(!expectedUserEdit, _sub.UserEdit);
        Assert.False(_sub.ChangingText);
        Assert.False(_sub.IsHandleCreated);

        // Set same.
        _sub.SelectedItem = value;
        Assert.Equal(expectedSelectedIndex, _sub.SelectedIndex);
        Assert.Equal(expected, _sub.SelectedItem);
        Assert.Equal(expectedText, _sub.Text);
        Assert.Equal(!expectedUserEdit, _sub.UserEdit);
        Assert.False(_sub.ChangingText);
        Assert.False(_sub.IsHandleCreated);

        // Set no such item.
        _sub.SelectedItem = "NoSuchItem";
        Assert.Equal(expectedSelectedIndex, _sub.SelectedIndex);
        Assert.Equal(expected, _sub.SelectedItem);
        Assert.Equal(expectedText, _sub.Text);
        Assert.Equal(!expectedUserEdit, _sub.UserEdit);
        Assert.False(_sub.ChangingText);
        Assert.False(_sub.IsHandleCreated);

        // Set none.
        _sub.SelectedItem = null;
        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.Null(_sub.SelectedItem);
        Assert.Equal(expectedText, _sub.Text);
        Assert.True(_sub.UserEdit);
        Assert.False(_sub.ChangingText);
        Assert.False(_sub.IsHandleCreated);
    }

    [WinFormsFact]
    public void DomainUpDown_SelectedItem_SetWithHandler_CallsSelectedItemChanged()
    {
        _control.Items.Add("Item1");
        _control.Items.Add("Item2");

        int textChangedCallCount = 0;
        int callCount = 0;
        EventHandler textChangedHandler = (sender, e) =>
        {
            Assert.Same(_control, sender);
            Assert.Same(EventArgs.Empty, e);
            textChangedCallCount++;
        };
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(_control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(textChangedCallCount - 1, callCount);
            callCount++;
        };
        _control.TextChanged += textChangedHandler;
        _control.SelectedItemChanged += handler;

        // Set different.
        _control.SelectedItem = "Item1";
        Assert.Equal("Item1", _control.SelectedItem);
        Assert.Equal(1, textChangedCallCount);
        Assert.Equal(1, callCount);

        // Set same.
        _control.SelectedItem = "Item1";
        Assert.Equal("Item1", _control.SelectedItem);
        Assert.Equal(1, textChangedCallCount);
        Assert.Equal(1, callCount);

        // Set different.
        _control.SelectedItem = "Item2";
        Assert.Equal("Item2", _control.SelectedItem);
        Assert.Equal(2, textChangedCallCount);
        Assert.Equal(2, callCount);

        // Set none.
        _control.SelectedItem = null;
        Assert.Null(_control.SelectedItem);
        Assert.Equal(2, textChangedCallCount);
        Assert.Equal(2, callCount);

        // Remove handler.
        _control.TextChanged -= textChangedHandler;
        _control.SelectedItemChanged -= handler;
        _control.SelectedItem = "Item1";
        Assert.Equal("Item1", _control.SelectedItem);
        Assert.Equal(2, textChangedCallCount);
        Assert.Equal(2, callCount);
    }

    public static IEnumerable<object[]> Sorted_Set_TestData()
    {
        foreach (bool userEdit in new bool[] { true, false })
        {
            yield return new object[] { userEdit, true };
            yield return new object[] { userEdit, false };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Sorted_Set_TestData))]
    public void DomainUpDown_Sorted_Set_GetReturnsExpected(bool userEdit, bool value)
    {
        _sub.UserEdit = userEdit;
        _sub.Sorted = value;

        Assert.Equal(value, _sub.Sorted);
        Assert.Empty(_sub.Items);
        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.Equal(userEdit, _sub.UserEdit);
        Assert.False(_sub.IsHandleCreated);

        // Set same.
        _sub.Sorted = value;
        Assert.Equal(value, _sub.Sorted);
        Assert.Empty(_sub.Items);
        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.Equal(userEdit, _sub.UserEdit);
        Assert.False(_sub.IsHandleCreated);

        // Set different.
        _sub.Sorted = !value;
        Assert.Equal(!value, _sub.Sorted);
        Assert.Empty(_sub.Items);
        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.Equal(userEdit, _sub.UserEdit);
        Assert.False(_sub.IsHandleCreated);
    }

    public static IEnumerable<object[]> Sorted_WithItems_TestData()
    {
        foreach (bool userEdit in new bool[] { true, false })
        {
            yield return new object[] { userEdit, true, new string[] { "a", "a", "B", "c", "d" } };
            yield return new object[] { userEdit, false, new string[] { "c", "B", "a", "a", "d" } };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Sorted_WithItems_TestData))]
    public void DomainUpDown_Sorted_SetWithItems_GetReturnsExpected(bool userEdit, bool value, string[] expectedItems)
    {
        _sub.Items.Add("c");
        _sub.Items.Add("B");
        _sub.Items.Add("a");
        _sub.Items.Add("a");
        _sub.Items.Add("d");
        _sub.UserEdit = userEdit;

        _sub.Sorted = value;
        Assert.Equal(value, _sub.Sorted);
        Assert.Equal(expectedItems, _sub.Items.Cast<string>());
        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.False(_sub.IsHandleCreated);

        // Set same.
        _sub.Sorted = value;
        Assert.Equal(value, _sub.Sorted);
        Assert.Equal(expectedItems, _sub.Items.Cast<string>());
        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.Equal(userEdit, _sub.UserEdit);
        Assert.False(_sub.IsHandleCreated);

        // Set different.
        _sub.Sorted = !value;
        Assert.Equal(!value, _sub.Sorted);
        Assert.Equal(new string[] { "a", "a", "B", "c", "d" }, _sub.Items.Cast<string>());
        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.Equal(userEdit, _sub.UserEdit);
        Assert.False(_sub.IsHandleCreated);
    }

    public static IEnumerable<object[]> Sorted_WithItemsWithSelection_TestData()
    {
        yield return new object[] { true, true, new string[] { "a", "a", "B", "c", "d" }, -1 };
        yield return new object[] { false, true, new string[] { "a", "a", "B", "c", "d" }, 1 };
        yield return new object[] { true, false, new string[] { "c", "B", "a", "a", "d" }, -1 };
        yield return new object[] { false, false, new string[] { "c", "B", "a", "a", "d" }, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Sorted_WithItemsWithSelection_TestData))]
    public void DomainUpDown_Sorted_SetWithItemsWithSelection_GetReturnsExpected(bool userEdit, bool value, string[] expectedItems, int expectedSelectedIndex)
    {
        _sub.Items.Add("c");
        _sub.Items.Add("B");
        _sub.Items.Add("a");
        _sub.Items.Add("a");
        _sub.Items.Add("d");
        _sub.SelectedItem = "B";
        _sub.UserEdit = userEdit;

        _sub.Sorted = value;
        Assert.Equal(value, _sub.Sorted);
        Assert.Equal(expectedItems, _sub.Items.Cast<string>());
        Assert.Equal(expectedSelectedIndex, _sub.SelectedIndex);
        Assert.False(_sub.IsHandleCreated);

        // Set same.
        _sub.Sorted = value;
        Assert.Equal(value, _sub.Sorted);
        Assert.Equal(expectedItems, _sub.Items.Cast<string>());
        Assert.Equal(expectedSelectedIndex, _sub.SelectedIndex);
        Assert.Equal(userEdit, _sub.UserEdit);
        Assert.False(_sub.IsHandleCreated);

        // Set different.
        _sub.Sorted = !value;
        Assert.Equal(!value, _sub.Sorted);
        Assert.Equal(new string[] { "a", "a", "B", "c", "d" }, _sub.Items.Cast<string>());
        Assert.Equal(expectedSelectedIndex, _sub.SelectedIndex);
        Assert.Equal(userEdit, _sub.UserEdit);
        Assert.False(_sub.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Sorted_Set_TestData))]
    public void DomainUpDown_Sorted_SetWithHandle_GetReturnsExpected(bool userEdit, bool value)
    {
        _sub.UserEdit = userEdit;
        Assert.NotEqual(IntPtr.Zero, _sub.Handle);
        int invalidatedCallCount = 0;
        _sub.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        _sub.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        _sub.HandleCreated += (sender, e) => createdCallCount++;

        _sub.Sorted = value;
        Assert.Equal(value, _sub.Sorted);
        Assert.Empty(_sub.Items);
        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.Equal(userEdit, _sub.UserEdit);
        Assert.True(_sub.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        _sub.Sorted = value;
        Assert.Equal(value, _sub.Sorted);
        Assert.Empty(_sub.Items);
        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.Equal(userEdit, _sub.UserEdit);
        Assert.True(_sub.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        _sub.Sorted = !value;
        Assert.Equal(!value, _sub.Sorted);
        Assert.Empty(_sub.Items);
        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.Equal(userEdit, _sub.UserEdit);
        Assert.True(_sub.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Sorted_WithItems_TestData))]
    public void DomainUpDown_Sorted_SetWithItemsWithHandle_GetReturnsExpected(bool userEdit, bool value, string[] expectedItems)
    {
        _sub.Items.Add("c");
        _sub.Items.Add("B");
        _sub.Items.Add("a");
        _sub.Items.Add("a");
        _sub.Items.Add("d");
        Assert.NotEqual(IntPtr.Zero, _sub.Handle);
        int invalidatedCallCount = 0;
        _sub.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        _sub.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        _sub.HandleCreated += (sender, e) => createdCallCount++;
        _sub.UserEdit = userEdit;

        _sub.Sorted = value;
        Assert.Equal(value, _sub.Sorted);
        Assert.Equal(expectedItems, _sub.Items.Cast<string>());
        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.True(_sub.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        _sub.Sorted = value;
        Assert.Equal(value, _sub.Sorted);
        Assert.Equal(expectedItems, _sub.Items.Cast<string>());
        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.Equal(userEdit, _sub.UserEdit);
        Assert.True(_sub.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        _sub.Sorted = !value;
        Assert.Equal(!value, _sub.Sorted);
        Assert.Equal(new string[] { "a", "a", "B", "c", "d" }, _sub.Items.Cast<string>());
        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.Equal(userEdit, _sub.UserEdit);
        Assert.True(_sub.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Sorted_WithItemsWithSelection_TestData))]
    public void DomainUpDown_Sorted_SetWithItemsWithSelectionWithHandle_GetReturnsExpected(bool userEdit, bool value, string[] expectedItems, int expectedSelectedIndex)
    {
        _sub.Items.Add("c");
        _sub.Items.Add("B");
        _sub.Items.Add("a");
        _sub.Items.Add("a");
        _sub.Items.Add("d");
        Assert.NotEqual(IntPtr.Zero, _sub.Handle);
        int invalidatedCallCount = 0;
        _sub.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        _sub.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        _sub.HandleCreated += (sender, e) => createdCallCount++;
        _sub.SelectedItem = "B";
        _sub.UserEdit = userEdit;

        _sub.Sorted = value;
        Assert.Equal(value, _sub.Sorted);
        Assert.Equal(expectedItems, _sub.Items.Cast<string>());
        Assert.Equal(expectedSelectedIndex, _sub.SelectedIndex);
        Assert.True(_sub.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        _sub.Sorted = value;
        Assert.Equal(value, _sub.Sorted);
        Assert.Equal(expectedItems, _sub.Items.Cast<string>());
        Assert.Equal(expectedSelectedIndex, _sub.SelectedIndex);
        Assert.Equal(userEdit, _sub.UserEdit);
        Assert.True(_sub.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        _sub.Sorted = !value;
        Assert.Equal(!value, _sub.Sorted);
        Assert.Equal(new string[] { "a", "a", "B", "c", "d" }, _sub.Items.Cast<string>());
        Assert.Equal(expectedSelectedIndex, _sub.SelectedIndex);
        Assert.Equal(userEdit, _sub.UserEdit);
        Assert.True(_sub.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void DomainUpDown_Wrap_Set_GetReturnsExpected(bool value)
    {
        _control.Wrap = value;

        Assert.Equal(value, _control.Wrap);
        Assert.False(_control.IsHandleCreated);

        // Set same.
        _control.Wrap = value;
        Assert.Equal(value, _control.Wrap);
        Assert.False(_control.IsHandleCreated);

        // Set different.
        _control.Wrap = !value;
        Assert.Equal(!value, _control.Wrap);
        Assert.False(_control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void DomainUpDown_Wrap_SetWithHandle_GetReturnsExpected(bool value)
    {
        Assert.NotEqual(IntPtr.Zero, _control.Handle);
        int invalidatedCallCount = 0;
        _control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        _control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        _control.HandleCreated += (sender, e) => createdCallCount++;

        _control.Wrap = value;
        Assert.Equal(value, _control.Wrap);
        Assert.True(_control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        _control.Wrap = value;
        Assert.Equal(value, _control.Wrap);
        Assert.True(_control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        _control.Wrap = !value;
        Assert.Equal(!value, _control.Wrap);
        Assert.True(_control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void DomainUpDown_CreateAccessibilityInstance_Invoke_ReturnsExpected()
    {
        Control.ControlAccessibleObject instance = Assert.IsAssignableFrom<Control.ControlAccessibleObject>(_sub.CreateAccessibilityInstance());
        Assert.NotNull(instance);
        Assert.Same(_sub, instance.Owner);
        Assert.Equal(AccessibleRole.SpinButton, instance.Role);
        Assert.NotSame(_sub.CreateAccessibilityInstance(), instance);
        Assert.NotSame(_sub.AccessibilityObject, instance);
    }

    [WinFormsFact]
    public void DomainUpDown_CreateAccessibilityInstance_InvokeWithCustomRole_ReturnsExpected()
    {
        _sub.AccessibleRole = AccessibleRole.HelpBalloon;
        Control.ControlAccessibleObject instance = Assert.IsAssignableFrom<Control.ControlAccessibleObject>(_sub.CreateAccessibilityInstance());
        Assert.NotNull(instance);
        Assert.Same(_sub, instance.Owner);
        Assert.Equal(AccessibleRole.HelpBalloon, instance.Role);
        Assert.NotSame(_sub.CreateAccessibilityInstance(), instance);
        Assert.NotSame(_sub.AccessibilityObject, instance);
    }

    public static IEnumerable<object[]> DownButton_TestData()
    {
        foreach (bool userEdit in new bool[] { true, false })
        {
            foreach (bool wrap in new bool[] { true, false })
            {
                yield return new object[] { userEdit, wrap };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DownButton_TestData))]
    public void DomainUpDown_DownButton_InvokeWithoutItems_Nop(bool userEdit, bool wrap)
    {
        _sub.UserEdit = userEdit;
        _sub.Wrap = wrap;

        _sub.DownButton();
        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.Equal(userEdit, _sub.UserEdit);

        // Call again.
        _sub.DownButton();
        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.Equal(userEdit, _sub.UserEdit);
    }

    [WinFormsTheory]
    [MemberData(nameof(DownButton_TestData))]
    public void DomainUpDown_DownButton_InvokeEmpty_Nop(bool userEdit, bool wrap)
    {
        _sub.UserEdit = userEdit;
        _sub.Wrap = wrap;

        Assert.Empty(_sub.Items);

        _sub.DownButton();
        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.Equal(userEdit, _sub.UserEdit);

        // Call again.
        _sub.DownButton();
        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.Equal(userEdit, _sub.UserEdit);
    }

    public static IEnumerable<object[]> DownButton_WithItems_TestData()
    {
        yield return new object[] { true, true, 0 };
        yield return new object[] { true, false, 2 };
        yield return new object[] { false, true, 0 };
        yield return new object[] { false, false, 2 };
    }

    [WinFormsTheory]
    [MemberData(nameof(DownButton_WithItems_TestData))]
    public void DomainUpDown_DownButton_InvokeWithItems_Nop(bool userEdit, bool wrap, int expectedWrapSelectedIndex)
    {
        _sub.UserEdit = userEdit;
        _sub.Wrap = wrap;
        _sub.Items.Add("a");
        _sub.Items.Add("b");
        _sub.Items.Add("c");

        _sub.DownButton();
        Assert.Equal(0, _sub.SelectedIndex);
        Assert.False(_sub.UserEdit);

        // Call again.
        _sub.DownButton();
        Assert.Equal(1, _sub.SelectedIndex);
        Assert.False(_sub.UserEdit);

        // Call again.
        _sub.DownButton();
        Assert.Equal(2, _sub.SelectedIndex);
        Assert.False(_sub.UserEdit);

        // Call again.
        _sub.DownButton();
        Assert.Equal(expectedWrapSelectedIndex, _sub.SelectedIndex);
        Assert.False(_sub.UserEdit);
    }

    [WinFormsFact]
    public void DomainUpDown_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        Assert.Equal(AutoSizeMode.GrowOnly, _sub.GetAutoSizeMode());
    }

    [WinFormsTheory]
    [InlineData(0, true)]
    [InlineData(SubDomainUpDown.ScrollStateAutoScrolling, false)]
    [InlineData(SubDomainUpDown.ScrollStateFullDrag, false)]
    [InlineData(SubDomainUpDown.ScrollStateHScrollVisible, false)]
    [InlineData(SubDomainUpDown.ScrollStateUserHasScrolled, false)]
    [InlineData(SubDomainUpDown.ScrollStateVScrollVisible, false)]
    [InlineData(int.MaxValue, false)]
    [InlineData((-1), false)]
    public void DomainUpDown_GetScrollState_Invoke_ReturnsExpected(int bit, bool expected)
    {
        Assert.Equal(expected, _sub.GetScrollState(bit));
    }

    [WinFormsTheory]
    [InlineData(ControlStyles.ContainerControl, true)]
    [InlineData(ControlStyles.UserPaint, true)]
    [InlineData(ControlStyles.Opaque, true)]
    [InlineData(ControlStyles.ResizeRedraw, true)]
    [InlineData(ControlStyles.FixedWidth, false)]
    [InlineData(ControlStyles.FixedHeight, true)]
    [InlineData(ControlStyles.StandardClick, false)]
    [InlineData(ControlStyles.Selectable, true)]
    [InlineData(ControlStyles.UserMouse, false)]
    [InlineData(ControlStyles.SupportsTransparentBackColor, false)]
    [InlineData(ControlStyles.StandardDoubleClick, true)]
    [InlineData(ControlStyles.AllPaintingInWmPaint, false)]
    [InlineData(ControlStyles.CacheText, false)]
    [InlineData(ControlStyles.EnableNotifyMessage, false)]
    [InlineData(ControlStyles.DoubleBuffer, false)]
    [InlineData(ControlStyles.OptimizedDoubleBuffer, false)]
    [InlineData(ControlStyles.UseTextForAccessibility, false)]
    [InlineData((ControlStyles)0, true)]
    [InlineData((ControlStyles)int.MaxValue, false)]
    [InlineData((ControlStyles)(-1), false)]
    public void DomainUpDown_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
    {
        Assert.Equal(expected, _sub.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, _sub.GetStyle(flag));
    }

    [WinFormsFact]
    public void DomainUpDown_GetTopLevel_Invoke_ReturnsExpected()
    {
        Assert.False(_sub.GetTopLevel());
    }

    [WinFormsTheory]
    [InlineData("cow", 0, 3)]
    [InlineData("cow", 4, 3)]
    [InlineData("foo", 0, 0)]
    [InlineData("foo", 3, 4)]
    [InlineData("foo", 4, 4)]
    [InlineData("foo", 5, 0)]
    [InlineData("foo", 100, 0)]
    [InlineData("foo", -1, 4)]
    [InlineData("foo", -100, 4)]
    [InlineData("foo2", 0, 1)]
    [InlineData("foo5", 0, -1)]
    [InlineData("foo5", 4, -1)]
    [InlineData("", 0, -1)]
    public void DomainUpDown_MatchIndex(string text, int start, int expected)
    {
        _control.Items.Add("foo1");
        _control.Items.Add("foo2");
        _control.Items.Add("foo3");
        _control.Items.Add("Cowman");
        _control.Items.Add("foo4");
        Assert.Equal(expected, _control.MatchIndex(text, false, start));
    }

    [WinFormsFact]
    public void DomainUpDown_MatchIndex_NullText_ThrowsNullReferenceException()
    {
        _control.Items.Add("item1");
        Assert.Throws<NullReferenceException>(() => _control.MatchIndex(null, false, 0));
    }

    public static IEnumerable<object[]> OnChanged_TestData()
    {
        yield return new object[] { null, null };
        yield return new object[] { new(), new EventArgs() };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnChanged_TestData))]
    public void DomainUpDown_OnChanged_Invoke_CallsSelectedItemChanged(object source, EventArgs eventArgs)
    {
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(_sub, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        _sub.SelectedItemChanged += handler;
        _sub.OnSelectedItemChanged(source, eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        _sub.SelectedItemChanged -= handler;
        _sub.OnSelectedItemChanged(source, eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnChanged_TestData))]
    public void DomainUpDown_OnSelectedItemChanged_Invoke_CallsSelectedItemChanged(object source, EventArgs eventArgs)
    {
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(_sub, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        _sub.SelectedItemChanged += handler;
        _sub.OnSelectedItemChanged(source, eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        _sub.SelectedItemChanged -= handler;
        _sub.OnSelectedItemChanged(source, eventArgs);
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> UpButton_TestData()
    {
        foreach (bool userEdit in new bool[] { true, false })
        {
            foreach (bool wrap in new bool[] { true, false })
            {
                yield return new object[] { userEdit, wrap };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(UpButton_TestData))]
    public void DomainUpDown_UpButton_InvokeWithoutItems_Nop(bool userEdit, bool wrap)
    {
        _sub.UserEdit = userEdit;
        _sub.Wrap = wrap;

        _sub.UpButton();
        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.Equal(userEdit, _sub.UserEdit);

        // Call again.
        _sub.UpButton();
        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.Equal(userEdit, _sub.UserEdit);
    }

    [WinFormsTheory]
    [MemberData(nameof(UpButton_TestData))]
    public void DomainUpDown_UpButton_InvokeEmpty_Nop(bool userEdit, bool wrap)
    {
         _sub.UserEdit = userEdit;
        _sub.Wrap = wrap;

        Assert.Empty(_sub.Items);

        _sub.UpButton();
        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.Equal(userEdit, _sub.UserEdit);

        // Call again.
        _sub.UpButton();
        Assert.Equal(-1, _sub.SelectedIndex);
        Assert.Equal(userEdit, _sub.UserEdit);
    }

    public static IEnumerable<object[]> UpButton_WithItems_TestData()
    {
        yield return new object[] { true, true, 2, 1 };
        yield return new object[] { true, false, 0, 0 };
        yield return new object[] { false, true, 2, 1 };
        yield return new object[] { false, false, 0, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(UpButton_WithItems_TestData))]
    public void DomainUpDown_UpButton_InvokeWithItems_Nop(bool userEdit, bool wrap, int expectedWrapSelectedIndex1, int expectedWrapSelectedIndex2)
    {
        _sub.UserEdit = userEdit;
        _sub.Wrap = wrap;
        _sub.Items.Add("a");
        _sub.Items.Add("b");
        _sub.Items.Add("c");
        _sub.SelectedIndex = 2;

        _sub.UpButton();
        Assert.Equal(1, _sub.SelectedIndex);
        Assert.False(_sub.UserEdit);

        // Call again.
        _sub.UpButton();
        Assert.Equal(0, _sub.SelectedIndex);
        Assert.False(_sub.UserEdit);

        // Call again.
        _sub.UpButton();
        Assert.Equal(expectedWrapSelectedIndex1, _sub.SelectedIndex);
        Assert.False(_sub.UserEdit);

        // Call again.
        _sub.UpButton();
        Assert.Equal(expectedWrapSelectedIndex2, _sub.SelectedIndex);
        Assert.False(_sub.UserEdit);
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void DomainUpDown_UpdateEditText_InvokeEmpty_Success(bool userEdit, bool changingText)
    {
        _sub.UserEdit = userEdit;
        _sub.ChangingText = changingText;

        _sub.UpdateEditText();
        Assert.Empty(_sub.Text);
        Assert.False(_sub.UserEdit);
        Assert.False(_sub.ChangingText);
        Assert.False(_sub.IsHandleCreated);

        // Call again.
        _sub.UpdateEditText();
        Assert.Empty(_sub.Text);
        Assert.False(_sub.UserEdit);
        Assert.False(_sub.ChangingText);
        Assert.False(_sub.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void DomainUpDown_UpdateEditText_InvokeNotEmpty_Success(bool userEdit, bool changingText)
    {
        _sub.Items.Add("Item1");
        _sub.Items.Add("Item2");
        _sub.UserEdit = userEdit;
        _sub.ChangingText = changingText;

        _sub.UpdateEditText();
        Assert.Empty(_sub.Text);
        Assert.False(_sub.UserEdit);
        Assert.False(_sub.ChangingText);
        Assert.False(_sub.IsHandleCreated);

        // Call again.
        _sub.UpdateEditText();
        Assert.Empty(_sub.Text);
        Assert.False(_sub.UserEdit);
        Assert.False(_sub.ChangingText);
        Assert.False(_sub.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void DomainUpDown_UpdateEditText_InvokeNotEmptyWithSelection_Success(bool userEdit, bool changingText)
    {
        _sub.Items.Add("Item1");
        _sub.Items.Add("Item2");
        _sub.SelectedIndex = 0;
        _sub.Text = "Text";
        _sub.UserEdit = userEdit;
        _sub.ChangingText = changingText;

        _sub.UpdateEditText();
        Assert.Equal("Item1", _sub.Text);
        Assert.False(_sub.UserEdit);
        Assert.False(_sub.ChangingText);
        Assert.False(_sub.IsHandleCreated);

        // Call again.
        _sub.UpdateEditText();
        Assert.Equal("Item1", _sub.Text);
        Assert.False(_sub.UserEdit);
        Assert.False(_sub.ChangingText);
        Assert.False(_sub.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0, 1, false)]
    [InlineData(1, 2, false)]
    [InlineData(2, 2, false)] 
    [InlineData(2, 0, true)] 
    public void DomainUpDown_DownButton_InvokeWithItems_ChangesSelectedIndex(int initialIndex, int expectedIndex, bool wrap)
    {
        _control.Items.AddRange(new string[] { "Item1", "Item2", "Item3" });

        _control.SelectedIndex = initialIndex;
        _control.Wrap = wrap;

        _control.DownButton();

        _control.SelectedIndex.Should().Be(expectedIndex);
    }

    [WinFormsTheory]
    [InlineData(0, 2, false)]
    [InlineData(1, 2, false)]
    [InlineData(2, 2, false)] 
    [InlineData(2, 1, true)] 
    public void DomainUpDown_DownButton_InvokeMultipleTimes_ChangesSelectedIndex(int initialIndex, int expectedIndex, bool wrap)
    {
        _control.Items.AddRange(new string[] { "Item1", "Item2", "Item3" });

        _control.SelectedIndex = initialIndex;
        _control.Wrap = wrap;

        _control.DownButton();
        _control.DownButton(); 

        _control.SelectedIndex.Should().Be(expectedIndex);
    }

    [WinFormsTheory]
    [InlineData(new string[] { "Item1", "Item2", "Item3" }, 1)]
    [InlineData(new string[] { "ItemA", "ItemB", "ItemC", "ItemD" }, 3)]
    public void DomainUpDown_ToString_Invoke_ReturnsExpected(string[] items, int selectedIndex)
    {
        _control.Items.Clear();
        _control.Items.AddRange(items);
        _control.SelectedIndex = selectedIndex;

        string s = _control.ToString();

        s.Should().Contain($"Items.Count: {items.Length}");
        s.Should().Contain($"SelectedIndex: {selectedIndex}");
    }

    [WinFormsTheory]
    [InlineData("Item1", 0)]
    [InlineData("Item2", 1)]
    public void DomainUpDown_UpButton_MatchFound(string text, int expectedIndex)
    {
        _control.Items.AddRange(new string[] { "Item1", "Item2", "Item3" });
        _control.Text = text;
        _control.UpButton();
        _control.SelectedIndex.Should().Be(expectedIndex);
    }

    [WinFormsFact]
    public void DomainUpDown_UpButton_NoMatchFound()
    {
        _control.Text = "NoSuchItem";
        _control.UpButton();
        _control.SelectedIndex.Should().Be(-1);
    }

    [WinFormsFact]
    public void DomainUpDown_UpButton_DecrementSelectedIndex()
    {
        _control.Items.AddRange(new string[] { "Item1", "Item2", "Item3" });
        _control.SelectedIndex = 2;
        _control.UpButton();
        _control.SelectedIndex.Should().Be(1);
    }

    [WinFormsFact]
    public void DomainUpDown_UpButton_SelectedIndexAtStart_NoWrap()
    {
        _control.Items.AddRange(new string[] { "Item1", "Item2", "Item3" });
        _control.SelectedIndex = 0;
        _control.Wrap = false;
        _control.UpButton();
        _control.SelectedIndex.Should().Be(0);
    }

    [WinFormsFact]
    public void DomainUpDown_UpButton_SelectedIndexAtStart_Wrap()
    {
        _control.Items.AddRange(new string[] { "Item1", "Item2", "Item3" });
        _control.SelectedIndex = 0;
        _control.Wrap = true;
        _control.UpButton();
        _control.SelectedIndex.Should().Be(2);
    }

    [WinFormsFact]
    public void DomainUpDown_UpButton_EmptyItems()
    {
        _control.Items.Clear();
        _control.UpButton();
        _control.SelectedIndex.Should().Be(-1);
    }

    public class SubDomainUpDown : DomainUpDown
    {
        public new const int ScrollStateAutoScrolling = ScrollableControl.ScrollStateAutoScrolling;

        public new const int ScrollStateHScrollVisible = ScrollableControl.ScrollStateHScrollVisible;

        public new const int ScrollStateVScrollVisible = ScrollableControl.ScrollStateVScrollVisible;

        public new const int ScrollStateUserHasScrolled = ScrollableControl.ScrollStateUserHasScrolled;

        public new const int ScrollStateFullDrag = ScrollableControl.ScrollStateFullDrag;

        public new SizeF AutoScaleFactor => base.AutoScaleFactor;

        public new bool CanEnableIme => base.CanEnableIme;

        public new bool CanRaiseEvents => base.CanRaiseEvents;

        public new bool ChangingText
        {
            get => base.ChangingText;
            set => base.ChangingText = value;
        }

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

        public new bool HScroll
        {
            get => base.HScroll;
            set => base.HScroll = value;
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

        public new bool UserEdit
        {
            get => base.UserEdit;
            set => base.UserEdit = value;
        }

        public new bool VScroll
        {
            get => base.VScroll;
            set => base.VScroll = value;
        }

        public new AccessibleObject CreateAccessibilityInstance() => base.CreateAccessibilityInstance();

        public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

        public new bool GetScrollState(int bit) => base.GetScrollState(bit);

        public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

        public new bool GetTopLevel() => base.GetTopLevel();

        public new void OnChanged(object source, EventArgs e) => base.OnChanged(source, e);

        public new void OnTextBoxKeyPress(object source, KeyPressEventArgs e) => base.OnTextBoxKeyPress(source, e);

        public new void OnSelectedItemChanged(object source, EventArgs e) => base.OnSelectedItemChanged(source, e);

        public new void UpdateEditText() => base.UpdateEditText();

        public new void ValidateEditText() => base.ValidateEditText();

        public new void WndProc(ref Message m) => base.WndProc(ref m);
    }
}
