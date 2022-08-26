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
        internal class ListViewItemImageAccessibleObject : AccessibleObject
        {
            private readonly ListViewItem _owningItem;

            public ListViewItemImageAccessibleObject(ListViewItem owner)
            {
                _owningItem = owner.OrThrowIfNull();
            }

            public override Rectangle Bounds
            {
                get
                {
                    Rectangle imageRectangle = _owningItem.ListView.GetItemRect(_owningItem.Index, ItemBoundsPortion.Icon);
                    return _owningItem.ListView.RectangleToScreen(imageRectangle);
                }
            }

            public override string DefaultAction => string.Empty;

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
                => _owningItem.ListView.AccessibilityObject;

            public override AccessibleObject Parent => _owningItem.AccessibilityObject;

            internal override int[] RuntimeId
            {
                get
                {
                    int[] owningItemRuntimeId = Parent.RuntimeId;

                    Debug.Assert(owningItemRuntimeId.Length >= 4);

                    return new[]
                    {
                        owningItemRuntimeId[0],
                        owningItemRuntimeId[1],
                        owningItemRuntimeId[2],
                        owningItemRuntimeId[3],
                        GetHashCode()
                    };
                }
            }

            public override int GetChildCount() => 0;

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.ImageControlTypeId,
                    UiaCore.UIA.HasKeyboardFocusPropertyId => false,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId => false,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
                => direction switch
                {
                    UiaCore.NavigateDirection.Parent => Parent,
                    UiaCore.NavigateDirection.NextSibling => Parent.GetChild(1),
                    UiaCore.NavigateDirection.PreviousSibling => null,
                    _ => base.FragmentNavigate(direction)
                };
        }
    }
}
