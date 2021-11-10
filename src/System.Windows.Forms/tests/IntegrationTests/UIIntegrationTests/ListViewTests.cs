// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.UI.IntegrationTests.Infra;
using Xunit;
using static Interop;
using static Interop.User32;
using static Interop.UiaCore;

namespace System.Windows.Forms.UI.IntegrationTests
{
    [ConfigureJoinableTaskFactory]
    public class ListViewTests
    {
        [StaFact]
        public void Click_On_Second_Column_Does_Not_Alter_Checkboxes()
        {
            RunTest(listView =>
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

                KeyboardHelper.SendKey(Keys.ShiftKey, true);

                var pt = MouseHelper.GetCenter(listView.RectangleToScreen(listView.Items[0].SubItems[1].Bounds));
                MouseHelper.SendClick(pt.X, pt.Y);

                pt = MouseHelper.GetCenter(listView.RectangleToScreen(listView.Items[2].SubItems[1].Bounds));
                MouseHelper.SendClick(pt.X, pt.Y);

                KeyboardHelper.SendKey(Keys.ShiftKey, false);
                Application.DoEvents();

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

        [StaFact]
        public void ListView_DoubleClick_Updates_Checkbox_State()
        {
            RunTest(listView =>
            {
                InitializeItems(listView, View.Details, virtualModeEnabled: false, checkBoxesEnabled: true);

                bool checkBoxState = listView.Items[0].Checked;
                ExecuteDoubleClickOnItem(listView, listView.Items[0]);

                Assert.NotEqual(listView.Items[0].Checked, checkBoxState);
            });
        }

        [StaFact]
        public void ListView_CheckBoxDisabled_DoubleClick_DoesNotUpdate_Checkbox_State()
        {
            RunTest(listView =>
            {
                InitializeItems(listView, View.Details, virtualModeEnabled: false, checkBoxesEnabled: false);

                bool checkBoxState = listView.Items[0].Checked;
                ExecuteDoubleClickOnItem(listView, listView.Items[0]);

                Assert.Equal(listView.Items[0].Checked, checkBoxState);
            });
        }

        [StaFact]
        public void ListView_VirtualMode_DoubleClick_DoesNotUpdate_Checkbox_State()
        {
            RunTest(listView =>
            {
                InitializeItems(listView, View.Details, virtualModeEnabled: true, checkBoxesEnabled: true);

                bool checkBoxState = listView.Items[0].Checked;
                ExecuteDoubleClickOnItem(listView, listView.Items[0]);

                Assert.Equal(listView.Items[0].Checked, checkBoxState);
            });
        }

        [StaFact]
        public void ListView_Tile_FragmentNavigate_FirstChild_ReturnsSubItem()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 2, subItemsCount: 2, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;
                IRawElementProviderFragment? actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.FirstChild);
                AccessibleObject expectedAccessibleObject = listView.Items[0].SubItems[1].AccessibilityObject;

                Assert.Equal(actualAccessibleObject, expectedAccessibleObject);
            });
        }

        [StaFact]
        public void ListView_Tile_FragmentNavigate_FirstChild_ReturnsNull_WithoutSubItems()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 1, subItemsCount: 0, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;
                IRawElementProviderFragment? actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.FirstChild);

                Assert.Null(actualAccessibleObject);
            });
        }

        [StaFact]
        public void ListView_Tile_FragmentNavigate_FirstChild_ReturnsNull_For_Single_Column()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 1, subItemsCount: 2, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;
                IRawElementProviderFragment? actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.FirstChild);

                Assert.Null(actualAccessibleObject);
            });
        }

        [StaFact]
        public void ListView_Tile_FragmentNavigate_FirstChild_ReturnsNull_For_SmallSize()
        {
            RunTest(listView =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 2, subItemsCount: 1, tileSize: new Size(10, 10));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;
                IRawElementProviderFragment? actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.FirstChild);

                Assert.Null(actualAccessibleObject);
            });
        }

        [StaFact]
        public void ListView_Tile_FragmentNavigate_LastChild_ReturnsSubItem()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 4, subItemsCount: 3, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;
                IRawElementProviderFragment? actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.LastChild);
                AccessibleObject expectedAccessibleObject = listView.Items[0].SubItems[3].AccessibilityObject;

                Assert.Equal(actualAccessibleObject, expectedAccessibleObject);
            });
        }

        [StaFact]
        public void ListView_Tile_FragmentNavigate_LastChild_ReturnsNull_WithoutSubItems()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 4, subItemsCount: 1, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;
                IRawElementProviderFragment? actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.LastChild);
                AccessibleObject expectedAccessibleObject = listView.Items[0].SubItems[1].AccessibilityObject;

                Assert.Equal(actualAccessibleObject, expectedAccessibleObject);
            });
        }

        [StaFact]
        public void ListView_Tile_FragmentNavigate_LastChild_ReturnsSubItem_SubItemsMoreThanColumns()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 2, subItemsCount: 5, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;
                IRawElementProviderFragment? actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.LastChild);
                AccessibleObject expectedAccessibleObject = listView.Items[0].SubItems[1].AccessibilityObject;

                Assert.Equal(actualAccessibleObject, expectedAccessibleObject);
            });
        }

        [StaFact]
        public void ListView_Tile_FragmentNavigate_LastChild_ReturnsNull_For_Single_Column()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 1, subItemsCount: 2, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;
                IRawElementProviderFragment? actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.FirstChild);

                Assert.Null(actualAccessibleObject);
            });
        }

        [StaFact]
        public void ListView_Tile_FragmentNavigate_LastChild_ReturnsLastVisibleSubItems()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 10, subItemsCount: 10, tileSize: new Size(100, 100));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;
                IRawElementProviderFragment? actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.LastChild);
                AccessibleObject expectedAccessibleObject = listView.Items[0].SubItems[5].AccessibilityObject;

                Assert.Equal(actualAccessibleObject, expectedAccessibleObject);
            });
        }

        [StaFact]
        public void ListView_Tile_SubItem_FragmentNavigate_NextSibling_ReturnsExpected()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 4, subItemsCount: 4, tileSize: new Size(100, 100));

                Application.DoEvents();

                AccessibleObject accessibleObject1 = listView.Items[0].SubItems[1].AccessibilityObject;
                AccessibleObject? accessibleObject2 = (AccessibleObject?)accessibleObject1.FragmentNavigate(NavigateDirection.NextSibling)!;
                AccessibleObject? accessibleObject3 = (AccessibleObject?)accessibleObject2.FragmentNavigate(NavigateDirection.NextSibling)!;
                AccessibleObject? accessibleObject4 = (AccessibleObject?)accessibleObject3.FragmentNavigate(NavigateDirection.NextSibling)!;

                Assert.Equal(accessibleObject2, listView.Items[0].SubItems[2].AccessibilityObject);

                Assert.Equal(accessibleObject3, listView.Items[0].SubItems[3].AccessibilityObject);

                Assert.Null(accessibleObject4);
            });
        }

        [StaFact]
        public void ListView_Tile_SubItem_FragmentNavigate_Sibling_ReturnsNull_For_Single_SubItem()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 4, subItemsCount: 1, tileSize: new Size(100, 100));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].SubItems[1].AccessibilityObject;
                IRawElementProviderFragment? nextAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.NextSibling);
                IRawElementProviderFragment? previousAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.PreviousSibling);

                Assert.Null(nextAccessibleObject);

                Assert.Null(previousAccessibleObject);
            });
        }

        [StaFact]
        public void ListView_Tile_SubItem_FragmentNavigate_Sibling_ReturnsNull_For_Single_Column()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 2, subItemsCount: 3, tileSize: new Size(100, 100));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].SubItems[1].AccessibilityObject;
                IRawElementProviderFragment? nextAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.NextSibling);
                IRawElementProviderFragment? previousAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.PreviousSibling);

                Assert.Null(nextAccessibleObject);

                Assert.Null(previousAccessibleObject);
            });
        }

        [StaFact]
        public void ListView_Tile_SubItem_FragmentNavigate_Sibling_ReturnsNull_For_SmallSize()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 5, subItemsCount: 5, tileSize: new Size(50, 40));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].SubItems[1].AccessibilityObject;
                IRawElementProviderFragment? nextAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.NextSibling);
                IRawElementProviderFragment? previousAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.PreviousSibling);

                Assert.Null(nextAccessibleObject);

                Assert.Null(previousAccessibleObject);
            });
        }

        [StaFact]
        public void ListView_Tile_SubItem_FragmentNavigate_Child_ReturnsNull()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 5, subItemsCount: 5, tileSize: new Size(50, 40));

                Application.DoEvents();

                AccessibleObject? accessibleObject = (AccessibleObject?)listView.Items[0].AccessibilityObject.FragmentNavigate(NavigateDirection.FirstChild);

                Assert.Null(accessibleObject!.FragmentNavigate(NavigateDirection.FirstChild));

                Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.LastChild));
            });
        }

        [StaFact]
        public void ListView_Tile_SubItem_HitTest_ReturnExpected()
        {
            RunTest(listView =>
            {
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
            });
        }

        [StaFact]
        public void ListView_Tile_GetChildCount_ReturnsExpected()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 4, subItemsCount: 3, tileSize: new Size(150, 150));

                Application.DoEvents();

                Assert.Equal(3, listView.Items[0].AccessibilityObject.GetChildCount());
            });
        }

        [StaFact]
        public void ListView_Tile_GetChildCount_ReturnsZero_WithoutSubItems()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 4, subItemsCount: 0, tileSize: new Size(150, 150));

                Application.DoEvents();

                Assert.Equal(0, listView.Items[0].AccessibilityObject.GetChildCount());
            });
        }

        [StaFact]
        public void ListView_Tile_GetChildCount_ReturnsExpected_ColumnsMoreThanSubItems()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 4, subItemsCount: 1, tileSize: new Size(150, 150));

                Application.DoEvents();

                Assert.Equal(1, listView.Items[0].AccessibilityObject.GetChildCount());
            });
        }

        [StaFact]
        public void ListView_Tile_GetChildCount_ReturnsExpected_SubItemsMoreThanColumns()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 2, subItemsCount: 5, tileSize: new Size(150, 150));

                Application.DoEvents();

                Assert.Equal(1, listView.Items[0].AccessibilityObject.GetChildCount());
            });
        }

        [StaFact]
        public void ListView_Tile_GetChildCount_ReturnsZero_For_Single_Column()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 1, subItemsCount: 2, tileSize: new Size(150, 150));

                Application.DoEvents();

                Assert.Equal(0, listView.Items[0].AccessibilityObject.GetChildCount());
            });
        }

        [StaFact]
        public void ListView_Tile_GetChildCount_ReturnsZero_For_SmallSize()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 2, subItemsCount: 1, tileSize: new Size(10, 10));

                Application.DoEvents();

                Assert.Equal(0, listView.Items[0].AccessibilityObject.GetChildCount());
            });
        }

        [StaFact]
        public void ListView_Tile_GetChildCount_ReturnsExpected_For_BigSize()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 10, subItemsCount: 10, tileSize: new Size(100, 100));

                Application.DoEvents();

                Assert.Equal(5, listView.Items[0].AccessibilityObject.GetChildCount());
            });
        }

        [StaFact]
        public void ListView_Tile_GetChild_ReturnsExpected()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 4, subItemsCount: 3, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

                Assert.Null(accessibleObject.GetChild(-1));

                Assert.Equal(accessibleObject.GetChild(0), listView.Items[0].SubItems[1].AccessibilityObject);

                Assert.Equal(accessibleObject.GetChild(1), listView.Items[0].SubItems[2].AccessibilityObject);

                Assert.Equal(accessibleObject.GetChild(2), listView.Items[0].SubItems[3].AccessibilityObject);

                Assert.Null(accessibleObject.GetChild(3));
            });
        }

        [StaFact]
        public void ListView_Tile_GetChild_ReturnsNull_WithoutSubItems()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 4, subItemsCount: 0, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

                Assert.Null(accessibleObject.GetChild(-1));

                Assert.Null(accessibleObject.GetChild(0));

                Assert.Null(accessibleObject.GetChild(1));
            });
        }

        [StaFact]
        public void ListView_Tile_GetChild_ReturnsExpected_ColumnsMoreThanSubItems()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 4, subItemsCount: 1, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

                Assert.Null(accessibleObject.GetChild(-1));

                Assert.Equal(accessibleObject.GetChild(0), listView.Items[0].SubItems[1].AccessibilityObject);

                Assert.Null(accessibleObject.GetChild(1));
            });
        }

        [StaFact]
        public void ListView_Tile_GetChild_ReturnsExpected_SubItemsMoreThanColumns()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 2, subItemsCount: 5, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

                Assert.Null(accessibleObject.GetChild(-1));

                Assert.Equal(accessibleObject.GetChild(0), listView.Items[0].SubItems[1].AccessibilityObject);

                Assert.Null(accessibleObject.GetChild(1));
            });
        }

        [StaFact]
        public void ListView_Tile_GetChild_ReturnsNull_For_Single_Column()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 1, subItemsCount: 2, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

                Assert.Null(accessibleObject.GetChild(-1));

                Assert.Null(accessibleObject.GetChild(0));

                Assert.Null(accessibleObject.GetChild(1));
            });
        }

        [StaFact]
        public void ListView_Tile_GetChild_ReturnsNull_For_SmallSize()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 2, subItemsCount: 1, tileSize: new Size(10, 10));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

                Assert.Null(accessibleObject.GetChild(-1));

                Assert.Null(accessibleObject.GetChild(0));

                Assert.Null(accessibleObject.GetChild(1));
            });
        }

        [StaFact]
        public void ListView_Tile_GetChild_ReturnsExpected_For_BigSize()
        {
            RunTest(listView =>
            {
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
            });
        }

        [StaFact]
        public void ListView_Tile_GetChildIndex_ReturnsExpected()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 4, subItemsCount: 3, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

                Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[0].AccessibilityObject));

                Assert.Equal(1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[1].AccessibilityObject));

                Assert.Equal(2, accessibleObject.GetChildIndex(listView.Items[0].SubItems[2].AccessibilityObject));

                Assert.Equal(3, accessibleObject.GetChildIndex(listView.Items[0].SubItems[3].AccessibilityObject));
            });
        }

        [StaFact]
        public void ListView_Tile_GetChildIndex_ReturnsExpected_ColumnsMoreThanSubItems()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 4, subItemsCount: 1, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

                Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[0].AccessibilityObject));

                Assert.Equal(1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[1].AccessibilityObject));
            });
        }

        [StaFact]
        public void ListView_Tile_GetChildIndex_ReturnsExpected_SubItemsMoreThanColumns()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 2, subItemsCount: 5, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

                Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[0].AccessibilityObject));

                Assert.Equal(1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[1].AccessibilityObject));

                Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[2].AccessibilityObject));

                Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[3].AccessibilityObject));

                Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[4].AccessibilityObject));
            });
        }

        [StaFact]
        public void ListView_Tile_GetChildIndex_ReturnsMinusOne_For_Single_Column()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 1, subItemsCount: 2, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

                Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[0].AccessibilityObject));

                Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[1].AccessibilityObject));

                Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[2].AccessibilityObject));
            });
        }

        [StaFact]
        public void ListView_Tile_GetChildIndex_ReturnsMinusOne_For_SmallSize()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 2, subItemsCount: 1, tileSize: new Size(10, 10));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

                Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[0].AccessibilityObject));

                Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[1].AccessibilityObject));
            });
        }

        [StaFact]
        public void ListView_Tile_GetChildIndex_ReturnsExpected_For_BigSize()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 10, subItemsCount: 10, tileSize: new Size(100, 100));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

                Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[0].AccessibilityObject));

                for (int i = 1; i <= 5; i++)
                {
                    Assert.Equal(i, accessibleObject.GetChildIndex(listView.Items[0].SubItems[i].AccessibilityObject));
                }

                for (int i = 6; i <= 10; i++)
                {
                    Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[i].AccessibilityObject));
                }
            });
        }

        [StaFact]
        public void ListView_Tile_ColumnProperty_ReturnsMinusOne()
        {
            RunTest(listView =>
            {
                InitializeTileList(listView, columnCount: 4, subItemsCount: 3, tileSize: new Size(150, 150));

                Application.DoEvents();

                Assert.Equal(-1, listView.Items[0].SubItems[0].AccessibilityObject.Column);

                Assert.Equal(-1, listView.Items[0].SubItems[1].AccessibilityObject.Column);

                Assert.Equal(-1, listView.Items[0].SubItems[2].AccessibilityObject.Column);

                Assert.Equal(-1, listView.Items[0].SubItems[2].AccessibilityObject.Column);
            });
        }

        private void ExecuteDoubleClickOnItem(ListView listView, ListViewItem listViewItem)
        {
            Point previousPosition = new Point();
            BOOL setOldCursorPos = GetPhysicalCursorPos(ref previousPosition);

            try
            {
                Rectangle pt = listView.RectangleToScreen(listViewItem.Bounds);

                // We shouldn't move the cursor to the old position immediately after double-clicking,
                // because the ListView uses the cursor position to get data about the item that was double-clicked.
                MouseHelper.SendDoubleClick(pt.X, pt.Y);
                Application.DoEvents();
            }
            finally
            {
                if (setOldCursorPos.IsTrue())
                {
                    // Move cursor to old position
                    MouseHelper.ChangeMousePosition(previousPosition.X, previousPosition.Y);
                    Application.DoEvents();
                }
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

            ListViewItem listViewItem = new ListViewItem("Test");

            for (int i = 0; i < subItemsCount; i++)
            {
                listViewItem.SubItems.Add(new ListViewItem.ListViewSubItem() { Text = $"Test SubItem{i}" });
            }

            listView.Items.Add(listViewItem);
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

        private void RunTest(Action<ListView> runTest)
        {
            UITest.RunControl(
                createControl: form =>
                {
                    ListView listView = new()
                    {
                        Parent = form,
                        Size = new System.Drawing.Size(439, 103),
                    };
                    return listView;
                },
                runTestAsync: async listView =>
                {
                    // Wait for pending operations so the Control is loaded completely before testing it
                    await AsyncTestHelper.JoinPendingOperationsAsync(AsyncTestHelper.UnexpectedTimeout);

                    runTest(listView);
                });
        }
    }
}
