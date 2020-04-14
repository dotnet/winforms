// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using Accessibility;
using static Interop;
using static Interop.UiaCore;

namespace System.Windows.Forms
{
    public partial class ListView
    {
        [ComVisible(true)]
        internal class ListViewAccessibleObject : ControlAccessibleObject
        {
            private readonly ListView owner;
            private readonly Dictionary<ListViewItem, ListViewItemAccessibleObject> _itemAccessibleObjects;
            private readonly IAccessible _systemIAccessible;

            internal ListViewAccessibleObject(ListView owner) : base(owner)
            {
                this.owner = owner;
                _itemAccessibleObjects = new Dictionary<ListViewItem, ListViewItemAccessibleObject>();
                _systemIAccessible = GetSystemIAccessibleInternal();
            }

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => this;

            private ListView OwningListView => Owner as ListView;

            internal override bool IsPatternSupported(UIA patternId)
            {
                if (patternId == UIA.SelectionPatternId ||
                    patternId == UIA.MultipleViewPatternId ||
                    patternId == UIA.LegacyIAccessiblePatternId ||
                    (patternId == UIA.GridPatternId && OwningListView.View == View.Details) ||
                    (patternId == UIA.TablePatternId && OwningListView.View == View.Details))
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                int childCount = OwningListView.Items.Count;

                if (childCount == 0)
                {
                    return null;
                }

                switch (direction)
                {
                    case NavigateDirection.FirstChild:
                        return GetChild(0);
                    case NavigateDirection.LastChild:
                        return GetChild(childCount - 1);
                    default:
                        return base.FragmentNavigate(direction);
                }
            }

            internal override bool CanSelectMultiple => true;

            internal override object GetPropertyValue(UIA propertyID)
            {
                switch (propertyID)
                {
                    case UIA.NamePropertyId:
                        return Name;
                    case UIA.AutomationIdPropertyId:
                        return OwningListView.IsHandleCreated ? OwningListView.Name : string.Empty;
                    case UIA.RuntimeIdPropertyId:
                        return RuntimeId;
                    case UIA.ControlTypePropertyId:
                        return UIA.ListControlTypeId;
                    case UIA.IsMultipleViewPatternAvailablePropertyId:
                        return IsPatternSupported(UIA.MultipleViewPatternId);
                    case UIA.ItemStatusPropertyId:
                        return GetItemStatus();
                }

                return base.GetPropertyValue(propertyID);
            }

            internal override int[] RuntimeId
            {
                get
                {
                    var runtimeId = new int[2];
                    runtimeId[0] = 0x2a;
                    runtimeId[1] = (int)(long)OwningListView.Handle;
                    return runtimeId;
                }
            }

            private string GetItemStatus()
            {
                switch (owner.Sorting)
                {
                    case SortOrder.None:
                        return SR.NotSortedAccessibleStatus;
                    case SortOrder.Ascending:
                        return SR.SortedAscendingAccessibleStatus;
                    case SortOrder.Descending:
                        return SR.SortedDescendingAccessibleStatus;
                }

                return null;
            }

            public override AccessibleObject GetChild(int index)
            {
                if (index < 0 || index >= OwningListView.Items.Count)
                {
                    return null;
                }

                ListViewItem item = OwningListView.Items[index];
                if (!_itemAccessibleObjects.ContainsKey(item))
                {
                    _itemAccessibleObjects.Add(item, new ListViewItemAccessibleObject(OwningListView, item, this));
                }

                return _itemAccessibleObjects[item];
            }

            public override int GetChildCount()
            {
                return OwningListView.IsHandleCreated ? OwningListView.Items.Count : 0;
            }

            public Dictionary<ListViewItem, ListViewItemAccessibleObject> ItemAccessibleObjects
            {
                get
                {
                    return _itemAccessibleObjects;
                }
            }

            internal override IRawElementProviderSimple[] GetSelection()
            {
                List<IRawElementProviderSimple> selectedItemProviders = new List<IRawElementProviderSimple>();

                if (!OwningListView.IsHandleCreated)
                {
                    return selectedItemProviders.ToArray();
                }

                var selectedItems = OwningListView.SelectedItems;
                foreach (var selectedItem in selectedItems)
                {
                    selectedItemProviders.Add(_itemAccessibleObjects[(ListViewItem)selectedItem] as IRawElementProviderSimple);
                }

                return selectedItemProviders.ToArray();
            }

            internal override IRawElementProviderFragment ElementProviderFromPoint(double x, double y)
            {
                return HitTest((int)x, (int)y);
            }

            public override AccessibleObject HitTest(int x, int y)
            {
                Point pt = owner.PointToClient(new Point(x, y));
                ListViewHitTestInfo hti = owner.HitTest(pt.X, pt.Y);
                if (!_itemAccessibleObjects.ContainsKey(hti.Item))
                {
                    _itemAccessibleObjects.Add(hti.Item, new ListViewItemAccessibleObject(OwningListView, hti.Item, this));
                }
                return ItemAccessibleObjects[hti.Item].SubItemAccessibleObjects[hti.SubItem];
            }

            internal override int ColumnCount
            {
                get
                {
                    int columnCount = 0;

                    var items = OwningListView.Items;
                    if (items.Count > 0)
                    {
                        columnCount = items[0].SubItems.Count;
                    }

                    return columnCount;
                }
            }

            internal override int RowCount => OwningListView.Items.Count;

            internal override IRawElementProviderSimple[] GetRowHeaders() => null;

            internal override IRawElementProviderSimple[] GetColumnHeaders()
            {
                var columnHeaders = new List<IRawElementProviderSimple>();
                foreach (var columnHeader in OwningListView.Columns)
                {
                    columnHeaders.Add(new ListViewColumnAccessibleObject(columnHeader as ColumnHeader));
                }

                return columnHeaders.ToArray();
            }

            internal override RowOrColumnMajor RowOrColumnMajor => RowOrColumnMajor.RowMajor;

            internal override int GetMultiViewProviderCurrentView()
            {
                return (int)OwningListView.View;
            }

            internal override int[] GetMultiViewProviderSupportedViews()
            {
                var allViews = Enum.GetValues(typeof(View));
                var allViewIds = new List<int>();
                foreach (var view in allViews)
                {
                    allViewIds.Add((int)view);
                }

                return allViewIds.ToArray();
            }

            internal override string GetMultiViewProviderViewName(int viewId)
            {
                var allViews = Enum.GetValues(typeof(View));
                foreach (var view in allViews)
                {
                    if ((int)view == viewId)
                    {
                        return view.ToString();
                    }
                }

                return null;
            }

            internal override void SetMultiViewProviderCurrentView(int viewId)
            {
                var allViews = Enum.GetValues(typeof(View));
                foreach (var view in allViews)
                {
                    if ((int)view == viewId)
                    {
                        OwningListView.View = (View)view;
                    }
                }
            }
        }
    }
}
