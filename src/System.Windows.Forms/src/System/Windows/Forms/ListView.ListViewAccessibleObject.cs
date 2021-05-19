﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class ListView
    {
        internal class ListViewAccessibleObject : ControlAccessibleObject
        {
            private readonly ListView _owningListView;
            private static readonly int[] s_enumViewValues = (int[])Enum.GetValues(typeof(View));

            internal ListViewAccessibleObject(ListView owningListView) : base(owningListView)
            {
                _owningListView = owningListView;
            }

            internal override bool CanSelectMultiple
                => _owningListView.IsHandleCreated;

            internal override int ColumnCount
                => _owningListView.Columns.Count;

            internal bool OwnerHasDefaultGroup
            {
                get
                {
                    if (!_owningListView.GroupsDisplayed)
                    {
                        return false;
                    }

                    foreach (ListViewItem? item in _owningListView.Items)
                    {
                        // If there are groups in the ListView, then the items which do not belong
                        // to any of the group and have null as the item.Group value, so these items
                        // are put into the default group and thereby the ListView itself starts
                        // containing Default group.
                        if (item is not null && item.Group is null)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }

            internal override int RowCount
                => _owningListView.Items.Count;

            internal override UiaCore.RowOrColumnMajor RowOrColumnMajor
                => UiaCore.RowOrColumnMajor.RowMajor;

            internal override int[]? RuntimeId
            {
                get
                {
                    if (!_owningListView.IsHandleCreated)
                    {
                        return base.RuntimeId;
                    }

                    var runtimeId = new int[2];
                    runtimeId[0] = RuntimeIDFirstItem;
                    runtimeId[1] = PARAM.ToInt(_owningListView.Handle);
                    return runtimeId;
                }
            }

            internal override UiaCore.IRawElementProviderFragment? ElementProviderFromPoint(double x, double y)
            {
                AccessibleObject? element = HitTest((int)x, (int)y);

                return element ?? base.ElementProviderFromPoint(x, y);
            }

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (!_owningListView.IsHandleCreated)
                {
                    return null;
                }

                return direction switch
                {
                    UiaCore.NavigateDirection.FirstChild => GetChild(0),
                    UiaCore.NavigateDirection.LastChild => GetLastChild(),
                    _ => base.FragmentNavigate(direction)
                };
            }

            public override AccessibleObject? GetChild(int index)
            {
                if (!_owningListView.IsHandleCreated || index < 0)
                {
                    return null;
                }

                if (_owningListView.GroupsDisplayed)
                {
                    IReadOnlyList<ListViewGroup> visibleGroups = GetVisibleGroups();
                    return index < visibleGroups.Count ? visibleGroups[index].AccessibilityObject : null;
                }

                return index < _owningListView.Items.Count ? _owningListView.Items[index].AccessibilityObject : null;
            }

            public override int GetChildCount()
            {
                if (!_owningListView.IsHandleCreated)
                {
                    return -1;
                }

                return _owningListView.GroupsDisplayed ? GetVisibleGroups().Count : _owningListView.Items.Count;
            }

            private int GetItemIndex(AccessibleObject? child)
            {
                if (child is null)
                {
                    return -1;
                }

                for (int i = 0; i < _owningListView.Items.Count; i++)
                {
                    if (_owningListView.Items[i].AccessibilityObject == child)
                    {
                        return i;
                    }
                }

                return -1;
            }

            private int GetGroupIndex(AccessibleObject? child)
            {
                if (child is null)
                {
                    return -1;
                }

                IReadOnlyList<ListViewGroup> visibleGroups = GetVisibleGroups();
                for (int i = 0; i < visibleGroups.Count; i++)
                {
                    if (visibleGroups[i].AccessibilityObject == child)
                    {
                        return i;
                    }
                }

                return -1;
            }

            internal override int GetChildIndex(AccessibleObject? child) => _owningListView.GroupsDisplayed ? GetGroupIndex(child) : GetItemIndex(child);

            private string GetItemStatus()
                => _owningListView.Sorting switch
                {
                    SortOrder.Ascending => SR.SortedAscendingAccessibleStatus,
                    SortOrder.Descending => SR.SortedDescendingAccessibleStatus,
                    _ => SR.NotSortedAccessibleStatus
                };

            internal override UiaCore.IRawElementProviderSimple[]? GetColumnHeaders()
            {
                UiaCore.IRawElementProviderSimple[] columnHeaders = new UiaCore.IRawElementProviderSimple[_owningListView.Columns.Count];
                for (int i = 0; i < columnHeaders.Length; i++)
                {
                    columnHeaders[i] = new ColumnHeader.ListViewColumnHeaderAccessibleObject(_owningListView.Columns[i]);
                }

                return columnHeaders;
            }

            internal override UiaCore.IRawElementProviderFragment? GetFocus()
            {
                if (!_owningListView.IsHandleCreated)
                {
                    return null;
                }

                return _owningListView.FocusedItem?.AccessibilityObject ?? _owningListView.FocusedGroup?.AccessibilityObject;
            }

            internal override int GetMultiViewProviderCurrentView()
                => (int)_owningListView.View;

            internal override int[] GetMultiViewProviderSupportedViews()
                => new int[] { (int)View.Details };

            internal override string GetMultiViewProviderViewName(int viewId)
            {
                foreach (int view in s_enumViewValues)
                {
                    if (view == viewId)
                    {
                        return view.ToString();
                    }
                }

                return string.Empty;
            }

            private AccessibleObject? GetLastChild()
            {
                if (_owningListView.GroupsDisplayed)
                {
                    IReadOnlyList<ListViewGroup> visibleGroups = GetVisibleGroups();
                    return visibleGroups.Count == 0 ? null : visibleGroups[visibleGroups.Count - 1].AccessibilityObject;
                }

                return _owningListView.Items.Count == 0 ? null : _owningListView.Items[_owningListView.Items.Count - 1].AccessibilityObject;
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.NamePropertyId => Name,
                    UiaCore.UIA.AutomationIdPropertyId => _owningListView.Name,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
                    UiaCore.UIA.HasKeyboardFocusPropertyId => false,
                    UiaCore.UIA.RuntimeIdPropertyId => RuntimeId,
                    // If we don't set a default role for the accessible object
                    // it will be retrieved from Windows.
                    // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                    UiaCore.UIA.ControlTypePropertyId => _owningListView.AccessibleRole == AccessibleRole.Default
                                                         ? UiaCore.UIA.ListControlTypeId
                                                         : base.GetPropertyValue(propertyID),
                    UiaCore.UIA.IsMultipleViewPatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.MultipleViewPatternId),
                    UiaCore.UIA.ItemStatusPropertyId => GetItemStatus(),
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override UiaCore.IRawElementProviderSimple[]? GetRowHeaders()
                => null;

            internal override UiaCore.IRawElementProviderSimple[] GetSelection()
            {
                if (!_owningListView.IsHandleCreated)
                {
                    return Array.Empty<UiaCore.IRawElementProviderSimple>();
                }

                UiaCore.IRawElementProviderSimple[] selectedItemProviders = new UiaCore.IRawElementProviderSimple[_owningListView.SelectedIndices.Count];
                for (int i = 0; i < selectedItemProviders.Length; i++)
                {
                    selectedItemProviders[i] = _owningListView.Items[_owningListView.SelectedIndices[i]].AccessibilityObject;
                }

                return selectedItemProviders;
            }

            internal IReadOnlyList<ListViewGroup> GetVisibleGroups()
            {
                List<ListViewGroup> list = new();
                if (OwnerHasDefaultGroup)
                {
                    list.Add(_owningListView.DefaultGroup);
                }

                foreach (ListViewGroup listViewGroup in _owningListView.Groups)
                {
                    if (listViewGroup.AccessibilityObject is ListViewGroup.ListViewGroupAccessibleObject listViewGroupAccessibleObject
                        && listViewGroupAccessibleObject.GetVisibleItems().Count > 0)
                    {
                        list.Add(listViewGroup);
                    }
                }

                return list;
            }

            public override AccessibleObject? HitTest(int x, int y)
            {
                if (!_owningListView.IsHandleCreated)
                {
                    return null;
                }

                Point hitTestPoint = new(x, y);
                Point point = _owningListView.PointToClient(hitTestPoint);
                ListViewHitTestInfo hitTestInfo = _owningListView.HitTest(point.X, point.Y);
                if (hitTestInfo.Item is null && _owningListView.GroupsDisplayed)
                {
                    IReadOnlyList<ListViewGroup> visibleGroups = GetVisibleGroups();
                    for (int i = 0; i < visibleGroups.Count; i++)
                    {
                        if (visibleGroups[i].AccessibilityObject.Bounds.Contains(hitTestPoint))
                        {
                            return visibleGroups[i].AccessibilityObject;
                        }
                    }

                    return null;
                }

                if (hitTestInfo.Item is not null)
                {
                    if (hitTestInfo.SubItem is not null)
                    {
                        return _owningListView.View switch
                        {
                            View.Details => hitTestInfo.SubItem.AccessibilityObject,

                            // Only additional ListViewSubItem are displayed in the accessibility tree if the ListView
                            // in the "Tile" view (the first ListViewSubItem is responsible for the ListViewItem)
                            View.Tile => hitTestInfo.SubItem.Index > 0 ? hitTestInfo.SubItem.AccessibilityObject : hitTestInfo.Item.AccessibilityObject,
                            _ => hitTestInfo.Item.AccessibilityObject
                        };
                    }

                    if (_owningListView.View == View.Details)
                    {
                        var itemAccessibleObject = (ListViewItem.ListViewItemAccessibleObject)hitTestInfo.Item.AccessibilityObject;

                        for (int i = 1; i < _owningListView.Columns.Count; i++)
                        {
                            if (itemAccessibleObject.GetSubItemBounds(i).Contains(point))
                            {
                                return itemAccessibleObject.GetDetailsSubItemOrFake(i);
                            }
                        }
                    }

                    return hitTestInfo.Item.AccessibilityObject;
                }

                return null;
            }

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.SelectionPatternId ||
                    patternId == UiaCore.UIA.MultipleViewPatternId ||
                    (patternId == UiaCore.UIA.GridPatternId && _owningListView.View == View.Details) ||
                    (patternId == UiaCore.UIA.TablePatternId && _owningListView.View == View.Details))
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override void SetMultiViewProviderCurrentView(int viewId)
            {
                foreach (var view in s_enumViewValues)
                {
                    if (view == viewId)
                    {
                        _owningListView.View = (View)view;
                    }
                }
            }
        }
    }
}
