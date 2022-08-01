// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using WindowsInput.Native;
using Xunit;
using Xunit.Abstractions;
using static Interop.ComCtl32;
using static Interop;
using static Interop.UiaCore;

namespace System.Windows.Forms.UITests
{
    // NOTE: This class contains many tests which don't require user input. Although they arguably belong to the unit
    // tests project, these tests assert behaviours of ListView.View=View.Tile, which doesn't work correctly unless
    // we ran an app.

    public class ListViewTests : ControlTestBase
    {
        public ListViewTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        public static IEnumerable<object[]> Handle_GetWithGroups_TestData()
        {
            foreach (bool showGroups in new bool[] { true, false })
            {
                yield return new object[] { showGroups, null!, HorizontalAlignment.Left, null!, HorizontalAlignment.Right, string.Empty, string.Empty, LVGA.HEADER_LEFT, LVGA.HEADER_LEFT };
                yield return new object[] { showGroups, null!, HorizontalAlignment.Center, null!, HorizontalAlignment.Center, string.Empty, string.Empty, LVGA.HEADER_LEFT, LVGA.HEADER_CENTER };
                yield return new object[] { showGroups, null!, HorizontalAlignment.Right, null!, HorizontalAlignment.Left, string.Empty, string.Empty, LVGA.HEADER_LEFT, LVGA.HEADER_RIGHT };

                yield return new object[] { showGroups, string.Empty, HorizontalAlignment.Left, string.Empty, HorizontalAlignment.Right, string.Empty, string.Empty, LVGA.HEADER_LEFT, LVGA.HEADER_LEFT };
                yield return new object[] { showGroups, string.Empty, HorizontalAlignment.Center, string.Empty, HorizontalAlignment.Center, string.Empty, string.Empty, LVGA.HEADER_LEFT, LVGA.HEADER_CENTER };
                yield return new object[] { showGroups, string.Empty, HorizontalAlignment.Right, string.Empty, HorizontalAlignment.Left, string.Empty, string.Empty, LVGA.HEADER_LEFT, LVGA.HEADER_RIGHT };

                yield return new object[] { showGroups, "header", HorizontalAlignment.Left, "footer", HorizontalAlignment.Right, "header", "footer", LVGA.HEADER_LEFT, LVGA.HEADER_LEFT | LVGA.FOOTER_RIGHT };
                yield return new object[] { showGroups, "header", HorizontalAlignment.Center, "footer", HorizontalAlignment.Center, "header", "footer", LVGA.HEADER_LEFT, LVGA.HEADER_CENTER | LVGA.FOOTER_CENTER };
                yield return new object[] { showGroups, "header", HorizontalAlignment.Right, "footer", HorizontalAlignment.Left, "header", "footer", LVGA.HEADER_LEFT, LVGA.HEADER_RIGHT | LVGA.FOOTER_LEFT };

                yield return new object[] { showGroups, "he\0der", HorizontalAlignment.Left, "fo\0oter", HorizontalAlignment.Right, "he", "fo", LVGA.HEADER_LEFT, LVGA.HEADER_LEFT | LVGA.FOOTER_RIGHT };
                yield return new object[] { showGroups, "he\0der", HorizontalAlignment.Center, "fo\0oter", HorizontalAlignment.Center, "he", "fo", LVGA.HEADER_LEFT, LVGA.HEADER_CENTER | LVGA.FOOTER_CENTER };
                yield return new object[] { showGroups, "he\0der", HorizontalAlignment.Right, "fo\0oter", HorizontalAlignment.Left, "he", "fo", LVGA.HEADER_LEFT, LVGA.HEADER_RIGHT | LVGA.FOOTER_LEFT };
            }
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

        [WinFormsFact]
        public async Task ListView_Group_NavigateKeyboard_SucceedsAsync()
        {
            await RunTestAsync(async (form, listView) =>
            {
                var group = new ListViewGroup($"Group 1", HorizontalAlignment.Left) { CollapsedState = ListViewGroupCollapsedState.Expanded };
                var item1 = new ListViewItem("g1-1") { Group = group };
                var item2 = new ListViewItem("g1-2") { Group = group };
                var item3 = new ListViewItem("g1-3") { Group = group };

                listView.Groups.Add(group);
                listView.Items.AddRange(new[] { item1, item2, item3 });
                listView.Focus();

                bool collapsedStateChangedFired = false;
                listView.GroupCollapsedStateChanged += (sender, e) => collapsedStateChangedFired = true;

                item1.Selected = true;

                await InputSimulator.SendAsync(form, inputSimulator => inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RIGHT));
                Assert.False(item1.Selected);
                Assert.True(item2.Selected);
                Assert.False(item3.Selected);
                Assert.False(collapsedStateChangedFired);

                await InputSimulator.SendAsync(form, inputSimulator => inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RIGHT));
                Assert.False(item1.Selected);
                Assert.False(item2.Selected);
                Assert.True(item3.Selected);
                Assert.False(collapsedStateChangedFired);

                await InputSimulator.SendAsync(form, inputSimulator => inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RIGHT));
                Assert.False(item1.Selected);
                Assert.False(item2.Selected);
                Assert.True(item3.Selected);
                Assert.False(collapsedStateChangedFired);

                await InputSimulator.SendAsync(form, inputSimulator => inputSimulator.Keyboard.KeyPress(VirtualKeyCode.LEFT).KeyPress(VirtualKeyCode.LEFT));
                Assert.True(item1.Selected);
                Assert.False(item2.Selected);
                Assert.False(item3.Selected);
                Assert.False(collapsedStateChangedFired);

                await InputSimulator.SendAsync(form, inputSimulator => inputSimulator.Keyboard.KeyPress(VirtualKeyCode.LEFT));
                Assert.True(item1.Selected);
                Assert.False(item2.Selected);
                Assert.False(item3.Selected);
                Assert.False(collapsedStateChangedFired);

                // Selects header, which selects all items in group
                await InputSimulator.SendAsync(form, inputSimulator => inputSimulator.Keyboard.KeyPress(VirtualKeyCode.UP));
                Assert.True(item2.Selected);
                Assert.True(item2.Selected);
                Assert.True(item2.Selected);
                Assert.False(collapsedStateChangedFired);

                // Collapse group
                await InputSimulator.SendAsync(form, inputSimulator => inputSimulator.Keyboard.KeyPress(VirtualKeyCode.LEFT).KeyPress(VirtualKeyCode.UP).KeyPress(VirtualKeyCode.LEFT));
                Assert.Equal(ListViewGroupCollapsedState.Collapsed, group.CollapsedState);
                Assert.True(collapsedStateChangedFired);

                // Expand group
                collapsedStateChangedFired = false;
                await InputSimulator.SendAsync(form, inputSimulator => inputSimulator.Keyboard.KeyPress(VirtualKeyCode.UP).KeyPress(VirtualKeyCode.RIGHT));
                Assert.True(collapsedStateChangedFired);
                Assert.Equal(ListViewGroupCollapsedState.Expanded, group.CollapsedState);
            });
        }

        [WinFormsFact]
        public unsafe void ListView_Handle_GetWithGroups_Success()
        {
            foreach (object[] data in Handle_GetWithGroups_TestData())
            {
                bool showGroups = (bool)data[0];
                string header = (string)data[1];
                HorizontalAlignment headerAlignment = (HorizontalAlignment)data[2];
                string footer = (string)data[3];
                HorizontalAlignment footerAlignment = (HorizontalAlignment)data[4];
                string expectedHeaderText = (string)data[5];
                string expectedFooterText = (string)data[6];
                LVGA expectedAlignGroup1 = (LVGA)data[7];
                LVGA exptectedAlignGroup2 = (LVGA)data[8];
                string? headerText = header is not null && header.Contains('\0') ? header[..header.IndexOf('\0')] : header;
                string? footerText = footer is not null && footer.Contains('\0') ? footer[..footer.IndexOf('\0')] : footer;
                int headerSize = !string.IsNullOrEmpty(headerText) ? headerText.Length + 1 : 0;
                int footerSize = !string.IsNullOrEmpty(footerText) ? footerText.Length + 1 : 0;
                char* headerBuffer = stackalloc char[headerSize];
                char* footerBuffer = stackalloc char[footerSize];

                Application.EnableVisualStyles();

                using var listView = new ListView
                {
                    ShowGroups = showGroups
                };
                var group1 = new ListViewGroup();
                var group2 = new ListViewGroup
                {
                    Header = header,
                    HeaderAlignment = headerAlignment,
                    Footer = footer,
                    FooterAlignment = footerAlignment
                };
                listView.Groups.Add(group1);
                listView.Groups.Add(group2);

                Assert.Equal(2, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETGROUPCOUNT));

                var lvgroup1 = new LVGROUPW
                {
                    cbSize = (uint)sizeof(LVGROUPW),
                    mask = LVGF.HEADER | LVGF.FOOTER | LVGF.GROUPID | LVGF.ALIGN,
                    pszHeader = null,
                    cchHeader = 0,
                    pszFooter = null,
                    cchFooter = 0,
                };
                Assert.Equal(1, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETGROUPINFOBYINDEX, 0, ref lvgroup1));
                Assert.Equal("ListViewGroup", new string(lvgroup1.pszHeader));
                Assert.Empty(new string(lvgroup1.pszFooter));
                Assert.True(lvgroup1.iGroupId >= 0);
                Assert.Equal(expectedAlignGroup1, lvgroup1.uAlign);

                var lvgroup2 = new LVGROUPW
                {
                    cbSize = (uint)sizeof(LVGROUPW),
                    mask = LVGF.HEADER | LVGF.FOOTER | LVGF.GROUPID | LVGF.ALIGN,
                    pszHeader = headerBuffer,
                    cchHeader = headerSize,
                    pszFooter = footerBuffer,
                    cchFooter = footerSize,
                };
                Assert.Equal(1, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETGROUPINFOBYINDEX, 1, ref lvgroup2));
                Assert.Equal(expectedHeaderText, new string(lvgroup2.pszHeader));
                Assert.Equal(expectedFooterText, new string(lvgroup2.pszFooter));
                Assert.True(lvgroup2.iGroupId > 0);
                Assert.Equal(exptectedAlignGroup2, lvgroup2.uAlign);
                Assert.True(lvgroup2.iGroupId > lvgroup1.iGroupId);
            }
        }

        [WinFormsTheory]
        [InlineData(2, 2, 150, 150, 0, 1, (int)NavigateDirection.FirstChild)]
        [InlineData(4, 3, 150, 150, 0, 3, (int)NavigateDirection.LastChild)]
        [InlineData(4, 1, 150, 150, 0, 1, (int)NavigateDirection.LastChild)]
        [InlineData(2, 5, 150, 150, 0, 1, (int)NavigateDirection.LastChild)]
        [InlineData(10, 10, 100, 100, 0, 5, (int)NavigateDirection.LastChild)]
        public async Task ListView_Tile_FragmentNavigate_WorksExpectedAsync(int columnCount, int subItemsCount, int width, int height, int itemIndex, int subItemIndex, int direction)
        {
            await RunTestAsync((form, listView) =>
            {
                InitializeTileList(listView, columnCount, subItemsCount, tileSize: new Size(width, height));

                Application.DoEvents();
                AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;
                IRawElementProviderFragment? actualAccessibleObject = accessibleObject.FragmentNavigate((NavigateDirection)direction);
                AccessibleObject? expectedAccessibleObject = listView.Items[itemIndex].SubItems[subItemIndex].AccessibilityObject;

                Assert.Equal(expectedAccessibleObject, actualAccessibleObject);

                return Task.CompletedTask;
            });
        }

        [WinFormsTheory]
        [InlineData(1, 0, 150, 150, (int)NavigateDirection.FirstChild)]
        [InlineData(1, 2, 150, 150, (int)NavigateDirection.FirstChild)]
        [InlineData(2, 1, 10, 10, (int)NavigateDirection.FirstChild)]
        [InlineData(4, 0, 150, 150, (int)NavigateDirection.LastChild)]
        [InlineData(1, 2, 150, 150, (int)NavigateDirection.LastChild)]
        [InlineData(2, 1, 10, 10, (int)NavigateDirection.LastChild)]
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
                IRawElementProviderFragment? nextAccessibleObject = accessibleObject?.FragmentNavigate(NavigateDirection.NextSibling);
                IRawElementProviderFragment? previousAccessibleObject = accessibleObject?.FragmentNavigate(NavigateDirection.PreviousSibling);

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
                AccessibleObject? accessibleObject2 = (AccessibleObject?)accessibleObject1?.FragmentNavigate(NavigateDirection.NextSibling);
                AccessibleObject? accessibleObject3 = (AccessibleObject?)accessibleObject2?.FragmentNavigate(NavigateDirection.NextSibling);
                AccessibleObject? accessibleObject4 = (AccessibleObject?)accessibleObject3?.FragmentNavigate(NavigateDirection.NextSibling);

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
                IRawElementProviderFragment? nextAccessibleObject = accessibleObject?.FragmentNavigate(NavigateDirection.NextSibling);
                IRawElementProviderFragment? previousAccessibleObject = accessibleObject?.FragmentNavigate(NavigateDirection.PreviousSibling);

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
                AccessibleObject accessibleObject = (AccessibleObject)listView.Items[0].AccessibilityObject.FragmentNavigate(NavigateDirection.FirstChild)!;

                Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.FirstChild));
                Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.LastChild));

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
                   inputSimulator => inputSimulator.Keyboard.KeyDown(VirtualKeyCode.SHIFT)
                                                    .Mouse.LeftButtonClick());
                listViewCenter = GetCenter(listView.RectangleToScreen(listView.Items[2].SubItems[1].Bounds));
                await MoveMouseAsync(form, listViewCenter);
                await InputSimulator.SendAsync(
                   form,
                   inputSimulator => inputSimulator.Mouse.LeftButtonClick()
                                                   .Keyboard.KeyUp(VirtualKeyCode.SHIFT));

                foreach (ListViewItem item in listView.Items)
                {
                    Assert.Equal(0, item.StateImageIndex);
                    Assert.True(item.Selected);
                }
            });
        }

        [WinFormsTheory]
        [InlineData(Keys.Down, 2)]
        [InlineData(Keys.Up, 1)]
        public unsafe void ListView_WmReflectNotify_LVN_KEYDOWN_WithGroups_and_SelectedItems_FocusedGroupIsExpected(Keys key, int expectedGroupIndex)
        {
            Application.EnableVisualStyles();

            using var control = new ListView();
            ListViewGroup group1 = new ListViewGroup("Test group1");
            ListViewGroup group2 = new ListViewGroup("Test group2");
            ListViewGroup group3 = new ListViewGroup("Test group3");
            ListViewItem item1 = new ListViewItem(group1);
            item1.Text = "First";
            ListViewItem item2 = new ListViewItem(group2);
            item2.Text = "Second";
            ListViewItem item3 = new ListViewItem(group3);
            item3.Text = "Third";
            control.Items.Add(item1);
            control.Items.Add(item2);
            control.Items.Add(item3);
            control.Groups.Add(group1);
            control.Groups.Add(group2);
            control.Groups.Add(group3);
            control.VirtualMode = false;
            control.CreateControl();

            item2.Selected = true;

            // https://docs.microsoft.com/windows/win32/inputdev/wm-keydown
            // The MSDN page tells us what bits of lParam to use for each of the parameters.
            // All we need to do is some bit shifting to assemble lParam
            // lParam = repeatCount | (scanCode << 16)
            uint keyCode = (uint)key;
            uint lParam = 0x00000001 | keyCode << 16;

            User32.SendMessageW(control, User32.WM.KEYDOWN, (nint)keyCode, (nint)lParam);
            Assert.True(control.GroupsEnabled);
            Assert.True(control.Items.Count > 0);
            Assert.Equal(control.Groups[expectedGroupIndex], control.FocusedGroup);
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
}
