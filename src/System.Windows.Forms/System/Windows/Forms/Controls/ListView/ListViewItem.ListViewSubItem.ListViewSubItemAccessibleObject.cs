// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ListViewItem
{
    public partial class ListViewSubItem
    {
        internal sealed class ListViewSubItemAccessibleObject : AccessibleObject
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

            internal override IRawElementProviderFragmentRoot.Interface FragmentRoot => _owningListView.AccessibilityObject;

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
                    // but using UIA we found out that PInvokeCore.SendMessage work incorrectly.
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

            internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
                => direction switch
                {
                    NavigateDirection.NavigateDirection_Parent
                        => ParentInternal,
                    NavigateDirection.NavigateDirection_NextSibling
                        => ParentInternal.GetChildInternal(ParentInternal.GetChildIndex(this) + 1),
                    NavigateDirection.NavigateDirection_PreviousSibling
                        => ParentInternal.GetChildInternal(ParentInternal.GetChildIndex(this) - 1),
                    NavigateDirection.NavigateDirection_FirstChild => GetChild(),
                    NavigateDirection.NavigateDirection_LastChild => GetChild(),
                    _ => base.FragmentNavigate(direction)
                };

            internal override int Column
                => _owningListView.View == View.Details
                    ? ParentInternal.GetChildIndex(this) - ParentInternal.FirstSubItemIndex
                    : InvalidIndex;

            /// <summary>
            ///  Gets or sets the accessible name.
            /// </summary>
            public override string? Name => base.Name ?? OwningSubItem?.Text ?? string.Empty;

            internal override bool CanGetNameInternal => false;

            public override AccessibleObject Parent => ParentInternal;

            private protected override bool IsInternal => true;

            private ListViewItemBaseAccessibleObject ParentInternal
                => (ListViewItemBaseAccessibleObject)_owningItem.AccessibilityObject;

            internal override int[] RuntimeId
            {
                get
                {
                    int[] id = Parent.RuntimeId;

                    Debug.Assert(id.Length >= 4);

                    return [id[0], id[1], id[2], id[3], GetHashCode()];
                }
            }

            internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
                => propertyID switch
                {
                    // All subitems are "text". Some of them can be editable, if ListView.LabelEdit is true.
                    // In this case, an edit field appears when editing. This field has own accessible object, that
                    // has the "edit" control type, and it supports the Text pattern. And its owning subitem accessible
                    // object has the "text" control type, because it is just a container for the edit field.
                    UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_TextControlTypeId,
                    UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => (VARIANT)(_owningListView.Focused && _owningListView.FocusedItem == _owningItem),
                    UIA_PROPERTY_ID.UIA_IsEnabledPropertyId => (VARIANT)_owningListView.Enabled,
                    UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => (VARIANT)State.HasFlag(AccessibleStates.Focusable),
                    UIA_PROPERTY_ID.UIA_ProcessIdPropertyId => (VARIANT)Environment.ProcessId,
                    _ => base.GetPropertyValue(propertyID)
                };

            /// <summary>
            ///  Gets the accessible state.
            /// </summary>
            public override AccessibleStates State
                => AccessibleStates.Focusable;

            internal override IRawElementProviderSimple.Interface ContainingGrid
                => _owningListView.AccessibilityObject;

            internal override int Row => _owningItem.Index;

            internal override IRawElementProviderSimple.Interface[]? GetColumnHeaderItems()
                => _owningListView.View == View.Details && Column > -1
                    ? [_owningListView.Columns[Column].AccessibilityObject]
                    : null;

            internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            {
                if (patternId is UIA_PATTERN_ID.UIA_GridItemPatternId or UIA_PATTERN_ID.UIA_TableItemPatternId)
                {
                    return _owningListView.View == View.Details;
                }

                return base.IsPatternSupported(patternId);
            }

            private protected override string AutomationId
                => $"{nameof(ListViewSubItem)}-{ParentInternal.GetChildIndex(this)}";

            private AccessibleObject? GetChild()
            {
                if (_owningListView._labelEdit is { } labelEdit)
                {
                    return labelEdit.AccessibilityObject;
                }

                return null;
            }
        }
    }
}
