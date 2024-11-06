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
        _control = new();
        _sub = new();
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
        _sub.ActiveControl.Should().BeNull();
        _sub.AllowDrop.Should().BeFalse();
        _sub.Anchor.Should().Be(AnchorStyles.Top | AnchorStyles.Left);
        _sub.AutoScroll.Should().BeFalse();
        _sub.AutoScaleDimensions.Should().Be(SizeF.Empty);
        _sub.AutoScaleFactor.Should().Be(new SizeF(1, 1));
        _sub.AutoScrollMargin.Should().Be(Size.Empty);
        _sub.AutoScaleMode.Should().Be(AutoScaleMode.Inherit);
        _sub.AutoScrollMinSize.Should().Be(Size.Empty);
        _sub.AutoScrollPosition.Should().Be(Point.Empty);
        _sub.AutoSize.Should().BeFalse();
        _sub.BackColor.Should().Be(SystemColors.Window);
        _sub.BackgroundImage.Should().BeNull();
        _sub.BackgroundImageLayout.Should().Be(ImageLayout.Tile);
        _sub.BindingContext.Should().NotBeNull();
        _sub.BorderStyle.Should().Be(BorderStyle.Fixed3D);
        _sub.Bottom.Should().Be(_sub.PreferredHeight);
        _sub.Bounds.Should().Be(new Rectangle(0, 0, 120, _sub.PreferredHeight));
        _sub.CanEnableIme.Should().BeFalse();
        _sub.CanFocus.Should().BeFalse();
        _sub.CanRaiseEvents.Should().BeTrue();
        _sub.CausesValidation.Should().BeTrue();
        _sub.ChangingText.Should().BeFalse();
        if (Application.UseVisualStyles)
        {
            _sub.ClientRectangle.Should().Be(new Rectangle(0, 0, 120, Control.DefaultFont.Height + 7));
            _sub.DisplayRectangle.Should().Be(new Rectangle(0, 0, 120, Control.DefaultFont.Height + 7));
            _sub.ClientSize.Should().Be(new Size(120, Control.DefaultFont.Height + 7));
            _sub.PreferredSize.Should().Be(new Size(16, _sub.PreferredHeight));
        }
        else
        {
            _sub.ClientRectangle.Should().Be(new Rectangle(0, 0, 116, Control.DefaultFont.Height + 3));
            _sub.DisplayRectangle.Should().Be(new Rectangle(0, 0, 116, Control.DefaultFont.Height + 3));
            _sub.ClientSize.Should().Be(new Size(116, Control.DefaultFont.Height + 3));
            _sub.PreferredSize.Should().Be(new Size(20, _sub.PreferredHeight));
        }

        _sub.Container.Should().BeNull();
        _sub.ContainsFocus.Should().BeFalse();
        _sub.ContextMenuStrip.Should().BeNull();
        _sub.Controls.Should().NotBeNull();
        _sub.Controls.Should().BeSameAs(_sub.Controls);
        _sub.Created.Should().BeFalse();
        _sub.CurrentAutoScaleDimensions.Should().Be(SizeF.Empty);
        _sub.Cursor.Should().Be(Cursors.Default);
        _sub.DefaultCursor.Should().Be(Cursors.Default);
        _sub.DefaultImeMode.Should().Be(ImeMode.Inherit);
        _sub.DefaultMargin.Should().Be(new Padding(3));
        _sub.DefaultMaximumSize.Should().Be(Size.Empty);
        _sub.DefaultMinimumSize.Should().Be(Size.Empty);
        _sub.DefaultPadding.Should().Be(Padding.Empty);
        _sub.DefaultSize.Should().Be(new Size(120, _sub.PreferredHeight));
        _sub.DesignMode.Should().BeFalse();
        _sub.Dock.Should().Be(DockStyle.None);
        _sub.DockPadding.Should().NotBeNull();
        _sub.DockPadding.Should().BeSameAs(_sub.DockPadding);
        _sub.DockPadding.Top.Should().Be(0);
        _sub.DockPadding.Bottom.Should().Be(0);
        _sub.DockPadding.Left.Should().Be(0);
        _sub.DockPadding.Right.Should().Be(0);
        _sub.DoubleBuffered.Should().BeFalse();
        _sub.Enabled.Should().BeTrue();
        _sub.Events.Should().NotBeNull();
        _sub.Events.Should().BeSameAs(_sub.Events);
        _sub.Focused.Should().BeFalse();
        _sub.Font.Should().Be(Control.DefaultFont);
        _sub.FontHeight.Should().Be(_sub.Font.Height);
        _sub.ForeColor.Should().Be(SystemColors.WindowText);
        _sub.HasChildren.Should().BeTrue();
        _sub.Height.Should().Be(_sub.PreferredHeight);
        _sub.HorizontalScroll.Should().NotBeNull();
        _sub.HorizontalScroll.Should().BeSameAs(_sub.HorizontalScroll);
        _sub.HScroll.Should().BeFalse();
        _sub.ImeMode.Should().Be(ImeMode.NoControl);
        _sub.ImeModeBase.Should().Be(ImeMode.NoControl);
        _sub.InterceptArrowKeys.Should().BeTrue();
        _sub.InvokeRequired.Should().BeFalse();
        _sub.IsAccessible.Should().BeFalse();
        _sub.IsMirrored.Should().BeFalse();
        _sub.Items.Count.Should().Be(0);
        _sub.Items.Should().BeSameAs(_sub.Items);
        _sub.LayoutEngine.Should().NotBeNull();
        _sub.LayoutEngine.Should().BeSameAs(_sub.LayoutEngine);
        _sub.Left.Should().Be(0);
        _sub.Location.Should().Be(Point.Empty);
        _sub.Margin.Should().Be(new Padding(3));
        _sub.MaximumSize.Should().Be(Size.Empty);
        _sub.MinimumSize.Should().Be(Size.Empty);
        _sub.Padding.Should().Be(Padding.Empty);
        _sub.Parent.Should().BeNull();
        _sub.PreferredHeight.Should().Be(Control.DefaultFont.Height + 7);
        _sub.ProductName.Should().Be("Microsoft\u00AE .NET");
        _sub.ReadOnly.Should().BeFalse();
        _sub.RecreatingHandle.Should().BeFalse();
        _sub.Region.Should().BeNull();
        _sub.ResizeRedraw.Should().BeTrue();
        _sub.Right.Should().Be(120);
        _sub.RightToLeft.Should().Be(RightToLeft.No);
        _sub.SelectedIndex.Should().Be(-1);
        _sub.SelectedItem.Should().BeNull();
        _sub.ShowFocusCues.Should().BeTrue();
        _sub.ShowKeyboardCues.Should().BeTrue();
        _sub.Site.Should().BeNull();
        _sub.Size.Should().Be(new Size(120, _sub.PreferredHeight));
        _sub.TabIndex.Should().Be(0);
        _sub.TabStop.Should().BeTrue();
        _sub.Text.Should().BeEmpty();
        _sub.TextAlign.Should().Be(HorizontalAlignment.Left);
        _sub.Top.Should().Be(0);
        _sub.TopLevelControl.Should().BeNull();
        _sub.UpDownAlign.Should().Be(LeftRightAlignment.Right);
        _sub.UserEdit.Should().BeFalse();
        _sub.UseWaitCursor.Should().BeFalse();
        _sub.Visible.Should().BeTrue();
        _sub.VerticalScroll.Should().NotBeNull();
        _sub.VerticalScroll.Should().BeSameAs(_sub.VerticalScroll);
        _sub.VScroll.Should().BeFalse();
        _sub.Width.Should().Be(120);
        _sub.Wrap.Should().BeFalse();

        _sub.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsFact]
    public void DomainUpDown_CreateParams_GetDefault_ReturnsExpected()
    {
        CreateParams createParams = _sub.CreateParams;
        createParams.Caption.Should().BeNull();
        createParams.ClassName.Should().BeNull();

        ((WNDCLASS_STYLES)createParams.ClassStyle).Should().Be(WNDCLASS_STYLES.CS_DBLCLKS);
        ((WINDOW_STYLE)createParams.Style).Should().Be(WINDOW_STYLE.WS_MAXIMIZEBOX | WINDOW_STYLE.WS_CLIPCHILDREN | WINDOW_STYLE.WS_CLIPSIBLINGS
            | WINDOW_STYLE.WS_VISIBLE | WINDOW_STYLE.WS_CHILD);

        if (Application.UseVisualStyles)
        {
            ((WINDOW_EX_STYLE)createParams.ExStyle).Should().Be(WINDOW_EX_STYLE.WS_EX_CONTROLPARENT);
        }
        else
        {
            ((WINDOW_EX_STYLE)createParams.ExStyle).Should().Be(WINDOW_EX_STYLE.WS_EX_CLIENTEDGE | WINDOW_EX_STYLE.WS_EX_CONTROLPARENT);
        }

        createParams.Height.Should().Be(_sub.PreferredHeight);
        createParams.Parent.Should().Be(IntPtr.Zero);
        createParams.Param.Should().BeNull();
        createParams.Width.Should().Be(120);
        createParams.X.Should().Be(0);
        createParams.Y.Should().Be(0);
        createParams.Should().BeSameAs(_sub.CreateParams);
        _sub.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaddingNormalizedTheoryData))]
    public void DomainUpDown_Padding_Set_GetReturnsExpected(Padding value, Padding expected)
    {
        _control.Padding = value;

        _control.Padding.Should().Be(expected);
        _control.IsHandleCreated.Should().BeFalse();

        _control.Padding = value;
        _control.Padding.Should().Be(expected);
        _control.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaddingNormalizedTheoryData))]
    public void DomainUpDown_Padding_SetWithHandle_GetReturnsExpected(Padding value, Padding expected)
    {
        _control.Handle.Should().NotBe(IntPtr.Zero);
        int invalidatedCallCount = 0;
        _control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        _control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        _control.HandleCreated += (sender, e) => createdCallCount++;

        _control.Padding = value;
        _control.Padding.Should().Be(expected);
        _control.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(0);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);

        _control.Padding = value;
        _control.Padding.Should().Be(expected);
        _control.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(0);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);
    }

    [WinFormsFact]
    public void DomainUpDown_Padding_SetWithHandler_CallsPaddingChanged()
    {
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(_control);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };
        _control.PaddingChanged += handler;

        // Set different.
        Padding padding1 = new(1);
        _control.Padding = padding1;
        _control.Padding.Should().Be(padding1);
        callCount.Should().Be(1);

        // Set same.
        _control.Padding = padding1;
        _control.Padding.Should().Be(padding1);
        callCount.Should().Be(1);

        // Set different.
        Padding padding2 = new(2);
        _control.Padding = padding2;
        _control.Padding.Should().Be(padding2);
        callCount.Should().Be(2);

        // Remove handler.
        _control.PaddingChanged -= handler;
        _control.Padding = padding1;
        _control.Padding.Should().Be(padding1);
        callCount.Should().Be(2);
    }

    [WinFormsFact]
    public void DomainUpDown_SelectedIndex_SetEmpty_Nop()
    {
        _sub.SelectedIndex = -1;

        _sub.SelectedIndex.Should().Be(-1);
        _sub.SelectedItem.Should().BeNull();
        _sub.Text.Should().BeEmpty();
        _sub.UserEdit.Should().BeFalse();
        _sub.ChangingText.Should().BeFalse();
        _sub.IsHandleCreated.Should().BeFalse();

        _sub.SelectedIndex = -1;
        _sub.SelectedIndex.Should().Be(-1);
        _sub.SelectedItem.Should().BeNull();
        _sub.Text.Should().BeEmpty();
        _sub.UserEdit.Should().BeFalse();
        _sub.ChangingText.Should().BeFalse();
        _sub.IsHandleCreated.Should().BeFalse();
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
        _sub.SelectedIndex.Should().Be(value);
        _sub.SelectedItem.Should().Be(expected);
        _sub.Text.Should().Be(expectedText);
        _sub.UserEdit.Should().BeFalse();
        _sub.ChangingText.Should().BeFalse();
        _sub.IsHandleCreated.Should().BeFalse();

        // Set same.
        _sub.SelectedIndex = value;
        _sub.SelectedIndex.Should().Be(value);
        _sub.SelectedItem.Should().Be(expected);
        _sub.Text.Should().Be(expectedText);
        _sub.UserEdit.Should().BeFalse();
        _sub.ChangingText.Should().BeFalse();
        _sub.IsHandleCreated.Should().BeFalse();

        // Set none.
        _sub.SelectedIndex = -1;
        _sub.SelectedIndex.Should().Be(-1);
        _sub.SelectedItem.Should().BeNull();
        _sub.Text.Should().Be(expectedText);
        _sub.UserEdit.Should().Be(expectedUserEdit);
        _sub.ChangingText.Should().BeFalse();
        _sub.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsTheory]
    [InlineData(0, "Item1", "Item1", true)]
    [InlineData(1, "Item2", "Item2", true)]
    [InlineData(-1, null, "", false)]
    public void DomainUpDown_SelectedIndex_SetUserEdit_GetReturnsExpected(int value, object expected, string expectedText, bool expectedUserEdit)
    {
        _sub.UserEdit = true;
        _sub.Items.Add("Item1");
        _sub.Items.Add("Item2");

        _sub.SelectedIndex = value;
        _sub.SelectedIndex.Should().Be(value);
        _sub.SelectedItem.Should().Be(expected);
        _sub.Text.Should().Be(expectedText);
        _sub.UserEdit.Should().Be(!expectedUserEdit);
        _sub.ChangingText.Should().BeFalse();
        _sub.IsHandleCreated.Should().BeFalse();

        // Set same.
        _sub.SelectedIndex = value;
        _sub.SelectedIndex.Should().Be(value);
        _sub.SelectedItem.Should().Be(expected);
        _sub.Text.Should().Be(expectedText);
        _sub.UserEdit.Should().Be(!expectedUserEdit);
        _sub.ChangingText.Should().BeFalse();
        _sub.IsHandleCreated.Should().BeFalse();

        // Set none.
        _sub.SelectedIndex = -1;
        _sub.SelectedIndex.Should().Be(-1);
        _sub.SelectedItem.Should().BeNull();
        _sub.Text.Should().Be(expectedText);
        _sub.UserEdit.Should().BeTrue();
        _sub.ChangingText.Should().BeFalse();
        _sub.IsHandleCreated.Should().BeFalse();
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
            sender.Should().Be(_control);
            e.Should().Be(EventArgs.Empty);
            textChangedCallCount++;
        };
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(_control);
            e.Should().Be(EventArgs.Empty);
            (textChangedCallCount - 1).Should().Be(callCount);
            callCount++;
        };
        _control.TextChanged += textChangedHandler;
        _control.SelectedItemChanged += handler;

        // Set different.
        _control.SelectedIndex = 0;
        _control.SelectedIndex.Should().Be(0);
        textChangedCallCount.Should().Be(1);
        callCount.Should().Be(1);

        // Set same.
        _control.SelectedIndex = 0;
        _control.SelectedIndex.Should().Be(0);
        textChangedCallCount.Should().Be(1);
        callCount.Should().Be(1);

        // Set different.
        _control.SelectedIndex = 1;
        _control.SelectedIndex.Should().Be(1);
        textChangedCallCount.Should().Be(2);
        callCount.Should().Be(2);

        // Set none.
        _control.SelectedIndex = -1;
        _control.SelectedIndex.Should().Be(-1);
        textChangedCallCount.Should().Be(2);
        callCount.Should().Be(2);

        // Remove handler.
        _control.TextChanged -= textChangedHandler;
        _control.SelectedItemChanged -= handler;
        _control.SelectedIndex = 0;
        _control.SelectedIndex.Should().Be(0);
        textChangedCallCount.Should().Be(2);
        callCount.Should().Be(2);
    }

    [WinFormsTheory]
    [InlineData(-2)]
    [InlineData(0)]
    [InlineData(1)]
    public void DomainUpDown_SelectedIndex_SetInvalidValueEmpty_ThrowsArgumentOutOfRangeException(int value)
    {
        _control.Invoking(c => c.SelectedIndex = value).Should().Throw<ArgumentOutOfRangeException>().And.ParamName.Should().Be("value");
    }

    [WinFormsTheory]
    [InlineData(-2)]
    [InlineData(1)]
    [InlineData(2)]
    public void DomainUpDown_SelectedIndex_SetInvalidValueNotEmpty_ThrowsArgumentOutOfRangeException(int value)
    {
        _control.Items.Add("Item");
        _control.Invoking(c => c.SelectedIndex = value).Should().Throw<ArgumentOutOfRangeException>().And.ParamName.Should().Be("value");
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("NoSuchItem")]
    public void DomainUpDown_SelectedItem_SetEmpty_Nop(object value)
    {
        _sub.SelectedItem = value;
        _sub.SelectedIndex.Should().Be(-1);
        _sub.SelectedItem.Should().BeNull();
        _sub.Text.Should().BeEmpty();
        _sub.UserEdit.Should().BeFalse();
        _sub.ChangingText.Should().BeFalse();
        _sub.IsHandleCreated.Should().BeFalse();

        // Set same.
        _sub.SelectedItem = value;
        _sub.SelectedIndex.Should().Be(-1);
        _sub.SelectedItem.Should().BeNull();
        _sub.Text.Should().BeEmpty();
        _sub.UserEdit.Should().BeFalse();
        _sub.ChangingText.Should().BeFalse();
        _sub.IsHandleCreated.Should().BeFalse();
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
        _sub.SelectedIndex.Should().Be(expectedSelectedIndex);
        _sub.SelectedItem.Should().Be(expected);
        _sub.Text.Should().Be(expectedText);
        _sub.UserEdit.Should().BeFalse();
        _sub.ChangingText.Should().BeFalse();
        _sub.IsHandleCreated.Should().BeFalse();

        // Set same.
        _sub.SelectedItem = value;
        _sub.SelectedIndex.Should().Be(expectedSelectedIndex);
        _sub.SelectedItem.Should().Be(expected);
        _sub.Text.Should().Be(expectedText);
        _sub.UserEdit.Should().BeFalse();
        _sub.ChangingText.Should().BeFalse();
        _sub.IsHandleCreated.Should().BeFalse();

        // Set no such item.
        _sub.SelectedItem = "NoSuchItem";
        _sub.SelectedIndex.Should().Be(expectedSelectedIndex);
        _sub.SelectedItem.Should().Be(expected);
        _sub.Text.Should().Be(expectedText);
        _sub.UserEdit.Should().BeFalse();
        _sub.ChangingText.Should().BeFalse();
        _sub.IsHandleCreated.Should().BeFalse();

        // Set none.
        _sub.SelectedItem = null;
        _sub.SelectedIndex.Should().Be(-1);
        _sub.SelectedItem.Should().BeNull();
        _sub.Text.Should().Be(expectedText);
        _sub.UserEdit.Should().Be(expectedUserEdit);
        _sub.ChangingText.Should().BeFalse();
        _sub.IsHandleCreated.Should().BeFalse();
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
        _sub.SelectedIndex.Should().Be(expectedSelectedIndex);
        _sub.SelectedItem.Should().Be(expected);
        _sub.Text.Should().Be(expectedText);
        _sub.UserEdit.Should().Be(!expectedUserEdit);
        _sub.ChangingText.Should().BeFalse();
        _sub.IsHandleCreated.Should().BeFalse();

        // Set same.
        _sub.SelectedItem = value;
        _sub.SelectedIndex.Should().Be(expectedSelectedIndex);
        _sub.SelectedItem.Should().Be(expected);
        _sub.Text.Should().Be(expectedText);
        _sub.UserEdit.Should().Be(!expectedUserEdit);
        _sub.ChangingText.Should().BeFalse();
        _sub.IsHandleCreated.Should().BeFalse();

        // Set no such item.
        _sub.SelectedItem = "NoSuchItem";
        _sub.SelectedIndex.Should().Be(expectedSelectedIndex);
        _sub.SelectedItem.Should().Be(expected);
        _sub.Text.Should().Be(expectedText);
        _sub.UserEdit.Should().Be(!expectedUserEdit);
        _sub.ChangingText.Should().BeFalse();
        _sub.IsHandleCreated.Should().BeFalse();

        // Set none.
        _sub.SelectedItem = null;
        _sub.SelectedIndex.Should().Be(-1);
        _sub.SelectedItem.Should().BeNull();
        _sub.Text.Should().Be(expectedText);
        _sub.UserEdit.Should().BeTrue();
        _sub.ChangingText.Should().BeFalse();
        _sub.IsHandleCreated.Should().BeFalse();
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
            sender.Should().Be(_control);
            e.Should().Be(EventArgs.Empty);
            textChangedCallCount++;
        };
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(_control);
            e.Should().Be(EventArgs.Empty);
            (textChangedCallCount - 1).Should().Be(callCount);
            callCount++;
        };
        _control.TextChanged += textChangedHandler;
        _control.SelectedItemChanged += handler;

        // Set different.
        _control.SelectedItem = "Item1";
        _control.SelectedItem.Should().Be("Item1");
        textChangedCallCount.Should().Be(1);
        callCount.Should().Be(1);

        // Set same.
        _control.SelectedItem = "Item1";
        _control.SelectedItem.Should().Be("Item1");
        textChangedCallCount.Should().Be(1);
        callCount.Should().Be(1);

        // Set different.
        _control.SelectedItem = "Item2";
        _control.SelectedItem.Should().Be("Item2");
        textChangedCallCount.Should().Be(2);
        callCount.Should().Be(2);

        // Set none.
        _control.SelectedItem = null;
        _control.SelectedItem.Should().BeNull();
        textChangedCallCount.Should().Be(2);
        callCount.Should().Be(2);

        // Remove handler.
        _control.TextChanged -= textChangedHandler;
        _control.SelectedItemChanged -= handler;
        _control.SelectedItem = "Item1";
        _control.SelectedItem.Should().Be("Item1");
        textChangedCallCount.Should().Be(2);
        callCount.Should().Be(2);
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

        _sub.Sorted.Should().Be(value);
        _sub.Items.Count.Should().Be(0);
        _sub.SelectedIndex.Should().Be(-1);
        _sub.UserEdit.Should().Be(userEdit);
        _sub.IsHandleCreated.Should().BeFalse();

        // Set same.
        _sub.Sorted = value;
        _sub.Sorted.Should().Be(value);
        _sub.Items.Count.Should().Be(0);
        _sub.SelectedIndex.Should().Be(-1);
        _sub.UserEdit.Should().Be(userEdit);
        _sub.IsHandleCreated.Should().BeFalse();

        // Set different.
        _sub.Sorted = !value;
        _sub.Sorted.Should().Be(!value);
        _sub.Items.Count.Should().Be(0);
        _sub.SelectedIndex.Should().Be(-1);
        _sub.UserEdit.Should().Be(userEdit);
        _sub.IsHandleCreated.Should().BeFalse();
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
        _sub.Sorted.Should().Be(value);
        _sub.Items.Cast<string>().Should().Equal(expectedItems);
        _sub.SelectedIndex.Should().Be(-1);
        _sub.IsHandleCreated.Should().BeFalse();

        // Set same.
        _sub.Sorted = value;
        _sub.Sorted.Should().Be(value);
        _sub.Items.Cast<string>().Should().Equal(expectedItems);
        _sub.SelectedIndex.Should().Be(-1);
        _sub.UserEdit.Should().Be(userEdit);
        _sub.IsHandleCreated.Should().BeFalse();

        // Set different.
        _sub.Sorted = !value;
        _sub.Sorted.Should().Be(!value);
        _sub.Items.Cast<string>().Should().Equal(["a", "a", "B", "c", "d"]);
        _sub.SelectedIndex.Should().Be(-1);
        _sub.UserEdit.Should().Be(userEdit);
        _sub.IsHandleCreated.Should().BeFalse();
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
        _sub.Sorted.Should().Be(value);
        _sub.Items.Cast<string>().Should().Equal(expectedItems);
        _sub.SelectedIndex.Should().Be(expectedSelectedIndex);
        _sub.IsHandleCreated.Should().BeFalse();

        // Set same.
        _sub.Sorted = value;
        _sub.Sorted.Should().Be(value);
        _sub.Items.Cast<string>().Should().Equal(expectedItems);
        _sub.SelectedIndex.Should().Be(expectedSelectedIndex);
        _sub.UserEdit.Should().Be(userEdit);
        _sub.IsHandleCreated.Should().BeFalse();

        // Set different.
        _sub.Sorted = !value;
        _sub.Sorted.Should().Be(!value);
        _sub.Items.Cast<string>().Should().Equal(["a", "a", "B", "c", "d"]);
        _sub.SelectedIndex.Should().Be(expectedSelectedIndex);
        _sub.UserEdit.Should().Be(userEdit);
        _sub.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsTheory]
    [MemberData(nameof(Sorted_Set_TestData))]
    public void DomainUpDown_Sorted_SetWithHandle_GetReturnsExpected(bool userEdit, bool value)
    {
        _sub.UserEdit = userEdit;
        _sub.Handle.Should().NotBe(IntPtr.Zero);
        int invalidatedCallCount = 0;
        _sub.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        _sub.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        _sub.HandleCreated += (sender, e) => createdCallCount++;

        _sub.Sorted = value;
        _sub.Sorted.Should().Be(value);
        _sub.Items.Count.Should().Be(0);
        _sub.SelectedIndex.Should().Be(-1);
        _sub.UserEdit.Should().Be(userEdit);
        _sub.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(0);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);

        // Set same.
        _sub.Sorted = value;
        _sub.Sorted.Should().Be(value);
        _sub.Items.Count.Should().Be(0);
        _sub.SelectedIndex.Should().Be(-1);
        _sub.UserEdit.Should().Be(userEdit);
        _sub.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(0);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);

        // Set different.
        _sub.Sorted = !value;
        _sub.Sorted.Should().Be(!value);
        _sub.Items.Count.Should().Be(0);
        _sub.SelectedIndex.Should().Be(-1);
        _sub.UserEdit.Should().Be(userEdit);
        _sub.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(0);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);
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
        _sub.Handle.Should().NotBe(IntPtr.Zero);
        int invalidatedCallCount = 0;
        _sub.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        _sub.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        _sub.HandleCreated += (sender, e) => createdCallCount++;
        _sub.UserEdit = userEdit;

        _sub.Sorted = value;
        _sub.Sorted.Should().Be(value);
        _sub.Items.Cast<string>().Should().Equal(expectedItems);
        _sub.SelectedIndex.Should().Be(-1);
        _sub.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(0);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);

        // Set same.
        _sub.Sorted = value;
        _sub.Sorted.Should().Be(value);
        _sub.Items.Cast<string>().Should().Equal(expectedItems);
        _sub.SelectedIndex.Should().Be(-1);
        _sub.UserEdit.Should().Be(userEdit);
        _sub.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(0);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);

        // Set different.
        _sub.Sorted = !value;
        _sub.Sorted.Should().Be(!value);
        _sub.Items.Cast<string>().Should().Equal(["a", "a", "B", "c", "d"]);
        _sub.SelectedIndex.Should().Be(-1);
        _sub.UserEdit.Should().Be(userEdit);
        _sub.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(0);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);
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
        _sub.Handle.Should().NotBe(IntPtr.Zero);
        int invalidatedCallCount = 0;
        _sub.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        _sub.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        _sub.HandleCreated += (sender, e) => createdCallCount++;
        _sub.SelectedItem = "B";
        _sub.UserEdit = userEdit;

        _sub.Sorted = value;
        _sub.Sorted.Should().Be(value);
        _sub.Items.Cast<string>().Should().Equal(expectedItems);
        _sub.SelectedIndex.Should().Be(expectedSelectedIndex);
        _sub.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(0);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);

        // Set same.
        _sub.Sorted = value;
        _sub.Sorted.Should().Be(value);
        _sub.Items.Cast<string>().Should().Equal(expectedItems);
        _sub.SelectedIndex.Should().Be(expectedSelectedIndex);
        _sub.UserEdit.Should().Be(userEdit);
        _sub.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(0);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);

        // Set different.
        _sub.Sorted = !value;
        _sub.Sorted.Should().Be(!value);
        _sub.Items.Cast<string>().Should().Equal(["a", "a", "B", "c", "d"]);
        _sub.SelectedIndex.Should().Be(expectedSelectedIndex);
        _sub.UserEdit.Should().Be(userEdit);
        _sub.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(0);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);
    }

    [WinFormsTheory]
    [BoolData]
    public void DomainUpDown_Wrap_Set_GetReturnsExpected(bool value)
    {
        _control.Wrap = value;

        _control.Wrap.Should().Be(value);
        _control.IsHandleCreated.Should().BeFalse();

        // Set same.
        _control.Wrap = value;
        _control.Wrap.Should().Be(value);
        _control.IsHandleCreated.Should().BeFalse();

        // Set different.
        _control.Wrap = !value;
        _control.Wrap.Should().Be(!value);
        _control.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsTheory]
    [BoolData]
    public void DomainUpDown_Wrap_SetWithHandle_GetReturnsExpected(bool value)
    {
        _control.Handle.Should().NotBe(IntPtr.Zero);
        int invalidatedCallCount = 0;
        _control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        _control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        _control.HandleCreated += (sender, e) => createdCallCount++;

        _control.Wrap = value;
        _control.Wrap.Should().Be(value);
        _control.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(0);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);

        // Set same.
        _control.Wrap = value;
        _control.Wrap.Should().Be(value);
        _control.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(0);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);

        // Set different.
        _control.Wrap = !value;
        _control.Wrap.Should().Be(!value);
        _control.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(0);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);
    }

    [WinFormsFact]
    public void DomainUpDown_CreateAccessibilityInstance_Invoke_ReturnsExpected()
    {
        UpDownBase.UpDownBaseAccessibleObject instance = _sub.CreateAccessibilityInstance() as UpDownBase.UpDownBaseAccessibleObject;
        instance.Should().NotBeNull().And.BeOfType<UpDownBase.UpDownBaseAccessibleObject>();
        instance.Owner.Should().Be(_sub);
        instance.Role.Should().Be(AccessibleRole.SpinButton);
        _sub.CreateAccessibilityInstance().Should().NotBeSameAs(instance);
        _sub.AccessibilityObject.Should().NotBeSameAs(instance);
    }

    [WinFormsFact]
    public void DomainUpDown_CreateAccessibilityInstance_InvokeWithCustomRole_ReturnsExpected()
    {
        _sub.AccessibleRole = AccessibleRole.HelpBalloon;
        var instance = _sub.CreateAccessibilityInstance() as UpDownBase.UpDownBaseAccessibleObject;

        instance.Should().NotBeNull().And.BeOfType<UpDownBase.UpDownBaseAccessibleObject>();
        instance.Owner.Should().Be(_sub);
        instance.Role.Should().Be(AccessibleRole.HelpBalloon);
        _sub.CreateAccessibilityInstance().Should().NotBeSameAs(instance);
        _sub.AccessibilityObject.Should().NotBeSameAs(instance);
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
        _sub.SelectedIndex.Should().Be(-1);
        _sub.UserEdit.Should().Be(userEdit);

        // Call again.
        _sub.DownButton();
        _sub.SelectedIndex.Should().Be(-1);
        _sub.UserEdit.Should().Be(userEdit);
    }

    [WinFormsTheory]
    [MemberData(nameof(DownButton_TestData))]
    public void DomainUpDown_DownButton_InvokeEmpty_Nop(bool userEdit, bool wrap)
    {
        _sub.UserEdit = userEdit;
        _sub.Wrap = wrap;

        _sub.Items.Count.Should().Be(0);

        _sub.DownButton();
        _sub.SelectedIndex.Should().Be(-1);
        _sub.UserEdit.Should().Be(userEdit);

        // Call again.
        _sub.DownButton();
        _sub.SelectedIndex.Should().Be(-1);
        _sub.UserEdit.Should().Be(userEdit);
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
        _sub.SelectedIndex.Should().Be(0);
        _sub.UserEdit.Should().BeFalse();

        // Call again.
        _sub.DownButton();
        _sub.SelectedIndex.Should().Be(1);
        _sub.UserEdit.Should().BeFalse();

        // Call again.
        _sub.DownButton();
        _sub.SelectedIndex.Should().Be(2);
        _sub.UserEdit.Should().BeFalse();

        // Call again.
        _sub.DownButton();
        _sub.SelectedIndex.Should().Be(expectedWrapSelectedIndex);
        _sub.UserEdit.Should().BeFalse();
    }

    [WinFormsFact]
    public void DomainUpDown_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        _sub.GetAutoSizeMode().Should().Be(AutoSizeMode.GrowOnly);
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
        _sub.GetScrollState(bit).Should().Be(expected);
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
        _sub.GetStyle(flag).Should().Be(expected);

        // Call again to test caching.
        _sub.GetStyle(flag).Should().Be(expected);
    }

    [WinFormsFact]
    public void DomainUpDown_GetTopLevel_Invoke_ReturnsExpected()
    {
        _sub.GetTopLevel().Should().BeFalse();
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
        _control.MatchIndex(text, false, start).Should().Be(expected);
    }

    [WinFormsFact]
    public void DomainUpDown_MatchIndex_NullText_ThrowsNullReferenceException()
    {
        _control.Items.Add("item1");
        Action act = () => _control.MatchIndex(null, false, 0);
        act.Should().Throw<NullReferenceException>();
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
            sender.Should().Be(_sub);
            e.Should().Be(eventArgs);
            callCount++;
        };

        // Call with handler.
        _sub.SelectedItemChanged += handler;
        _sub.OnSelectedItemChanged(source, eventArgs);
        callCount.Should().Be(1);

        // Remove handler.
        _sub.SelectedItemChanged -= handler;
        _sub.OnSelectedItemChanged(source, eventArgs);
        callCount.Should().Be(1);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnChanged_TestData))]
    public void DomainUpDown_OnSelectedItemChanged_Invoke_CallsSelectedItemChanged(object source, EventArgs eventArgs)
    {
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(_sub);
            e.Should().Be(eventArgs);
            callCount++;
        };

        // Call with handler.
        _sub.SelectedItemChanged += handler;
        _sub.OnSelectedItemChanged(source, eventArgs);
        callCount.Should().Be(1);

        // Remove handler.
        _sub.SelectedItemChanged -= handler;
        _sub.OnSelectedItemChanged(source, eventArgs);
        callCount.Should().Be(1);
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
        _sub.SelectedIndex.Should().Be(-1);
        _sub.UserEdit.Should().Be(userEdit);

        // Call again.
        _sub.UpButton();
        _sub.SelectedIndex.Should().Be(-1);
        _sub.UserEdit.Should().Be(userEdit);
    }

    [WinFormsTheory]
    [MemberData(nameof(UpButton_TestData))]
    public void DomainUpDown_UpButton_InvokeEmpty_Nop(bool userEdit, bool wrap)
    {
        _sub.UserEdit = userEdit;
        _sub.Wrap = wrap;

        _sub.Items.Count.Should().Be(0);

        _sub.UpButton();
        _sub.SelectedIndex.Should().Be(-1);
        _sub.UserEdit.Should().Be(userEdit);

        // Call again.
        _sub.UpButton();
        _sub.SelectedIndex.Should().Be(-1);
        _sub.UserEdit.Should().Be(userEdit);
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
        _sub.SelectedIndex.Should().Be(1);
        _sub.UserEdit.Should().BeFalse();

        // Call again.
        _sub.UpButton();
        _sub.SelectedIndex.Should().Be(0);
        _sub.UserEdit.Should().BeFalse();

        // Call again.
        _sub.UpButton();
        _sub.SelectedIndex.Should().Be(expectedWrapSelectedIndex1);
        _sub.UserEdit.Should().BeFalse();

        // Call again.
        _sub.UpButton();
        _sub.SelectedIndex.Should().Be(expectedWrapSelectedIndex2);
        _sub.UserEdit.Should().BeFalse();
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
        _sub.Text.Should().BeEmpty();
        _sub.UserEdit.Should().BeFalse();
        _sub.ChangingText.Should().BeFalse();
        _sub.IsHandleCreated.Should().BeFalse();

        // Call again.
        _sub.UpdateEditText();
        _sub.Text.Should().BeEmpty();
        _sub.UserEdit.Should().BeFalse();
        _sub.ChangingText.Should().BeFalse();
        _sub.IsHandleCreated.Should().BeFalse();
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
        _sub.Text.Should().BeEmpty();
        _sub.UserEdit.Should().BeFalse();
        _sub.ChangingText.Should().BeFalse();
        _sub.IsHandleCreated.Should().BeFalse();

        // Call again.
        _sub.UpdateEditText();
        _sub.Text.Should().BeEmpty();
        _sub.UserEdit.Should().BeFalse();
        _sub.ChangingText.Should().BeFalse();
        _sub.IsHandleCreated.Should().BeFalse();
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
        _sub.Text.Should().Be("Item1");
        _sub.UserEdit.Should().BeFalse();
        _sub.ChangingText.Should().BeFalse();
        _sub.IsHandleCreated.Should().BeFalse();

        // Call again.
        _sub.UpdateEditText();
        _sub.Text.Should().Be("Item1");
        _sub.UserEdit.Should().BeFalse();
        _sub.ChangingText.Should().BeFalse();
        _sub.IsHandleCreated.Should().BeFalse();
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
