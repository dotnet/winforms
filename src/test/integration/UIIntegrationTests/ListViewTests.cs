// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests;

// NOTE: This class contains many tests which don't require user input. Although they arguably belong to the unit
// tests project, these tests assert behaviors of ListView.View=View.Tile, which doesn't work correctly unless
// we ran an app.

public class ListViewTests : ControlTestBase
{
    public ListViewTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [WinFormsTheory]
    [InlineData(false, true, true)]
    [InlineData(false, false, false)]
    [InlineData(true, true, false)]
    public async Task ListView_CheckBox_DoubleClick_WorksExpectedAsync(bool virtualModeEnabled, bool checkBoxesEnabled, bool expected)
    {
        await RunTestAsync(async (form, listView) =>
        {
            InitializeItems(listView, View.Details, virtualModeEnabled, checkBoxesEnabled);

            Point itemCenter = GetCenter(listView.RectangleToScreen(listView.Items[0].Bounds));
            await MoveMouseAsync(form, itemCenter);
            await InputSimulator.SendAsync(
                form,
                inputSimulator => inputSimulator.Mouse.LeftButtonDoubleClick());

            Assert.Equal(listView.Items[0].Checked, expected);
        });
    }

    [ActiveIssue("https://github.com/dotnet/winforms/issues/11328")]
    [WinFormsFact]
    [SkipOnArchitecture(TestArchitectures.X64,
        "Flaky tests, see: https://github.com/dotnet/winforms/issues/11328")]
    public async Task ListView_Group_NavigateKeyboard_SucceedsAsync()
    {
        await RunTestAsync(async (form, listView) =>
        {
            ListViewGroup group = new($"Group 1", HorizontalAlignment.Left) { CollapsedState = ListViewGroupCollapsedState.Expanded };
            ListViewItem item1 = new("g1-1") { Group = group };
            ListViewItem item2 = new("g1-2") { Group = group };
            ListViewItem item3 = new("g1-3") { Group = group };

            listView.Groups.Add(group);
            listView.Items.AddRange(item1, item2, item3);
            listView.Focus();

            bool collapsedStateChangedFired = false;
            listView.GroupCollapsedStateChanged += (sender, e) => collapsedStateChangedFired = true;

            item1.Selected = true;

            await InputSimulator.SendAsync(form, inputSimulator => inputSimulator.Keyboard.KeyPress(VIRTUAL_KEY.VK_RIGHT));
            Assert.False(item1.Selected);
            Assert.True(item2.Selected);
            Assert.False(item3.Selected);
            Assert.False(collapsedStateChangedFired);

            await InputSimulator.SendAsync(form, inputSimulator => inputSimulator.Keyboard.KeyPress(VIRTUAL_KEY.VK_RIGHT));
            Assert.False(item1.Selected);
            Assert.False(item2.Selected);
            Assert.True(item3.Selected);
            Assert.False(collapsedStateChangedFired);

            await InputSimulator.SendAsync(form, inputSimulator => inputSimulator.Keyboard.KeyPress(VIRTUAL_KEY.VK_RIGHT));
            Assert.False(item1.Selected);
            Assert.False(item2.Selected);
            Assert.True(item3.Selected);
            Assert.False(collapsedStateChangedFired);

            await InputSimulator.SendAsync(form, inputSimulator => inputSimulator.Keyboard.KeyPress(VIRTUAL_KEY.VK_LEFT).KeyPress(VIRTUAL_KEY.VK_LEFT));
            Assert.True(item1.Selected);
            Assert.False(item2.Selected);
            Assert.False(item3.Selected);
            Assert.False(collapsedStateChangedFired);

            await InputSimulator.SendAsync(form, inputSimulator => inputSimulator.Keyboard.KeyPress(VIRTUAL_KEY.VK_LEFT));
            Assert.True(item1.Selected);
            Assert.False(item2.Selected);
            Assert.False(item3.Selected);
            Assert.False(collapsedStateChangedFired);

            // Selects header, which selects all items in group
            await InputSimulator.SendAsync(form, inputSimulator => inputSimulator.Keyboard.KeyPress(VIRTUAL_KEY.VK_UP));
            Assert.True(item2.Selected);
            Assert.True(item2.Selected);
            Assert.True(item2.Selected);
            Assert.False(collapsedStateChangedFired);

            // Collapse group
            await InputSimulator.SendAsync(form, inputSimulator => inputSimulator.Keyboard.KeyPress(VIRTUAL_KEY.VK_LEFT).KeyPress(VIRTUAL_KEY.VK_UP).KeyPress(VIRTUAL_KEY.VK_LEFT));
            Assert.Equal(ListViewGroupCollapsedState.Collapsed, group.CollapsedState);
            Assert.True(collapsedStateChangedFired);

            // Expand group
            collapsedStateChangedFired = false;
            await InputSimulator.SendAsync(form, inputSimulator => inputSimulator.Keyboard.KeyPress(VIRTUAL_KEY.VK_UP).KeyPress(VIRTUAL_KEY.VK_RIGHT));
            Assert.True(collapsedStateChangedFired);
            Assert.Equal(ListViewGroupCollapsedState.Expanded, group.CollapsedState);
        });
    }

    [WinFormsTheory]
    [InlineData(2, 2, 150, 150, 0, 1, (int)NavigateDirection.NavigateDirection_FirstChild)]
    [InlineData(4, 3, 150, 150, 0, 3, (int)NavigateDirection.NavigateDirection_LastChild)]
    [InlineData(4, 1, 150, 150, 0, 1, (int)NavigateDirection.NavigateDirection_LastChild)]
    [InlineData(2, 5, 150, 150, 0, 1, (int)NavigateDirection.NavigateDirection_LastChild)]
    [InlineData(10, 10, 100, 100, 0, 5, (int)NavigateDirection.NavigateDirection_LastChild)]
    public async Task ListView_Tile_FragmentNavigate_WorksExpectedAsync(int columnCount, int subItemsCount, int width, int height, int itemIndex, int subItemIndex, int direction)
    {
        await RunTestAsync((form, listView) =>
        {
            InitializeTileList(listView, columnCount, subItemsCount, tileSize: new Size(width, height));

            Application.DoEvents();
            AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;
            IRawElementProviderFragment.Interface? actualAccessibleObject = accessibleObject.FragmentNavigate((NavigateDirection)direction);
            AccessibleObject? expectedAccessibleObject = listView.Items[itemIndex].SubItems[subItemIndex].AccessibilityObject;

            Assert.Equal(expectedAccessibleObject, actualAccessibleObject);

            return Task.CompletedTask;
        });
    }

    [WinFormsTheory]
    [InlineData(1, 0, 150, 150, (int)NavigateDirection.NavigateDirection_FirstChild)]
    [InlineData(1, 2, 150, 150, (int)NavigateDirection.NavigateDirection_FirstChild)]
    [InlineData(2, 1, 10, 10, (int)NavigateDirection.NavigateDirection_FirstChild)]
    [InlineData(4, 0, 150, 150, (int)NavigateDirection.NavigateDirection_LastChild)]
    [InlineData(1, 2, 150, 150, (int)NavigateDirection.NavigateDirection_LastChild)]
    [InlineData(2, 1, 10, 10, (int)NavigateDirection.NavigateDirection_LastChild)]
    public async Task ListView_Tile_FragmentNavigate_ReturnsNullAsync(int columnCount, int subItemsCount, int width, int height, int direction)
    {
        await RunTestAsync((form, listView) =>
        {
            InitializeTileList(listView, columnCount, subItemsCount, tileSize: new Size(width, height));

            AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

            Assert.Null(accessibleObject.FragmentNavigate((NavigateDirection)direction));

            return Task.CompletedTask;
        });
    }

    [WinFormsTheory]
    [InlineData(4, 1)]
    [InlineData(2, 3)]
    public async Task ListView_Tile_SubItem_FragmentNavigate_Sibling_ReturnsNullAsync(int columnCount, int subItemsCount)
    {
        await RunTestAsync((form, listView) =>
        {
            InitializeTileList(listView, columnCount, subItemsCount, tileSize: new Size(100, 100));

            AccessibleObject? accessibleObject = listView.Items[0].SubItems[1].AccessibilityObject;
            IRawElementProviderFragment.Interface? nextAccessibleObject = accessibleObject?.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling);
            IRawElementProviderFragment.Interface? previousAccessibleObject = accessibleObject?.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling);

            Assert.Null(nextAccessibleObject);
            Assert.Null(previousAccessibleObject);

            return Task.CompletedTask;
        });
    }

    [WinFormsTheory]
    [InlineData(4, 3, 150, 150, 3)]
    [InlineData(4, 0, 150, 150, 0)]
    [InlineData(4, 1, 150, 150, 1)]
    [InlineData(2, 5, 150, 150, 1)]
    [InlineData(1, 2, 150, 150, 0)]
    [InlineData(2, 1, 10, 10, 0)]
    [InlineData(10, 10, 100, 100, 5)]
    public async Task ListView_Tile_GetChildCount_ReturnsExpectedAsync(int columnCount, int subItemsCount, int width, int height, int expected)
    {
        await RunTestAsync((form, listView) =>
        {
            InitializeTileList(listView, columnCount, subItemsCount, tileSize: new Size(width, height));

            Application.DoEvents();

            Assert.Equal(expected, listView.Items[0].AccessibilityObject.GetChildCount());

            return Task.CompletedTask;
        });
    }

    [WinFormsTheory]
    [InlineData(4, 1)]
    [InlineData(2, 5)]
    public async Task ListView_Tile_GetChildAsync(int columnCount, int subItemsCount)
    {
        await RunTestAsync((form, listView) =>
        {
            InitializeTileList(listView, columnCount, subItemsCount, tileSize: new Size(150, 150));

            Application.DoEvents();
            AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

            Assert.Null(accessibleObject.GetChild(-1));
            Assert.Equal(listView.Items[0].SubItems[1].AccessibilityObject, accessibleObject.GetChild(0));
            Assert.Null(accessibleObject.GetChild(1));

            return Task.CompletedTask;
        });
    }

    [WinFormsTheory]
    [InlineData(4, 0, 150, 150)]
    [InlineData(1, 2, 150, 150)]
    [InlineData(2, 1, 10, 10)]
    public async Task ListView_Tile_GetChild_ReturnsNullAsync(int columnCount, int subItemsCount, int width, int height)
    {
        await RunTestAsync((form, listView) =>
        {
            InitializeTileList(listView, columnCount, subItemsCount, tileSize: new Size(width, height));

            AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

            Assert.Null(accessibleObject.GetChild(-1));
            Assert.Null(accessibleObject.GetChild(0));
            Assert.Null(accessibleObject.GetChild(1));

            return Task.CompletedTask;
        });
    }

    [WinFormsTheory]
    [InlineData(4, 1, 150, 150, -1, 1)]
    [InlineData(2, 1, 10, 10, -1, -1)]
    public async Task ListView_Tile_GetChildIndex_ForDifferentSize_ReturnsExpectedAsync(int columnCount, int subItemsCount, int width, int height, int expected1, int expected2)
    {
        await RunTestAsync((form, listView) =>
        {
            InitializeTileList(listView, columnCount, subItemsCount, tileSize: new Size(width, height));

            Application.DoEvents();
            AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

            Assert.Equal(expected1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[0].AccessibilityObject));
            Assert.Equal(expected2, accessibleObject.GetChildIndex(listView.Items[0].SubItems[1].AccessibilityObject));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task ListView_Tile_SubItem_FragmentNavigate_NextSibling_ReturnsExpectedAsync()
    {
        await RunTestAsync((form, listView) =>
        {
            InitializeTileList(listView, columnCount: 4, subItemsCount: 4, tileSize: new Size(100, 100));

            Application.DoEvents();
            AccessibleObject? accessibleObject1 = listView.Items[0].SubItems[1].AccessibilityObject;
            AccessibleObject? accessibleObject2 = (AccessibleObject?)accessibleObject1?.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling);
            AccessibleObject? accessibleObject3 = (AccessibleObject?)accessibleObject2?.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling);
            AccessibleObject? accessibleObject4 = (AccessibleObject?)accessibleObject3?.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling);

            Assert.Equal(listView.Items[0].SubItems[2].AccessibilityObject, accessibleObject2);
            Assert.Equal(listView.Items[0].SubItems[3].AccessibilityObject, accessibleObject3);
            Assert.Null(accessibleObject4);

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task ListView_Tile_SubItem_HitTest_ReturnExpectedAsync()
    {
        await RunTestAsync((form, listView) =>
        {
            InitializeTileList(listView, columnCount: 3, subItemsCount: 2, tileSize: new Size(100, 100));

            Application.DoEvents();

            AccessibleObject? expectedAccessibleItem1 = listView.Items[0].SubItems[1].AccessibilityObject;
            AccessibleObject? expectedAccessibleItem2 = listView.Items[0].SubItems[2].AccessibilityObject;
            AccessibleObject actualAccessibleItem1 = HitTest(GetSubItemLocation(0, 1));
            AccessibleObject actualAccessibleItem2 = HitTest(GetSubItemLocation(0, 2));

            Assert.Equal(expectedAccessibleItem1, actualAccessibleItem1);
            Assert.Equal(expectedAccessibleItem2, actualAccessibleItem2);

            AccessibleObject HitTest(Point point) =>
                listView.AccessibilityObject.HitTest(point.X, point.Y)!;
            Point GetSubItemLocation(int itemIndex, int subItemIndex) =>
                listView.PointToScreen(listView.GetSubItemRect(itemIndex, subItemIndex, ItemBoundsPortion.Label).Location);

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task ListView_Tile_SubItem_FragmentNavigate_Sibling_ReturnsNull_For_SmallSizeAsync()
    {
        await RunTestAsync((form, listView) =>
        {
            InitializeTileList(listView, columnCount: 5, subItemsCount: 5, tileSize: new Size(50, 40));

            AccessibleObject? accessibleObject = listView.Items[0].SubItems[1].AccessibilityObject;
            IRawElementProviderFragment.Interface? nextAccessibleObject = accessibleObject?.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling);
            IRawElementProviderFragment.Interface? previousAccessibleObject = accessibleObject?.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling);

            Assert.Null(nextAccessibleObject);
            Assert.Null(previousAccessibleObject);

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task ListView_Tile_SubItem_FragmentNavigate_Child_ReturnsNullAsync()
    {
        await RunTestAsync((form, listView) =>
        {
            InitializeTileList(listView, columnCount: 5, subItemsCount: 5, tileSize: new Size(50, 40));

            Application.DoEvents();
            AccessibleObject accessibleObject = (AccessibleObject)listView.Items[0].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild)!;

            Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
            Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task ListView_Tile_GetChild_ReturnsExpectedAsync()
    {
        await RunTestAsync((form, listView) =>
        {
            InitializeTileList(listView, columnCount: 4, subItemsCount: 3, tileSize: new Size(150, 150));

            Application.DoEvents();
            AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

            Assert.Null(accessibleObject.GetChild(-1));
            Assert.Equal(listView.Items[0].SubItems[1].AccessibilityObject, accessibleObject.GetChild(0));
            Assert.Equal(listView.Items[0].SubItems[2].AccessibilityObject, accessibleObject.GetChild(1));
            Assert.Equal(listView.Items[0].SubItems[3].AccessibilityObject, accessibleObject.GetChild(2));
            Assert.Null(accessibleObject.GetChild(3));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task ListView_Tile_GetChild_ReturnsExpected_For_BigSizeAsync()
    {
        await RunTestAsync((form, listView) =>
        {
            InitializeTileList(listView, columnCount: 10, subItemsCount: 10, tileSize: new Size(100, 100));

            Application.DoEvents();
            AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

            Assert.Null(accessibleObject.GetChild(-1));
            Assert.Equal(listView.Items[0].SubItems[1].AccessibilityObject, accessibleObject.GetChild(0));
            Assert.Equal(listView.Items[0].SubItems[2].AccessibilityObject, accessibleObject.GetChild(1));
            Assert.Equal(listView.Items[0].SubItems[3].AccessibilityObject, accessibleObject.GetChild(2));
            Assert.Equal(listView.Items[0].SubItems[4].AccessibilityObject, accessibleObject.GetChild(3));
            Assert.Equal(listView.Items[0].SubItems[5].AccessibilityObject, accessibleObject.GetChild(4));
            Assert.Null(accessibleObject.GetChild(5));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task ListView_Tile_GetChildIndex_ReturnsExpectedAsync()
    {
        await RunTestAsync((form, listView) =>
        {
            InitializeTileList(listView, columnCount: 4, subItemsCount: 3, tileSize: new Size(150, 150));

            Application.DoEvents();
            AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[0].AccessibilityObject));
            Assert.Equal(1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[1].AccessibilityObject));
            Assert.Equal(2, accessibleObject.GetChildIndex(listView.Items[0].SubItems[2].AccessibilityObject));
            Assert.Equal(3, accessibleObject.GetChildIndex(listView.Items[0].SubItems[3].AccessibilityObject));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task ListView_Tile_GetChildIndex_ReturnsExpected_SubItemsMoreThanColumnsAsync()
    {
        await RunTestAsync((form, listView) =>
        {
            InitializeTileList(listView, columnCount: 2, subItemsCount: 5, tileSize: new Size(150, 150));

            Application.DoEvents();
            AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[0].AccessibilityObject));
            Assert.Equal(1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[1].AccessibilityObject));
            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[2].AccessibilityObject));
            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[3].AccessibilityObject));
            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[4].AccessibilityObject));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task ListView_Tile_GetChildIndex_ReturnsMinusOne_For_Single_ColumnAsync()
    {
        await RunTestAsync((form, listView) =>
        {
            InitializeTileList(listView, columnCount: 1, subItemsCount: 2, tileSize: new Size(150, 150));

            AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[0].AccessibilityObject));
            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[1].AccessibilityObject));
            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[2].AccessibilityObject));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task ListView_Tile_GetChildIndex_ReturnsExpected_For_BigSizeAsync()
    {
        await RunTestAsync((form, listView) =>
        {
            InitializeTileList(listView, columnCount: 10, subItemsCount: 10, tileSize: new Size(100, 100));

            Application.DoEvents();
            AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[0].AccessibilityObject));
            Assert.Equal(1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[1].AccessibilityObject));
            Assert.Equal(2, accessibleObject.GetChildIndex(listView.Items[0].SubItems[2].AccessibilityObject));
            Assert.Equal(3, accessibleObject.GetChildIndex(listView.Items[0].SubItems[3].AccessibilityObject));
            Assert.Equal(4, accessibleObject.GetChildIndex(listView.Items[0].SubItems[4].AccessibilityObject));
            Assert.Equal(5, accessibleObject.GetChildIndex(listView.Items[0].SubItems[5].AccessibilityObject));
            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[6].AccessibilityObject));
            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[7].AccessibilityObject));
            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[8].AccessibilityObject));
            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[9].AccessibilityObject));
            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[10].AccessibilityObject));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task ListView_Tile_ColumnProperty_ReturnsMinusOneAsync()
    {
        await RunTestAsync((form, listView) =>
        {
            InitializeTileList(listView, columnCount: 4, subItemsCount: 3, tileSize: new Size(150, 150));

            Assert.Equal(listView.Items[0].SubItems[0].AccessibilityObject?.Column, -1);
            Assert.Equal(listView.Items[0].SubItems[1].AccessibilityObject?.Column, -1);
            Assert.Equal(listView.Items[0].SubItems[2].AccessibilityObject?.Column, -1);
            Assert.Equal(listView.Items[0].SubItems[2].AccessibilityObject?.Column, -1);

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task ListView_Click_On_Second_Column_Does_Not_Alter_CheckBoxesAsync()
    {
        await RunTestAsync(async (form, listView) =>
        {
            InitializeItems(listView, View.Details, virtualModeEnabled: false, checkBoxesEnabled: true);

            foreach (ListViewItem item in listView.Items)
            {
                Assert.Equal(0, item.StateImageIndex);
                Assert.False(item.Selected);
            }

            Point listViewCenter = GetCenter(listView.RectangleToScreen(listView.Items[0].SubItems[1].Bounds));
            await MoveMouseAsync(form, listViewCenter);
            await InputSimulator.SendAsync(
               form,
               inputSimulator => inputSimulator.Keyboard.KeyDown(VIRTUAL_KEY.VK_SHIFT)
                                                .Mouse.LeftButtonClick());
            listViewCenter = GetCenter(listView.RectangleToScreen(listView.Items[2].SubItems[1].Bounds));
            await MoveMouseAsync(form, listViewCenter);
            await InputSimulator.SendAsync(
               form,
               inputSimulator => inputSimulator.Mouse.LeftButtonClick()
                                               .Keyboard.KeyUp(VIRTUAL_KEY.VK_SHIFT));

            foreach (ListViewItem item in listView.Items)
            {
                Assert.Equal(0, item.StateImageIndex);
                Assert.True(item.Selected);
            }
        });
    }

    private void InitializeItems(ListView listView, View view, bool virtualModeEnabled, bool checkBoxesEnabled)
    {
        ColumnHeader columnHeader1 = new() { Text = "ColumnHeader1", Width = 140 };
        ColumnHeader columnHeader2 = new() { Text = "ColumnHeader2", Width = 140 };
        ColumnHeader columnHeader3 = new() { Text = "ColumnHeader3", Width = 140 };
        listView.Columns.AddRange([columnHeader1, columnHeader2, columnHeader3]);
        ListViewItem listViewItem1 = new(["row1", "row1Col2", "row1Col3"], -1) { StateImageIndex = 0 };
        ListViewItem listViewItem2 = new(["row2", "row2Col2", "row2Col3"], -1) { StateImageIndex = 0 };
        ListViewItem listViewItem3 = new(["row3", "row3Col2", "row3Col3"], -1) { StateImageIndex = 0 };
        listView.RetrieveVirtualItem += (s, e) =>
        {
            e.Item = e.ItemIndex switch
            {
                0 => listViewItem1,
                1 => listViewItem2,
                2 => listViewItem3,
                _ => listViewItem1,
            };
        };

        listView.CheckBoxes = checkBoxesEnabled;
        listView.FullRowSelect = true;
        listView.View = view;
        listView.VirtualMode = virtualModeEnabled;
        listView.VirtualListSize = 3;
        if (!virtualModeEnabled)
        {
            listView.Items.AddRange(listViewItem1, listViewItem2, listViewItem3);
        }
    }

    private void InitializeTileList(ListView listView, int columnCount, int subItemsCount, Size tileSize)
    {
        listView.View = View.Tile;
        listView.TileSize = tileSize;

        for (int i = 0; i < columnCount; i++)
        {
            listView.Columns.Add(new ColumnHeader() { Text = $"ColumnHeader{i}" });
        }

        ListViewItem listViewItem = new("Test");
        for (int i = 0; i < subItemsCount; i++)
        {
            listViewItem.SubItems.Add(new ListViewItem.ListViewSubItem() { Text = $"Test SubItem{i}" });
        }

        listView.Items.Add(listViewItem);
    }

    private async Task RunTestAsync(Func<Form, ListView, Task> runTest)
    {
        await RunSingleControlTestAsync(
            testDriverAsync: runTest,
            createControl: () =>
            {
                ListView control = new()
                {
                    Dock = DockStyle.Fill
                };

                return control;
            },
            createForm: () =>
            {
                return new()
                {
                    Size = new Size(500, 400)
                };
            });
    }
}
