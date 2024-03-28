// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ListViewItem
{
    internal sealed class ListViewItemImageAccessibleObject : AccessibleObject
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
                Rectangle imageRectangle = GetImageRectangle();
                return _owningItem.ListView!.RectangleToScreen(imageRectangle);
            }
        }

        public override string DefaultAction => string.Empty;

        internal override bool CanGetDefaultActionInternal => false;

        internal override IRawElementProviderFragmentRoot.Interface? FragmentRoot => _owningItem.ListView?.AccessibilityObject;

        public override AccessibleObject Parent => _owningItem.AccessibilityObject;

        private protected override bool IsInternal => true;

        internal override int[] RuntimeId
        {
            get
            {
                int[] id = Parent.RuntimeId;

                Debug.Assert(id.Length >= 4);

                return [id[0], id[1], id[2], id[3], GetHashCode()];
            }
        }

        public override int GetChildCount() => 0;

        internal Rectangle GetImageRectangle() => _owningItem.ListView!.GetItemRect(_owningItem.Index, ItemBoundsPortion.Icon);

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_ImageControlTypeId,
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => VARIANT.False,
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => VARIANT.False,
                _ => base.GetPropertyValue(propertyID)
            };

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
            => direction switch
            {
                NavigateDirection.NavigateDirection_Parent => Parent,
                NavigateDirection.NavigateDirection_NextSibling => Parent.GetChild(1),
                NavigateDirection.NavigateDirection_PreviousSibling => null,
                _ => base.FragmentNavigate(direction)
            };
    }
}
