// Licensed to the .NET Foundation under one or more agreements.
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
                    _owningItem = owningItem.OrThrowIfNull();
                    _owningListView = owningItem.ListView ?? owningItem.Group?.ListView ?? throw new InvalidOperationException(nameof(owningItem.ListView));
                }

                internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
                    => _owningListView.AccessibilityObject;

                public override Rectangle Bounds
                {
                    get
                    {
                        int index = ParentInternal.GetChildIndex(this);
                        if (index == InvalidIndex)
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

                        if (!_owningListView.FullRowSelect && index == ParentInternal.FirstSubItemIndex && _owningListView.Columns.Count > 1)
                        {
                            width = ParentInternal.GetSubItemBounds(ParentInternal.FirstSubItemIndex + 1).X - bounds.X;
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

                internal override int Column
                    => _owningListView.View == View.Details
                        ? ParentInternal.GetChildIndex(this)
                        : InvalidIndex;

                /// <summary>
                ///  Gets or sets the accessible name.
                /// </summary>
                public override string? Name
                {
                    get => base.Name ?? OwningSubItem?.Text ?? string.Empty;
                    set => base.Name = value;
                }

                public override AccessibleObject Parent => ParentInternal;

                private ListViewItemBaseAccessibleObject ParentInternal
                    => (ListViewItemBaseAccessibleObject)_owningItem.AccessibilityObject;

                internal override int[] RuntimeId
                {
                    get
                    {
                        var owningItemRuntimeId = Parent.RuntimeId;

                        Debug.Assert(owningItemRuntimeId.Length >= 4);

                        return new int[]
                        {
                            owningItemRuntimeId[0],
                            owningItemRuntimeId[1],
                            owningItemRuntimeId[2],
                            owningItemRuntimeId[3],
                            GetHashCode()
                        };
                    }
                }

                internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                    => propertyID switch
                    {
                        // All subitems are "text". Some of them can be editable, if ListView.LabelEdit is true.
                        // In this case, an edit field appears when editing. This field has own accessible object, that
                        // has the "edit" control type, and it supports the Text pattern. And its owning subitem accessible
                        // object has the "text" control type, because it is just a container for the edit field.
                        UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.TextControlTypeId,
                        UiaCore.UIA.HasKeyboardFocusPropertyId => _owningListView.Focused && _owningListView.FocusedItem == _owningItem,
                        UiaCore.UIA.IsEnabledPropertyId => _owningListView.Enabled,
                        UiaCore.UIA.IsKeyboardFocusablePropertyId => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
                        UiaCore.UIA.ProcessIdPropertyId => Environment.ProcessId,
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

                private protected override string AutomationId
                    => $"{typeof(ListViewItem.ListViewSubItem).Name}-{ParentInternal.GetChildIndex(this)}";
            }
        }
    }
}
