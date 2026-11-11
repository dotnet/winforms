// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.TestUtilities;

namespace System.Windows.Forms.Tests;

public class CheckedListBoxTests
{
    [WinFormsFact]
    public void CheckedListBox_Constructor()
    {
        using CheckedListBox box = new();

        Assert.NotNull(box);

        // Check default values
        box.UseCompatibleTextRendering.Should().BeTrue();
    }

    [WinFormsTheory]
    [BoolData]
    public void CheckedListBox_CheckOnClick(bool expected)
    {
        using CheckedListBox box = new()
        {
            CheckOnClick = expected
        };

        Assert.Equal(expected, box.CheckOnClick);
    }

    [WinFormsFact]
    public void CheckedListBox_CheckedIndexCollectionNotNull()
    {
        using CheckedListBox box = new();

        CheckedListBox.CheckedIndexCollection collection = box.CheckedIndices;

        Assert.NotNull(collection);
    }

    [WinFormsFact]
    public void CheckedListBox_CheckedItemCollectionNotNull()
    {
        using CheckedListBox box = new();

        CheckedListBox.CheckedItemCollection collection = box.CheckedItems;

        Assert.NotNull(collection);
    }

    [WinFormsTheory]
    [StringData]
    public void CheckedListBox_DisplayMember(string expected)
    {
        using CheckedListBox box = new()
        {
            DisplayMember = expected
        };

        Assert.Equal(expected, box.DisplayMember);
    }

    [WinFormsFact]
    public void CheckedListBox_DrawModeReturnsNormalOnly()
    {
        using CheckedListBox box = new();

        DrawMode result = box.DrawMode;

        Assert.Equal(DrawMode.Normal, result);
    }

    [WinFormsFact]
    public void CheckedListBox_DrawMode_GetSet_ReturnsExpected()
    {
        using CheckedListBox box = new();

        box.DrawMode.Should().Be(DrawMode.Normal);

        box.DrawMode = DrawMode.OwnerDrawFixed;
        box.DrawMode.Should().Be(DrawMode.Normal);

        box.DrawMode = DrawMode.OwnerDrawVariable;
        box.DrawMode.Should().Be(DrawMode.Normal);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.None)]
    [InlineData(SelectionMode.One)]
    public void CheckedListBox_SelectionModeGetSet(SelectionMode expected)
    {
        using CheckedListBox box = new()
        {
            SelectionMode = expected
        };

        Assert.Equal(expected, box.SelectionMode);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiSimple)]
    [InlineData(SelectionMode.MultiExtended)]
    public void CheckedListBox_SelectionModeGetSetInvalidFromEnum(SelectionMode expected)
    {
        using CheckedListBox box = new();

        ArgumentException ex = Assert.Throws<ArgumentException>(() => box.SelectionMode = expected);
    }

    [WinFormsTheory]
    [InvalidEnumData<SelectionMode>]
    public void CheckedListBox_SelectionMode_SetInvalidValue_ThrowsInvalidEnumArgumentException(SelectionMode value)
    {
        using CheckedListBox control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.SelectionMode = value);
    }

    [WinFormsTheory]
    [BoolData]
    public void CheckedListBox_ThreeDCheckBoxes(bool expected)
    {
        using CheckedListBox box = new()
        {
            ThreeDCheckBoxes = expected
        };

        Assert.Equal(expected, box.ThreeDCheckBoxes);
    }

    [WinFormsTheory]
    [BoolData]
    public void CheckedListBox_ThreeDCheckBoxes_SetWithItems_ReturnsExpected(bool expected)
    {
        using CheckedListBox box = new()
        {
            ThreeDCheckBoxes = expected
        };

        box.Items.Add("item1");

        box.ThreeDCheckBoxes.Should().Be(expected);
    }

    [WinFormsTheory]
    [StringData]
    public void CheckedListBox_ValueMember(string expected)
    {
        using CheckedListBox box = new()
        {
            ValueMember = expected
        };

        Assert.Equal(expected, box.ValueMember);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaddingNormalizedTheoryData))]
    public void CheckedListBox_Padding_Set_GetReturnsExpected(Padding value, Padding expected)
    {
        using CheckedListBox control = new()
        {
            Padding = value
        };
        Assert.Equal(expected, control.Padding);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Padding = value;
        Assert.Equal(expected, control.Padding);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Padding_SetWithHandle_TestData()
    {
        yield return new object[] { default(Padding), default(Padding), 0, 0 };
        yield return new object[] { new Padding(1, 2, 3, 4), new Padding(1, 2, 3, 4), 1, 1 };
        yield return new object[] { new Padding(1), new Padding(1), 1, 1 };
        yield return new object[] { new Padding(-1, -2, -3, -4), Padding.Empty, 1, 2 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Padding_SetWithHandle_TestData))]
    public void CheckedListBox_Padding_SetWithHandle_GetReturnsExpected(Padding value, Padding expected, int expectedInvalidatedCallCount1, int expectedInvalidatedCallCount2)
    {
        using CheckedListBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Padding = value;
        Assert.Equal(expected, control.Padding);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Padding = value;
        Assert.Equal(expected, control.Padding);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void CheckedListBox_Padding_SetWithHandler_CallsPaddingChanged()
    {
        using CheckedListBox control = new();
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

    [WinFormsTheory]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    public void CheckedListBox_GetItemCheckStateOutOfRange(int index)
    {
        using CheckedListBox box = new();

        ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(() => box.GetItemCheckState(index));
        Assert.Equal("index", ex.ParamName);
    }

    [WinFormsFact]
    public void CheckedListBox_RefreshItems_InvokeEmpty_Success()
    {
        using SubCheckedListBox control = new();
        control.RefreshItems();
        Assert.Empty(control.Items);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.RefreshItems();
        Assert.Empty(control.Items);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CheckedListBox_RefreshItems_InvokeNotEmpty_Success()
    {
        using SubCheckedListBox control = new();
        control.Items.Add("item1");
        control.Items.Add("item2");

        control.RefreshItems();
        Assert.Equal(new object[] { "item1", "item2" }, control.Items.Cast<object>());
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.RefreshItems();
        Assert.Equal(new object[] { "item1", "item2" }, control.Items.Cast<object>());
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0, 3, false)]
    [InlineData(0, 3, true)]
    [InlineData(1, 3, false)]
    [InlineData(1, 3, true)]
    [InlineData(3, 1, false)]
    [InlineData(3, 1, true)]
    [InlineData(3, 3, false)]
    [InlineData(3, 3, true)]
    public void CheckedListBox_RefreshItems_DataSourceCountChanged_PreservesCheckStates(int oldCount, int newCount, bool createHandle)
    {
        List<string> dataSource = Enumerable.Range(0, oldCount).Select(i => $"item{i}").ToList();
        using SubCheckedListBox control = new()
        {
            BindingContext = new BindingContext(),
            DataSource = dataSource
        };
        CheckState[] checkStates = [CheckState.Checked, CheckState.Indeterminate, CheckState.Unchecked];
        for (int i = 0; i < oldCount; i++)
        {
            control.SetItemCheckState(i, checkStates[i]);
        }

        if (createHandle)
        {
            Assert.NotEqual(IntPtr.Zero, control.Handle);
        }

        if (newCount > oldCount)
        {
            dataSource.AddRange(Enumerable.Range(oldCount, newCount - oldCount).Select(i => $"item{i}"));
        }
        else
        {
            dataSource.RemoveRange(newCount, oldCount - newCount);
        }

        control.RefreshItems();

        control.Items.Cast<string>().Should().Equal(dataSource);
        for (int i = 0; i < newCount; i++)
        {
            control.GetItemCheckState(i).Should().Be(i < oldCount ? checkStates[i] : CheckState.Unchecked);
        }

        control.IsHandleCreated.Should().Be(createHandle);
    }

    [WinFormsFact]
    public void CheckedListBox_HorizontalScrollbar_SetWithCachedSelectedItemsCount_PreservesSelection()
    {
        using CheckedListBox control = new();
        control.Items.Add("item1");
        control.SelectedIndex = 0;
#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.
        Assert.Equal(1, control.SelectedItems.Count);
#pragma warning restore xUnit2013 // Do not use equality check to check for collection size.
        control.Items.Add("item2");

        control.HorizontalScrollbar = true;

        Assert.True(control.HorizontalScrollbar);
        Assert.Equal(0, control.SelectedIndex);
        Assert.Equal("item1", control.SelectedItem);
#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.
        Assert.Equal(1, control.SelectedItems.Count);
#pragma warning restore xUnit2013 // Do not use equality check to check for collection size.
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CheckedListBox_RefreshItems_InvokeEmptyWithHandle_Success()
    {
        using SubCheckedListBox control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.RefreshItems();
        Assert.Empty(control.Items);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.RefreshItems();
        Assert.Empty(control.Items);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void CheckedListBox_RefreshItems_InvokeNotEmptyWithHandle_Success()
    {
        using SubCheckedListBox control = new();
        control.Items.Add("item1");
        control.Items.Add("item2");
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.RefreshItems();
        Assert.Equal(new object[] { "item1", "item2" }, control.Items.Cast<object>());
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.RefreshItems();
        Assert.Equal(new object[] { "item1", "item2" }, control.Items.Cast<object>());
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    public void CheckedListBox_SetItemCheckStateOutOfRange(int index)
    {
        using CheckedListBox box = new();

        ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(() => box.SetItemCheckState(index, CheckState.Checked));
        Assert.Equal("index", ex.ParamName);
    }

    [WinFormsTheory]
    [EnumData<CheckState>]
    public void CheckedListBox_SetItemCheckState_Invoke_GetReturnsExpected(CheckState value)
    {
        using CheckedListBox control = new();
        control.Items.Add(new CheckBox(), false);

        control.SetItemCheckState(0, value);
        Assert.Equal(value, control.GetItemCheckState(0));
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SetItemCheckState(0, value);
        Assert.Equal(value, control.GetItemCheckState(0));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InvalidEnumData<CheckState>]
    public void CheckedListBox_SetItemCheckState_InvokeInvalidValue_ThrowsInvalidEnumArgumentException(CheckState value)
    {
        using CheckedListBox control = new();
        control.Items.Add(new CheckBox(), false);
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.SetItemCheckState(0, value));
    }

    [WinFormsTheory]
    [InlineData(true, CheckState.Checked)]
    [InlineData(false, CheckState.Unchecked)]
    public void CheckedListBox_SetItemChecked(bool send, CheckState expected)
    {
        using CheckedListBox box = new();
        box.Items.Add(new CheckBox(), false);

        box.SetItemChecked(0, send);

        Assert.Equal(expected, box.GetItemCheckState(0));
    }

    private static CheckedListBox CreateCheckedListBox(bool createHandle)
    {
        CheckedListBox control = new();
        control.Items.AddRange(["zero", "one", "two", "three"]);
        if (createHandle)
        {
            control.CreateControl();
        }

        return control;
    }

    [WinFormsTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void CheckedListBox_SetItemCheckState_RaisesItemCheckWithOldAndNewState(bool createHandle)
    {
        // Programmatic checking raises ItemCheck once per real change with the index, the old state and the
        // requested new state, and does not raise it when the state is unchanged.
        using CheckedListBox control = CreateCheckedListBox(createHandle);
        List<(int Index, CheckState Current, CheckState New, object Sender)> events = [];
        control.ItemCheck += (sender, e) => events.Add((e.Index, e.CurrentValue, e.NewValue, sender));

        control.SetItemCheckState(1, CheckState.Checked);
        control.SetItemCheckState(1, CheckState.Indeterminate);
        control.SetItemChecked(1, false);
        control.SetItemCheckState(1, CheckState.Unchecked);
        control.SetItemChecked(3, true);

        events.Should().Equal(
            (1, CheckState.Unchecked, CheckState.Checked, control),
            (1, CheckState.Checked, CheckState.Indeterminate, control),
            (1, CheckState.Indeterminate, CheckState.Unchecked, control),
            (3, CheckState.Unchecked, CheckState.Checked, control));
        control.GetItemCheckState(1).Should().Be(CheckState.Unchecked);
        control.GetItemCheckState(3).Should().Be(CheckState.Checked);
        control.IsHandleCreated.Should().Be(createHandle);
    }

    [WinFormsTheory]
    [InlineData(false, CheckState.Checked, CheckState.Indeterminate)]
    [InlineData(false, CheckState.Checked, CheckState.Unchecked)]
    [InlineData(false, CheckState.Indeterminate, CheckState.Checked)]
    [InlineData(true, CheckState.Checked, CheckState.Indeterminate)]
    [InlineData(true, CheckState.Checked, CheckState.Unchecked)]
    [InlineData(true, CheckState.Indeterminate, CheckState.Checked)]
    public void CheckedListBox_SetItemCheckState_HandlerChangesNewValue_CommitsHandlerValue(bool createHandle, CheckState requested, CheckState replacement)
    {
        // A handler can replace NewValue, and the replacement (not the requested value) is what gets committed.
        // Replacing it with the current Unchecked state means the change is vetoed.
        using CheckedListBox control = CreateCheckedListBox(createHandle);
        control.ItemCheck += (sender, e) => e.NewValue = replacement;

        control.SetItemCheckState(2, requested);

        control.GetItemCheckState(2).Should().Be(replacement);
        control.CheckedIndices.Cast<int>().Should().Equal(replacement == CheckState.Unchecked ? Array.Empty<int>() : new[] { 2 });
        control.IsHandleCreated.Should().Be(createHandle);
    }

    [WinFormsTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void CheckedListBox_SetItemCheckState_HandlerRestoresCurrentValue_KeepsStateAndCollections(bool createHandle)
    {
        // A handler that sets NewValue back to the current value cancels the change and leaves the state alone.
        using CheckedListBox control = CreateCheckedListBox(createHandle);
        control.SetItemChecked(0, true);
        control.ItemCheck += (sender, e) => e.NewValue = e.CurrentValue;

        control.SetItemCheckState(0, CheckState.Indeterminate);
        control.SetItemCheckState(1, CheckState.Checked);

        control.GetItemCheckState(0).Should().Be(CheckState.Checked);
        control.GetItemCheckState(1).Should().Be(CheckState.Unchecked);
        control.CheckedIndices.Cast<int>().Should().Equal(0);
        control.CheckedItems.Cast<string>().Should().Equal("zero");
    }

    [WinFormsTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void CheckedListBox_CheckedCollections_FollowCheckingNotSelection(bool createHandle)
    {
        // CheckedItems/CheckedIndices track the checked (including indeterminate) items in index order, and
        // are unaffected by which items are selected.
        using CheckedListBox control = CreateCheckedListBox(createHandle);
        control.SelectedIndex = 0;

        control.SetItemCheckState(3, CheckState.Indeterminate);
        control.SetItemChecked(1, true);

        control.SelectedIndex.Should().Be(0);
        control.CheckedIndices.Cast<int>().Should().Equal(1, 3);
        control.CheckedItems.Cast<string>().Should().Equal("one", "three");
        control.CheckedItems.Contains("zero").Should().BeFalse();
        control.CheckedIndices.Contains(0).Should().BeFalse();
        control.GetItemChecked(0).Should().BeFalse();
        control.GetItemChecked(1).Should().BeTrue();
        control.GetItemChecked(3).Should().BeTrue();

        control.SelectedIndex = 2;
        control.CheckedIndices.Cast<int>().Should().Equal(1, 3);

        control.SetItemChecked(1, false);
        control.CheckedIndices.Cast<int>().Should().Equal(3);
        control.CheckedItems.Cast<string>().Should().Equal("three");
        control.SelectedIndex.Should().Be(2);
    }

    [WinFormsFact]
    public void CheckedListBox_RecreateHandle_PreservesIndeterminateAndCheckedStates()
    {
        // Check states, including Indeterminate, are cached by the control and survive native handle recreation.
        using SubCheckedListBox control = new();
        control.Items.AddRange(["zero", "one", "two", "three"]);
        control.CreateControl();
        control.SetItemCheckState(0, CheckState.Indeterminate);
        control.SetItemCheckState(2, CheckState.Checked);
        control.SelectedIndex = 1;
        IntPtr oldHandle = control.Handle;

        control.RecreateHandle();

        control.IsHandleCreated.Should().BeTrue();
        control.Handle.Should().NotBe(oldHandle);
        control.GetItemCheckState(0).Should().Be(CheckState.Indeterminate);
        control.GetItemCheckState(1).Should().Be(CheckState.Unchecked);
        control.GetItemCheckState(2).Should().Be(CheckState.Checked);
        control.CheckedIndices.Cast<int>().Should().Equal(0, 2);
        control.SelectedIndex.Should().Be(1);
    }

    private static void ClickItem(CheckedListBox control, int index)
    {
        RECT rect = default;
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.LB_GETITEMRECT, (WPARAM)index, ref rect));
        Point point = new(rect.left + (rect.Width / 2), rect.top + (rect.Height / 2));
        PInvokeCore.SendMessage(control, PInvokeCore.WM_LBUTTONDOWN, 1, PARAM.FromPoint(point));
        PInvokeCore.SendMessage(control, PInvokeCore.WM_LBUTTONUP, 0, PARAM.FromPoint(point));
    }

    private static CheckedListBox CreateHostedCheckedListBox(Form form, bool checkOnClick)
    {
        CheckedListBox control = CreateCheckedListBox(createHandle: false);
        control.CheckOnClick = checkOnClick;
        control.Dock = DockStyle.Fill;
        form.Controls.Add(control);
        form.CreateControl();
        _ = control.Handle;
        return control;
    }

    [WinFormsFact]
    public void CheckedListBox_UserClick_CheckOnClick_TogglesWithItemCheckAndSelectionSeparately()
    {
        // With CheckOnClick, a single click both selects the item and toggles it, raising ItemCheck with the
        // old and new state; the checked collections reflect the check, not the selection.
        using Form form = new();
        CheckedListBox control = CreateHostedCheckedListBox(form, checkOnClick: true);
        List<(int Index, CheckState Current, CheckState New)> events = [];
        control.ItemCheck += (sender, e) => events.Add((e.Index, e.CurrentValue, e.NewValue));

        ClickItem(control, 1);

        events.Should().Equal((1, CheckState.Unchecked, CheckState.Checked));
        control.SelectedIndex.Should().Be(1);
        control.CheckedIndices.Cast<int>().Should().Equal(1);

        // Selecting and checking a different item leaves the first one checked but no longer selected.
        ClickItem(control, 2);

        events.Should().Equal(
            (1, CheckState.Unchecked, CheckState.Checked),
            (2, CheckState.Unchecked, CheckState.Checked));
        control.SelectedIndex.Should().Be(2);
        control.CheckedIndices.Cast<int>().Should().Equal(1, 2);
        control.CheckedItems.Cast<string>().Should().Equal("one", "two");

        // Clicking a checked item unchecks it.
        ClickItem(control, 1);

        events.Should().HaveCount(3);
        events[2].Should().Be((1, CheckState.Checked, CheckState.Unchecked));
        control.GetItemCheckState(1).Should().Be(CheckState.Unchecked);
        control.CheckedIndices.Cast<int>().Should().Equal(2);
    }

    [WinFormsFact]
    public void CheckedListBox_UserClick_WithoutCheckOnClick_RequiresSecondClickOnSelectedItem()
    {
        // Without CheckOnClick, the first click only selects an item (no ItemCheck, nothing checked); clicking the
        // already-selected item then toggles it.
        using Form form = new();
        CheckedListBox control = CreateHostedCheckedListBox(form, checkOnClick: false);
        List<(int Index, CheckState Current, CheckState New)> events = [];
        control.ItemCheck += (sender, e) => events.Add((e.Index, e.CurrentValue, e.NewValue));

        ClickItem(control, 2);

        events.Should().BeEmpty();
        control.SelectedIndex.Should().Be(2);
        control.CheckedIndices.Count.Should().Be(0);

        ClickItem(control, 2);

        events.Should().Equal((2, CheckState.Unchecked, CheckState.Checked));
        control.SelectedIndex.Should().Be(2);
        control.CheckedIndices.Cast<int>().Should().Equal(2);
    }

    [WinFormsFact]
    public void CheckedListBox_UserClick_IndeterminateItem_UnchecksWithItemCheck()
    {
        // Clicking an indeterminate item counts as checked-to-unchecked: ItemCheck reports Indeterminate as the
        // old state and Unchecked as the new one, and the item leaves both checked collections.
        using Form form = new();
        CheckedListBox control = CreateHostedCheckedListBox(form, checkOnClick: true);
        control.SetItemCheckState(1, CheckState.Indeterminate);
        control.SetItemChecked(2, true);
        List<(int Index, CheckState Current, CheckState New)> events = [];
        control.ItemCheck += (sender, e) => events.Add((e.Index, e.CurrentValue, e.NewValue));

        ClickItem(control, 1);

        events.Should().Equal((1, CheckState.Indeterminate, CheckState.Unchecked));
        control.GetItemCheckState(1).Should().Be(CheckState.Unchecked);
        control.CheckedIndices.Cast<int>().Should().Equal(2);
        control.CheckedItems.Cast<string>().Should().Equal("two");
    }

    [WinFormsTheory]
    [InlineData(CheckState.Indeterminate)]
    [InlineData(CheckState.Unchecked)]
    public void CheckedListBox_UserClick_HandlerChangesNewValue_CommitsHandlerValue(CheckState replacement)
    {
        // When a handler replaces NewValue during a user click, the replacement is committed: Indeterminate stays
        // indeterminate (and counts as checked), while Unchecked vetoes the check.
        using Form form = new();
        CheckedListBox control = CreateHostedCheckedListBox(form, checkOnClick: true);
        control.ItemCheck += (sender, e) => e.NewValue = replacement;

        ClickItem(control, 3);

        control.GetItemCheckState(3).Should().Be(replacement);
        control.SelectedIndex.Should().Be(3);
        control.CheckedIndices.Cast<int>().Should().Equal(replacement == CheckState.Unchecked ? Array.Empty<int>() : new[] { 3 });
    }

    public static IEnumerable<object[]> OnDrawItem_TestData()
    {
        yield return new object[] { null, Rectangle.Empty, 0, DrawItemState.Default, Color.Red, Color.Blue };
        yield return new object[] { null, new Rectangle(1, 2, 3, 4), 1, DrawItemState.None, Color.Red, Color.Blue };
        yield return new object[] { null, new Rectangle(1, 2, 3, 4), 1, DrawItemState.Checked, Color.Red, Color.Blue };
        yield return new object[] { new Font("Arial", 8.25f), new Rectangle(10, 20, 30, 40), 1, DrawItemState.Default, Color.Red, Color.Blue };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnDrawItem_TestData))]
    public void CheckedListBox_OnDrawItem_Invoke_Success(Font font, Rectangle rect, int index, DrawItemState state, Color foreColor, Color backColor)
    {
        using SubCheckedListBox control = new();
        control.Items.Add("item1");
        control.Items.Add("item2");

        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        DrawItemEventArgs e = new(graphics, font, rect, index, state, foreColor, backColor);
        control.OnDrawItem(e);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnDrawItem_TestData))]
    public void CheckedListBox_OnDrawItem_InvokeWithHandle_Success(Font font, Rectangle rect, int index, DrawItemState state, Color foreColor, Color backColor)
    {
        using SubCheckedListBox control = new();
        control.Items.Add("item1");
        control.Items.Add("item2");
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        DrawItemEventArgs e = new(graphics, font, rect, index, state, foreColor, backColor);
        control.OnDrawItem(e);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void CheckedListBox_OnDrawItem_NullE_ThrowsNullReferenceException()
    {
        using SubCheckedListBox control = new();
        Assert.Throws<NullReferenceException>(() => control.OnDrawItem(null));
    }

    [WinFormsFact]
    public void CheckedListBox_OnDrawItem_NegativeEIndex_Success()
    {
        using SubCheckedListBox control = new();
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        DrawItemEventArgs e = new(graphics, null, new Rectangle(1, 2, 3, 4), -1, DrawItemState.Default);
        control.OnDrawItem(e);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CheckedListBox_OnDrawItem_LargeEIndexEmpty_Success()
    {
        using SubCheckedListBox control = new();
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        DrawItemEventArgs e = new(graphics, null, new Rectangle(1, 2, 3, 4), 1, DrawItemState.Default);
        control.OnDrawItem(e);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CheckedListBox_OnDrawItem_LargeEIndexNotEmpty_Success()
    {
        using SubCheckedListBox control = new();
        control.Items.Add("item1");

        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        DrawItemEventArgs e = new(graphics, null, new Rectangle(1, 2, 3, 4), 2, DrawItemState.Default);
        control.OnDrawItem(e);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void CheckedListBox_Remove_NotSelectedItems(bool createControl)
    {
        using CheckedListBox checkedListBox = new();

        if (createControl)
        {
            checkedListBox.CreateControl();
        }

        checkedListBox.Items.AddRange((object[])["1", "2", "3"]);
        checkedListBox.SelectedItem = checkedListBox.Items[0];

        Assert.Equal(3, checkedListBox.Items.Count);
        Assert.Equal(checkedListBox.Items[0], checkedListBox.SelectedItem);
        Assert.Equal(0, checkedListBox.SelectedIndex);
        Assert.Single(checkedListBox.SelectedIndices);
        Assert.Single(checkedListBox.SelectedItems);

        checkedListBox.Items.Remove(checkedListBox.Items[2]);

        Assert.Equal(2, checkedListBox.Items.Count);
        Assert.Equal(checkedListBox.Items[0], checkedListBox.SelectedItem);
        Assert.Equal(0, checkedListBox.SelectedIndex);
        Assert.Single(checkedListBox.SelectedIndices);
        Assert.Single(checkedListBox.SelectedItems);

        checkedListBox.Items.Remove(checkedListBox.Items[1]);

        Assert.Single(checkedListBox.Items);
        Assert.Equal(checkedListBox.Items[0], checkedListBox.SelectedItem);
        Assert.Equal(0, checkedListBox.SelectedIndex);
        Assert.Single(checkedListBox.SelectedIndices);
        Assert.Single(checkedListBox.SelectedItems);
        Assert.Equal(createControl, checkedListBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void CheckedListBox_Remove_SelectedItem(bool createControl)
    {
        using CheckedListBox checkedListBox = new();

        if (createControl)
        {
            checkedListBox.CreateControl();
        }

        checkedListBox.Items.AddRange((object[])["1", "2", "3"]);

        for (int count = checkedListBox.Items.Count; count > 1; count -= 1)
        {
            checkedListBox.SelectedItem = checkedListBox.Items[0];

            Assert.Equal(checkedListBox.Items[0], checkedListBox.SelectedItem);
            Assert.Equal(0, checkedListBox.SelectedIndex);
            Assert.Single(checkedListBox.SelectedIndices);
            Assert.Single(checkedListBox.SelectedItems);

            checkedListBox.Items.Remove(checkedListBox.Items[0]);
            count -= 1;

            Assert.Equal(count, checkedListBox.Items.Count);
            Assert.Null(checkedListBox.SelectedItem);
            Assert.Equal(-1, checkedListBox.SelectedIndex);
            Assert.Empty(checkedListBox.SelectedIndices);
            Assert.Empty(checkedListBox.SelectedItems);
        }
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void CheckedListBox_Remove_UncheckedItems(bool createControl)
    {
        using CheckedListBox checkedListBox = new();

        if (createControl)
        {
            checkedListBox.CreateControl();
        }

        checkedListBox.Items.AddRange((object[])["1", "2", "3"]);

        checkedListBox.SetItemChecked(0, true);

        for (int count = checkedListBox.Items.Count; count > 1; count -= 1)
        {
            Assert.Single(checkedListBox.CheckedIndices);
            Assert.Single(checkedListBox.CheckedItems);

            checkedListBox.Items.Remove(checkedListBox.Items[2]);

            count -= 1;

            Assert.Equal(count, checkedListBox.Items.Count);
            Assert.Single(checkedListBox.CheckedIndices);
            Assert.Single(checkedListBox.CheckedItems);
        }
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void CheckedListBox_Remove_CheckedItems(bool createControl)
    {
        using CheckedListBox checkedListBox = new();

        if (createControl)
        {
            checkedListBox.CreateControl();
        }

        checkedListBox.Items.AddRange((object[])["1", "2", "3"]);

        for (int count = checkedListBox.Items.Count; count > 1; count -= 1)
        {
            checkedListBox.SetItemChecked(0, true);

            Assert.Single(checkedListBox.CheckedIndices);
            Assert.Single(checkedListBox.CheckedItems);

            checkedListBox.Items.Remove(checkedListBox.Items[0]);

            count -= 1;

            Assert.Equal(count, checkedListBox.Items.Count);
            Assert.Empty(checkedListBox.CheckedIndices);
            Assert.Empty(checkedListBox.CheckedItems);
        }
    }

    [WinFormsFact]
    public void CheckedListBox_DataSource_GetSet_ReturnsExpected()
    {
        using CheckedListBox checkedListBox = new();
        List<string> dataSource1 = ["item1", "item2", "item3"];
        List<string> dataSource2 = ["item4", "item5", "item6"];

        int callCount = 0;
        checkedListBox.DataSourceChanged += (sender, e) => callCount++;

        // Test initial DataSource set
        checkedListBox.DataSource = dataSource1;
        checkedListBox.DataSource.Should().BeEquivalentTo(dataSource1);
        callCount.Should().Be(1);

        // Test changing DataSource
        checkedListBox.DataSource = dataSource2;
        checkedListBox.DataSource.Should().BeEquivalentTo(dataSource2);
        callCount.Should().Be(2);

        // Test setting DataSource to null
        checkedListBox.DataSource = null;
        checkedListBox.DataSource.Should().BeNull();
        callCount.Should().Be(3);

        // Test setting DataSource to DataSource
        checkedListBox.DataSource = dataSource2;
        checkedListBox.DataSource.Should().BeEquivalentTo(dataSource2);
        callCount.Should().Be(4);
    }

    [WinFormsTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void CheckedListBox_DataSource_SetWithDisplayMember_PreservesDisplayMember(bool createHandle)
    {
        using CheckedListBox control = new()
        {
            BindingContext = new BindingContext(),
            DisplayMember = "Name"
        };
        if (createHandle)
        {
            Assert.NotEqual(IntPtr.Zero, control.Handle);
        }

        foreach (int count in new[] { 3, 5, 1, 0 })
        {
            var dataSource = Enumerable.Range(0, count).Select(i => new { Name = $"item{i}" }).ToList();

            control.DataSource = dataSource;

            control.DataSource.Should().BeSameAs(dataSource);
            control.DisplayMember.Should().Be("Name");
            control.Items.Cast<object>().Should().Equal(dataSource);
            for (int i = 0; i < count; i++)
            {
                control.GetItemText(control.Items[i]).Should().Be(dataSource[i].Name);
                control.GetItemCheckState(i).Should().Be(CheckState.Unchecked);
            }

            control.IsHandleCreated.Should().Be(createHandle);
        }
    }

    [WinFormsFact]
    public void CheckedListBox_ItemHeight_Get_ReturnsExpected()
    {
        using SubCheckedListBox checkedListBox = new();
        int expectedHeight = checkedListBox.Font.Height + checkedListBox._listItemBordersHeight;

        checkedListBox.ItemHeight.Should().Be(expectedHeight);
    }

    [WinFormsFact]
    public void CheckedListBox_ItemHeight_Set_DoesNotChangeValue()
    {
        using CheckedListBox checkedListBox = new();
        int originalHeight = checkedListBox.ItemHeight;

        checkedListBox.ItemHeight = 50;
        checkedListBox.ItemHeight.Should().Be(originalHeight);

        checkedListBox.ItemHeight = -1;
        checkedListBox.ItemHeight.Should().Be(originalHeight);
    }

    [WinFormsTheory]
    [BoolData]
    public void CheckedListBox_UseCompatibleTextRendering_GetSet_ReturnsExpected(bool value)
    {
        using CheckedListBox checkedListBox = new();

        checkedListBox.UseCompatibleTextRendering = value;
        checkedListBox.UseCompatibleTextRendering.Should().Be(value);
        checkedListBox.UseCompatibleTextRenderingInternal.Should().Be(value);
    }

    [WinFormsFact]
    public void CheckedListBox_DataSourceChanged_AddRemove_Success()
    {
        using SubCheckedListBox checkedListBox = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            callCount++;
            sender.Should().Be(checkedListBox);
            e.Should().Be(EventArgs.Empty);
        };

        checkedListBox.DataSourceChanged += handler;
        checkedListBox.OnDataSourceChanged(EventArgs.Empty);
        callCount.Should().Be(1);

        checkedListBox.DataSourceChanged -= handler;
        checkedListBox.OnDataSourceChanged(EventArgs.Empty);
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void CheckedListBox_DisplayMemberChanged_AddRemove_Success()
    {
        using SubCheckedListBox checkedListBox = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            callCount++;
            sender.Should().Be(checkedListBox);
            e.Should().Be(EventArgs.Empty);
        };

        checkedListBox.DisplayMemberChanged += handler;
        checkedListBox.OnDisplayMemberChanged(EventArgs.Empty);
        callCount.Should().Be(1);

        checkedListBox.DisplayMemberChanged -= handler;
        checkedListBox.OnDisplayMemberChanged(EventArgs.Empty);
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void CheckedListBox_ItemCheck_AddRemove_Success()
    {
        using SubCheckedListBox checkedListBox = new();
        int callCount = 0;
        ItemCheckEventHandler handler = (sender, e) =>
        {
            callCount++;
            sender.Should().Be(checkedListBox);
            e.Should().NotBeNull();
        };

        ItemCheckEventArgs eventArgs = new(0, CheckState.Checked, CheckState.Unchecked);

        checkedListBox.ItemCheck += handler;
        checkedListBox.OnItemCheck(eventArgs);
        callCount.Should().Be(1);

        checkedListBox.ItemCheck -= handler;
        checkedListBox.OnItemCheck(eventArgs);
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void CheckedListBox_Click_AddRemove_Success()
    {
        using SubCheckedListBox checkedListBox = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            callCount++;
            sender.Should().Be(checkedListBox);
            e.Should().Be(EventArgs.Empty);
        };

        checkedListBox.Click += handler;
        checkedListBox.OnClick(EventArgs.Empty);
        callCount.Should().Be(1);

        checkedListBox.Click -= handler;
        checkedListBox.OnClick(EventArgs.Empty);
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void CheckedListBox_MouseClick_AddRemove_Success()
    {
        using SubCheckedListBox checkedListBox = new();
        int callCount = 0;
        MouseEventHandler handler = (sender, e) =>
        {
            callCount++;
            sender.Should().Be(checkedListBox);
            e.Should().NotBeNull();
        };

        MouseEventArgs eventArgs = new(MouseButtons.Left, 1, 0, 0, 0);

        checkedListBox.MouseClick += handler;
        checkedListBox.OnMouseClick(eventArgs);
        callCount.Should().Be(1);

        checkedListBox.MouseClick -= handler;
        checkedListBox.OnMouseClick(eventArgs);
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void CheckedListBox_MeasureItem_AddRemove_Success()
    {
        using SubCheckedListBox checkedListBox = new();
        int callCount = 0;
        MeasureItemEventHandler handler = (sender, e) =>
        {
            callCount++;
            sender.Should().Be(checkedListBox);
            e.Should().NotBeNull();
        };

        using Bitmap image = new(10, 10);
        MeasureItemEventArgs eventArgs = new(Graphics.FromImage(image), 1);

        checkedListBox.MeasureItem += handler;
        checkedListBox.OnMeasureItem(eventArgs);
        callCount.Should().Be(1);

        checkedListBox.MeasureItem -= handler;
        checkedListBox.OnMeasureItem(eventArgs);
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void CheckedListBox_ValueMemberChanged_AddRemove_Success()
    {
        using CheckedListBox checkedListBox = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            callCount++;
            sender.Should().Be(checkedListBox);
            e.Should().Be(EventArgs.Empty);
        };

        checkedListBox.ValueMemberChanged += handler;
        checkedListBox.ValueMember = "Value";
        callCount.Should().Be(1);

        checkedListBox.ValueMemberChanged -= handler;
        checkedListBox.ValueMember = "NewValue";
        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void CheckedListBox_DrawItemEvent_AddRemove_Success()
    {
        using SubCheckedListBox checkedListBox = new();
        DrawItemEventHandler handler = (sender, e) => { };

        Action actAdd = () => checkedListBox.DrawItem += handler;
        Action actRemove = () => checkedListBox.DrawItem -= handler;

        actAdd.Should().NotThrow();
        actRemove.Should().NotThrow();
    }

    private class SubCheckedListBox : CheckedListBox
    {
        public new int _listItemBordersHeight => base._listItemBordersHeight;

        public new void RefreshItems() => base.RefreshItems();

        public new void RecreateHandle() => base.RecreateHandle();

        public new void OnDrawItem(DrawItemEventArgs e) => base.OnDrawItem(e);

        public new void OnDataSourceChanged(EventArgs e) => base.OnDataSourceChanged(e);

        public new void OnDisplayMemberChanged(EventArgs e) => base.OnDisplayMemberChanged(e);

        public new void OnItemCheck(ItemCheckEventArgs e) => base.OnItemCheck(e);

        public new void OnClick(EventArgs e) => base.OnClick(e);

        public new void OnMouseClick(MouseEventArgs e) => base.OnMouseClick(e);

        public new void OnMeasureItem(MeasureItemEventArgs e) => base.OnMeasureItem(e);
    }
}
