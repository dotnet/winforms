// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.IntegrationTests.Common;
using ReflectTools;
using WFCTestLib.Log;
using static Interop;
using static Interop.User32;
using static Interop.UiaCore;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiListViewTests : ReflectBase
    {
        private readonly ListView _listView;

        public MauiListViewTests(string[] args) : base(args)
        {
            this.BringToForeground();
            _listView = new ListView { Size = new System.Drawing.Size(439, 103) };
            Controls.Add(_listView);
        }

        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiListViewTests(args));
        }

        [Scenario(true)]
        public ScenarioResult Click_On_Second_Column_Does_Not_Alter_Checkboxes(TParams p)
        {
            InitializeItems(View.Details, virtualModeEnabled: false, checkBoxesEnabled: true);

            foreach (ListViewItem item in _listView.Items)
            {
                if (item.StateImageIndex != 0)
                    return new ScenarioResult(false, "Precondition failed: all checkboxes must be unmarked");
            }

            foreach (ListViewItem item in _listView.Items)
            {
                if (item.Selected)
                    return new ScenarioResult(false, "Precondition failed: all items must be unselected");
            }

            KeyboardHelper.SendKey(Keys.ShiftKey, true);

            var pt = MouseHelper.GetCenter(_listView.RectangleToScreen(_listView.Items[0].SubItems[1].Bounds));
            MouseHelper.SendClick(pt.X, pt.Y);

            pt = MouseHelper.GetCenter(_listView.RectangleToScreen(_listView.Items[2].SubItems[1].Bounds));
            MouseHelper.SendClick(pt.X, pt.Y);

            KeyboardHelper.SendKey(Keys.ShiftKey, false);
            Application.DoEvents();

            foreach (ListViewItem item in _listView.Items)
            {
                if (item.StateImageIndex != 0)
                    return new ScenarioResult(false, "All checkboxes must be unmarked");
            }

            foreach (ListViewItem item in _listView.Items)
            {
                if (!item.Selected)
                    return new ScenarioResult(false, "All items must be selected");
            }

            return new ScenarioResult(true);
        }

        [Scenario(true)]
        public ScenarioResult ListView_DoubleClick_Updates_Checkbox_State(TParams p)
        {
            InitializeItems(View.Details, virtualModeEnabled: false, checkBoxesEnabled: true);

            bool checkBoxState = _listView.Items[0].Checked;
            ExecuteDoubleClickOnItem(_listView.Items[0]);

            return new ScenarioResult(_listView.Items[0].Checked != checkBoxState, "Double click doesn't update Checkbox state");
        }

        [Scenario(true)]
        public ScenarioResult ListView_CheckBoxDisabled_DoubleClick_DoesNotUpdate_Checkbox_State(TParams p)
        {
            InitializeItems(View.Details, virtualModeEnabled: false, checkBoxesEnabled: false);

            bool checkBoxState = _listView.Items[0].Checked;
            ExecuteDoubleClickOnItem(_listView.Items[0]);

            return new ScenarioResult(_listView.Items[0].Checked == checkBoxState,
                "Double click doesn't update Checkbox state");
        }

        [Scenario(true)]
        public ScenarioResult ListView_VirtualMode_DoubleClick_DoesNotUpdate_Checkbox_State(TParams p)
        {
            InitializeItems(View.Details, virtualModeEnabled: true, checkBoxesEnabled: true);

            bool checkBoxState = _listView.Items[0].Checked;
            ExecuteDoubleClickOnItem(_listView.Items[0]);

            return new ScenarioResult(_listView.Items[0].Checked == checkBoxState,
                "Double click updates Checkbox state when ListView is in virtual mode");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_FragmentNavigate_FirstChild_ReturnsSubItem(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 2, subItemsCount: 2, tileSize: new Size(150, 150));

            Application.DoEvents();

            AccessibleObject accessibleObject = _listView.Items[0].AccessibilityObject;
            IRawElementProviderFragment actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.FirstChild);
            AccessibleObject expectedAccessibleObject = _listView.Items[0].SubItems[1].AccessibilityObject;

            return new ScenarioResult(actualAccessibleObject == expectedAccessibleObject,
                "FragmentNavigate-FirstChild returns incorrect accessible object");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_FragmentNavigate_FirstChild_ReturnsNull_WithoutSubItems(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 1, subItemsCount: 0, tileSize: new Size(150, 150));

            Application.DoEvents();

            AccessibleObject accessibleObject = _listView.Items[0].AccessibilityObject;
            IRawElementProviderFragment actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.FirstChild);

            return new ScenarioResult(actualAccessibleObject is null,
                "FragmentNavigate-FirstChild doesn't return null for ListViewItem without ListViewSubItem");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_FragmentNavigate_FirstChild_ReturnsNull_For_Single_Column(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 1, subItemsCount: 2, tileSize: new Size(150, 150));

            Application.DoEvents();

            AccessibleObject accessibleObject = _listView.Items[0].AccessibilityObject;
            IRawElementProviderFragment actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.FirstChild);

            return new ScenarioResult(actualAccessibleObject is null,
                "FragmentNavigate-FirstChild doesn't return null for ListViewItem without column");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_FragmentNavigate_FirstChild_ReturnsNull_For_SmallSize(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 2, subItemsCount: 1, tileSize: new Size(10, 10));

            Application.DoEvents();

            AccessibleObject accessibleObject = _listView.Items[0].AccessibilityObject;
            IRawElementProviderFragment actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.FirstChild);

            return new ScenarioResult(actualAccessibleObject is null,
                "FragmentNavigate-FirstChild doesn't return null for ListViewItem with small size");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_FragmentNavigate_LastChild_ReturnsSubItem(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 4, subItemsCount: 3, tileSize: new Size(150, 150));

            Application.DoEvents();

            AccessibleObject accessibleObject = _listView.Items[0].AccessibilityObject;
            IRawElementProviderFragment actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.LastChild);
            AccessibleObject expectedAccessibleObject = _listView.Items[0].SubItems[3].AccessibilityObject;

            return new ScenarioResult(actualAccessibleObject == expectedAccessibleObject,
                "FragmentNavigate-LastChild returns incorrect accessible object");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_FragmentNavigate_LastChild_ReturnsNull_WithoutSubItems(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 4, subItemsCount: 0, tileSize: new Size(150, 150));

            Application.DoEvents();

            AccessibleObject accessibleObject = _listView.Items[0].AccessibilityObject;
            IRawElementProviderFragment actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.LastChild);

            return new ScenarioResult(actualAccessibleObject is null,
                "FragmentNavigate-LastChild doesn't return null for ListViewItem without ListViewSubItem");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_FragmentNavigate_LastChild_ReturnsSubItem_ColumnsMoreThanSubItems(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);

            InitializeTileList(columnCount: 4, subItemsCount: 1, tileSize: new Size(150, 150));

            Application.DoEvents();

            AccessibleObject accessibleObject = _listView.Items[0].AccessibilityObject;
            IRawElementProviderFragment actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.LastChild);
            AccessibleObject expectedAccessibleObject = _listView.Items[0].SubItems[1].AccessibilityObject;

            return new ScenarioResult(actualAccessibleObject == expectedAccessibleObject,
                "FragmentNavigate-LastChild returns incorrect accessible object when the number of columns is more than subitems");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_FragmentNavigate_LastChild_ReturnsSubItem_SubItemsMoreThanColumns(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);

            InitializeTileList(columnCount: 2, subItemsCount: 5, tileSize: new Size(150, 150));

            Application.DoEvents();

            AccessibleObject accessibleObject = _listView.Items[0].AccessibilityObject;
            IRawElementProviderFragment actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.LastChild);
            AccessibleObject expectedAccessibleObject = _listView.Items[0].SubItems[1].AccessibilityObject;

            return new ScenarioResult(actualAccessibleObject == expectedAccessibleObject,
                "FragmentNavigate-LastChild returns incorrect accessible object when the number of subitems is more than columns");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_FragmentNavigate_LastChild_ReturnsNull_For_Single_Column(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 1, subItemsCount: 2, tileSize: new Size(150, 150));

            Application.DoEvents();

            AccessibleObject accessibleObject = _listView.Items[0].AccessibilityObject;
            IRawElementProviderFragment actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.FirstChild);

            return new ScenarioResult(actualAccessibleObject is null,
                "FragmentNavigate-LastChild doesn't return null for ListViewItem without column");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_FragmentNavigate_LastChild_ReturnsNull_For_SmallSize(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 2, subItemsCount: 1, tileSize: new Size(10, 10));

            Application.DoEvents();

            AccessibleObject accessibleObject = _listView.Items[0].AccessibilityObject;
            IRawElementProviderFragment actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.FirstChild);

            return new ScenarioResult(actualAccessibleObject is null,
                "FragmentNavigate-LastChild doesn't return null for ListViewItem with small size");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_FragmentNavigate_LastChild_ReturnsLastVisibleSubItems(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 10, subItemsCount: 10, tileSize: new Size(100, 100));

            Application.DoEvents();

            AccessibleObject accessibleObject = _listView.Items[0].AccessibilityObject;
            IRawElementProviderFragment actualAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.LastChild);
            AccessibleObject expectedAccessibleObject = _listView.Items[0].SubItems[5].AccessibilityObject;

            return new ScenarioResult(actualAccessibleObject == expectedAccessibleObject,
                "FragmentNavigate-LastChild returns incorrect accessible object when the number of subitems is more than columns");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_SubItem_FragmentNavigate_NextSibling_ReturnsExpected(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 4, subItemsCount: 4, tileSize: new Size(100, 100));

            Application.DoEvents();

            AccessibleObject accessibleObject1 = _listView.Items[0].SubItems[1].AccessibilityObject;
            AccessibleObject accessibleObject2 = (AccessibleObject)accessibleObject1.FragmentNavigate(NavigateDirection.NextSibling);
            AccessibleObject accessibleObject3 = (AccessibleObject)accessibleObject2.FragmentNavigate(NavigateDirection.NextSibling);
            AccessibleObject accessibleObject4 = (AccessibleObject)accessibleObject3.FragmentNavigate(NavigateDirection.NextSibling);

            if (accessibleObject2 != _listView.Items[0].SubItems[2].AccessibilityObject)
            {
                return new ScenarioResult(false, "FragmentNavigate-NextSibling returns incorrect accessible object for first subitem");
            }

            if (accessibleObject3 != _listView.Items[0].SubItems[3].AccessibilityObject)
            {
                return new ScenarioResult(false, "FragmentNavigate-NextSibling returns incorrect accessible object for second subitem");
            }

            return new ScenarioResult(accessibleObject4 is null,
                "FragmentNavigate-NextSibling returns incorrect accessible object for third subitem");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_SubItem_FragmentNavigate_Sibling_ReturnsNull_For_Single_SubItem(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 4, subItemsCount: 1, tileSize: new Size(100, 100));

            Application.DoEvents();

            AccessibleObject accessibleObject = _listView.Items[0].SubItems[1].AccessibilityObject;
            IRawElementProviderFragment nextAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.NextSibling);
            IRawElementProviderFragment previousAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.PreviousSibling);

            if (nextAccessibleObject is not null)
            {
                return new ScenarioResult(false, "FragmentNavigate-NextSibling doesn't return null for single subitem");
            }

            return new ScenarioResult(previousAccessibleObject is null,
                "FragmentNavigate-PreviousSibling doesn't return null for single subitem");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_SubItem_FragmentNavigate_Sibling_ReturnsNull_For_Single_Column(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 2, subItemsCount: 3, tileSize: new Size(100, 100));

            Application.DoEvents();

            AccessibleObject accessibleObject = _listView.Items[0].SubItems[1].AccessibilityObject;
            IRawElementProviderFragment nextAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.NextSibling);
            IRawElementProviderFragment previousAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.PreviousSibling);

            if (nextAccessibleObject is not null)
            {
                return new ScenarioResult(false, "FragmentNavigate-NextSibling doesn't return null for single column");
            }

            return new ScenarioResult(previousAccessibleObject is null,
                "FragmentNavigate-NextSibling doesn't return null for single column");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_SubItem_FragmentNavigate_Sibling_ReturnsNull_For_SmallSize(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 5, subItemsCount: 5, tileSize: new Size(50, 40));

            Application.DoEvents();

            AccessibleObject accessibleObject = _listView.Items[0].SubItems[1].AccessibilityObject;
            IRawElementProviderFragment nextAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.NextSibling);
            IRawElementProviderFragment previousAccessibleObject = accessibleObject.FragmentNavigate(NavigateDirection.PreviousSibling);

            if (nextAccessibleObject is not null)
            {
                return new ScenarioResult(false, "FragmentNavigate-FirstChild doesn't return null for ListViewItem with small size");
            }

            return new ScenarioResult(previousAccessibleObject is null,
                "FragmentNavigate-FirstChild doesn't return null for ListViewItem with small size");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_SubItem_FragmentNavigate_Child_ReturnsNull(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 5, subItemsCount: 5, tileSize: new Size(50, 40));

            Application.DoEvents();

            AccessibleObject accessibleObject = (AccessibleObject)_listView.Items[0].AccessibilityObject.FragmentNavigate(NavigateDirection.FirstChild);

            if (accessibleObject.FragmentNavigate(NavigateDirection.FirstChild) is not null)
            {
                return new ScenarioResult(false, "FragmentNavigate-FirstChild doesn't return null for ListViewSubItem");
            }

            return new ScenarioResult(accessibleObject.FragmentNavigate(NavigateDirection.LastChild) is null, "FragmentNavigate-FirstChild doesn't return null for ListViewSubItem");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_SubItem_HitTest_ReturnExpected(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 3, subItemsCount: 2, tileSize: new Size(100, 100));

            Application.DoEvents();

            AccessibleObject expectedAccessibleItem1 = _listView.Items[0].SubItems[1].AccessibilityObject;
            AccessibleObject expectedAccessibleItem2 = _listView.Items[0].SubItems[2].AccessibilityObject;
            AccessibleObject actualAccessibleItem1 = HitTest(GetSubItemLocation(0, 1));
            AccessibleObject actualAccessibleItem2 = HitTest(GetSubItemLocation(0, 2));

            if (actualAccessibleItem1 != expectedAccessibleItem1)
            {
                return new ScenarioResult(false, "HitTest method returns incorrect accessible object for first ListViewSubItem");
            }

            return new ScenarioResult(actualAccessibleItem2 == expectedAccessibleItem2,
                "HitTest method returns incorrect accessible object for second ListViewSubItem");

            AccessibleObject HitTest(Point point) =>
            _listView.AccessibilityObject.HitTest(point.X, point.Y);

            Point GetSubItemLocation(int itemIndex, int subItemIndex) =>
                _listView.PointToScreen(_listView.GetSubItemRect(itemIndex, subItemIndex, ItemBoundsPortion.Label).Location);
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_GetChildCount_ReturnsExpected(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 4, subItemsCount: 3, tileSize: new Size(150, 150));

            Application.DoEvents();

            return new ScenarioResult(3 == _listView.Items[0].AccessibilityObject.GetChildCount(),
                "GetChildCount method returns incorrect value");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_GetChildCount_ReturnsZero_WithoutSubItems(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 4, subItemsCount: 0, tileSize: new Size(150, 150));

            Application.DoEvents();

            return new ScenarioResult(0 == _listView.Items[0].AccessibilityObject.GetChildCount(),
                "GetChildCount method returns incorrect value when ListViewItem has no ListViewSubItem");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_GetChildCount_ReturnsExpected_ColumnsMoreThanSubItems(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);

            InitializeTileList(columnCount: 4, subItemsCount: 1, tileSize: new Size(150, 150));

            Application.DoEvents();

            return new ScenarioResult(1 == _listView.Items[0].AccessibilityObject.GetChildCount(),
                "GetChildCount method returns incorrect value when the number of columns is more than subitems");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_GetChildCount_ReturnsExpected_SubItemsMoreThanColumns(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);

            InitializeTileList(columnCount: 2, subItemsCount: 5, tileSize: new Size(150, 150));

            Application.DoEvents();

            return new ScenarioResult(1 == _listView.Items[0].AccessibilityObject.GetChildCount(),
                "GetChildCount method returns incorrect value when the number of subitems is more than columns");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_GetChildCount_ReturnsZero_For_Single_Column(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 1, subItemsCount: 2, tileSize: new Size(150, 150));

            Application.DoEvents();

            return new ScenarioResult(0 == _listView.Items[0].AccessibilityObject.GetChildCount(),
                "GetChildCount method returns incorrect value for ListViewItem without column");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_GetChildCount_ReturnsZero_For_SmallSize(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 2, subItemsCount: 1, tileSize: new Size(10, 10));

            Application.DoEvents();

            return new ScenarioResult(0 == _listView.Items[0].AccessibilityObject.GetChildCount(),
                "GetChildCount method returns incorrect value for ListViewItem with small size");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_GetChildCount_ReturnsExpected_For_BigSize(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 10, subItemsCount: 10, tileSize: new Size(100, 100));

            Application.DoEvents();

            return new ScenarioResult(5 == _listView.Items[0].AccessibilityObject.GetChildCount(),
                "GetChildCount method returns incorrect value when the number of subitems is more than columns");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_GetChild_ReturnsExpected(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 4, subItemsCount: 3, tileSize: new Size(150, 150));

            Application.DoEvents();

            AccessibleObject accessibleObject = _listView.Items[0].AccessibilityObject;

            if (accessibleObject.GetChild(-1) is not null)
            {
                return new ScenarioResult(false, "GetChild(-1) does not return null for the non-existing item");
            }

            if (accessibleObject.GetChild(0) != _listView.Items[0].SubItems[1].AccessibilityObject)
            {
                return new ScenarioResult(false, "GetChild(0) does not return the accessibility object of the first item");
            }

            if (accessibleObject.GetChild(1) != _listView.Items[0].SubItems[2].AccessibilityObject)
            {
                return new ScenarioResult(false, "GetChild(1) does not return the accessibility object of the second item");
            }

            if (accessibleObject.GetChild(2) != _listView.Items[0].SubItems[3].AccessibilityObject)
            {
                return new ScenarioResult(false, "GetChild(2) does not return the accessibility object of the third item");
            }

            return new ScenarioResult(accessibleObject.GetChild(3) is null, "GetChild(3) does not return null for the non-existing item");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_GetChild_ReturnsNull_WithoutSubItems(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 4, subItemsCount: 0, tileSize: new Size(150, 150));

            Application.DoEvents();

            AccessibleObject accessibleObject = _listView.Items[0].AccessibilityObject;

            if (accessibleObject.GetChild(-1) is not null)
            {
                return new ScenarioResult(false, "GetChild(-1) does not return null for the non-existing item");
            }

            if (accessibleObject.GetChild(0) is not null)
            {
                return new ScenarioResult(false, "GetChild(0) does not return null for the non-existing item");
            }

            return new ScenarioResult(accessibleObject.GetChild(1) is null, "GetChild(1) does not return null for the non-existing item");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_GetChild_ReturnsExpected_ColumnsMoreThanSubItems(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);

            InitializeTileList(columnCount: 4, subItemsCount: 1, tileSize: new Size(150, 150));

            Application.DoEvents();

            AccessibleObject accessibleObject = _listView.Items[0].AccessibilityObject;

            if (accessibleObject.GetChild(-1) is not null)
            {
                return new ScenarioResult(false, "GetChild(-1) does not return null for the non-existing item");
            }

            if (accessibleObject.GetChild(0) != _listView.Items[0].SubItems[1].AccessibilityObject)
            {
                return new ScenarioResult(false, "GetChild(0) does not return the accessibility object of the first item");
            }

            return new ScenarioResult(accessibleObject.GetChild(1) is null, "GetChild(1) does not return null for the non-existing item");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_GetChild_ReturnsExpected_SubItemsMoreThanColumns(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);

            InitializeTileList(columnCount: 2, subItemsCount: 5, tileSize: new Size(150, 150));

            Application.DoEvents();

            AccessibleObject accessibleObject = _listView.Items[0].AccessibilityObject;

            if (accessibleObject.GetChild(-1) is not null)
            {
                return new ScenarioResult(false, "GetChild(-1) does not return null for the non-existing item");
            }

            if (accessibleObject.GetChild(0) != _listView.Items[0].SubItems[1].AccessibilityObject)
            {
                return new ScenarioResult(false, "GetChild(0) does not return the accessibility object of the first item");
            }

            return new ScenarioResult(accessibleObject.GetChild(1) is null, "GetChild(1) does not return null for the non-existing item");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_GetChild_ReturnsNull_For_Single_Column(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 1, subItemsCount: 2, tileSize: new Size(150, 150));

            Application.DoEvents();

            AccessibleObject accessibleObject = _listView.Items[0].AccessibilityObject;

            if (accessibleObject.GetChild(-1) is not null)
            {
                return new ScenarioResult(false, "GetChild(-1) does not return null for the non-existing item");
            }

            if (accessibleObject.GetChild(0) is not null)
            {
                return new ScenarioResult(false, "GetChild(0) does not return null for the non-existing item");
            }

            return new ScenarioResult(accessibleObject.GetChild(1) is null, "GetChild(1) does not return null for the non-existing item");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_GetChild_ReturnsNull_For_SmallSize(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 2, subItemsCount: 1, tileSize: new Size(10, 10));

            Application.DoEvents();

            AccessibleObject accessibleObject = _listView.Items[0].AccessibilityObject;

            if (accessibleObject.GetChild(-1) is not null)
            {
                return new ScenarioResult(false, "GetChild(-1) does not return null for the non-existing item");
            }

            if (accessibleObject.GetChild(0) is not null)
            {
                return new ScenarioResult(false, "GetChild(0) does not return null for the non-existing item");
            }

            return new ScenarioResult(accessibleObject.GetChild(1) is null, "GetChild(1) does not return null for the non-existing item");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_GetChild_ReturnsExpected_For_BigSize(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 10, subItemsCount: 10, tileSize: new Size(100, 100));

            Application.DoEvents();

            AccessibleObject accessibleObject = _listView.Items[0].AccessibilityObject;

            if (accessibleObject.GetChild(-1) is not null)
            {
                return new ScenarioResult(false, "GetChild(-1) does not return null for the non-existing item");
            }

            if (accessibleObject.GetChild(0) != _listView.Items[0].SubItems[1].AccessibilityObject)
            {
                return new ScenarioResult(false, "GetChild(0) does not return the accessibility object of the first item");
            }

            if (accessibleObject.GetChild(1) != _listView.Items[0].SubItems[2].AccessibilityObject)
            {
                return new ScenarioResult(false, "GetChild(1) does not return the accessibility object of the second item");
            }

            if (accessibleObject.GetChild(2) != _listView.Items[0].SubItems[3].AccessibilityObject)
            {
                return new ScenarioResult(false, "GetChild(2) does not return the accessibility object of the third item");
            }

            if (accessibleObject.GetChild(3) != _listView.Items[0].SubItems[4].AccessibilityObject)
            {
                return new ScenarioResult(false, "GetChild(3) does not return the accessibility object of the fourth item");
            }

            if (accessibleObject.GetChild(4) != _listView.Items[0].SubItems[5].AccessibilityObject)
            {
                return new ScenarioResult(false, "GetChild(4) does not return the accessibility object of the fifth item");
            }

            return new ScenarioResult(accessibleObject.GetChild(5) is null, "GetChild(5) does not return null for the non-existing item");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_GetChildIndex_ReturnsExpected(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 4, subItemsCount: 3, tileSize: new Size(150, 150));

            Application.DoEvents();

            AccessibleObject accessibleObject = _listView.Items[0].AccessibilityObject;

            if (accessibleObject.GetChildIndex(_listView.Items[0].SubItems[0].AccessibilityObject) != -1)
            {
                return new ScenarioResult(false, "GetChildIndex does not return -1 for the first SubItem");
            }

            if (accessibleObject.GetChildIndex(_listView.Items[0].SubItems[1].AccessibilityObject) != 1)
            {
                return new ScenarioResult(false, "GetChildIndex does not return 1 for the second SubItem");
            }

            if (accessibleObject.GetChildIndex(_listView.Items[0].SubItems[2].AccessibilityObject) != 2)
            {
                return new ScenarioResult(false, "GetChildIndex does not return 2 for the third SubItem");
            }

            return new ScenarioResult(accessibleObject.GetChildIndex(_listView.Items[0].SubItems[3].AccessibilityObject) == 3,
                "GetChildIndex does not return 3 for the fourth SubItem");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_GetChildIndex_ReturnsExpected_ColumnsMoreThanSubItems(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);

            InitializeTileList(columnCount: 4, subItemsCount: 1, tileSize: new Size(150, 150));

            Application.DoEvents();

            AccessibleObject accessibleObject = _listView.Items[0].AccessibilityObject;

            if (accessibleObject.GetChildIndex(_listView.Items[0].SubItems[0].AccessibilityObject) != -1)
            {
                return new ScenarioResult(false, "GetChildIndex does not return -1 for the first SubItem");
            }

            return new ScenarioResult(accessibleObject.GetChildIndex(_listView.Items[0].SubItems[1].AccessibilityObject) == 1,
                "GetChildIndex does not return 1 for the second SubItem");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_GetChildIndex_ReturnsExpected_SubItemsMoreThanColumns(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);

            InitializeTileList(columnCount: 2, subItemsCount: 5, tileSize: new Size(150, 150));

            Application.DoEvents();

            AccessibleObject accessibleObject = _listView.Items[0].AccessibilityObject;

            if (accessibleObject.GetChildIndex(_listView.Items[0].SubItems[0].AccessibilityObject) != -1)
            {
                return new ScenarioResult(false, "GetChildIndex does not return -1 for the first SubItem");
            }

            if (accessibleObject.GetChildIndex(_listView.Items[0].SubItems[1].AccessibilityObject) != 1)
            {
                return new ScenarioResult(false, "GetChildIndex does not return 1 for the second SubItem");
            }

            if (accessibleObject.GetChildIndex(_listView.Items[0].SubItems[2].AccessibilityObject) != -1)
            {
                return new ScenarioResult(false, "GetChildIndex does not return -1 for the third SubItem");
            }

            if (accessibleObject.GetChildIndex(_listView.Items[0].SubItems[3].AccessibilityObject) != -1)
            {
                return new ScenarioResult(false, "GetChildIndex does not return -1 for the fourth SubItem");
            }

            return new ScenarioResult(accessibleObject.GetChildIndex(_listView.Items[0].SubItems[4].AccessibilityObject) == -1,
                "GetChildIndex does not return -1 for the fifth SubItem");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_GetChildIndex_ReturnsMinusOne_For_Single_Column(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 1, subItemsCount: 2, tileSize: new Size(150, 150));

            Application.DoEvents();

            AccessibleObject accessibleObject = _listView.Items[0].AccessibilityObject;

            if (accessibleObject.GetChildIndex(_listView.Items[0].SubItems[0].AccessibilityObject) != -1)
            {
                return new ScenarioResult(false, "GetChildIndex does not return -1 for the first SubItem");
            }

            if (accessibleObject.GetChildIndex(_listView.Items[0].SubItems[1].AccessibilityObject) != -1)
            {
                return new ScenarioResult(false, "GetChildIndex does not return -1 for the second SubItem");
            }

            return new ScenarioResult(accessibleObject.GetChildIndex(_listView.Items[0].SubItems[2].AccessibilityObject) == -1,
                "GetChildIndex does not return -1 for the second SubItem");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_GetChildIndex_ReturnsMinusOne_For_SmallSize(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 2, subItemsCount: 1, tileSize: new Size(10, 10));

            Application.DoEvents();

            AccessibleObject accessibleObject = _listView.Items[0].AccessibilityObject;

            if (accessibleObject.GetChildIndex(_listView.Items[0].SubItems[0].AccessibilityObject) != -1)
            {
                return new ScenarioResult(false, "GetChildIndex does not return -1 for the first SubItem");
            }

            return new ScenarioResult(accessibleObject.GetChildIndex(_listView.Items[0].SubItems[1].AccessibilityObject) == -1,
                "GetChildIndex does not return -1 for the second SubItem");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_GetChildIndex_ReturnsExpected_For_BigSize(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 10, subItemsCount: 10, tileSize: new Size(100, 100));

            Application.DoEvents();

            AccessibleObject accessibleObject = _listView.Items[0].AccessibilityObject;

            if (accessibleObject.GetChildIndex(_listView.Items[0].SubItems[0].AccessibilityObject) != -1)
            {
                return new ScenarioResult(false, "GetChildIndex does not return -1 for the SubItem with the index 1");
            }

            if (accessibleObject.GetChildIndex(_listView.Items[0].SubItems[1].AccessibilityObject) != 1)
            {
                return new ScenarioResult(false, "GetChildIndex does not return 1 for the SubItem with the index 1");
            }

            if (accessibleObject.GetChildIndex(_listView.Items[0].SubItems[2].AccessibilityObject) != 2)
            {
                return new ScenarioResult(false, "GetChildIndex does not return 2 for the SubItem with the index 2");
            }

            if (accessibleObject.GetChildIndex(_listView.Items[0].SubItems[3].AccessibilityObject) != 3)
            {
                return new ScenarioResult(false, "GetChildIndex does not return 3 for the SubItem with the index 3");
            }

            if (accessibleObject.GetChildIndex(_listView.Items[0].SubItems[4].AccessibilityObject) != 4)
            {
                return new ScenarioResult(false, "GetChildIndex does not return 4 for the SubItem with the index 4");
            }

            if (accessibleObject.GetChildIndex(_listView.Items[0].SubItems[5].AccessibilityObject) != 5)
            {
                return new ScenarioResult(false, "GetChildIndex does not return 5 for the SubItem with the index 5");
            }

            if (accessibleObject.GetChildIndex(_listView.Items[0].SubItems[6].AccessibilityObject) != -1)
            {
                return new ScenarioResult(false, "GetChildIndex does not return -1 for the SubItem with the index 6");
            }

            if (accessibleObject.GetChildIndex(_listView.Items[0].SubItems[7].AccessibilityObject) != -1)
            {
                return new ScenarioResult(false, "GetChildIndex does not return -1 for the SubItem with the index 7");
            }

            if (accessibleObject.GetChildIndex(_listView.Items[0].SubItems[8].AccessibilityObject) != -1)
            {
                return new ScenarioResult(false, "GetChildIndex does not return -1 for the SubItem with the index 8");
            }

            if (accessibleObject.GetChildIndex(_listView.Items[0].SubItems[9].AccessibilityObject) != -1)
            {
                return new ScenarioResult(false, "GetChildIndex does not return -1 for the SubItem with the index 9");
            }

            return new ScenarioResult(accessibleObject.GetChildIndex(_listView.Items[0].SubItems[10].AccessibilityObject) == -1,
                "GetChildIndex does not return -1 for the SubItem with the index 10");
        }

        [Scenario(true)]
        public ScenarioResult ListView_Tile_ColumnProperty_ReturnsMinusOne(TParams p)
        {
            InitializeItems(View.Tile, virtualModeEnabled: false, checkBoxesEnabled: false);
            InitializeTileList(columnCount: 4, subItemsCount: 3, tileSize: new Size(150, 150));

            Application.DoEvents();

            if (_listView.Items[0].SubItems[0].AccessibilityObject.Column != -1)
            {
                return new ScenarioResult(false, "Column property does not return -1 value for the first SubItem");
            }

            if (_listView.Items[0].SubItems[1].AccessibilityObject.Column != -1)
            {
                return new ScenarioResult(false, "Column property does not return -1 value for the second SubItem");
            }

            if (_listView.Items[0].SubItems[2].AccessibilityObject.Column != -1)
            {
                return new ScenarioResult(false, "Column property does not return -1 value for the third SubItem");
            }

            return new ScenarioResult(_listView.Items[0].SubItems[2].AccessibilityObject.Column == -1,
                "Column property does not return -1 value for the fourth SubItem");
        }

        private void ExecuteDoubleClickOnItem(ListViewItem listViewItem)
        {
            Point previousPosition = new Point();
            BOOL setOldCursorPos = GetPhysicalCursorPos(ref previousPosition);

            try
            {
                Rectangle pt = _listView.RectangleToScreen(listViewItem.Bounds);

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

        private void InitializeTileList(int columnCount, int subItemsCount, Size tileSize)
        {
            _listView.VirtualListSize = 0;
            _listView.Columns.Clear();
            _listView.Items.Clear();
            _listView.VirtualMode = false;
            _listView.CheckBoxes = false;
            _listView.TileSize = tileSize;

            for (int i = 0; i < columnCount; i++)
            {
                _listView.Columns.Add(new ColumnHeader() { Text = $"ColumnHeader{i}" });
            }

            ListViewItem listViewItem = new ListViewItem("Test");

            for (int i = 0; i < subItemsCount; i++)
            {
                listViewItem.SubItems.Add(new ListViewItem.ListViewSubItem() { Text = $"Test SubItem{i}" });
            }

            _listView.Items.Add(listViewItem);
        }

        private void InitializeItems(View view, bool virtualModeEnabled, bool checkBoxesEnabled)
        {
            _listView.VirtualListSize = 0;
            _listView.Columns.Clear();
            _listView.VirtualMode = false;
            _listView.CheckBoxes = false;

            var columnHeader1 = new ColumnHeader { Text = "ColumnHeader1", Width = 140 };
            var columnHeader2 = new ColumnHeader { Text = "ColumnHeader2", Width = 140 };
            var columnHeader3 = new ColumnHeader { Text = "ColumnHeader3", Width = 140 };
            _listView.Columns.AddRange(new[] { columnHeader1, columnHeader2, columnHeader3 });

            var listViewItem1 = new ListViewItem(new[] { "row1", "row1Col2", "row1Col3" }, -1) { StateImageIndex = 0 };
            var listViewItem2 = new ListViewItem(new[] { "row2", "row2Col2", "row2Col3" }, -1) { StateImageIndex = 0 };
            var listViewItem3 = new ListViewItem(new[] { "row3", "row3Col2", "row3Col3" }, -1) { StateImageIndex = 0 };

            _listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    0 => listViewItem1,
                    1 => listViewItem2,
                    2 => listViewItem3,
                    _ => listViewItem1,
                };
            };

            _listView.Items.Clear();
            _listView.CheckBoxes = checkBoxesEnabled;
            _listView.FullRowSelect = true;
            _listView.View = view;
            _listView.VirtualMode = virtualModeEnabled;
            _listView.VirtualListSize = 3;

            if (!virtualModeEnabled)
            {
                _listView.Items.AddRange(new[] { listViewItem1, listViewItem2, listViewItem3 });
            }
        }
    }
}
