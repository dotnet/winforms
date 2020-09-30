// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

            private bool OwnerHasDefaultGroup
            {
                get
                {
                    if (!_owningListView.IsHandleCreated || !_owningListView.ShowGroups || _owningListView.VirtualMode)
                    {
                        return false;
                    }

                    foreach (ListViewItem? item in _owningListView.Items)
                    {
                        // If there are groups in the ListView, then the items which do not belong
                        // to any of the group and have null as the item.Group value, so these items
                        // are put into the default group and thereby the ListView itself starts
                        // containing Default group.
                        if (item != null && item.Group is null)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }

            private bool OwnerHasGroups
                => _owningListView.IsHandleCreated && _owningListView.Groups.Count > 0;

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

            internal override UiaCore.IRawElementProviderFragment ElementProviderFromPoint(double x, double y)
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

                int childCount = GetChildCount();
                if (childCount == 0)
                {
                    return null;
                }

                return direction switch
                {
                    UiaCore.NavigateDirection.FirstChild => GetChild(0),
                    UiaCore.NavigateDirection.LastChild => GetChild(childCount - 1),
                    _ => base.FragmentNavigate(direction)
                };
            }

            public override AccessibleObject? GetChild(int index)
            {
                if (!_owningListView.IsHandleCreated || index < 0 || index >= GetChildCount())
                {
                    return null;
                }

                if (!OwnerHasGroups)
                {
                    return _owningListView.Items[index].AccessibilityObject;
                }

                if (!OwnerHasDefaultGroup)
                {
                    return _owningListView.Groups[index].AccessibilityObject;
                }

                // Default group has the last index out of the Groups.Count
                // upper bound: so the DefaultGroup.Index == Groups.Count.
                // But IMPORTANT: in the accessible tree the position of
                // default group is the first before other groups.
                return index == 0
                    ? _owningListView.DefaultGroup.AccessibilityObject
                    : _owningListView.Groups[index - 1].AccessibilityObject;
            }

            public override int GetChildCount()
            {
                if (!_owningListView.IsHandleCreated)
                {
                    return 0;
                }

                if (_owningListView.Groups.Count > 0)
                {
                    return OwnerHasDefaultGroup ? _owningListView.Groups.Count + 1 : _owningListView.Groups.Count;
                }

                return _owningListView.Items.Count;
            }

            internal int GetChildIndex(AccessibleObject child)
            {
                if (child is null)
                {
                    return -1;
                }

                int childCount = GetChildCount();
                for (int i = 0; i < childCount; i++)
                {
                    AccessibleObject? currentChild = GetChild(i);
                    if (child == currentChild)
                    {
                        return i;
                    }
                }

                return -1;
            }

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
                    ColumnHeader columnHeader = _owningListView.Columns[i];
                    columnHeaders[i] = new ColumnHeader.ListViewColumnHeaderAccessibleObject(columnHeader);
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

            internal AccessibleObject? GetNextChild(AccessibleObject currentChild)
            {
                int currentChildIndex = GetChildIndex(currentChild);
                if (currentChildIndex == -1)
                {
                    return null;
                }

                int childCount = GetChildCount();
                if (currentChildIndex > childCount - 2) // is not the second to the last element.
                {
                    return null;
                }

                return GetChild(currentChildIndex + 1);
            }

            internal AccessibleObject? GetPreviousChild(AccessibleObject currentChild)
            {
                int currentChildIndex = GetChildIndex(currentChild);
                if (currentChildIndex <= 0)
                {
                    return null;
                }

                return GetChild(currentChildIndex - 1);
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.NamePropertyId => Name,
                    UiaCore.UIA.AutomationIdPropertyId => _owningListView.Name,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
                    UiaCore.UIA.HasKeyboardFocusPropertyId => false,
                    UiaCore.UIA.RuntimeIdPropertyId => RuntimeId,
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.ListControlTypeId,
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

            public override AccessibleObject? HitTest(int x, int y)
            {
                if (!_owningListView.IsHandleCreated)
                {
                    return null;
                }

                Point point = _owningListView.PointToClient(new Point(x, y));
                ListViewHitTestInfo hitTestInfo = _owningListView.HitTest(point.X, point.Y);
                if (hitTestInfo.Item is null && OwnerHasGroups)
                {
                    for (int i = 0; i < GetChildCount(); i++)
                    {
                        AccessibleObject? accessibilityObject = GetChild(i);
                        if (accessibilityObject != null &&
                            accessibilityObject.Bounds.Contains(new Point(x, y)))
                        {
                            return accessibilityObject;
                        }
                    }

                    return null;
                }

                if (hitTestInfo.Item != null)
                {
                    if (hitTestInfo.SubItem != null)
                    {
                        return hitTestInfo.SubItem.AccessibilityObject;
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
