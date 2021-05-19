﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class ListViewItem
    {
        public partial class ListViewSubItem
        {
            internal class ListViewSubItemAccessibleObject : AccessibleObject
            {
                private readonly ListView _owningListView;
                private readonly ListViewItem _owningItem;

                // This is necessary for the "Details" view,  when there is no ListViewSubItem,
                // but the cell for it is displayed and the user can interact with it.
                internal ListViewSubItem? OwningSubItem { get; private set; }

                public ListViewSubItemAccessibleObject(ListViewSubItem? owningSubItem, ListViewItem owningItem)
                {
                    OwningSubItem = owningSubItem;
                    _owningItem = owningItem ?? throw new ArgumentNullException(nameof(owningItem));
                    _owningListView = owningItem.ListView ?? owningItem.Group?.ListView ?? throw new InvalidOperationException(nameof(owningItem.ListView));
                }

                internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
                    => _owningListView.AccessibilityObject;

                public override Rectangle Bounds
                {
                    get
                    {
                        int index = ParentInternal.GetChildIndex(this);
                        if (index == -1)
                        {
                            return Rectangle.Empty;
                        }

                        Rectangle bounds = ParentInternal.GetSubItemBounds(index);
                        if (bounds.IsEmpty)
                        {
                            return bounds;
                        }

                        // Previously bounds was provided using MSAA,
                        // but using UIA we found out that SendMessageW work incorrectly.
                        // When we need to get bounds for first sub item it will return width of all item.
                        int width = bounds.Width;

                        if (index == 0 && _owningListView.Columns.Count > 1)
                        {
                            width = ParentInternal.GetSubItemBounds(subItemIndex: 1).X - bounds.X;
                        }

                        if (width <= 0)
                        {
                            return Rectangle.Empty;
                        }

                        return new Rectangle(
                            _owningListView.AccessibilityObject.Bounds.X + bounds.X,
                            _owningListView.AccessibilityObject.Bounds.Y + bounds.Y,
                            width, bounds.Height);
                    }
                }

                internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
                    => direction switch
                    {
                        UiaCore.NavigateDirection.Parent
                            => ParentInternal,
                        UiaCore.NavigateDirection.NextSibling
                            => ParentInternal.GetChildInternal(ParentInternal.GetChildIndex(this) + 1),
                        UiaCore.NavigateDirection.PreviousSibling
                            => ParentInternal.GetChildInternal(ParentInternal.GetChildIndex(this) - 1),
                        _ => base.FragmentNavigate(direction)
                    };

                internal override int Column => _owningListView.View == View.Details ? ParentInternal.GetChildIndex(this) : -1;

                /// <summary>
                ///  Gets or sets the accessible name.
                /// </summary>
                public override string? Name
                {
                    get => base.Name ?? OwningSubItem?.Text ?? string.Empty;
                    set => base.Name = value;
                }

                public override AccessibleObject Parent => ParentInternal;

                private ListViewItemAccessibleObject ParentInternal
                    => (ListViewItemAccessibleObject)_owningItem.AccessibilityObject;

                internal override int[]? RuntimeId
                {
                    get
                    {
                        var owningItemRuntimeId = Parent.RuntimeId;
                        if (owningItemRuntimeId is null)
                        {
                            return base.RuntimeId;
                        }

                        var runtimeId = new int[5];
                        runtimeId[0] = owningItemRuntimeId[0];
                        runtimeId[1] = owningItemRuntimeId[1];
                        runtimeId[2] = owningItemRuntimeId[2];
                        runtimeId[3] = owningItemRuntimeId[3];
                        runtimeId[4] = ParentInternal.GetChildIndex(this);
                        return runtimeId;
                    }
                }

                internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                    => propertyID switch
                    {
                        // All subitems are "text" except the first.
                        // It can be "edit" if ListView.LabelEdit is true.
                        UiaCore.UIA.ControlTypePropertyId => _owningListView.LabelEdit && ParentInternal.GetChildIndex(this) == 0
                                                             ? UiaCore.UIA.EditControlTypeId
                                                             : UiaCore.UIA.TextControlTypeId,
                        UiaCore.UIA.NamePropertyId => Name,
                        UiaCore.UIA.FrameworkIdPropertyId => NativeMethods.WinFormFrameworkId,
#pragma warning disable CA1837 // Use 'Environment.ProcessId'
                        UiaCore.UIA.ProcessIdPropertyId => Process.GetCurrentProcess().Id,
#pragma warning restore CA1837 // Use 'Environment.ProcessId'
                        UiaCore.UIA.AutomationIdPropertyId => AutomationId,
                        UiaCore.UIA.RuntimeIdPropertyId => RuntimeId,
                        UiaCore.UIA.HasKeyboardFocusPropertyId => _owningListView.Focused && _owningListView.FocusedItem == _owningItem,
                        UiaCore.UIA.IsKeyboardFocusablePropertyId => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
                        UiaCore.UIA.IsEnabledPropertyId => _owningListView.Enabled,
                        UiaCore.UIA.IsOffscreenPropertyId => (State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen,
                        UiaCore.UIA.BoundingRectanglePropertyId => Bounds,
                        UiaCore.UIA.IsGridItemPatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.GridItemPatternId),
                        UiaCore.UIA.IsTableItemPatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.TableItemPatternId),
                        _ => base.GetPropertyValue(propertyID)
                    };

                /// <summary>
                ///  Gets the accessible state.
                /// </summary>
                public override AccessibleStates State
                    => AccessibleStates.Focusable;

                internal override UiaCore.IRawElementProviderSimple ContainingGrid
                    => _owningListView.AccessibilityObject;

                internal override int Row => _owningItem.Index;

                internal override UiaCore.IRawElementProviderSimple[]? GetColumnHeaderItems()
                    => new UiaCore.IRawElementProviderSimple[] { _owningListView.Columns[Column].AccessibilityObject };

                internal override bool IsPatternSupported(UiaCore.UIA patternId)
                {
                    if (patternId == UiaCore.UIA.GridItemPatternId ||
                        patternId == UiaCore.UIA.TableItemPatternId)
                    {
                        return _owningListView.View == View.Details;
                    }

                    return base.IsPatternSupported(patternId);
                }

                private string AutomationId
                    => string.Format("{0}-{1}", typeof(ListViewItem.ListViewSubItem).Name, ParentInternal.GetChildIndex(this));
            }
        }
    }
}
