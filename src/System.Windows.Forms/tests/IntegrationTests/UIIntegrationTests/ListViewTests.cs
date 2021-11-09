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
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
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
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
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
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
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
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 4, subItemsCount: 3, tileSize: new Size(150, 150));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;
                IRawElementProviderFragment? actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.FirstChild);
                AccessibleObject expectedAccessibleObject = listView.Items[0].SubItems[3].AccessibilityObject;

                Assert.Equal(actualAccessibleObject, expectedAccessibleObject);
            });
        }

        [StaFact]
        public void ListView_Tile_FragmentNavigate_LastChild_ReturnsNull_WithoutSubItems()
        {
            RunTest(listView =>
            {
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);

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
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);

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
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
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
                InitializeItems(listView, View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
                InitializeTileList(listView, columnCount: 10, subItemsCount: 10, tileSize: new Size(100, 100));

                Application.DoEvents();

                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;
                IRawElementProviderFragment? actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.LastChild);
                AccessibleObject expectedAccessibleObject = listView.Items[0].SubItems[5].AccessibilityObject;

                Assert.Equal(actualAccessibleObject, expectedAccessibleObject);
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

        private void InitializeItems(ListView listView, View view, bool virtualModeEnabled, bool checkBoxesEnabled)
        {
            listView.VirtualListSize = 0;
            listView.Columns.Clear();
            listView.VirtualMode = false;
            listView.CheckBoxes = false;

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
