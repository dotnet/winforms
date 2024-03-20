// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
        yield return new object[] { new Padding(), new Padding(), 0, 0 };
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
        Assert.Equal(1, checkedListBox.SelectedIndices.Count);
        Assert.Equal(1, checkedListBox.SelectedItems.Count);

        checkedListBox.Items.Remove(checkedListBox.Items[2]);

        Assert.Equal(2, checkedListBox.Items.Count);
        Assert.Equal(checkedListBox.Items[0], checkedListBox.SelectedItem);
        Assert.Equal(0, checkedListBox.SelectedIndex);
        Assert.Equal(1, checkedListBox.SelectedIndices.Count);
        Assert.Equal(1, checkedListBox.SelectedItems.Count);

        checkedListBox.Items.Remove(checkedListBox.Items[1]);

        Assert.Equal(1, checkedListBox.Items.Count);
        Assert.Equal(checkedListBox.Items[0], checkedListBox.SelectedItem);
        Assert.Equal(0, checkedListBox.SelectedIndex);
        Assert.Equal(1, checkedListBox.SelectedIndices.Count);
        Assert.Equal(1, checkedListBox.SelectedItems.Count);
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
            Assert.Equal(1, checkedListBox.SelectedIndices.Count);
            Assert.Equal(1, checkedListBox.SelectedItems.Count);

            checkedListBox.Items.Remove(checkedListBox.Items[0]);
            count -= 1;

            Assert.Equal(count, checkedListBox.Items.Count);
            Assert.Null(checkedListBox.SelectedItem);
            Assert.Equal(-1, checkedListBox.SelectedIndex);
            Assert.Equal(0, checkedListBox.SelectedIndices.Count);
            Assert.Equal(0, checkedListBox.SelectedItems.Count);
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
            Assert.Equal(1, checkedListBox.CheckedIndices.Count);
            Assert.Equal(1, checkedListBox.CheckedItems.Count);

            checkedListBox.Items.Remove(checkedListBox.Items[2]);

            count -= 1;

            Assert.Equal(count, checkedListBox.Items.Count);
            Assert.Equal(1, checkedListBox.CheckedIndices.Count);
            Assert.Equal(1, checkedListBox.CheckedItems.Count);
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

            Assert.Equal(1, checkedListBox.CheckedIndices.Count);
            Assert.Equal(1, checkedListBox.CheckedItems.Count);

            checkedListBox.Items.Remove(checkedListBox.Items[0]);

            count -= 1;

            Assert.Equal(count, checkedListBox.Items.Count);
            Assert.Equal(0, checkedListBox.CheckedIndices.Count);
            Assert.Equal(0, checkedListBox.CheckedItems.Count);
        }
    }

    private class SubCheckedListBox : CheckedListBox
    {
        public new void RefreshItems() => base.RefreshItems();

        public new void OnDrawItem(DrawItemEventArgs e) => base.OnDrawItem(e);
    }
}
