// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ListViewItem
{
    internal abstract class ListViewItemWithImageAccessibleObject : ListViewItemBaseAccessibleObject
    {
        private const int ImageAccessibleObjectIndex = 0;

        private ListViewItemImageAccessibleObject? _imageAccessibleObject;

        private ListViewLabelEditAccessibleObject? _labelEditAccessibleObject;

        public ListViewItemWithImageAccessibleObject(ListViewItem owningItem) : base(owningItem)
        {
        }

        internal override int FirstSubItemIndex => HasImage ? 1 : 0;

        private ListViewItemImageAccessibleObject ImageAccessibleObject => _imageAccessibleObject ??= new(_owningItem);
        private ListViewLabelEditAccessibleObject? LabelEditAccessibleObject
            => _labelEditAccessibleObject ??= _owningListView._labelEdit is null
                ? null
                : new(_owningListView, _owningListView._labelEdit);

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
            => direction switch
            {
                NavigateDirection.NavigateDirection_FirstChild => GetChildCount() > 0 ? GetChild(0) : null,
                NavigateDirection.NavigateDirection_LastChild => GetChildCount() > 0 ? GetChild(GetChildCount() - 1) : null,
                _ => base.FragmentNavigate(direction),
            };

        internal AccessibleObject GetAccessibleObject(Point point)
        {
            return HasImage && ImageAccessibleObject.GetImageRectangle().Contains(point)
                ? ImageAccessibleObject
                : this;
        }

        public override AccessibleObject? GetChild(int index)
        {
            if (_owningListView.View != View)
            {
                throw new InvalidOperationException(string.Format(SR.ListViewItemAccessibilityObjectInvalidViewException, View.ToString()));
            }

            if (index == ImageAccessibleObjectIndex && HasImage)
            {
                return ImageAccessibleObject;
            }

            if (index >= ImageAccessibleObjectIndex || !HasImage)
            {
                return LabelEditAccessibleObject;
            }

            return null;
        }

        public override int GetChildCount()
        {
            if (_owningListView.View != View)
            {
                throw new InvalidOperationException(string.Format(SR.ListViewItemAccessibilityObjectInvalidViewException, View.ToString()));
            }

            if (!_owningListView.IsHandleCreated)
            {
                return InvalidIndex;
            }

            int _childCount = 0;

            if (HasImage)
            {
                _childCount++;
            }

            if (_owningListView._labelEdit is not null && _owningListView._listViewSubItem is null)
            {
                _childCount++;
            }

            return _childCount > 0 ? _childCount : InvalidIndex;
        }

        internal override int GetChildIndex(AccessibleObject? child)
            => child is ListViewItemImageAccessibleObject
                ? ImageAccessibleObjectIndex
                : InvalidIndex;

        internal override Rectangle GetSubItemBounds(int accessibleChildIndex)
            => _owningListView.IsHandleCreated && accessibleChildIndex == ImageAccessibleObjectIndex && HasImage
                ? ImageAccessibleObject.Bounds
                : Rectangle.Empty;

        /// <devdoc>
        ///  Caller should ensure that the current OS is Windows 8 or greater.
        /// </devdoc>
        internal override void ReleaseChildUiaProviders()
        {
            base.ReleaseChildUiaProviders();

            PInvoke.UiaDisconnectProvider(_imageAccessibleObject, skipOSCheck: true);
        }
    }
}
