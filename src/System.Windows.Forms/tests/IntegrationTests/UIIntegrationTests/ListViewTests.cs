// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using WindowsInput.Native;
using Xunit;
using Xunit.Abstractions;
using static Interop.UiaCore;

namespace System.Windows.Forms.UITests
{
    public class ListViewTests : ControlTestBase
    {
        public ListViewTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [WinFormsFact]
        public async Task Click_On_Second_Column_Does_Not_Alter_CheckboxesAsync()
        {
            await RunTestAsync(async (form, listView) =>
            {
                InitializeItems(listView, View.Details, virtualModeEnabled: false, checkBoxesEnabled: true);

                foreach (ListViewItem item in listView.Items)
                {
                    Assert.Equal(0, item.StateImageIndex);
                }

                foreach (ListViewItem item in listView.Items)
                {
                    Assert.False(item.Selected);
                }

                Point listViewCenter = GetCenter(listView.RectangleToScreen(listView.Items[0].SubItems[1].Bounds));

                await MoveMouseAsync(form, listViewCenter);

                await InputSimulator.SendAsync(
                   form,
                   inputSimulator => inputSimulator.Keyboard.KeyDown(VirtualKeyCode.SHIFT));

                await InputSimulator.SendAsync(
                    form,
                    inputSimulator => inputSimulator.Mouse.LeftButtonClick());

                listViewCenter = GetCenter(listView.RectangleToScreen(listView.Items[2].SubItems[1].Bounds));

                await MoveMouseAsync(form, listViewCenter);

                await InputSimulator.SendAsync(
                   form,
                   inputSimulator => inputSimulator.Mouse.LeftButtonClick());

                await InputSimulator.SendAsync(
                   form,
                   inputSimulator => inputSimulator.Keyboard.KeyUp(VirtualKeyCode.SHIFT));

                foreach (ListViewItem item in listView.Items)
                {
                    Assert.Equal(0, item.StateImageIndex);
                }

                foreach (ListViewItem item in listView.Items)
                {
                    Assert.True(item.Selected);
                }
            });
        }

        [WinFormsFact]
        public async Task ListView_DoubleClick_Updates_Checkbox_StateAsync()
        {
            await RunTestAsync(async (form, listView) =>
            {
                InitializeItems(listView, View.Details, virtualModeEnabled: false, checkBoxesEnabled: true);

                bool checkBoxState = listView.Items[0].Checked;

                Point itemCenter = GetCenter(listView.RectangleToScreen(listView.Items[0].Bounds));

                await MoveMouseAsync(form, itemCenter);

                await InputSimulator.SendAsync(
                    form,
                    inputSimulator => inputSimulator.Mouse.LeftButtonDoubleClick());

                Assert.NotEqual(listView.Items[0].Checked, checkBoxState);
            });
        }

        [WinFormsFact]
        public async Task ListView_CheckBoxDisabled_DoubleClick_DoesNotUpdate_Checkbox_StateAsync()
        {
            await RunTestAsync(async (form, listView) =>
            {
                InitializeItems(listView, View.Details, virtualModeEnabled: false, checkBoxesEnabled: false);

                bool checkBoxState = listView.Items[0].Checked;

                Point itemCenter = GetCenter(listView.RectangleToScreen(listView.Items[0].Bounds));

                await MoveMouseAsync(form, itemCenter);

                await InputSimulator.SendAsync(
                    form,
                    inputSimulator => inputSimulator.Mouse.LeftButtonDoubleClick());

                Assert.Equal(listView.Items[0].Checked, checkBoxState);
            });
        }

        [WinFormsFact]
        public async Task ListView_VirtualMode_DoubleClick_DoesNotUpdate_Checkbox_StateAsync()
        {
            await RunTestAsync(async (form, listView) =>
            {
                InitializeItems(listView, View.Details, virtualModeEnabled: true, checkBoxesEnabled: true);

                bool checkBoxState = listView.Items[0].Checked;

                Point itemCenter = GetCenter(listView.RectangleToScreen(listView.Items[0].Bounds));

                await MoveMouseAsync(form, itemCenter);

                await InputSimulator.SendAsync(
                    form,
                    inputSimulator => inputSimulator.Mouse.LeftButtonDoubleClick());

                Assert.Equal(listView.Items[0].Checked, checkBoxState);
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_FragmentNavigate_FirstChild_ReturnsSubItemAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);

                InitializeTileList(listView, columnCount: 2, subItemsCount: 2, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;
                IRawElementProviderFragment? actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.FirstChild);
                AccessibleObject expectedAccessibleObject = listView.Items[0].SubItems[1].AccessibilityObject;

                Assert.Equal(actualAccessibleObject, expectedAccessibleObject);
                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_FragmentNavigate_FirstChild_ReturnsNull_WithoutSubItemsAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 1, subItemsCount: 0, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;
                IRawElementProviderFragment? actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.FirstChild);

                Assert.Null(actualAccessibleObject);
                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_FragmentNavigate_FirstChild_ReturnsNull_For_Single_ColumnAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 1, subItemsCount: 2, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;
                IRawElementProviderFragment? actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.FirstChild);

                Assert.Null(actualAccessibleObject);
                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_FragmentNavigate_FirstChild_ReturnsNull_For_SmallSizeAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 2, subItemsCount: 1, tileSize: new Size(10, 10));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;
                IRawElementProviderFragment? actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.FirstChild);

                Assert.Null(actualAccessibleObject);
                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_FragmentNavigate_LastChild_ReturnsSubItemAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 4, subItemsCount: 3, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;
                IRawElementProviderFragment? actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.LastChild);
                AccessibleObject expectedAccessibleObject = listView.Items[0].SubItems[3].AccessibilityObject;

                Assert.Equal(actualAccessibleObject, expectedAccessibleObject);
                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_FragmentNavigate_LastChild_ReturnsNull_WithoutSubItemsAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 4, subItemsCount: 0, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;
                IRawElementProviderFragment? actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.LastChild);

                Assert.Null(actualAccessibleObject);
                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_FragmentNavigate_LastChild_ReturnsSubItem_ColumnsMoreThanSubItemsAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);

                InitializeTileList(listView, columnCount: 4, subItemsCount: 1, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;
                IRawElementProviderFragment? actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.LastChild);
                AccessibleObject expectedAccessibleObject = listView.Items[0].SubItems[1].AccessibilityObject;

                Assert.Equal(actualAccessibleObject, expectedAccessibleObject);
                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_FragmentNavigate_LastChild_ReturnsSubItem_SubItemsMoreThanColumnsAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);

                InitializeTileList(listView, columnCount: 2, subItemsCount: 5, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;
                IRawElementProviderFragment? actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.LastChild);
                AccessibleObject expectedAccessibleObject = listView.Items[0].SubItems[1].AccessibilityObject;

                Assert.Equal(actualAccessibleObject, expectedAccessibleObject);
                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_FragmentNavigate_LastChild_ReturnsNull_For_Single_ColumnAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 1, subItemsCount: 2, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;
                IRawElementProviderFragment? actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.FirstChild);

                Assert.Null(actualAccessibleObject);
                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_FragmentNavigate_LastChild_ReturnsNull_For_SmallSizeAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 2, subItemsCount: 1, tileSize: new Size(10, 10));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;
                IRawElementProviderFragment? actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.FirstChild);

                Assert.Null(actualAccessibleObject);
                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_FragmentNavigate_LastChild_ReturnsLastVisibleSubItemsAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 10, subItemsCount: 10, tileSize: new Size(100, 100));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;
                IRawElementProviderFragment? actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.LastChild);
                AccessibleObject expectedAccessibleObject = listView.Items[0].SubItems[5].AccessibilityObject;

                Assert.Equal(actualAccessibleObject, expectedAccessibleObject);
                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_SubItem_FragmentNavigate_NextSibling_ReturnsExpectedAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 4, subItemsCount: 4, tileSize: new Size(100, 100));

                Application.DoEvents();

                AccessibleObject accessibleObject1 = listView.Items[0].SubItems[1].AccessibilityObject;
                AccessibleObject accessibleObject2 = (AccessibleObject)accessibleObject1.FragmentNavigate(NavigateDirection.NextSibling)!;
                AccessibleObject accessibleObject3 = (AccessibleObject)accessibleObject2.FragmentNavigate(NavigateDirection.NextSibling)!;
                AccessibleObject accessibleObject4 = (AccessibleObject)accessibleObject3.FragmentNavigate(NavigateDirection.NextSibling)!;

                Assert.Equal(accessibleObject2, listView.Items[0].SubItems[2].AccessibilityObject);

                Assert.Equal(accessibleObject3, listView.Items[0].SubItems[3].AccessibilityObject);

                Assert.Null(accessibleObject4);
                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_SubItem_FragmentNavigate_Sibling_ReturnsNull_For_Single_SubItemAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 4, subItemsCount: 1, tileSize: new Size(100, 100));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].SubItems[1].AccessibilityObject;
                IRawElementProviderFragment? nextAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.NextSibling);
                IRawElementProviderFragment? previousAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.PreviousSibling);

                Assert.Null(nextAccessibleObject);

                Assert.Null(previousAccessibleObject);
                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_SubItem_FragmentNavigate_Sibling_ReturnsNull_For_Single_ColumnAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 2, subItemsCount: 3, tileSize: new Size(100, 100));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].SubItems[1].AccessibilityObject;
                IRawElementProviderFragment nextAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.NextSibling)!;
                IRawElementProviderFragment previousAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.PreviousSibling)!;

                Assert.Null(nextAccessibleObject);

                Assert.Null(previousAccessibleObject);

                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_SubItem_HitTest_ReturnExpectedAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 3, subItemsCount: 2, tileSize: new Size(100, 100));

                Application.DoEvents();

                AccessibleObject expectedAccessibleItem1 = listView.Items[0].SubItems[1].AccessibilityObject;
                AccessibleObject expectedAccessibleItem2 = listView.Items[0].SubItems[2].AccessibilityObject;
                AccessibleObject actualAccessibleItem1 = HitTest(GetSubItemLocation(0, 1));
                AccessibleObject actualAccessibleItem2 = HitTest(GetSubItemLocation(0, 2));

                Assert.Equal(actualAccessibleItem1, expectedAccessibleItem1);

                Assert.Equal(actualAccessibleItem2, expectedAccessibleItem2);

                AccessibleObject HitTest(Point point) =>
                listView.AccessibilityObject.HitTest(point.X, point.Y)!;

                Point GetSubItemLocation(int itemIndex, int subItemIndex) =>
                    listView.PointToScreen(listView.GetSubItemRect(itemIndex, subItemIndex, ItemBoundsPortion.Label).Location);

                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_GetChildCount_ReturnsExpectedAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 4, subItemsCount: 3, tileSize: new Size(150, 150));

                Application.DoEvents();

                Assert.Equal(3, listView.Items[0].AccessibilityObject.GetChildCount());

                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_GetChildCount_ReturnsZero_WithoutSubItemsAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 4, subItemsCount: 0, tileSize: new Size(150, 150));

                Application.DoEvents();

                Assert.Equal(0, listView.Items[0].AccessibilityObject.GetChildCount());

                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_GetChildCount_ReturnsExpected_ColumnsMoreThanSubItemsAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);

                InitializeTileList(listView, columnCount: 4, subItemsCount: 1, tileSize: new Size(150, 150));

                Application.DoEvents();

                Assert.Equal(1, listView.Items[0].AccessibilityObject.GetChildCount());

                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_GetChildCount_ReturnsExpected_SubItemsMoreThanColumnsAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);

                InitializeTileList(listView, columnCount: 2, subItemsCount: 5, tileSize: new Size(150, 150));

                Application.DoEvents();

                Assert.Equal(1, listView.Items[0].AccessibilityObject.GetChildCount());

                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_SubItem_FragmentNavigate_Sibling_ReturnsNull_For_SmallSizeAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 5, subItemsCount: 5, tileSize: new Size(50, 40));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].SubItems[1].AccessibilityObject;
                IRawElementProviderFragment nextAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.NextSibling)!;
                IRawElementProviderFragment previousAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.PreviousSibling)!;

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
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 5, subItemsCount: 5, tileSize: new Size(50, 40));

                Application.DoEvents();

                AccessibleObject accessibleObject = (AccessibleObject)listView.Items[0].AccessibilityObject.FragmentNavigate(NavigateDirection.FirstChild)!;

                Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.FirstChild));

                Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.LastChild));

                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_GetChildCount_ReturnsZero_For_Single_ColumnAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 1, subItemsCount: 2, tileSize: new Size(150, 150));

                Application.DoEvents();

                Assert.Equal(0, listView.Items[0].AccessibilityObject.GetChildCount());

                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_GetChildCount_ReturnsZero_For_SmallSizeAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 2, subItemsCount: 1, tileSize: new Size(10, 10));

                Application.DoEvents();

                Assert.Equal(0, listView.Items[0].AccessibilityObject.GetChildCount());

                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_GetChildCount_ReturnsExpected_For_BigSizeAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 10, subItemsCount: 10, tileSize: new Size(100, 100));

                Application.DoEvents();

                Assert.Equal(5, listView.Items[0].AccessibilityObject.GetChildCount());

                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_GetChild_ReturnsExpectedAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 4, subItemsCount: 3, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

                Assert.Null(accessibleObject.GetChild(-1));

                Assert.Equal(accessibleObject.GetChild(0), listView.Items[0].SubItems[1].AccessibilityObject);

                Assert.Equal(accessibleObject.GetChild(1), listView.Items[0].SubItems[2].AccessibilityObject);

                Assert.Equal(accessibleObject.GetChild(2), listView.Items[0].SubItems[3].AccessibilityObject);

                Assert.Null(accessibleObject.GetChild(3));

                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_GetChild_ReturnsNull_WithoutSubItemsAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 4, subItemsCount: 0, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

                Assert.Null(accessibleObject.GetChild(-1));

                Assert.Null(accessibleObject.GetChild(0));

                Assert.Null(accessibleObject.GetChild(1));

                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_GetChild_ReturnsExpected_ColumnsMoreThanSubItemsAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);

                InitializeTileList(listView, columnCount: 4, subItemsCount: 1, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

                Assert.Null(accessibleObject.GetChild(-1));

                Assert.Equal(accessibleObject.GetChild(0), listView.Items[0].SubItems[1].AccessibilityObject);

                Assert.Null(accessibleObject.GetChild(1));

                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_GetChild_ReturnsExpected_SubItemsMoreThanColumnsAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);

                InitializeTileList(listView, columnCount: 2, subItemsCount: 5, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

                Assert.Null(accessibleObject.GetChild(-1));

                Assert.Equal(accessibleObject.GetChild(0), listView.Items[0].SubItems[1].AccessibilityObject);

                Assert.Null(accessibleObject.GetChild(1));

                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_GetChild_ReturnsNull_For_Single_ColumnAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 1, subItemsCount: 2, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

                Assert.Null(accessibleObject.GetChild(-1));

                Assert.Null(accessibleObject.GetChild(0));

                Assert.Null(accessibleObject.GetChild(1));

                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_GetChild_ReturnsNull_For_SmallSizeAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 2, subItemsCount: 1, tileSize: new Size(10, 10));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

                Assert.Null(accessibleObject.GetChild(-1));

                Assert.Null(accessibleObject.GetChild(0));

                Assert.Null(accessibleObject.GetChild(1));

                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_GetChild_ReturnsExpected_For_BigSizeAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 10, subItemsCount: 10, tileSize: new Size(100, 100));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

                Assert.Null(accessibleObject.GetChild(-1));

                Assert.Equal(accessibleObject.GetChild(0), listView.Items[0].SubItems[1].AccessibilityObject);

                Assert.Equal(accessibleObject.GetChild(1), listView.Items[0].SubItems[2].AccessibilityObject);

                Assert.Equal(accessibleObject.GetChild(2), listView.Items[0].SubItems[3].AccessibilityObject);

                Assert.Equal(accessibleObject.GetChild(3), listView.Items[0].SubItems[4].AccessibilityObject);

                Assert.Equal(accessibleObject.GetChild(4), listView.Items[0].SubItems[5].AccessibilityObject);

                Assert.Null(accessibleObject.GetChild(5));

                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_GetChildIndex_ReturnsExpectedAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
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
        public async Task ListView_Tile_GetChildIndex_ReturnsExpected_ColumnsMoreThanSubItemsAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);

                InitializeTileList(listView, columnCount: 4, subItemsCount: 1, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

                Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[0].AccessibilityObject));

                Assert.Equal(1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[1].AccessibilityObject));

                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_GetChildIndex_ReturnsExpected_SubItemsMoreThanColumnsAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);

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
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 1, subItemsCount: 2, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

                Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[0].AccessibilityObject));

                Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[1].AccessibilityObject));

                Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[2].AccessibilityObject));

                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_GetChildIndex_ReturnsMinusOne_For_SmallSizeAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 2, subItemsCount: 1, tileSize: new Size(10, 10));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

                Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[0].AccessibilityObject));

                Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[1].AccessibilityObject));

                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ListView_Tile_GetChildIndex_ReturnsExpected_For_BigSizeAsync()
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
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
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 4, subItemsCount: 3, tileSize: new Size(150, 150));

                Application.DoEvents();

                Assert.Equal(-1, listView.Items[0].SubItems[0].AccessibilityObject.Column);

                Assert.Equal(-1, listView.Items[0].SubItems[1].AccessibilityObject.Column);

                Assert.Equal(-1, listView.Items[0].SubItems[2].AccessibilityObject.Column);

                Assert.Equal(-1, listView.Items[0].SubItems[2].AccessibilityObject.Column);

                return Task.CompletedTask;
            });
        }

        private void InitializeItems(ListView listView, View view, bool virtualModeEnabled, bool checkBoxesEnabled)
        {
            var columnHeader1 = new ColumnHeader { Text = "ColumnHeader1", Width = 140 };
            var columnHeader2 = new ColumnHeader { Text = "ColumnHeader2", Width = 140 };
            var columnHeader3 = new ColumnHeader { Text = "ColumnHeader3", Width = 140 };
            listView.Columns.AddRange(new[] { columnHeader1, columnHeader2, columnHeader3 });

            var listViewItem1 = new ListViewItem(new[] { "row1", "row1Col2", "row1Col3" }, -1) { StateImageIndex = 0 };
            var listViewItem2 = new ListViewItem(new[] { "row2", "row2Col2", "row2Col3" }, -1) { StateImageIndex = 0 };
            var listViewItem3 = new ListViewItem(new[] { "row3", "row3Col2", "row3Col3" }, -1) { StateImageIndex = 0 };

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

            listView.Items.Clear();
            listView.CheckBoxes = checkBoxesEnabled;
            listView.FullRowSelect = true;
            listView.View = view;
            listView.VirtualMode = virtualModeEnabled;
            listView.VirtualListSize = 3;

            if (!virtualModeEnabled)
            {
                listView.Items.AddRange(new[] { listViewItem1, listViewItem2, listViewItem3 });
            }
        }

        private void InitializeTileList(ListView listView, int columnCount, int subItemsCount, Size tileSize)
        {
            listView.VirtualListSize = 0;
            listView.Columns.Clear();
            listView.Items.Clear();
            listView.VirtualMode = false;
            listView.CheckBoxes = false;
            listView.TileSize = tileSize;

            for (int i = 0; i < columnCount; i++)
            {
                listView.Columns.Add(new ColumnHeader() { Text = $"ColumnHeader{i}" });
            }

            ListViewItem listViewItem = new ListViewItem("Test");

            for (int i = 0; i < subItemsCount; i++)
            {
                listViewItem.SubItems.Add(new ListViewItem.ListViewSubItem() { Text = $"Test SubItem{i}" });
            }

            listView.Items.Add(listViewItem);
        }

        public static Point GetCenter(Rectangle rect)
        {
            int x = rect.Left + ((rect.Right - rect.Left) / 2);
            int y = rect.Top + ((rect.Bottom - rect.Top) / 2);
            return new Point(x, y);
        }

        private async Task RunTestAsync(Func<Form, ListView, Task> runTest)
        {
            await RunSingleControlTestAsync(
                testDriverAsync: runTest,
                createControl: () =>
                {
                    ListView control = new()
                    {
                        Location = new Point(0, 0),
                        Size = new Size(439, 103)
                    };

                    return control;
                },
                createForm: () =>
                {
                    return new()
                    {
                        Size = new(500, 300),
                    };
                });
        }
    }
}
